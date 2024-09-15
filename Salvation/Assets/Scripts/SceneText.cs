using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneText : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public GameObject nextBtn;
    public int textType;

    string[] textArray;
    string text;
    int textCnt;
    float delay;
    bool isTyping;
    int idx;
    Coroutine runningCoroutine;

    void Start()
    {
        nextBtn.GetComponent<Button>().onClick.AddListener(NextText);

        switch (textType)
        {
            case 0:
                textArray = DataManager.instance.data.storySceneTexts;
                break;
            case 1:
                textArray = DataManager.instance.data.normalEndingTexts;
                break;
            case 2:
                textArray = DataManager.instance.data.happyEndingTexts;
                break;
        }
        textCnt = textArray.Length;
        delay = 0.07f;
        isTyping = false;
        idx = 0;

        text = textArray[idx];
        targetText.text = "";
        runningCoroutine = StartCoroutine(TextPrint(delay));
    }

    // 텍스트 하나씩 적히는 코루틴
    IEnumerator TextPrint(float delay)
    {
        int cnt = 0;
        while (cnt < text.Length)
        {
            isTyping = true;
            targetText.text += text[cnt].ToString();
            cnt++;

            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }

    // 텍스트 다음 버튼 클릭 함수
    public void NextText()
    {
        if (isTyping == true)
        {
            StopCoroutine(runningCoroutine);
            targetText.text = text;
        }
        else
        {
            if (idx < textCnt)
            {
                // 마지막 문장을 넘기면
                if (idx == textCnt - 1)
                {
                    SceneChangeManager.instance.ToMainMenu(); // 메인메뉴로 가기
                }
                else
                {
                    idx++;
                    text = textArray[idx];
                    if (text[0] == '*')
                    {
                        targetText.fontStyle = FontStyles.Italic;
                        text = text.Split('*')[1];
                    }
                    else
                        targetText.fontStyle = FontStyles.Normal;
                    targetText.text = "";
                    runningCoroutine = StartCoroutine(TextPrint(delay));
                }
            }
        }
        isTyping = false;
    }
}
