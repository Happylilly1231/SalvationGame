using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int stageNum; // 스테이지 번호
    public int maxStageNum = 3; // 마지막 스테이지 번호

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DataManager.instance.LoadGameData(); // 게임 데이터 로드
        SoundManager.instance.Init();
        SoundManager.instance.Play(Define.AudioClipType.Main, Define.Sound.Bgm);
    }

    // 게임 시작 버튼 함수
    public void GameStart()
    {
        if (!DataManager.instance.data.isStoryShow) // 스토리 안 봤으면
        {
            DataManager.instance.data.isStoryShow = true;
            DataManager.instance.SaveGameData();
            SceneChangeManager.instance.ShowStory(); // 스토리 씬으로 이동
        }
        else // 봤으면
        {
            SceneChangeManager.instance.ToMainMenu(); // 메인 메뉴 씬으로 이동
        }
    }

    // 일시 정지
    public void Pause()
    {
        if (Time.timeScale == 1)
        {
            Debug.Log("게임이 일시정지되었습니다.");
            Time.timeScale = 0; // 일시정지
            UIManager.instance.LoadSettingUI(); // 설정 UI 불러오기
        }
    }

    // 계속하기
    public void Continue()
    {
        UIManager.instance.HideSettingUI(); // 설정 UI 숨기기
        // 타이머 3초 구현하기
        Time.timeScale = 1;
    }

    // 스테이지 클리어(엔딩 포함)
    public void StageClear()
    {
        SoundManager.instance.Play(Define.AudioClipType.Clear);
        Debug.Log("스테이지" + stageNum + "를 클리어했습니다!");
        DataManager.instance.data.isClear[stageNum - 1] = true;
        DataManager.instance.data.memoryPieces[stageNum - 1] = Player.haveMemoryPiece;
        CheckHaveAllMemoryPieces();

        if (stageNum == maxStageNum) // 마지막 스테이지 클리어 시 -> 게임 엔딩
        {
            if (DataManager.instance.data.haveAllMemoryPieces) // 기억 조각을 전부 수집했을 경우
                SceneChangeManager.instance.HappyEnding(); // 해피 엔딩 씬 불러오기
            else // 전부 수집하지 못했을 경우
                SceneChangeManager.instance.NormalEnding(); // 노멀 엔딩 씬 불러오기
        }
        else
        {
            // 스테이지 클리어(엔딩 x)
            UIManager.instance.LoadClearUI(); // 스테이지 클리어 UI 불러오기
            DataManager.instance.data.isUnlock[stageNum] = true; // 다음 스테이지 언락
        }
        DataManager.instance.SaveGameData();
    }

    // 기억 조각 모두 모았는지 확인해서 업데이트
    public void CheckHaveAllMemoryPieces()
    {
        int flag = 0;
        for (int i = 0; i < DataManager.instance.data.memoryPieces.Length; i++)
        {
            if (DataManager.instance.data.memoryPieces[i] == false)
            {
                flag = 1;
                break;
            }
        }
        if (flag == 0)
            DataManager.instance.data.haveAllMemoryPieces = true;
    }

    // 게임 초기화
    public void GameReset()
    {
        DataManager.instance.data = new GameData();
        DataManager.instance.SaveGameData();
        GameStart();
    }

    // 게임 종료
    public void GameQuit()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }

    // 앱 종료
    void OnApplicationQuit()
    {
        DataManager.instance.SaveGameData(); // 게임 데이터 저장
    }
}
