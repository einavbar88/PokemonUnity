using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPokemons : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    public Pokemon GetRandom()
    {
        var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}
