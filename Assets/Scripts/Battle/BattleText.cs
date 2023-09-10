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
}
