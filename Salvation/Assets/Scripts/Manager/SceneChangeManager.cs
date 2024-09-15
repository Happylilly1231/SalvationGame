using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // 처음 스토리 씬 불러오기
    public void ShowStory()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        SoundManager.instance.Play(Define.AudioClipType.Story, Define.Sound.Bgm);
        SceneManager.LoadScene("StoryScene");
    }

    // 메인 메뉴 씬 불러오기
    public void ToMainMenu()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        SoundManager.instance.Play(Define.AudioClipType.Main, Define.Sound.Bgm);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("데이터 불러옴");
    }

    // 스테이지 씬 불러오기
    public void StageStart(int stageNumber)
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        SoundManager.instance.Play(Define.AudioClipType.Stage, Define.Sound.Bgm);
        GameManager.instance.stageNum = stageNumber;
        SceneManager.LoadScene("Stage" + stageNumber);
    }

    // 노멀 엔딩 씬 불러오기
    public void NormalEnding()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        SoundManager.instance.Play(Define.AudioClipType.NormalEnding, Define.Sound.Bgm);
        SceneManager.LoadScene("NormalEnding");
    }

    // 해피 엔딩 씬 불러오기
    public void HappyEnding()
    {
        SoundManager.instance.Play(Define.AudioClipType.UI);
        SoundManager.instance.Play(Define.AudioClipType.HappyEnding, Define.Sound.Bgm);
        SceneManager.LoadScene("HappyEnding");
    }
}
