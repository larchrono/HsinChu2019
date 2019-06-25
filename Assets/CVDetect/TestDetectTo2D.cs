using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDetectTo2D : MonoBehaviour
{
    public GameObject Prefab_pointObject;
    public Text debugMsg;

    // Update is called once per frame
    void Update()
    {
        if(debugMsg != null){
            debugMsg.text = "Total Point: " + PositionManager.instance.DetectResult.Count;
        }
        foreach (var item in PositionManager.instance.DetectResult)
        {
            GameObject temp = Instantiate(Prefab_pointObject,item,Quaternion.identity);
            Destroy(temp,1);
        }
    }
}
