using Assets.markins._2048.Runtime.Game.Services;
using Markins.Runtime.Game.Controllers;
using UnityEngine;

namespace Assets.markins._2048.Runtime.Game.Fabrics
{
    public interface IChipFabric
    {
        ChipController CreateChip(string nameChip, int power, Vector3 position, Transform parent);
    }

    public class ChipFabric : IChipFabric
    {
        private ChipCollection _chipCollection;
        private MaterialPropertyBlock _materialPropertyBlock;
        private IPowerToStringFormatterService _powerToStringFormatterService;

        public ChipFabric(ChipCollection config)
        {
            _chipCollection = config;
            _materialPropertyBlock = new MaterialPropertyBlock();
            _powerToStringFormatterService = new PowerToStringFormatterService();
        }

        public void Free()
        {

        }

        public ChipController CreateChip(string nameChip, int power, Vector3 position, Transform parent)
        {
            var config = _chipCollection.GetChipConfig(power);
            var chipControllerPrefab = _chipCollection.GetChipByName(nameChip);
            var chipViewPrefab = _chipCollection.GetChipViewByName(nameChip);
            var chipController = Object.Instantiate(chipControllerPrefab, parent);
            chipController.Init(config, _powerToStringFormatterService, _materialPropertyBlock, chipViewPrefab);
            chipController.transform.position = position;
            return chipController;
        }
    }
}