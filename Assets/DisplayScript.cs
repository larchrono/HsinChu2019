using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScript : MonoBehaviour
{
    public int displayMode = 0;

    public Camera CameraMonitor;
    public Camera CameraScene1;
    public Camera CameraScene2;

    // Use this for initialization
    void Start()
    {
        displayMode = PlayerPrefs.GetInt("DisplayMode", 0);

        SwitchDisplayMode(displayMode);

        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();

        
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.F5)){
            displayMode = 0;
            SaveDisplayKey();
            SwitchDisplayMode(displayMode);
        }
        if(Input.GetKeyDown(KeyCode.F6)){
            displayMode = 1;
            SaveDisplayKey();
            SwitchDisplayMode(displayMode);
        }
    }

    public void SwitchDisplayMode(int mode){
        if(mode == 0){
            CameraMonitor.targetDisplay = 2;
            CameraScene1.targetDisplay = 0;
            CameraScene2.targetDisplay = 1;
        }
        else if(mode == 1)
        {
            CameraMonitor.targetDisplay = 0;
            CameraScene1.targetDisplay = 1;
            CameraScene2.targetDisplay = 2;
        }
    }

    void SaveDisplayKey(){
        PlayerPrefs.SetInt("DisplayMode",displayMode);
    }
}
