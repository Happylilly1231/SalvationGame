using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutEffect : MonoBehaviour
{
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color color = image.color;
        while (image.color.a > 0)
        {
            color.a -= Time.deltaTime * 0.5f;
            image.color = color;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
