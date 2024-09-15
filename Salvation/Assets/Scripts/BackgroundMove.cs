using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public Renderer render;
    public static float speed;

    void Start()
    {
        speed = 0.3f;
    }

    void Update()
    {
        float move = Time.deltaTime * speed;
        render.material.mainTextureOffset -= Vector2.left * move;
    }
}
