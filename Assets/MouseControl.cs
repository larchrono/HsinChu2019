using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public static MouseControl instance;
    public Camera SceneCamera;

    public Vector2 MousePos = Vector2.zero;

    void Awake() {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        var screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; //distance of the plane from the camera
        //transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        MousePos = SceneCamera.ScreenToWorldPoint(screenPoint);
    }
}
