using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StageController : MonoBehaviour
{
    [Header("輸入數字鍵 0 ~ 6 切換場景，輸入 Insert 開啟動畫，Delete 關閉動畫")]
    [ReadOnly]
    public StageController instance;
    // Start is called before the first frame update
    void Start()
    {
        ObjectManager.instance.StopFlow();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad0)){
            ObjectManager.instance.SwitchToArt(0);
        }
        if(Input.GetKey(KeyCode.Keypad1)){
            ObjectManager.instance.SwitchToArt(1);
        }
        if(Input.GetKey(KeyCode.Keypad2)){
            ObjectManager.instance.SwitchToArt(2);
        }
        if(Input.GetKey(KeyCode.Keypad3)){
            ObjectManager.instance.SwitchToArt(3);
        }
        if(Input.GetKey(KeyCode.Keypad4)){
            ObjectManager.instance.SwitchToArt(4);
        }
        if(Input.GetKey(KeyCode.Keypad5)){
            ObjectManager.instance.SwitchToArt(5);
        }
        if(Input.GetKey(KeyCode.Keypad6)){
            ObjectManager.instance.SwitchToArt(6);
        }
        if(Input.GetKey(KeyCode.Insert)){
            ObjectManager.instance.StartFlow();
        }
        if(Input.GetKey(KeyCode.Delete)){
            ObjectManager.instance.StopFlow();
        }
    }
}
