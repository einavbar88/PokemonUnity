using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class BattleText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] TextMeshProUGUI pokemonLevel;
    [SerializeField] HP pokemonHp;
    [SerializeField] GameObject XP;
    
    public Pokemon pokemon;

    public void Set(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        pokemonName.text = pokemon.GetName();
        pokemonHp.Set((float)pokemon.HP / pokemon.GetMaxHp());
        SetLevel();
        SetXp();
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

    public void SetXp()
    {
        if (XP == null) return;

        int currentLevelXp = pokemon.CalculateLevelXp(pokemon.Level);
        int nextLevelXp = pokemon.CalculateLevelXp(pokemon.Level + 1);

        float normalizedXp = Mathf.Clamp01((float)(pokemon.XP - currentLevelXp) / (nextLevelXp - currentLevelXp));

        XP.transform.localScale = new Vector3(normalizedXp, 1, 1);
    }

    public IEnumerator UpdateXp(bool isLevelUp)
    {
        if (XP == null) yield break;
        if(isLevelUp) XP.transform.localScale = new Vector3(0, 1, 1);

        int currentLevelXp = pokemon.CalculateLevelXp(pokemon.Level);
        int nextLevelXp = pokemon.CalculateLevelXp(pokemon.Level + 1);

        float normalizedXp = Mathf.Clamp01((float)(pokemon.XP - currentLevelXp) / (nextLevelXp - currentLevelXp));

        yield return XP.transform.DOScaleX(normalizedXp, 1.5f).WaitForCompletion();
    }

    public void SetLevel()
    {
        pokemonLevel.text = "Lvl " + pokemon.Level;
    }
}
