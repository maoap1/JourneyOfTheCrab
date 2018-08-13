using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameSound : MonoBehaviour {

	void Start ()
    {
        GetComponent<AudioSource>().volume = GameObject.Find("Background Music").GetComponent<AudioSource>().volume;
        GetComponent<AudioSource>().Play();
    }
}
