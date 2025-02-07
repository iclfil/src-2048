using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour {
     private static readonly string _musicPrefKey = "Music_Muted";
     private static readonly string _soundPrefKey = "Sound_Muted";

     public static AudioSystem Instance = null;

     const bool AutoPause = true;

     const float DefaultMusicVolume = 1f;
     const float DefaultSoundVolume = 1f;

     const float MusicFadeTime = 2f;

     readonly string _baseFolder = "Audio/";

     AudioMixerGroup _musicAudioMixerGroup;
     AudioMixerGroup _soundAudioMixerGroup;

     struct SourceInfo {
          public AudioSource source;
          public float startTime;
     }

     List<SourceInfo> _sounds = new List<SourceInfo>();
     AudioSource _currentMusicSource;

     string _currentMusicName;

     bool isFading;
     float fadeTimer;

     struct MusicFader {
          public AudioSource audio;
          public float timer;
          public float fadingTime;
          public float startVolume;
          public float targetVolume;
          public bool destroyOnComplete;
     }

     private void Awake() {

          Instance = this;

     }

     private class LoopedSound {
          public LoopedSound(string name) {
               Name = name;
               Counter = 1;
          }
          public int Counter;
          public string Name;
     }

     private List<LoopedSound> _loopedSounds = new List<LoopedSound>();

     List<MusicFader> _musicFadings = new List<MusicFader>();

     float _volumeMusic = DefaultMusicVolume;
     float _volumeSound = DefaultSoundVolume;

     bool _mutedMusic = false;
     bool _mutedSound = false;

     void OnEnable() {
          gameObject.AddComponent<AudioListener>();
          AudioMixer mixer = Resources.Load(_baseFolder + "MasterMixer") as AudioMixer;
          Debug.Log("Mixer:" + mixer);
          _musicAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
          _soundAudioMixerGroup = mixer.FindMatchingGroups("Sound")[0];
          LoadSettings();
     }

     #region Public functions

     public void PlayMusic(string name) {
          PlayMusicInternal(name);
     }

     public void StopMusic() {
          StopMusicInternal();
     }

     public void PlaySound(string name, bool pausable = true, bool loop = false) {
          if (loop) {
               LoopedSound loopedSound = _loopedSounds.Find(sound => sound.Name == name);
               if (loopedSound != null) {
                    loopedSound.Counter++;
                    Debug.Log(string.Format("Increment {0} {1}", loopedSound.Name, loopedSound.Counter));
                    return;
               } else {
                    _loopedSounds.Add(new LoopedSound(name));
                    Debug.Log(string.Format("Add {0} 1", name));
                   // Debug.Log("Loop cout:" + _loopedSounds.Count);
               }
          }

          //Debug.Log("NameView Playing" + name);

          PlaySoundInternal(name, pausable, loop);
     }

     public void StopSound(string name) {
          LoopedSound loopedSound = _loopedSounds.Find(sound => sound.Name == name);
          if (loopedSound != null) {
               loopedSound.Counter--;
               Debug.Log(string.Format("Decrement {0} {1}", loopedSound.Name, loopedSound.Counter));
               if (loopedSound.Counter > 0) {
                    return;
               }
               _loopedSounds.Remove(loopedSound);
          }

          FindAndStopAudioSource(name);
     }

     private void FindAndStopAudioSource(string name) {

          foreach (SourceInfo sound in _sounds) {
               name = name.Insert(0,"Sound: ");

               if (sound.source.name == name && !sound.source.ignoreListenerPause) {
                    sound.source.loop = false;
                   // D//ebug.Log("STOP");
                    sound.source.Stop();
               }
          }
     }

     public void PlaySoundWithDelay(string name, float delay, bool pausable = true) {
          PlaySoundWithDelayInternal(name, delay, pausable);
     }

     public void Pause() {
          if (AutoPause)
               return;

          // Supress Unreachable code warning
#pragma warning disable
          AudioListener.pause = true;
#pragma warning restore
     }

     public void UnPause() {
          if (AutoPause)
               return;

          // Supress Unreachable code warning
#pragma warning disable
          AudioListener.pause = false;
#pragma warning restore
     }

     public void StopAllPausableSounds() {
          StopAllPausableSoundsInternal();
     }

     // Volume [0 - 1]
     public void SetMusicVolume(float volume) {
          SetMusicVolumeInternal(volume);
     }

     // Volume [0 - 1]
     public float GetMusicVolume() {
          return GetMusicVolumeInternal();
     }

     public void SetMusicMuted(bool mute) {
          SetMusicMutedInternal(mute);
     }

     public bool GetMusicMuted() {
          return GetMusicMutedInternal();
     }

     // Volume [0 - 1]
     public void SetSoundVolume(float volume) {
          SetSoundVolumeInternal(volume);
     }

     // Volume [0 - 1]
     public float GetSoundVolume() {
          return GetSoundVolumeInternal();
     }

     public void SetSoundMuted(bool mute) {
          SetSoundMutedInternal(mute);
     }

     public bool GetSoundMuted() {
          return GetSoundMutedInternal();
     }

     #endregion

     #region Settings

     void SetMusicVolumeInternal(float volume) {
          _volumeMusic = volume;
          SaveSettings();
          ApplyMusicVolume();
     }

     float GetMusicVolumeInternal() {
          return _volumeMusic;
     }

     void SetMusicMutedInternal(bool mute) {
          _mutedMusic = mute;
          SaveSettings();
          ApplyMusicMuted();
     }

     bool GetMusicMutedInternal() {
          return _mutedMusic;
     }

     void SetSoundVolumeInternal(float volume) {
          _volumeSound = volume;
          SaveSettings();
          ApplySoundVolume();
     }

     float GetSoundVolumeInternal() {
          return _volumeSound;
     }

     void SetSoundMutedInternal(bool mute) {
          _mutedSound = mute;
          SaveSettings();
          ApplySoundMuted();
     }

     bool GetSoundMutedInternal() {
          return _mutedSound;
     }

     #endregion // Settings

     #region Music

     void PlayMusicInternal(string musicName) {
          if (string.IsNullOrEmpty(musicName)) {
               Debug.Log("Music empty or null");
               return;
          }

          if (_currentMusicName == musicName) {
               Debug.Log("Music already playing: " + musicName);
               return;
          }

          StopMusicInternal();

          _currentMusicName = musicName;

          AudioClip musicClip = LoadClip("Music/" + musicName);

          GameObject music = new GameObject("Music: " + musicName);
          AudioSource musicSource = music.AddComponent<AudioSource>();

          music.transform.parent = transform;

          musicSource.outputAudioMixerGroup = _musicAudioMixerGroup;

          musicSource.loop = true;
          musicSource.priority = 0;
          musicSource.playOnAwake = false;
          musicSource.mute = _mutedMusic;
          musicSource.ignoreListenerPause = true;
          musicSource.clip = musicClip;
          musicSource.Play();
          ApplyMusicMuted();

          musicSource.volume = 0;
          StartFadeMusic(musicSource, MusicFadeTime, _volumeMusic * DefaultMusicVolume, false);
          //musicSource.DOFade(_volumeMusic * DefaultMusicVolume, MusicFadeTime).SetUpdate(true);

          _currentMusicSource = musicSource;
     }

     void StopMusicInternal() {
          _currentMusicName = "";
          if (_currentMusicSource != null) {
               StartFadeMusic(_currentMusicSource, MusicFadeTime, 0, true);
               _currentMusicSource = null;
          }
     }

     #endregion // Music

     #region Sound

     void PlaySoundInternal(string soundName, bool pausable, bool loop) {
          if (string.IsNullOrEmpty(soundName)) {
               Debug.Log("Sound null or empty " + soundName);
               return;
          }

          int sameCountGuard = 0;
          foreach (SourceInfo audioSource in _sounds) {
               if (audioSource.source.clip.name == soundName)
                    sameCountGuard++;
          }

          if (sameCountGuard > 22) {
               Debug.Log("Too much duplicates for sound: " + soundName);
               return;
          }

          if (_sounds.Count > 18) {
               Debug.Log("Too much sounds");
               return;
          }

          //PlaySoundInternalNow(soundName, pausable, loop);
          StartCoroutine(PlaySoundInternalSoon(soundName, pausable, loop));
     }

     void PlaySoundInternalNow(string soundName, bool pausable, bool loop) {
          AudioClip soundClip = LoadClip("Sounds/" + soundName);
          if (null == soundClip) {
               Debug.Log("Sound not loaded: " + soundName);
          }

          GameObject sound = new GameObject("Sound: " + soundName);
          AudioSource soundSource = sound.AddComponent<AudioSource>();
          sound.transform.parent = transform;

          soundSource.outputAudioMixerGroup = _soundAudioMixerGroup;
          soundSource.spatialBlend = 0f;
          soundSource.priority = 8;
          soundSource.playOnAwake = false;
          soundSource.mute = _mutedSound;
          soundSource.volume = _volumeSound * DefaultSoundVolume;
          soundSource.clip = soundClip;
          soundSource.Play();
          soundSource.ignoreListenerPause = !pausable;
          soundSource.loop = loop;

          SourceInfo soundInfo = new SourceInfo();
          soundInfo.source = soundSource;
          soundInfo.startTime = Time.time;

          _sounds.Add(soundInfo);
     }

     IEnumerator PlaySoundInternalSoon(string soundName, bool pausable, bool loop) {
          ResourceRequest request = LoadClipAsync("Sounds/" + soundName);
          while (!request.isDone) {
               yield return null;
          }

          AudioClip soundClip = (AudioClip)request.asset;
          if (null == soundClip) {
               Debug.Log("Sound not loaded: " + soundName);
          }

          GameObject sound = new GameObject("Sound: " + soundName);
          AudioSource soundSource = sound.AddComponent<AudioSource>();
          sound.transform.parent = transform;

          soundSource.outputAudioMixerGroup = _soundAudioMixerGroup;
          soundSource.spatialBlend = 0f;
          soundSource.priority = 8;
          soundSource.playOnAwake = false;
          soundSource.mute = _mutedSound;
          soundSource.volume = _volumeSound * DefaultSoundVolume;
          soundSource.clip = soundClip;
          soundSource.PlayDelayed(0);
          soundSource.ignoreListenerPause = !pausable;
          soundSource.loop = loop;

          SourceInfo soundInfo = new SourceInfo();
          soundInfo.source = soundSource;
          soundInfo.startTime = Time.time;

          _sounds.Add(soundInfo);
     }

     void PlaySoundWithDelayInternal(string soundName, float delay, bool pausable) {
          StartCoroutine(PlaySoundWithDelayCoroutine(soundName, delay, pausable));
     }

     void StopAllPausableSoundsInternal() {
          foreach (SourceInfo sound in _sounds) {
               if (!sound.source.ignoreListenerPause)
                    sound.source.Stop();
          }
     }

     #endregion // Sound

     #region Internal

     void Update() {
          float timeCut = Time.time - 1f;
          var soundsToDelete = _sounds.FindAll(sound => !sound.source.isPlaying && sound.startTime < timeCut);

          foreach (SourceInfo sound in soundsToDelete) {
               _sounds.Remove(sound);
               Destroy(sound.source.gameObject);
          }

          if (AutoPause) {
               bool curPause = Time.timeScale < 0.1f;
               if (curPause != AudioListener.pause) {
                    AudioListener.pause = curPause;
               }
          }

          for (int i = 0; i < _musicFadings.Count; i++) {
               MusicFader music = _musicFadings[i];
               if (music.audio == null) {
                    _musicFadings.RemoveAt(i);
                    i--;
               } else {
                    music.timer += Time.unscaledDeltaTime;
                    _musicFadings[i] = music;
                    if (music.timer >= music.fadingTime) {
                         music.audio.volume = music.targetVolume;
                         if (music.destroyOnComplete) {
                              Destroy(music.audio.gameObject);
                         }
                         _musicFadings.RemoveAt(i);
                         i--;
                    } else {
                         float k = Mathf.Clamp01(music.timer / music.fadingTime);
                         music.audio.volume = Mathf.Lerp(music.startVolume, music.targetVolume, k);
                    }
               }
          }
     }

     void StopFadingForMusic(AudioSource music) {
          for (int i = 0; i < _musicFadings.Count; i++) {
               MusicFader fader = _musicFadings[i];
               if (fader.audio == music) {
                    if (fader.destroyOnComplete) {
                         Destroy(fader.audio.gameObject);
                    }
                    _musicFadings.RemoveAt(i);
                    return;
               }
          }
     }

     void StartFadeMusic(AudioSource music, float duration, float targetVolume, bool destroyOnComplete) {
          MusicFader fader;
          fader.audio = music;
          fader.fadingTime = duration;
          fader.timer = 0;
          fader.startVolume = music.volume;
          fader.targetVolume = targetVolume;
          fader.destroyOnComplete = destroyOnComplete;
          _musicFadings.Add(fader);
     }

     private IEnumerator PlaySoundWithDelayCoroutine(string name, float delay, bool pausable) {
          float timer = delay;
          while (timer > 0) {
               timer -= pausable ? Time.deltaTime : Time.unscaledDeltaTime;
               yield return null;
          }

          PlaySound(name, pausable);
     }

     AudioClip LoadClip(string name) {
          AudioClip clip = Resources.Load<AudioClip>(_baseFolder + name);
          return clip;
     }

     ResourceRequest LoadClipAsync(string name) {
          return Resources.LoadAsync<AudioClip>(_baseFolder + name);
     }

     void SaveSettings() {
          //		PlayerPrefs.SetFloat("SM_MusicVolume", _volumeMusic);
          //		PlayerPrefs.SetFloat("SM_SoundVolume", _volumeSound);

          PlayerPrefs.SetInt(_musicPrefKey, _mutedMusic ? 1 : 0);
          PlayerPrefs.SetInt(_soundPrefKey, _mutedSound ? 1 : 0);
     }

     void LoadSettings() {
          //		_volumeMusic = PlayerPrefs.GetFloat("SM_MusicVolume", 1);
          //		_volumeSound = PlayerPrefs.GetFloat("SM_SoundVolume", 1);

          _mutedMusic = PlayerPrefs.GetInt(_musicPrefKey, 0) == 1;
          _mutedSound = PlayerPrefs.GetInt(_soundPrefKey, 0) == 1;

          ApplySoundVolume();
          ApplyMusicVolume();

          ApplySoundMuted();
          ApplyMusicMuted();
     }





     void ApplySoundVolume() {
          foreach (SourceInfo sound in _sounds) {
               sound.source.volume = _volumeSound * DefaultSoundVolume;
          }
     }

     void ApplyMusicVolume() {
          if (_currentMusicSource != null) {
               StopFadingForMusic(_currentMusicSource);
               _currentMusicSource.volume = _volumeMusic * DefaultMusicVolume;
          }
     }

     void ApplySoundMuted() {
          foreach (SourceInfo sound in _sounds) {
               sound.source.mute = _mutedSound;
          }
     }

     void ApplyMusicMuted() {
          if (_currentMusicSource != null) {
               _currentMusicSource.mute = _mutedMusic;
               if (_currentMusicSource.mute) {
                    _currentMusicSource.Pause();
               } else {
                    _currentMusicSource.UnPause();
               }
          }
     }

     #endregion // Internal
}
