using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Slider VolumeSlider;

    public void Start()
    {
        VolumeSlider.value = GameObject.Find("Background Music").GetComponent<AudioSource>().volume;
    }

    public void SliderChanged(float newValue)
    {
        GameObject.Find("Background Music").GetComponent<AudioSource>().volume = newValue;
    }
}
