using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public int dir_idx = 2; // 0: 상, 1: 하, 2: 좌, 3: 우
    public float speed = 5f; // 이동 속도

    Vector3 dir;

    void Start()
    {
        switch (dir_idx)
        {
            case 0:
                dir = Vector3.up;
                break;
            case 1:
                dir = Vector3.down;
                break;
            case 2:
                dir = Vector3.left;
                break;
            case 3:
                dir = Vector3.right;
                break;
        }
    }

    void Update()
    {
        if (transform.position.x < Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}
