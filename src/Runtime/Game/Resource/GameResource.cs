using System;
using UnityEngine;

namespace Markins.Runtime.Game.Storage.Resource
{
    public class GameResource
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private int _amount;
        [SerializeField] private string _name;

        public ResourceType Type => _type;
        public string Name => _name;
        public int Amount => _amount;

        public GameResource(ResourceType type, string name, int amount = 1)
        {
            _type = type;
            _amount = amount;
            _name = name;
        }

        public void IncomeResource(int amount)
        {
            _amount += amount;
        }

        public void DecomeResource(int amount)
        {
            _amount -= amount;
        }
    }
}