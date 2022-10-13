using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveFile saveFile;
   
    GameObject canvas;

    public UIManager uIManager { get; private set; }
      
    // Start is called before the first frame update
    void Awake()
    {
        canvas = FindObjectOfType<Canvas>().gameObject;
        Physics2D.IgnoreLayerCollision(7, 7);          
        Instance = this;
        uIManager = canvas.GetComponent<UIManager>();
        Debug.Log("load game");
    }

   
}
