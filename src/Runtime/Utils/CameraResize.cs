using System;
using UnityEngine;

namespace Markins.Runtime.Game.Utils
{
public class CameraResize : MonoBehaviour
{
    [SerializeField]
    private Vector2 DefaultResolution = new Vector2(640, 900);

    [SerializeField]
    private bool DefaultResolutionFromCameraSize = true;

    [Range(0f, 1f)]
    [SerializeField]
    private float WidthHeightMatch = 0f;

    private Camera _camera;

    /// <summary> Orthographic Size At Initialization. </summary>
    private float _initialSize;
    /// <summary> Field Of View Value At Initialization. </summary>
    private float _initialFov;

    /// <summary> X / Y. </summary>
    private float _ratio;
    /// <summary> For perspective mode. </summary>
    private float _horizontalFov;

    private void Start()
    {
        if(DefaultResolutionFromCameraSize)
            DefaultResolution = new Vector2(Screen.width, Screen.height);

        _camera = GetComponent<Camera>();

        _initialSize = _camera.orthographicSize;
        _initialFov = _camera.fieldOfView;
        _ratio = DefaultResolution.x / DefaultResolution.y;

        _horizontalFov = CalculationVerticalFov(_initialFov, 1 / _ratio);
    }
    private float CalculationVerticalFov(float HorizontalFov, float Ratio)
    {
        float side_ratio = Ratio;
        float HorizontalFovInRads = HorizontalFov * Mathf.Deg2Rad;

        float VerticalFovInRads = 2 * Mathf.Atan(Mathf.Tan(HorizontalFovInRads / 2) / side_ratio);

        return VerticalFovInRads * Mathf.Rad2Deg;
    }

    private void ScreenResize() {
        if (_camera.orthographic)
        {
            float WidthSize = _initialSize * (_ratio / _camera.aspect);
            _camera.orthographicSize = Mathf.Lerp(WidthSize, _initialSize, WidthHeightMatch);
        }
        else
        {
            float WidthFov = CalculationVerticalFov(_horizontalFov, _camera.aspect);
            _camera.fieldOfView = Mathf.Lerp(WidthFov, _initialFov, WidthHeightMatch);
        }
    }

    public void Update() => ScreenResize();

}

}
