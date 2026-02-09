using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public Transform topLeftScenePoint;
    public Transform bottomRigthScenePoint;
    public float cameraDefaultSize;
    public Camera mainCamera;
    private void Awake()
    {
        float sceneWidth = bottomRigthScenePoint.position.x - topLeftScenePoint.position.x;
        float sceneHeight = topLeftScenePoint.position.y - bottomRigthScenePoint.position.y;
        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;
        //print($"sceneWidth={sceneWidth}");
        //print($"sceneHeight={sceneHeight}");
        //print($"camHeight={camHeight}");
        //print($"camWidth={camWidth}");
        if (camHeight < sceneHeight)
        {
            mainCamera.orthographicSize = cameraDefaultSize * camHeight / sceneHeight;
        }
        if (camWidth < sceneWidth)
        {
            mainCamera.orthographicSize = cameraDefaultSize * sceneWidth / camWidth;
        }
        
    }
}
