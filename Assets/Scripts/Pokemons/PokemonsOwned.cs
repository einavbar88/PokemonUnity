using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonsOwned : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
    }
    void Start()
    {
        foreach (var pokemon in pokemons) pokemon.Init();
    }

    public Pokemon GetNotFaintedPokemon()
    {
        foreach(var pokemon in pokemons)
        {
            if (pokemon.HP > 0) return pokemon;
        }
        return null;
    }

    public void AddPokemon(Pokemon pokemon)
    {
        pokemons.Add(pokemon);
    }
}
