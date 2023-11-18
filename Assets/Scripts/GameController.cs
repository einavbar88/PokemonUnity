using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { OpenWorld, Battle, Dialog }

public class GameController : MonoBehaviour 
{
    GameState state;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Player player;
    [SerializeField] Camera playerCamera;

    void Start()
    {
        player.OnEncounter += StartBattle;
        battleSystem.OnEndOfBattle += EndBattle;
        GameObjectsDialogs.Instance.OnOpenDialog += () =>
        {
            state = GameState.Dialog;
        };
        GameObjectsDialogs.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog) state = GameState.OpenWorld;
        };
    }

    void Update()
    {
        switch (state)
        {
            case GameState.OpenWorld:
                player.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
            case GameState.Dialog:
                GameObjectsDialogs.Instance.HandleUpdate();
                break;
            default:
                break;
        }
    }

    void StartBattle()
    {
        state = GameState.Battle;
        player.IsMovingEnabled = false;
        battleSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        var playerPokemons = player.GetComponent<PokemonsOwned>();
        var wildPokemon = FindObjectOfType<WildPokemons>().GetComponent<WildPokemons>().GetRandom();
        battleSystem.StartBattle(playerPokemons, wildPokemon);
    }

    void EndBattle(bool didPlayerWin)
    {
        state = GameState.OpenWorld;
        battleSystem.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        player.IsMovingEnabled = true;
    }
}