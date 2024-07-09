using TMPro;
using UnityEngine;

namespace Markins.Runtime.Game.GUI.Views
{
    public class TargetSymbolView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _symbolText;

        public void SetNumber(string value)
        {
            _symbolText.text = value;
        }
    }
}

