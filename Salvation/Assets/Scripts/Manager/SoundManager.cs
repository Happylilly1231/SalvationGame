using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource[] audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    [SerializeField]
    AudioClip[] audioClips;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            DontDestroyOnLoad(root);

            // @Sound의 자식 오브젝트로 Bgm, Effect 오브젝트(소리 발생기) 생성
            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            audioSources[(int)Define.Sound.Bgm].volume = 0.3f;
            audioSources[(int)Define.Sound.Effect].volume = 0.2f;

            // Bgm 이면 무한 반복 재생
            audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Play(Define.AudioClipType type, Define.Sound sourceType = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = audioClips[(int)type];

        if (sourceType == Define.Sound.Bgm) // Bgm 재생
        {
            AudioSource audioSource = audioSources[(int)Define.Sound.Bgm]; // Bgm 오디오 소스

            // 다른 음악이 재생중이면 중지
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.Play();
        }
        else // Effect 재생
        {
            AudioSource audioSource = audioSources[(int)Define.Sound.Effect]; // Effect 오디오 소스
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip); // 클립을 1회 재생(중첩 재생 가능)
        }

    }
}
