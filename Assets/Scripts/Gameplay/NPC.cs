using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, StoryObjects
{
    [SerializeField] Dialog dialog;
    [SerializeField] Pokemon pokemon;
    [SerializeField] bool isPokemon;
    [SerializeField] bool isNurse;
    [SerializeField] string persistense;
    [SerializeField] string persistenseDisapearForever;

    void Start()
    {
        if (DataPersistance.dp[persistenseDisapearForever])
        {
            gameObject.SetActive(false);
        }

    }

    public IEnumerator Interact(Player player)
    {
        if (DataPersistance.dp[persistense]) yield break;
        else
        {
            yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
            if (isNurse)
            {
                List<Pokemon> pokemonsToHeal = player.GetComponent<PokemonsOwned>().Pokemons;
                foreach (Pokemon pokemon in pokemonsToHeal)
                {
                    pokemon.Heal(1);
                }
            }
            if (isPokemon)
            {
                JoinCrew(pokemon, player);
            }
        }
    }

    public void JoinCrew(Pokemon pokemon, Player player)
    {
        pokemon.Init();
        player.GetComponent<PokemonsOwned>().AddPokemon(pokemon);
        DataPersistance.dp[persistense] = true;
        this.gameObject.SetActive(false);
    }
}
