using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LearnNewMove : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> currentMoves;

    List<FightMove> fightMoves;

    public void Init(Pokemon pokemon)
    {
        fightMoves = pokemon.FightMoves;
        for (int i = 0; i < fightMoves.Count; i++)
        {
            currentMoves[i].text = fightMoves[i].Base.Name;
        }
    }

    public void UpdateSelection(int selected)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            if (i == selected) currentMoves[i].color = new Color(0.1294f, 0.588f, 0.953f, 1);
            else currentMoves[i].color = Color.black;
        }
    }
}
