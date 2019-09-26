using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour
{
    public Transform[] bgElements;

    // Update is called once per frame
    void Update()
    {
        bgElements[0].position = new Vector3(transform.position.x * .1f, bgElements[0].position.y, bgElements[0].position.z);
        bgElements[1].position = bgElements[0].position;

        for (int i = 2; i < bgElements.Length; i++)
        {
            bgElements[i].position = new Vector3(transform.position.x * i/5, bgElements[i].position.y, bgElements[i].position.z);
        }
    }
}
