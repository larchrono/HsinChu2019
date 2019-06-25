using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriangleWork : MonoBehaviour
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

    public Tweener selfTween;

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
    }

    void Update()
    {
        float transparentArt = ObjectManager.instance.transparentArt[ArtworkID];

        //Fade Area
        Color t_color = new Color(1, 1, 1, transparentArt);
        spr.color = Color.Lerp(spr.color , t_color, LerpSpeed);

        if(transparentArt <= 0 && selfTween != null){
            selfTween.Kill();
            selfTween = null;
            spr.DOFade(0, 0.5f).OnComplete(delegate {
                    Destroy(spr.gameObject);
            });
        }
    }

    public void SetToRandomScale(){
        float amp = Random.Range(0.4f,1.4f);
        transform.localScale = _saveScale * amp;
    }

    [ContextMenu("Reset Position")]
    public void ResetPosition(){
        transform.localPosition = _savePos;
        transform.localRotation = _saveRotation;
        transform.localScale = _saveScale;
    }
}
