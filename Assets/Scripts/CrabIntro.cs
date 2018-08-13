using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrabIntro : MonoBehaviour {

    private void Start()
    {
        InvokeRepeating("Shrink", 1, 0.03f);
    }

    void Shrink()
    {
        if (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(0.01f, 0.01f, 0);
        }
    }
}
