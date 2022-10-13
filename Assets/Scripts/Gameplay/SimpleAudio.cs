using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;

public class SimpleAudio : MonoBehaviour
{     
    public void Play(EventReference reference)
    {      
        RuntimeManager.PlayOneShot(reference, transform.position);       
    }
}