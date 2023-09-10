using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SelectionMode { PreBattle, InBattle}

public class BattleDialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialog;
    [SerializeField] GameObject battleOptions;
    [SerializeField] GameObject fightMoves;
    [SerializeField] GameObject moveDescription;

    [SerializeField] List<TextMeshProUGUI> optionsTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] TextMeshProUGUI pp;
    [SerializeField] TextMeshProUGUI type;

    public void Set(string dialog)
    {
        this.dialog.text = dialog;
    }

    public IEnumerator TypeText(string text)
    {
        dialog.text = "";
        foreach(char letter in text)
        {
            dialog.text += letter;
            yield return new WaitForSeconds(1f / 30);
        }
        yield return WaitForKeyDown.Key(KeyCode.Space);
    }

    public void dialogSwitch(bool isEnaled)
    {
        dialog.enabled = isEnaled;
        if (isEnaled)
        {
            battleOptions.SetActive(false);
            fightMoves.SetActive(false);
            moveDescription.SetActive(false);
        }
    }

    public void battleOptionsSwitch(bool isEnaled)
    {
        battleOptions.SetActive(isEnaled);
        UpdateSelection(0, SelectionMode.PreBattle);
    }


    public void fightMovesSwitch(bool isEnaled)
    {
        fightMoves.SetActive(isEnaled);
        moveDescription.SetActive(isEnaled);
        UpdateSelection(0, SelectionMode.InBattle);
    }

    public void UpdateSelection(int selected, SelectionMode selectionMode)
    {
        List<TextMeshProUGUI> selectFrom = selectionMode == SelectionMode.InBattle ? moveTexts : optionsTexts;
        PaintItBlack(selectFrom);
        Color color = new Color(0.1294f, 0.588f, 0.953f, 1);
        selectFrom[selected].color = color;
    }

    void PaintItBlack(List<TextMeshProUGUI> selected)
    {
        for(int i = 0; i < selected.Count; i++)
        {
            selected[i].color = Color.black;
        }
    }

    public void SetMovesText(List<FightMove> fightMoves)
    {
        for(int i=0; i < moveTexts.Count; i++)
        {
            if(i < fightMoves.Count)
            {
                moveTexts[i].text = fightMoves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

    public void SetTypeAndPP(FightMove fightMove)
    {
        pp.text = $"PP\t{fightMove.PP}/{fightMove.Base.PP}";
        type.text = $"{fightMove.Base.Type}";
    }

    public void ResetTypeAndPP()
    {
        pp.text = "";
        type.text = "";
    }
}

public class WaitForKeyDown 
{
    public static IEnumerator Key(KeyCode keyCode) 
    {
        while (!Input.GetKeyDown(keyCode))
        {
                yield return null;
        }
    }
}