using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleColor : MonoBehaviour
{
    Color[] randColors = new Color[5];

    SpriteRenderer spr;
    // Start is called before the first frame update
    void Start()
    {
        randColors[0] = Color.red;
        randColors[1] = Color.yellow;
        randColors[2] = Color.green;
        randColors[3] = Color.cyan;
        randColors[4] = Color.blue;

        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spr.color = randColors[Random.Range(0,5)];
        spr.color = new Color(spr.color.r, spr.color.g , spr.color.b, 0.5f);
    }
}
