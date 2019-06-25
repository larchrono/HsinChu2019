using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    public PostProcessVolume volume;

    [Header("恢復原圖時間")]
    [Range(0.1f,8)]
    public float ResumeTime = 3;

    [Header("光線強度")]
    [Range(1,40)]
    public float bloomIntensity = 20;

    [Header("模糊強度")]
    [Range(0.1f,2)]
    public float maxBlur = 0.1f;

    [Range(0.01f, Mathf.PI*2)]
    public float periodBlur = 3.14f;

    [Header("音效影響力")]
    [Range(0,3)]
    public float specAmpu = 1;

    [Header("互動距離")]
    [Range(0.5f,5)]
    public float MagnetRange = 2;

    [Header("動畫速度")]
    [Range(0,2)]
    public float MoveSpeed = 0.1f;
    [Range(0,2)]
    public float RotateSpeed = 0.1f;

    [Space(20)]

    [Header("Art1 相關")]
    public float _randMin = 0.2f;
    public float _randMAX = 1.5f;

    [Header("Art3 相關")]
    [Range(1,100)]
    public float flowerFallenSpeed;
    [Range(1,100)]
    public float flowerRotateSpeed;
    [Range(0.1f,5)]
    public float flowerMagnetRange;
    [Range(0,20)]
    public float flowerScalePeriod;

    [Header("Art5 相關")]
    [Range(0.1f,1.5f)]
    public float paperPunchThreshold = 1.4f;
    [Range(0.7f,0.99f)]
    public float paperPunchResume = 0.95f;
    [Range(0,100)]
    public float paperPunchAmp = 1;

    [Header("Art7 相關")]
    public GameObject PaintCanvas;
    [Range(1,12)]
    public int routerNum = 3;
    [Range(0,3)]
    public float robotRadius = 2;
    [Range(3,60)]
    public float fadeOutTime = 15f;
    [Range(0.1f,2)]
    public float TriangleCreateCD = 1f;

    [Header("執行中作品")]

    public int workingArtwork;
    [Range(0,1)]
    public List<float> transparentArt;


    float[] spec = new float[128];
    [HideInInspector]
    public bool Moving;
    Bloom bloomLayer = null;
    DepthOfField depthOfFieldLayer = null;

    private void Awake() {
        instance = this;
    }
    void Start(){
        volume.profile.TryGetSettings(out bloomLayer);
        volume.profile.TryGetSettings(out depthOfFieldLayer);

        Art7Init();

        Moving = true;
        //workingArtwork = 1;
    }
    void Update() {
        
        AudioListener.GetSpectrumData(spec, 0, FFTWindow.Rectangular);
        int id = 1;

        if(workingArtwork == 0)
        {
            //Voice Fraquence
            //foreach (var item in Artwork1)
            //{
            //    float amp = spec[id % 5] * specAmpu;
            //    item.SetDynamicScale(amp);
            //    id++;
            //}
        }
        else if(workingArtwork == 1)
        {
            //Voice Fraquence
            //foreach (var item in Artwork2)
            //{
            //    float amp = spec[id % 5] * specAmpu;
            //    item.SetDynamicScale(amp);
            //    id++;
            //}
        }
        else if(workingArtwork == 4)
        {
            foreach (var item in Artwork5)
            {
                //Voice Punch
                //float amp = spec[id % 64] * specAmpu;
                //item.PunchPaper(amp);
                id++;
            }
        }
        else if(workingArtwork == 5)
        {
            foreach (var item in Artwork6)
            {
                //Voice Punch
                //float amp = spec[id % 64] * specAmpu;
                //item.PunchPaper(amp);
                id++;
            }
        }
        else if(workingArtwork == 6)
        {
            Art7Works();
        }

        //Basic Post Processing
        //bloomLayer.intensity.value = bloomIntensity * (1 + useVolue*2);
        //depthOfFieldLayer.focusDistance.value = 3 - useVolue.Remap(0,1,0,4.9f);

        //Dynamic Post Processing
        bloomLayer.intensity.value = bloomIntensity * 0.5f + bloomIntensity * Mathf.Cos(Time.time/2) * 0.5f;
        depthOfFieldLayer.focusDistance.value = maxBlur + 2.5f + 2.5f * Mathf.Sin(Time.time/periodBlur);
    }

    public void SwitchToArt(int artId){
        workingArtwork = artId;
        for(int i = 0; i < transparentArt.Count; i++){
            if(i == workingArtwork){
                transparentArt[i] = 1;
            }
            else {
                transparentArt[i] = 0;
            }
        }
    }

    [Sirenix.OdinInspector.Button]
    public void StartFlow(){

        Moving = true;

        DOTween.To(()=> volume.weight, x => volume.weight = x, 1, ResumeTime);
        //DOTween.To(()=> bloomLayer.intensity.value, x => bloomLayer.intensity.value = x, bloomIntensity, ResumeTime);
        //DOTween.To(()=> depthOfFieldLayer.focusDistance.value, x => depthOfFieldLayer.focusDistance.value = x, focusDistance, ResumeTime);

    }

    [Sirenix.OdinInspector.Button]
    public void StopFlow(){

        Moving = false;

        DOTween.To(()=> volume.weight, x => volume.weight = x, 0, ResumeTime);
        //DOTween.To(()=> bloomLayer.intensity.value, x => bloomLayer.intensity.value = x, 0, ResumeTime);
        //DOTween.To(()=> depthOfFieldLayer.focusDistance.value, x => depthOfFieldLayer.focusDistance.value = x, 5, ResumeTime);
    }

    public List<PiecesObject> Artwork1;
    public List<PiecesObject> Artwork2;
    public List<PaperWork> Artwork5;
    public List<PaperWork> Artwork6;
    public List<TriangleWork> Artwork7;


    /////Art works

    
    List<Robot> robots;
    float max_x = 7;
    float max_y = 5;



    float remainTime;
    float speed = 1;

    int runtimeRobotNum;
    


    void Art7Init(){
        robots = new List<Robot>();
        for(int i=0 ; i < routerNum ; i++){
            Robot tempRobot = new Robot();
            tempRobot.pos = new Vector3(Random.Range(-max_x, max_x),Random.Range(-max_y, max_y), 0);
            robots.Add(tempRobot);
        }
        
        runtimeRobotNum = routerNum;
    }
    
    void Art7Works()
    {
        if(Moving == false)
        {
            return;
        }

        //Handle for Direction
        for(int i=0; i< runtimeRobotNum ; i++){

            robots[i].remainTime -= Time.deltaTime;
            robots[i].createCD -= Time.deltaTime;

            if(robots[i].remainTime < 0){
                robots[i].remainTime = 3f;

                robots[i].toDirection = new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), 0);
            }

            if(robots[i].createCD > 0 )
                continue;

            //Handle for Move
            robots[i].pos = robots[i].pos + robots[i].toDirection * speed;

            if(robots[i].pos.x < -max_x)
                robots[i].toDirection = new Vector3(Mathf.Abs(robots[i].toDirection.x), robots[i].toDirection.y, 0);
            if(robots[i].pos.x > max_x)
                robots[i].toDirection = new Vector3(-Mathf.Abs(robots[i].toDirection.x), robots[i].toDirection.y, 0);
            if(robots[i].pos.y < -max_y)
                robots[i].toDirection = new Vector3(robots[i].toDirection.x, Mathf.Abs(robots[i].toDirection.y), 0);
            if(robots[i].pos.y > max_y)
                robots[i].toDirection = new Vector3(robots[i].toDirection.x, -Mathf.Abs(robots[i].toDirection.y), 0);

            float _randomAngle = Random.Range(0, 6.28f);
            Vector3 targetPos = robots[i].pos + new Vector3(robotRadius * Mathf.Cos(_randomAngle), robotRadius * Mathf.Sin(_randomAngle), 0 );

            float _randomRotate = Random.Range(0, 360f);
            Quaternion q = Quaternion.Euler(0, 0, _randomRotate);

            float _randomColor = Random.Range(0.1f, 1);

            float _randomFadinTime = Random.Range(0.2f, 3);


            int triangleType = Random.Range(0, Artwork7.Count);

            TriangleWork tempTriangle = Instantiate(Artwork7[triangleType], targetPos, q, PaintCanvas.transform);
            tempTriangle.SetToRandomScale();
            SpriteRenderer spr = tempTriangle.GetComponent<SpriteRenderer>();
            spr.sortingOrder = 1;

            spr.color = new Color(1, 1, 1, 0);
            spr.DOColor(new Color(1, 1, 1, _randomColor), _randomFadinTime).SetEase(Ease.Linear).OnComplete( delegate {
                
                tempTriangle.selfTween = spr.DOColor(new Color(1, 1, 1, 0), fadeOutTime).OnComplete(delegate {
                    Destroy(spr.gameObject);
                });

            });

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //cube.transform.position = targetPos;
            //Destroy(cube,5f);

            robots[i].createCD = TriangleCreateCD;

        }
    }
}

public class Robot
{
    public Vector3 pos;
    public Vector3 toDirection;
    public float remainTime;

    public float createCD;

    public Robot() {
        remainTime = 0f;
        createCD = 0f;
    }
}


public static class ExtensionMethods {
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}