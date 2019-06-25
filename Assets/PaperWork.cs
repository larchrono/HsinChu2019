using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperWork : MonoBehaviour
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

    float storePunch = 2;

    SpriteRenderer spr;
    Rigidbody rigid;

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

        rigid = GetComponent<Rigidbody>();
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

        storePunch = storePunch * ObjectManager.instance.paperPunchResume;

        if(Moving){

            rigid.isKinematic = false;

            if(MouseControl.instance.MousePos.Count > 0){
                foreach (var m_pos in MouseControl.instance.MousePos)
                {
                    //Move Area , if user in , no move
                    if(Vector3.Distance(transform.position, m_pos) < ObjectManager.instance.MagnetRange){
                        rigid.isKinematic = true;
                        transform.localPosition = Vector3.Lerp(transform.localPosition, _savePos + new Vector3(0, 0, 2) , LerpSpeed);
                        break;
                    }
                    else
                    {
                        rigid.isKinematic = false;
                    }
                }
            }
            else
            {
                rigid.isKinematic = false;
            }

            spr.color = Color.Lerp(spr.color , new Color(1, 1, 1, transparentArt), LerpSpeed );

        }
        else {

            rigid.isKinematic = true;

            transform.localPosition = Vector3.Lerp(transform.localPosition, _savePos, LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            transform.localRotation = Quaternion.Lerp(transform.localRotation, _saveRotation, LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            spr.color = Color.Lerp(spr.color , new Color(1, 1, 1, transparentArt), LerpSpeed / (ObjectManager.instance.ResumeTime * 0.5f));

            //transform.localScale = Vector3.Lerp(transform.position, _saveScale, LerpSpeed);
        }
    }

    //Scale Area
    public void PunchPaper(float punch){
        //Debug.Log("Punch:" + punch / storePunch);
        if((punch / storePunch) > ObjectManager.instance.paperPunchThreshold){
            float force = punch / storePunch;
            storePunch = punch;

            rigid.AddForce(new Vector3(0, 0, -force * ObjectManager.instance.paperPunchAmp));
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
