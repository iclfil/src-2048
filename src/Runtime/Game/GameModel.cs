using System;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.Storage;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game
{
    [Serializable]
    public class GameModel
    {
        [SerializeField]
        private int _level;
        [SerializeField]
        private int _score;
        [SerializeField]
        private int _targetChip; //степень двойки
        [SerializeField]
        private int _targetScore;
        [SerializeField]
        private int _bestScore;

        public int TargetChip => _targetChip;

        [SerializeField] private int _scorePerMatch;
        [SerializeField] private int _scoreForNextLevel;

        [SerializeField] private int _rewardForScoresCollected;
        [SerializeField] private int _rewardForTargetCompleted;

        [SerializeField] private float _forceHitChip;

        [SerializeField] private GameView _prefabView;

        public int ScorePerMatch => _scorePerMatch;
        public int ScoreForNextLevel => _scoreForNextLevel;

        public int RewardForScoreCollected => _rewardForScoresCollected;
        public int RewardForTargetCompleted => _rewardForTargetCompleted;

        public float ForceHitChip => _forceHitChip;

        public GameView PrefabView => _prefabView;
        public int Score => _score;
        public int Level => _level;

        public bool ScoresCompleted { get; private set; } = false;
        public bool TargetCompleted { get; private set; } = false;
        public int BestScore { get; set; }

        //old, new
        public Action<int> OnLevelChanged;
        public Action<int> OnScoreChanged;
        public Action<int> OnBestScoreChanged;
        public Action<int> OnTargetScoreChanged;
        public Action<int> OnTargetChipChanged;

        public void Init(GameModel model)
        {
            _scorePerMatch = model.ScorePerMatch;

            SetScoreForNextLevel(model.ScoreForNextLevel);

            _rewardForScoresCollected = model._rewardForScoresCollected;
            _rewardForTargetCompleted = model._rewardForTargetCompleted;
            _forceHitChip = model._forceHitChip;
            _prefabView = model.PrefabView;
            SetLevel(model._level);
            SetScore(model._score);
            CalculateTargetScore();
            SetTargetChip(model.TargetChip);
        }

        private void SetScoreForNextLevel(int value)
        {
            _scoreForNextLevel = value;
            Signals.Get<OnScoreForNextChangedSignal>().Dispatch(_scoreForNextLevel);
        }

        public void NextLevel()
        {
            var nextLevel = _level + 1;
            SetLevel(nextLevel);
            CalculateTargetScore();
        }

        public void NextTargetChip()
        {
            TargetCompleted = false;
            var nextTarget = _targetChip + 1;
            SetTargetChip(nextTarget);
        }

        public void AddScore()
        {
            var newScore = _score + _scorePerMatch;
            SetScore(newScore);
        }

        //баг, уровень сохранился, грузится и сразу выигрышь. Надо пересчитать таргеты после обновления данных.
        public void CalculateTargetScore()
        {
            var value = (_level * ScoreForNextLevel) ;
            SetTargetScore(value);
        }

        public bool ScoreWinning()
        {
            if (_score >= _targetScore)
            {
                ScoresCompleted = true;
                return true;
            }

            return false;
        }

        public bool TargetWinning(FieldController fieldController)
        {
            if (fieldController.HasSymbolId(TargetChip))
            {
                TargetCompleted = true;
                return true;
            }

            return false;
        }

        public void SetScore(int score)
        {
            if (score < 0)
                return;

            if (_score == score)
                return;

            _score = score;

            OnScoreChanged?.Invoke(_score);
        }

        public void SetBestScore(int bestScore)
        {
            if (bestScore < 0)
                return;

            if (_score == bestScore) return;

            _bestScore = bestScore;
            OnBestScoreChanged?.Invoke(_bestScore);
        }

        public void SetTargetChip(int chip)
        {
            if (chip < 0)
                return;

            if (_targetChip == chip)
                return;

            _targetChip = chip;
            OnTargetChipChanged?.Invoke(_targetChip);
        }

        public void SetLevel(int level)
        {
            if (level < 0)
                return;

            if (_level == level)
                return;

            _level = level;
            OnLevelChanged?.Invoke(_level);
        }

        private void SetTargetScore(int score)
        {

            if (score < 0)
                return;

            if (_targetScore == score)
                return;

            _targetScore = score;

            OnTargetScoreChanged?.Invoke(_targetScore);
        }

    }
}