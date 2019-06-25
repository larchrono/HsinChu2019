using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesObject : MonoBehaviour
{
    Vector3 _savePos = Vector3.zero;
    Quaternion _saveRotation = Quaternion.identity;
    Vector3 _saveScale = Vector3.zero;

    public int ArtworkID;
    bool Moving = true;
    public float LerpSpeed = 0.05f;

    public float _randomRadius;
    public float _randomSpeed;

    public float _randomRadius2;
    public float _randomSpeed2;

    public float _randomRadius3;
    public float _randomSpeed3;

    public float _randomRotate;

    float _randFade;

    Vector3 targetScale;

    float elapsedTimeMove = 0;
    float elapsedTimeRotate = 0;

    float resumeElapse = 0;
    Vector3 _resumeStartPos;
    Quaternion _resumeStartRotation;
    Color _resumeStartColor;

    SpriteRenderer spr;

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

        _randomRotate = Random.Range(0f,1f);

        _randFade = Random.Range(0, 360);

        spr = GetComponent<SpriteRenderer>();
        spr.sortingOrder = Random.Range(-10,10);
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

            if(MouseControl.instance.MousePos.Count > 0){
                foreach (var m_pos in MouseControl.instance.MousePos)
                {
                    //Move Area , if user in , no move
                    if(Vector3.Distance(transform.position, m_pos) > ObjectManager.instance.MagnetRange){
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
                        transform.position = Vector3.Lerp(transform.position, m_pos, LerpSpeed);
                        break;
                    }
                }
            }
            else
            {
                Vector3 goal = new Vector3(_savePos.x + Mathf.Sin(elapsedTimeMove * _randomSpeed3 + _randomSpeed ) * Mathf.Cos(elapsedTimeMove * _randomSpeed ) * _randomRadius + 
                                                        Mathf.Cos(elapsedTimeMove * _randomSpeed2 )*_randomRadius2 + 
                                                        Mathf.Cos(elapsedTimeMove * _randomSpeed3 )*_randomRadius3,

                                            _savePos.y + Mathf.Cos(elapsedTimeMove * _randomSpeed3 + _randomSpeed ) * Mathf.Sin(elapsedTimeMove * _randomSpeed ) * _randomRadius + 
                                                        Mathf.Sin(elapsedTimeMove * _randomSpeed2 )*_randomRadius2 + 
                                                        Mathf.Sin(elapsedTimeMove * _randomSpeed3 )*_randomRadius3,
                                            0);

                transform.localPosition = Vector3.Lerp(transform.localPosition, goal, LerpSpeed);
            }
            

            //Rotate Area
            float f = 360 * _randomRotate  * Mathf.Cos(elapsedTimeRotate * _randomRotate + _randomRotate * 360 );
            Quaternion r = Quaternion.Euler(0, 0, f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, r,LerpSpeed);

            //Fade Area
            //Color t_color = new Color(1, 1, 1, (0.7f + 0.3f * Mathf.Cos(Time.time/2 + _randFade)) * transparentArt);  //Dynamic Alpha
            Color t_color = new Color(1, 1, 1, transparentArt);                                                         //Static Alpha

            spr.color = Color.Lerp(spr.color , t_color, LerpSpeed);
        }
        else {

            resumeElapse += Time.deltaTime / ObjectManager.instance.ResumeTime;

            transform.localPosition = Vector3.Lerp(transform.localPosition, _savePos, LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            transform.localRotation = Quaternion.Lerp(transform.localRotation, _saveRotation, LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            spr.color = Color.Lerp(spr.color , new Color(1, 1, 1, transparentArt), LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            //transform.localScale = Vector3.Lerp(transform.position, _saveScale, LerpSpeed);
        }
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

    public void StartResume(){
        Moving = false;
        //resumeElapse = 0;

        //_resumeStartPos = transform.localPosition;
        //_resumeStartRotation = transform.localRotation;
        //_resumeStartColor = spr.color;
    }

    [ContextMenu("Reset Position")]
    public void ResetPosition(){
        transform.localPosition = _savePos;
        transform.localRotation = _saveRotation;
        transform.localScale = _saveScale;
    }
}
