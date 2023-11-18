using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoosePokemonScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    List<Pokemon> pokemons;
    BattleText[] slots;

    public void Init()
    {
        slots = GetComponentsInChildren<BattleText>();
    }

    public void Set(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < pokemons.Count) slots[i].Set(pokemons[i]);
            else slots[i].gameObject.SetActive(false);
        }

        text.text = "Choose a Pokemon";
    }

    public void UpdateSelection(int selected)
    {
        for (int i = 0; i < this.pokemons.Count; i++)
        {
            slots[i].SetSelected(i == selected);
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
