using Sirenix.Serialization;
using UnityEngine;

namespace Assets.markins._2048.Runtime.Configs
{
    [CreateAssetMenu(fileName = "ChipConfig", menuName = "Game/Field/Create ChipConfig")]
    public class ChipConfig : ScriptableObject
    {
        [SerializeField] private string _name;
        [Header("Это степень двойки.")]
        [SerializeField] private int _power;
        [SerializeField] private int _numberOfPower;
        [SerializeField]
        [OdinSerialize] private Vector3 _size;
        [SerializeField] private Color _color;

        public string Name => _name;
        public int Power => _power;
        public int NumberOfPower => _numberOfPower;

        public Vector3 Size => _size;
        public Color Color => _color;

        public void SetName(string nameConfig)
        {
            _name = nameConfig;
        }
        public void SetPower(int power)
        {
            _power = power;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetNumberOfPower(float pow)
        {
            _numberOfPower = (int)pow;
        }

        public void SetSize(Vector3 size)
        {
            _size = size;
        }
    }
}