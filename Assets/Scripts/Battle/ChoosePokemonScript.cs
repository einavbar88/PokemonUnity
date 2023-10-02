using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoosePokemonScript : MonoBehaviour
{
    TextMeshProUGUI text;
    BattleText[] slots;

    public void Init()
    {
        slots = GetComponentsInChildren<BattleText>();
    }

    public void Set(List<Pokemon> pokemons)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < pokemons.Count) slots[i].Set(pokemons[i]);
            else slots[i].gameObject.SetActive(false);
        }
    }
}
