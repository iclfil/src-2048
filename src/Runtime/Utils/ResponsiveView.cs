using UnityEngine;

public class ResponsiveView : MonoBehaviour
{
    // Singelton instance
    private static ResponsiveView _instance;

    // Assign the main camera to be adjusted by this script 
    public new Camera camera;

    // Assign the object that you want to be always visible inside screen on all devices and resolutions
    public Transform playArea;

    // add padding on top of your play area
    public float PaddingAbovePlayArea = 1;

    // add padding between and under your play area
    public float PaddingUnderAndBetweenPlayArea = 1;

    // force the play area object to be sticked at bottom of screen. This also fixes some overflow issues
    public bool stickyPlayArea = false;

    // hide constuctor
    private ResponsiveView(){

    }

    // Access the singleton instance
    public static ResponsiveView instance(){
        if(_instance == null) _instance = new ResponsiveView();
        return _instance;
    }

    private void Start()
    {
        start();
    }

    // the main code should be run once at start of the scene,
    //but you can use Update() instead, if you want to debugg and see immedate changes
    void start()
    {
        // get the width of the play are object + padding
        float playAreaWidth = playArea.localScale.x + (PaddingUnderAndBetweenPlayArea * 2);

        // 1. scale the camera view to fit playArea height     
        Camera.main.orthographicSize = (playArea.localScale.y + PaddingAbovePlayArea) / 2;

        // 2. if the the play area is not completly visibile, then zoom the camera out to fit PlayArea width
        if (Camera.main.orthographicSize <= playAreaWidth)
            Camera.main.orthographicSize = playAreaWidth;

        // if set to true, move play area to bottom of screen if possible
        if (stickyPlayArea)
            CameraYRelativToSprite(-PaddingUnderAndBetweenPlayArea);
    }

    // this method tries to align camera and play area so they have the same minimum bottom boundry posistion
    public void CameraYRelativToSprite(float spriteBottomBoundry)
    {
        while (System.Math.Round(getCameraBottomBoundry(camera), 1) != System.Math.Round(spriteBottomBoundry, 1))
        {
            float camBottomBoundry = getCameraBottomBoundry(camera);
            if (camBottomBoundry > spriteBottomBoundry)
            {
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - 0.01f, -10);
            }
            else
            {
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + 0.01f, -10);
            }
        }
    }

    public static float getCameraBottomBoundry(Camera camera) //get the bounds's min y of camera... since there is no native method for it
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return (bounds.center - bounds.extents).y;
    }
}
