using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreeWork : MonoBehaviour
{
    Vector3 _savePos = Vector3.zero;
    Quaternion _saveRotation = Quaternion.identity;
    Vector3 _saveScale = Vector3.zero;

    public int ArtworkID;

    public TreeType thisType;
    bool Moving = true;
    public float LerpSpeed = 0.05f;

    public float _randomRadius;
    public float _randomSpeed;

    public float _randomRadius2;
    public float _randomSpeed2;

    public float _randomRadius3;
    public float _randomSpeed3;

    public float _randomRotate; 
    public float _randomScale;

    float _randFade;

    Vector3 targetScale;

    float elapsedTimeMove = 0;
    float elapsedTimeRotate = 0;

    float resumeElapse = 0;
    Vector3 _resumeStartPos;
    Quaternion _resumeStartRotation;
    Color _resumeStartColor;
    float rotateDir;


    SpriteRenderer spr;
    Rigidbody2D rg2d;

    float lifetime = 0;
    bool fallen = false;

    float nodeFadeFactor = 0;
    float scaleGrouth = 2;
    float grouthTime = 3;
    float nowAlpha = 1;

    public enum TreeType
    {
        Flower,
        Flying,
        Node,
        Base,
    }
    

    void Awake(){
        _savePos = transform.localPosition;
        _saveRotation = transform.localRotation;
        _saveScale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        _randomRadius = Random.Range(ObjectManager.instance._randMin, ObjectManager.instance._randMAX);
        _randomSpeed = Random.Range(ObjectManager.instance._randMin, ObjectManager.instance._randMAX);

        _randomRadius2 = Random.Range(ObjectManager.instance._randMin, ObjectManager.instance._randMAX);
        _randomSpeed2 = Random.Range(ObjectManager.instance._randMin, ObjectManager.instance._randMAX);

        _randomRadius3 = Random.Range(ObjectManager.instance._randMin, ObjectManager.instance._randMAX);
        _randomSpeed3 = Random.Range(ObjectManager.instance._randMin,ObjectManager.instance._randMAX);

        _randomRotate = Random.Range(0.5f,2f);
        _randomScale = Random.Range(0f,0.2f);

        _randFade = Random.Range(0, 360);

        rotateDir = Random.Range(0, 100) > 50 ? 1 : -1;

        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(1, 1, 1, 0);
        //spr.sortingOrder = Random.Range(-10,10);

        rg2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Moving = ObjectManager.instance.Moving;

        float moveSpeed = ObjectManager.instance.MoveSpeed;
        float rotateSpeed = ObjectManager.instance.RotateSpeed;

        elapsedTimeMove = elapsedTimeMove + Time.deltaTime * moveSpeed;
        elapsedTimeRotate = elapsedTimeRotate + Time.deltaTime * rotateSpeed;

        float transparentArt = ObjectManager.instance.transparentArt[ArtworkID];

        if(Moving){

            /*
            //Move Area , if user in , no move
            if(Vector3.Distance(transform.position, MouseControl.instance.MousePos) > ObjectManager.instance.MagnetRange){
                Vector3 goal = new Vector3(_savePos.x + Mathf.Sin(elapsedTimeMove * _randomSpeed3 + _randomSpeed ) * Mathf.Cos(elapsedTimeMove * _randomSpeed ) * _randomRadius + 
                                                        Mathf.Cos(elapsedTimeMove * _randomSpeed2 )*_randomRadius2 + 
                                                        Mathf.Cos(elapsedTimeMove * _randomSpeed3 )*_randomRadius3,

                                            _savePos.y + Mathf.Cos(elapsedTimeMove * _randomSpeed3 + _randomSpeed ) * Mathf.Sin(elapsedTimeMove * _randomSpeed ) * _randomRadius + 
                                                        Mathf.Sin(elapsedTimeMove * _randomSpeed2 )*_randomRadius2 + 
                                                        Mathf.Sin(elapsedTimeMove * _randomSpeed3 )*_randomRadius3,
                                            0);

                transform.localPosition = Vector3.Lerp(transform.localPosition, goal, LerpSpeed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, MouseControl.instance.MousePos, LerpSpeed);
            }
            */
            if(thisType == TreeType.Flower)
            {
                if(fallen == false){
                    foreach (var m_pos in MouseControl.instance.MousePos)
                    {
                        if(Vector3.Distance(transform.position, m_pos) < ObjectManager.instance.flowerMagnetRange){
                            fallen = true;
                            rg2d.bodyType = RigidbodyType2D.Dynamic;
                            break;
                        }
                    }
                }

                if(fallen)
                {
                    //transform.localPosition = transform.localPosition + new Vector3(0, -ObjectManager.instance.flowerFallenSpeed * Time.deltaTime, 0);
                    lifetime += Time.deltaTime;
                }
                if(lifetime > 7f){
                    lifetime = 0;

                    spr.DOColor(new Color(1, 1, 1, 0), 1f).OnComplete( () => {
                        rg2d.bodyType = RigidbodyType2D.Static;
                        transform.localPosition = _savePos;
                        fallen = false;
                        lifetime = 0;
                    });
                }

                //Rotate Area
                float f = (elapsedTimeRotate * ObjectManager.instance.flowerRotateSpeed * _randomRotate + _randomRotate * 360) % 360;
                Quaternion r = Quaternion.Euler(0, 0, rotateDir * f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, r,LerpSpeed);

                //Scale Area
                float targetScale = 1 + _randomScale * Mathf.Sin(Time.time * (_randomScale * ObjectManager.instance.flowerScalePeriod));
                transform.localScale = Vector3.Lerp(transform.localScale, _saveScale * targetScale, LerpSpeed);
            }
            else if(thisType == TreeType.Flying)
            {
                //float f = (elapsedTimeRotate * ObjectManager.instance.flowerRotateSpeed * _randomRotate + _randomRotate * 360) % 360;
                float f = 15 * Mathf.Cos(elapsedTimeRotate * _randomRotate + _randomRotate * 360 );
                Quaternion r = Quaternion.Euler(0, 0, rotateDir * f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, r,LerpSpeed);

                //Scale Area
                float targetScale = 1 + _randomScale * 2 * Mathf.Sin(Time.time * (_randomScale * ObjectManager.instance.flowerScalePeriod));
                transform.localScale = Vector3.Lerp(transform.localScale, _saveScale * targetScale, LerpSpeed);
            }
            else if(thisType == TreeType.Node)
            {
                //float f = (elapsedTimeRotate * ObjectManager.instance.flowerRotateSpeed * _randomRotate + _randomRotate * 360) % 360;
                float f = 15 * Mathf.Cos(elapsedTimeRotate * _randomRotate + _randomRotate * 360 );
                Quaternion r = Quaternion.Euler(0, 0, rotateDir * f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, r,LerpSpeed);

                //Scale Node
                if(nodeFadeFactor == 0){
                    transform.localScale = Vector3.zero;
                    Tweener grouth = transform.DOScale(_saveScale * 1.5f, grouthTime);
                }

                nodeFadeFactor += Time.deltaTime;
                
                nowAlpha = Mathf.Max(0, (grouthTime - nodeFadeFactor)/grouthTime);

                if(nodeFadeFactor > grouthTime){
                    nodeFadeFactor = 0;
                    transform.localScale = _saveScale;
                }

                float targetScale = 1 + _randomScale * 2 * Mathf.Sin(Time.time * (_randomScale * ObjectManager.instance.flowerScalePeriod));
                transform.localScale = Vector3.Lerp(transform.localScale, _saveScale * targetScale, LerpSpeed);
            }
            

            //Fade Area
            Color t_color = new Color(1, 1, 1, nowAlpha * transparentArt);
            spr.color = Color.Lerp(spr.color , t_color, LerpSpeed);
        }
        else {

            spr.color = Color.Lerp(spr.color , new Color(1, 1, 1, 0), LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

        }

        //Debug.Log(spr.color);
    }

    //Scale Area
    public void SetDynamicScale(float scale){

        if(_saveScale == Vector3.zero)
            return;

        if(Moving){
            transform.localScale = _saveScale * (1 + scale );
        } else {
            transform.localScale = _saveScale;
        }
    }

    [ContextMenu("Setup Connected Anchor")]
    public void SetupSpringPos(){
        SpringJoint2D joint = GetComponent<SpringJoint2D>();
        //Debug.Log(joint);
        if(joint != null){
            joint.connectedAnchor = transform.position;
        }
    }

    [ContextMenu("Reset Position")]
    public void ResetPosition(){
        transform.localPosition = _savePos;
        transform.localRotation = _saveRotation;
        transform.localScale = _saveScale;
    }
}
