using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBase : MonoBehaviour
{
    SpriteRenderer spr;

    public int ArtworkID;
    public float LerpSpeed = 0.05f;
    
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float transparentArt = ObjectManager.instance.transparentArt[ArtworkID];

        //Fade Area
        Color t_color = new Color(1, 1, 1, transparentArt);
        spr.color = Color.Lerp(spr.color , t_color, LerpSpeed);
    }
}
