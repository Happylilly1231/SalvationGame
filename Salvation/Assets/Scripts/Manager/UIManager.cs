using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject settingUI;
    public GameObject rushRadarUI;
    public GameObject clearUI;
    public GameObject dieUI;
    public Button menuBtn;
    public Button continueBtn;

    public Image radar; // 레이더 이미지
    public Image rushRadar; // 러쉬 화면 레이더 이미지

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        menuBtn.GetComponent<Button>().onClick.AddListener(GameManager.instance.Pause);
        continueBtn.GetComponent<Button>().onClick.AddListener(GameManager.instance.Continue);
    }

    // 설정 화면
    // 불러오기
    public void LoadSettingUI()
    {
        SoundManager.instance.audioSources[(int)Define.Sound.Bgm].Pause();
        settingUI.SetActive(true);
    }

    // 숨기기
    public void HideSettingUI()
    {
        SoundManager.instance.audioSources[(int)Define.Sound.Bgm].UnPause();
        settingUI.SetActive(false);
    }


    // 러쉬 레이더 표시 화면
    // 불러오기
    public void LoadRushRadarUI()
    {
        rushRadarUI.SetActive(true);
    }

    // 숨기기
    public void HideRushRadarUI()
    {
        rushRadarUI.SetActive(false);
    }


    // 스테이지 클리어 화면 불러오기
    public void LoadClearUI()
    {
        clearUI.SetActive(true);
    }

    // 죽음 화면 불러오기
    public void LoadDieUI()
    {
        dieUI.SetActive(true);
    }
}
