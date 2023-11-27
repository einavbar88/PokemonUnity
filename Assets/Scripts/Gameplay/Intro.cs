using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Intro : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] List<string> script;
    [SerializeField] SceneSwitchers sceneSwitcher;
    bool isTyping = false;

    void Start()
    {
        StartCoroutine(TypeScript());
    }
    public IEnumerator TypeScript()
    {
        int i = script.Count;
        while (i > 0)
        {
            if (!isTyping)
            {
                int line = script.Count - i;
                yield return TypeText(script[line]);
                i--;
            }
            yield return null;
        }

        yield return WaitForKeyDown.Key(KeyCode.Space);
        yield return sceneSwitcher.StartGame();
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in text)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / 25);
        }
        yield return WaitForKeyDown.Key(KeyCode.Space);
        isTyping = false;
    }
}
