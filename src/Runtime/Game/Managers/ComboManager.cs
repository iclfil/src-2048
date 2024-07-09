using Markins.Runtime.Game;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.Feature
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] private float ComboDuration = 0.5f;

        private float _comboTimer = 0;
        private int _comboCount = 0;
        private bool _hasCombo = false;

        public void Awake()
        {
            Init();
        }
        public void Init()
        {
            Signals.Get<OnChipsMatchedSignal>().AddListener(ComboHandler);
        }
        private void Update()
        {
            if (_hasCombo == false)
            {
                return;
            }

            _comboTimer -= Time.deltaTime;

            if (_comboTimer < 0)
            {
                ComboTimerFinish();
            }
        }

        public void Free()
        {
            Signals.Get<OnChipsMatchedSignal>().RemoveListener(ComboHandler);
        }

        private void ComboHandler(Vector3 matchPosition)
        {
            _hasCombo = true;
            _comboCount++;
            _comboTimer = ComboDuration;

            if (_comboCount >= 2)
            {
                Signals.Get<OnComboCollectedSignal>().Dispatch(_comboCount, matchPosition);
            }

        }

        private void ComboTimerFinish()
        {
            _comboCount = 0;
            _comboTimer = 0;
            _hasCombo = false;
        }
    }
}