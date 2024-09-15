using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager instance;
    public GameObject settingUI;
    public GameObject resetPopUp;
    public Button resetOkBtn;
    public Button gameQuitBtn;
    public GameObject tutorialFirstText;
    public GameObject tutorialUI;
    public Image tutorialImg;
    public Sprite[] tutorialImgs;
    public TextMeshProUGUI tutorialText;
    public Button tutorialPrevBtn;
    public Button tutorialNextBtn;
    public GameObject gameEndingBtn;

    public GameObject[] stageBtns;
    public GameObject[] stageClearEffects;
    public GameObject[] stageMemoryPieces;

    int tutorialIdx;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        resetOkBtn.GetComponent<Button>().onClick.AddListener(GameManager.instance.GameReset);
        gameQuitBtn.GetComponent<Button>().onClick.AddListener(GameManager.instance.GameQuit);

        DataManager.instance.LoadGameData(); // 게임 데이터 로드
        for (int i = 0; i < stageBtns.Length; i++)
        {
            Debug.Log(DataManager.instance.data.isUnlock[i]);
            stageBtns[i].GetComponent<Button>().interactable = DataManager.instance.data.isUnlock[i];

            // 클리어한 스테이지 표시
            if (DataManager.instance.data.isClear[i])
            {
                stageClearEffects[i].SetActive(true);
            }

            // 기억 조각 모은 스테이지 표시
            if (DataManager.instance.data.memoryPieces[i])
            {
                stageMemoryPieces[i].SetActive(true);
            }
        }

        if (!DataManager.instance.data.isTutorialShow)
        {
            LoadTutorialUI();
            tutorialFirstText.SetActive(true);
        }

        if (DataManager.instance.data.isUnlock[GameManager.instance.maxStageNum - 1] == true)
        {
            gameEndingBtn.SetActive(true);
        }
    }

    public void LoadSettingUI()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        settingUI.SetActive(true);
    }

    public void HideSettingUI()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        settingUI.SetActive(false);
    }

    public void LoadResetPopUp()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        resetPopUp.SetActive(true);
    }

    public void HideResetPopUp()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        resetPopUp.SetActive(false);
    }

    public void LoadTutorialUI()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        tutorialUI.SetActive(true);
        tutorialIdx = 0;
        tutorialImg.sprite = tutorialImgs[tutorialIdx];
        tutorialText.text = DataManager.instance.data.tutorialTexts[tutorialIdx];
        tutorialPrevBtn.interactable = false;
    }

    public void HideTutorialUI()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        tutorialUI.SetActive(false);
        if (tutorialFirstText.activeSelf)
        {
            DataManager.instance.data.isTutorialShow = true;
            DataManager.instance.SaveGameData();
            tutorialFirstText.SetActive(false);
        }
    }

    public void PrevTutorialPart()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        tutorialPrevBtn.interactable = true;
        tutorialNextBtn.interactable = true;
        tutorialIdx--;
        tutorialImg.sprite = tutorialImgs[tutorialIdx];
        tutorialText.text = DataManager.instance.data.tutorialTexts[tutorialIdx];
        if (tutorialIdx == 0)
            tutorialPrevBtn.interactable = false;
    }

    public void NextTutorialPart()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        tutorialPrevBtn.interactable = true;
        tutorialNextBtn.interactable = true;
        tutorialIdx++;
        tutorialImg.sprite = tutorialImgs[tutorialIdx];
        tutorialText.text = DataManager.instance.data.tutorialTexts[tutorialIdx];
        if (tutorialIdx == tutorialImgs.Length - 1)
            tutorialNextBtn.interactable = false;
    }

    public void GameEndingShow()
    {
        for (int i = 0; i < 3; i++)
        {
            if (DataManager.instance.data.haveAllMemoryPieces) // 기억 조각을 전부 수집했을 경우
                SceneChangeManager.instance.HappyEnding(); // 해피 엔딩 씬 불러오기
            else // 전부 수집하지 못했을 경우
                SceneChangeManager.instance.NormalEnding(); // 노멀 엔딩 씬 불러오기
        }
    }
}
