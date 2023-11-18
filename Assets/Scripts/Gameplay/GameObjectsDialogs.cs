using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class GameObjectsDialogs : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    public bool IsTyping { get; set; }
    public bool IsDone { get; set; }

    public static GameObjectsDialogs Instance { get; private set; }

    public event Action OnOpenDialog;
    public event Action OnCloseDialog;
    int currentLine = 0;
    Dialog dialog;
    

    private void Awake()
    {
        Instance = this;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsTyping)
        {
            currentLine++;
            if (currentLine < dialog.Script.Count)
            {
                StartCoroutine(TypeText(dialog.Script[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                IsDone = true;
                OnCloseDialog.Invoke();
            }
        }
    }

    public IEnumerator Open(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        IsDone = false;
        OnOpenDialog?.Invoke();
        this.dialog = dialog;

        dialogBox.SetActive(true);
        StartCoroutine(TypeText(dialog.Script[0]));

        while(!IsDone)
        {
            yield return null;
        }
    }

    public IEnumerator TypeText(string text)
    {
        IsTyping = true;
        dialogText.text = "";
        foreach (char letter in text)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / 50);
        }
        IsTyping = false;
    }
}
