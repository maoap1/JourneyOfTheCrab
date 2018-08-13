using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeScript : MonoBehaviour {

	void Start ()
    {
        GetComponent<AudioSource>().volume = GameObject.Find("Background Music").GetComponent<AudioSource>().volume;
    }
}
