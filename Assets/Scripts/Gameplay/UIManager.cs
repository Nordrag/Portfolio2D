using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform settingsHolder, gameStateHolder;
    [SerializeField] Vector3 uiStartPos, uiEndPos;

    public bool IsGuiOnScreen { get; private set; }
    ActiveGUI guiOnScreen = ActiveGUI.None;
    int stars = 0;
    [SerializeField] float guiTravelTime;
    Camera cam;
    [SerializeField] Image hpImage;
    [SerializeField] TextMeshProUGUI starCount;
    [SerializeField] GameObject floatingStar, starTarget, canvas;
    [SerializeField] List<Image> carrots;
    [SerializeField] Vector3 endSize;
    [SerializeField] float targetTime;

    private void Awake()
    {
        settingsHolder.localPosition = uiEndPos;
        gameStateHolder.localPosition = uiEndPos;
        cam = Camera.main;
        starCount.SetText(stars.ToString());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            switch (guiOnScreen)
            {
                case ActiveGUI.GameState:
                    MoveGameState(false);
                    guiOnScreen = ActiveGUI.None;                   
                    break;
                case ActiveGUI.Settings:
                    MoveSettings(false);
                    MoveGameState(true);
                    guiOnScreen = ActiveGUI.GameState;
                    break;
                case ActiveGUI.None:
                    MoveGameState(true);
                    guiOnScreen = ActiveGUI.GameState;                   
                    break;
                default:
                    break;
            }
        }
    }

    public void OnPlayerHpChanged(int hp)
    {

        carrots.ForEach(x => x.gameObject.SetActive(false));

        int iterator = hp > 0 ? hp : 0;
        for (int i = 0; i < iterator; i++)
        {
            carrots[i].gameObject.SetActive(true);                
        }

    }

    public void ChangeStarCount(int toAdd)
    {
        stars += toAdd;
        starCount.SetText(stars.ToString());
    }

    public void SpawnStar(Vector2 currPos)
    {
        var spawn = Instantiate(floatingStar, canvas.transform);
        Vector2 newPos = cam.WorldToScreenPoint(currPos);
        spawn.transform.position = newPos;
        spawn.GetComponent<FloatingStarAnim>().OnCreate(starTarget.transform.position);
    }

    public void TweenStar()
    {
        starTarget.transform.DOScale(endSize, targetTime).SetEase(Ease.Linear).OnComplete(() => starTarget.transform.DOScale(Vector3.one, .2f).SetEase(Ease.Flash));
    }


    public void MoveSettings(bool start)
    {
        if (start)
        {
            settingsHolder.DOLocalMove(uiStartPos, guiTravelTime).SetEase(Ease.InBounce);           
            return;
        }
        settingsHolder.DOLocalMove(uiEndPos, guiTravelTime).SetEase(Ease.OutBounce);      
    }

    public void MoveGameState(bool start)
    {
        if (start)
        {
            gameStateHolder.DOLocalMove(uiStartPos, guiTravelTime).SetEase(Ease.InBounce);            
            return;
        }
        gameStateHolder.DOLocalMove(uiEndPos, guiTravelTime).SetEase(Ease.OutBounce);
    }

    public void Btn_ToSettings()
    {
        MoveSettings(true);
        MoveGameState(false);
        guiOnScreen = ActiveGUI.Settings;
    }

    public void Btn_ToGameState()
    {
        MoveSettings(false);
        MoveGameState(true);
        guiOnScreen = ActiveGUI.GameState;
    }

    public void Btn_Continue()
    {
        MoveSettings(false);
        MoveGameState(false);
        guiOnScreen = ActiveGUI.None;
    }

    public void Btn_Quit()
    {
        Debug.Log("save");
        Application.Quit();
    }

    [ContextMenu("record")]
    public void RecordEndPos()
    {
        uiEndPos = settingsHolder.localPosition;
    }

    enum ActiveGUI { GameState, Settings, None }
}
