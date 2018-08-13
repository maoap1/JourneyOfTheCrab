using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Invoke("GoToMainMenu", 5);
	}
	
	void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
