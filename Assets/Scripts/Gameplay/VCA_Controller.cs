using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VCA_Controller : MonoBehaviour
{   
    [SerializeField] string vcaName;
    VCA vca;

    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SliderChanged);
        vca = RuntimeManager.GetVCA("vca:/" + vcaName);
        vca.setVolume(slider.value);
    }

    void SliderChanged(float val)
    {
        vca.setVolume(val);
    }
}
