using System;
using System.Collections.Generic;
using Assets.markins._2048.Runtime.Game.Fabrics;
using Assets.markins._2048.Runtime.Game.Services;
using Markins.Runtime.Game.Storage.Models;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.Controllers
{
    public class ChipsManager
    {
        private IChipFabric _chipFabric;
        private IPowerGenerator _powerGenerator;

        private readonly ChipCollection _chipCollection;
        public readonly List<ChipController> Chips = new();
        private readonly Transform _chipsParent;
        private string _chipName;

        public Action<ChipController, ChipController, Vector3> OnMatchChips;

        public ChipsManager(ChipCollection chipCollection, Transform chipsParent)
        {
            _chipCollection = chipCollection;
            _chipsParent = chipsParent;
            _chipFabric = new ChipFabric(_chipCollection);
            _powerGenerator = new PowerGenerator();
            Signals.Get<OnSelectChipSkinChangedSignal>().AddListener(ChipChangeName);
        }

        private void ChipChangeName(string chipName)
        {
            _chipName = chipName;
            Debug.Log("Change Name Chip" + chipName);
        }

        public int GetRandomChipPower()
        {
            var number = _powerGenerator.GetRandomPower();
            return number;
        }

        public void CreateRandomChip(Vector3 randomPosition)
        {
            var randomPower = GetRandomChipPower();
            var chip = _chipFabric.CreateChip(_chipName,randomPower, randomPosition, _chipsParent);
            chip.Spawn();
            AddChip(chip);
        }

        public ChipController CreateRandomChip2(Vector3 randomPosition)
        {
            var randomPower = _powerGenerator.GetRandomPower();
            var chip = _chipFabric.CreateChip(_chipName, randomPower, randomPosition, _chipsParent);
            chip.Spawn();
            AddChip(chip);
            return chip;
        }

        private void ChipCollisionHandler(ChipController first, ChipController second, Vector3 positionCollision)
        {
            OnMatchChips?.Invoke(first, second, positionCollision);
        }

        public void CreateChip(int currentPower, Vector3 position)
        {
            var chip = _chipFabric.CreateChip(_chipName,currentPower, position, _chipsParent);
            chip.Spawn();
            AddChip(chip);
        }

     
        public void CreateNextChip(int currentPower, Vector3 position)
        {
            var nextPower = currentPower += 1;
            var chip = _chipFabric.CreateChip(_chipName,nextPower, position, _chipsParent);
            chip.Spawn();
            AddChip(chip);
        }

        private void AddChip(ChipController chip)
        {
            FieldController.instance.AddChipHandler(chip);
            chip.ChipCollideInPosition += ChipCollisionHandler;
            Chips.Add(chip);
        }

        public void RemoveChip(ChipController chip)
        {
            FieldController.instance.RemoveChipHandler(chip);
            chip.ChipCollideInPosition -= ChipCollisionHandler;
            chip.Free();
            Chips.Remove(chip);
        }

        public void DeleteAllChips()
        {
            foreach (var chip in Chips)
            {
                chip.Free();
                chip.Destroy();
            }

            Chips.Clear();
        }
    }
}