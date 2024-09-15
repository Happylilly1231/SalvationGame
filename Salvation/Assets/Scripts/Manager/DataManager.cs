using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public GameData data = new GameData();

    string gameDataFileName = "GameData.json";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        string filePath = Application.persistentDataPath + "/" + gameDataFileName;
        if (!File.Exists(filePath))
        {
            string toJsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, toJsonData);
            Debug.Log("파일 생성 완료");
        }

        Debug.Log(filePath);
    }

    // 게임 데이터 로드
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        if (File.Exists(filePath))
        {
            string fromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(fromJsonData);
            Debug.Log("불러오기 완료");
        }
    }

    // 게임 데이터 저장
    public void SaveGameData()
    {
        string toJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        File.WriteAllText(filePath, toJsonData);

        Debug.Log("저장 완료");
    }
}
