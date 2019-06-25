using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public static MouseControl instance;
    public Camera SceneCamera;

    //For mouse , single user
    //public Vector2 MousePos = Vector2.zero;
    public Queue<Vector3> MousePos = new Queue<Vector3>();
    public int maxPointStay = 25;
    [Header("若時間內的Queue 小於此值，表示沒人在動，因此Dequeue全部")]
    [Range(1,25)]
    public int DequeueTheshold = 3;

    [Header("除錯檢視用,目前的Queue值")]
    public int NowQueueNumber = 0;

    
    float dequeuePeriod = 5f;       //每次Dequeue 間隔
    int queueNumberInTime = 0;      //此次 Queue 數量
    float remainTime = 0;           //程式用，距離下次Queue時間

    void Awake() {
        instance = this;
        remainTime = dequeuePeriod;
        queueNumberInTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse potision , single user
        //var screenPoint = Input.mousePosition;
        //screenPoint.z = 10.0f; //distance of the plane from the camera
        //MousePos = SceneCamera.ScreenToWorldPoint(screenPoint);

        NowQueueNumber = MousePos.Count;

        //Use Queue to save point for detect use
        foreach (var item in PositionManager.instance.DetectResult)
        {
            MousePos.Enqueue(item);
            queueNumberInTime++;

            if(MousePos.Count > maxPointStay){
                MousePos.Dequeue();
            }
        }

        remainTime -= Time.deltaTime;

        if(remainTime <= 0){
            remainTime = dequeuePeriod;
            //計算每個時間間隔內所 Queue的數量
            queueNumberInTime = Mathf.Min(queueNumberInTime, maxPointStay);

            //如果該時間內Queue的數量過少，表示沒有人在動，因此Dequeue所有 Queue
            if(queueNumberInTime < DequeueTheshold){
                for(int i=0 ; i < maxPointStay ; i++){
                    if(MousePos != null && MousePos.Count > 0)
                        MousePos.Dequeue();
                }
            }
            queueNumberInTime = 0;
        }
    }
}
