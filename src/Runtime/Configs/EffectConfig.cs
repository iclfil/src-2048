using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectConfig", menuName = "Game/Effects/Create EffectConfig")]
public class EffectConfig : SerializedScriptableObject
{
    public string Name;
    public float LifeTime;
    public Vector3 SpawnOffset;
    public GameObject EffectPrefab;
    public bool AttachToObject = false;
}
