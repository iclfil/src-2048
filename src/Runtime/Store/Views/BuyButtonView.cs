using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButtonView : MonoBehaviour
{
    public Button button;
    [SerializeField]
    private TextMeshProUGUI TxtPrice;
    [SerializeField]
    private Image spriteCrystall;

    public Color disableColor = Color.white;

    private bool _enabled = true;
    private int _price = 0;

    public bool Enabled
    {
        get { return _enabled; }

        set
        {
            _enabled = value;
            button.interactable = value;

            if (button.interactable)
            {
                spriteCrystall.color = Color.white;
                TxtPrice.color = Color.white;
            }
            else
            {
                spriteCrystall.color = disableColor;
                TxtPrice.color = disableColor;
            }
        }
    }

    public int Price
    {
        get { return _price; }

        set
        {
            _price = value;
            TxtPrice.text = _price.ToString();
        }
    }
}
