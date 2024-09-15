using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGoGenerator : MonoBehaviour
{
    [System.Serializable]
    public class PatternObjectInfo
    {
        public string mapGoName; // 오브젝트 이름
        public Vector2 generatePos; // 생성 위치
        public Sprite radarShowImg; // 레이더 표시 이미지
    }

    public PatternObjectInfo[] obsPatternObjectInfos = null; // 장애물 오브젝트 정보 클래스 배열
    public PatternObjectInfo[] itemPatternObjectInfos = null; // 아이템 오브젝트 정보 클래스 배열
    public PatternObjectInfo clearZoneObjectInfo = null; // 클리어존 오브젝트 정보
    PatternObjectInfo patternObjectInfo;
    int idx;

    public Sprite radarDefault; // 기본
    public Sprite radarUp; // 상
    public Sprite radarDown; // 하
    public Sprite radarLeft; // 좌
    public Sprite radarRight; // 우

    string mapGoName;
    GameObject mapGo;
    Vector2 generatePos;
    float generateWaitTime = 0f;
    Sprite radarShowImg;

    int[][] rushPatterns = new int[3][];

    List<Dictionary<string, object>> patternData;

    void Start()
    {
        // 패턴 데이터 csv 파일에서 가져오기
        patternData = CSVReader.Read("salvation_mapGo_pattern");

        // 러쉬 패턴
        rushPatterns[0] = new int[] { 0, 1, 2, 1, 3 };
        rushPatterns[1] = new int[] { 2, 0, 4, 3, 2 };
        rushPatterns[2] = new int[] { 3, 2, 0, 4, 3 };

        // 맵 오브젝트 생성 코루틴 실행
        StartCoroutine(GenerateMapGo(GameManager.instance.stageNum - 1));
    }

    // 맵 오브젝트 생성 코루틴
    IEnumerator GenerateMapGo(int stageIndex)
    {
        for (int i = 0; i < patternData.Count; i++) // 가장 긴 패턴 길이 내에서
        {
            if (patternData[i]["patterns" + stageIndex].ToString() == "") break; // 패턴이 끝났으면 중단

            // 러쉬 타임!!!
            if (Player.flyOver) // 플레이어가 다 날고 떨어졌을 때
            {
                Player.flyOver = false; // 변수 초기화(난 다음 떨어진 상태가 아니니까)

                // // 만약 장애물 생성되면 제거(러쉬 패턴 보여질 때, 장애물에 맞으면 안되니까)
                // if (mapGo != null)
                //     ObjectPoolManager.instance.OnReturnToPool(mapGo);

                // 러쉬 레이더 표시 코루틴 실행
                yield return StartCoroutine(RushRadarCoroutine(stageIndex));

                yield return new WaitForSeconds(1f); // 1초 대기

                // 러쉬 장애물 생성 코루틴 실행
                yield return StartCoroutine(RushCoroutine(stageIndex));
            }
            // ---------------------------------------------------------

            // 러쉬 타임 X

            // 다음 장애물의 번호
            idx = (int)patternData[i]["patterns" + stageIndex];

            // 오브젝트 종류 판별
            if (idx < 10) // 0 ~ 4 -> 장애물(망령(상), 지옥불, 번개, 망령(좌), 망령(우))
            {
                patternObjectInfo = obsPatternObjectInfos[idx];
            }
            else if (idx < 20) // 10 ~ 12 -> 아이템(안개꽃, 빛, 기억 조각)
            {
                idx = idx % 10;
                patternObjectInfo = itemPatternObjectInfos[idx];
            }
            else if (idx == 100) // 100 -> 클리어존
            {
                patternObjectInfo = clearZoneObjectInfo;
            }
            else // 20 -> 플레이어가 나비로 변신했을 동안 나올 장애물이 모두 끝났을 때
            {
                yield return new WaitForSeconds(generateWaitTime);
                continue;
            }

            // 오브젝트 정보 할당
            mapGoName = patternObjectInfo.mapGoName; // 오브젝트 이름
            generatePos = patternObjectInfo.generatePos; // 생성 위치
            radarShowImg = patternObjectInfo.radarShowImg; // 레이더 표시 이미지
            generateWaitTime = (int)patternData[i]["generateWaitTimePatterns" + stageIndex]; // 생성 대기 시간
            Debug.Log("generateWaitTime: " + generateWaitTime + " / " + mapGoName);

            yield return new WaitForSeconds(generateWaitTime - 1.5f);

            // 공격 방향을 표시해야 할 때(0.5초 표시)
            if (radarShowImg != radarDefault)
            {
                // 0.5초간 공격 방향 표시
                UIManager.instance.radar.sprite = radarShowImg; // 공격 방향 보여주기
                UIManager.instance.radar.color = new Color(0, 238, 255);
                yield return new WaitForSeconds(0.5f);

                // 다시 기본 이미지로 돌아오기
                UIManager.instance.radar.sprite = radarDefault;
                UIManager.instance.radar.color = Color.white;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            // 0.5초 대기 후 맵 오브젝트 생성
            yield return new WaitForSeconds(1f);
            SoundManager.instance.Play(Define.AudioClipType.Appear);
            mapGo = ObjectPoolManager.instance.GetGo(mapGoName);
            mapGo.transform.position = generatePos;
            Debug.Log((int)patternData[i]["patterns" + stageIndex] + " -> " + Player.time);
        }
    }

    // 러쉬 레이더 표시 코루틴
    IEnumerator RushRadarCoroutine(int stageIndex)
    {
        SoundManager.instance.Play(Define.AudioClipType.Stage, Define.Sound.Bgm, 0.5f);
        BackgroundMove.speed = 0.1f; // 배경 느려지기
        UIManager.instance.LoadRushRadarUI(); // 러쉬 레이더 UI 불러오기
        for (int i = 0; i < rushPatterns[stageIndex].Length; i++)
        {
            idx = rushPatterns[stageIndex][i];
            patternObjectInfo = obsPatternObjectInfos[idx];

            radarShowImg = patternObjectInfo.radarShowImg; // 레이더 표시 이미지

            if (radarShowImg != radarDefault)
            {
                UIManager.instance.rushRadar.sprite = radarShowImg;
                UIManager.instance.rushRadar.color = new Color(0, 238, 255);

                yield return new WaitForSeconds(0.5f);

                UIManager.instance.rushRadar.sprite = radarDefault;
                UIManager.instance.rushRadar.color = Color.white;
            }

            yield return new WaitForSeconds(1f);
        }
        UIManager.instance.HideRushRadarUI(); // 러쉬 레이더 UI 끄기
        BackgroundMove.speed = 0.3f; // 배경 다시 원래 속도로 설정
        SoundManager.instance.Play(Define.AudioClipType.Stage, Define.Sound.Bgm);
    }

    // 러쉬 장애물 생성 코루틴
    IEnumerator RushCoroutine(int stageIndex)
    {
        for (int i = 0; i < rushPatterns[stageIndex].Length; i++)
        {

            idx = rushPatterns[stageIndex][i];
            patternObjectInfo = obsPatternObjectInfos[idx];

            mapGoName = patternObjectInfo.mapGoName; // 오브젝트 이름
            generatePos = patternObjectInfo.generatePos; // 생성 위치

            SoundManager.instance.Play(Define.AudioClipType.Appear);
            mapGo = ObjectPoolManager.instance.GetGo(mapGoName);
            mapGo.transform.position = generatePos;

            yield return new WaitForSeconds(2f);
        }
    }
}