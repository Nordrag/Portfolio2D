using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    float lenght, startpos;
    Transform cam;
    [SerializeField] float pEffect;
    SpriteRenderer sRender;
    [SerializeField] int repetitons = 1;
    [SerializeField] bool autoConfig = true;

    int actualReps => repetitons / 2;

    private void Awake()
    {
        startpos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        sRender = GetComponent<SpriteRenderer>();
        cam = Camera.main.transform;

        if (repetitons == 1)
        {
            repetitons = 6;
        }
        if (repetitons % 2 != 0)
        {
            repetitons++;
        }


        if (autoConfig)
        {
            for (int i = 0; i <= actualReps; i++)
            {
                var left = new GameObject();
                var right = new GameObject();
                left.transform.parent = transform;
                right.transform.parent = transform;

                var lRender = left.AddComponent<SpriteRenderer>();
                var rRender = right.AddComponent<SpriteRenderer>();

                lRender.sprite = sRender.sprite;
                rRender.sprite = sRender.sprite;

                lRender.sortingLayerID = sRender.sortingLayerID;
                rRender.sortingLayerID = sRender.sortingLayerID;
                rRender.sortingOrder = sRender.sortingOrder;
                lRender.sortingOrder = sRender.sortingOrder;

                left.transform.localPosition = new Vector3(-lenght * (i + 1), 0, 0);
                right.transform.localPosition = new Vector3(lenght * (i + 1), 0, 0);
            }
        }
       
    }

    private void Update()
    {
        float temp = (transform.position.x * (1 - pEffect));
        float dist = (cam.position.x * pEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if(temp > startpos + lenght) startpos += lenght;
        else if(temp < startpos - lenght) startpos -= lenght;
    }
}
