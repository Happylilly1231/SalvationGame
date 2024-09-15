using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    private class ObjectInfo // 오브젝트 정보 클래스
    {
        public string objectName; // 오브젝트 이름
        public GameObject prefab; // 프리팹
        public int count; // 미리 생성할 개수
    }

    [SerializeField]
    ObjectInfo[] objectInfos = null; // 오브젝트 정보 클래스 배열

    public static ObjectPoolManager instance;

    public bool IsReady { get; private set; } // 오브젝트 풀링 준비 여부

    string objectName; // 오브젝트 이름 키

    Dictionary<string, IObjectPool<GameObject>> ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>(); // 풀 딕셔너리

    Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>(); // 오브젝트 딕셔너리

    // 싱글톤
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Init();
    }

    // 초기화
    void Init()
    {
        IsReady = false;

        for (int i = 0; i < objectInfos.Length; i++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnToPool,
        OnDestroyPoolObject, true, objectInfos[i].count, objectInfos[i].count); // 풀 인터페이스 변수에 풀 인터페이스를 상속받아 구현한 ObjectPool 클래스의 풀 인터페이스를 할당함

            if (goDic.ContainsKey(objectInfos[i].objectName)) // 프리팹 이름 키가 오브젝트 딕셔너리에 존재한다면
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[i].objectName);
                return;
            }

            // 존재하지 않는다면
            goDic.Add(objectInfos[i].objectName, objectInfos[i].prefab); // 오브젝트 딕셔너리에 해당 이름의 키와 프리팹 추가
            ojbectPoolDic.Add(objectInfos[i].objectName, pool); // 풀 딕셔너리에도 해당 이름의 키와 해당 프리팹이 속할 풀 추가

            // 미리 오브젝트 생성 해놓기
            for (int j = 0; j < objectInfos[i].count; j++)
            {
                objectName = objectInfos[i].objectName; // 오브젝트 이름
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>(); // 오브젝트 생성, PoolAble 클래스 객체로 생성 함수에서 반환된 오브젝트의 PoolAble 컴포넌트 할당
                poolAbleGo.Pool.Release(poolAbleGo.gameObject); // 생성한 오브젝트 반환(비활성화), PoolAble 클래스 객체에 있는 풀의 반환 함수 실행
            }
        }

        Debug.Log("오브젝트 풀링 준비 완료");
        IsReady = true;
    }

    // 생성
    GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(goDic[objectName]); //오브젝트 딕셔너리에서 objectName 키로 찾은 프리팹 생성하고, poolGo에 할당
        poolGo.GetComponent<PoolAble>().Pool = ojbectPoolDic[objectName]; // poolGo의 PoolAble 스크립트의 Pool(IObjectPool<GameObject> 인터페이스 변수)을 풀 딕셔너리에서 objectName 키로 찾은 풀로 함
        return poolGo; // 해당 poolGo 반환

    }

    // 대여
    void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true); // 활성화
    }

    // 반환
    public void OnReturnToPool(GameObject poolGo)
    {
        poolGo.SetActive(false); // 비활성화
    }

    // 삭제
    void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo); // 파괴
    }

    // 오브젝트 이름으로 풀에서 가져오기
    public GameObject GetGo(string goName)
    {
        objectName = goName; // 오브젝트 이름 키

        if (goDic.ContainsKey(goName) == false) // 오브젝트 딕셔너리에 이 키가 존재하지 않으면
        {
            // 오브젝트 풀에 등록되지 않은 오브젝트이므로 null 반환하며 종료
            Debug.LogFormat("{0} 오브젝트 풀에 등록되지 않은 오브젝트입니다.", goName);
            return null;
        }

        // 존재하면
        return ojbectPoolDic[goName].Get(); // 풀 딕셔너리에서 오브젝트 이름 키로 가져온 풀에서 Get 함수 실행
    }
}
