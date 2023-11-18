using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BattleText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] TextMeshProUGUI pokemonLevel;
    [SerializeField] HP pokemonHp;
    
    public Pokemon pokemon;

    public void Set(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        pokemonName.text = pokemon.GetName();
        pokemonLevel.text = "Lvl " + pokemon.Level;
        pokemonHp.Set((float)pokemon.HP / pokemon.GetMaxHp());
    }

    public IEnumerator UpdateHP(float remainingHp)
    {
        yield return pokemonHp.BarAnimation(remainingHp / pokemon.GetMaxHp());
    }

    public void SetSelected(bool isSelected) 
    {
        if (isSelected) pokemonName.color = new Color(0.1294f, 0.588f, 0.953f, 1);
        else pokemonName.color = Color.black;
    }
}
