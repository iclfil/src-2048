using UnityEngine;
using TMPro;

namespace Markins.Runtime.Game
{
    public class ChipView : MonoBehaviour
    {
        public MeshRenderer BaseMesh;
        public TextMeshPro NumberText;
        public TrailRenderer Trail;
        public Transform Aim;
        public string NameColorFieldFromShader = "_Color";
        private MaterialPropertyBlock _propertyBlock;

        public void Init(MaterialPropertyBlock propertyBlock)
        {
            _propertyBlock = propertyBlock;
            BaseMesh.SetPropertyBlock(_propertyBlock);
        }

        public void SetColor(Color color)
        {
            BaseMesh.GetPropertyBlock(_propertyBlock); // Get previously set values. They will reset otherwise
            _propertyBlock.SetColor(NameColorFieldFromShader, color);
            BaseMesh.SetPropertyBlock(_propertyBlock);
           // BaseMesh.sharedMaterial.SetColor(NameColorFieldFromShader, color);
           // _propertyBlock.SetColor(NameColorFieldFromShader, color);
        }

        public void SetSymbol(string symbol)
        {
            NumberText.text = symbol;
        }
    }
}


//private IEnumerator MergeEffect(NumberData._data data)
//{
//    yield return new WaitForSeconds(0.10f);
//    _mergeFx.Play();
//    transform.localScale = new Vector3(data.sizeNumber - 0.5f, data.sizeNumber - 0.5f, data.sizeNumber - 0.5f);
//    InitColorAndNumber(data);
//    yield return new WaitForSeconds(0.6f);
//    PucnhScale(data);
//}

//private void PucnhScale(NumberData._data data)
//{
//    transform.DOScale(data.sizeNumber, _gameConfig.timeToScaleMergePiece);
//}



//public void OnPointerDown(PointerEventData eventData)
//{
//    isPress = true;
//    _aimRoot.gameObject.SetActive(true);
//    Bouncing = 0;
//    _startMousePos = transform.position;
//    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13));
//}
//public void OnPointerUp(PointerEventData eventData)
//{
//    if (!isPress)
//        return;

//    _endMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13));

//    Vector3 delta = _startMousePos - _endMousePos;
//    IsPlayerShoot = true;
//    isPress = false;
//    _aimRoot.gameObject.SetActive(false);
//    ShootPiece(delta);
//}
