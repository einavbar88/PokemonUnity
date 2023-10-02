using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, BattleOptions, FightMoves, Opponent, Busy };

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleVisuals playerPokemon;
    [SerializeField] BattleText playerPokemonText;
    [SerializeField] BattleVisuals opponentPokemon;
    [SerializeField] BattleText opponentPokemonText;
    [SerializeField] BattleDialog battleDialog;
    [SerializeField] ChoosePokemonScript choosePokemon;

    public event Action<bool> OnEndOfBattle;

    PokemonsOwned playerPokemons;
    Pokemon wildPokemon;

    BattleState state;
    int[] currentSelection = { 0, 0 };

    public void StartBattle(PokemonsOwned playerPokemons, Pokemon wildPokemon)
    {
        this.playerPokemons = playerPokemons;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleState.BattleOptions:
                HandleSelection(SelectionMode.PreBattle);
                break;
            case BattleState.FightMoves:
                HandleSelection(SelectionMode.InBattle);
                break;
            default:
                break;
        }
    }

    public IEnumerator SetupBattle()
    {
        playerPokemon.Set(playerPokemons.GetNotFaintedPokemon());
        playerPokemonText.Set(playerPokemon.Pokemon);
        opponentPokemon.Set(wildPokemon);
        opponentPokemonText.Set(opponentPokemon.Pokemon);
        choosePokemon.Init();

        battleDialog.dialogSwitch(true);
        battleDialog.SetMovesText(playerPokemon.Pokemon.FightMoves);
        battleDialog.SetTypeAndPP(playerPokemon.Pokemon.FightMoves[0]);

        yield return battleDialog.TypeText($"A wild {opponentPokemon.Pokemon.PokemonBase.Name} appeared!");
        yield return WaitForKeyDown.Key(KeyCode.Space);
        yield return BattleOptions();

    }

    public IEnumerator BattleOptions()
    {
        state = BattleState.BattleOptions;
        StartCoroutine(battleDialog.TypeText("What Will You Do?"));
        yield return new WaitForSeconds(0.7f);
        battleDialog.battleOptionsSwitch(true);
    }

    public void FightMoves()
    {
        state = BattleState.FightMoves;
        battleDialog.battleOptionsSwitch(false);
        battleDialog.dialogSwitch(false);
        battleDialog.fightMovesSwitch(true);
    }

 public IEnumerator PlayerMove()
    {
        state = BattleState.Busy;

        yield return ExecuteMove();
    }

  public IEnumerator OpponentMove()
    {
        state = BattleState.Opponent;
       
        yield return ExecuteMove();
    }

    public IEnumerator ExecuteMove()
    {
        // init variables
        bool isPlayerTurn = state != BattleState.Opponent;
        Pokemon attackingPokemon = isPlayerTurn ? playerPokemon.Pokemon : opponentPokemon.Pokemon;
        Pokemon defendingPokemon = isPlayerTurn ? opponentPokemon.Pokemon : playerPokemon.Pokemon;

        BattleVisuals attackingPokemonVisual = isPlayerTurn ? playerPokemon : opponentPokemon;
        BattleVisuals defendingPokemonVisual = isPlayerTurn ? opponentPokemon : playerPokemon;

        int selectedMove = GetSelectedOption();
        var fightMove = isPlayerTurn ? attackingPokemon.FightMoves[selectedMove] : attackingPokemon.GetRandomFightMove();

        var battleText = isPlayerTurn ? opponentPokemonText: playerPokemonText;
        fightMove.PP--;
        battleDialog.SetTypeAndPP(fightMove);

        yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} used {fightMove.Base.Name}");
        attackingPokemonVisual.AttackAnimation();
        yield return new WaitForSeconds(1f);
        defendingPokemonVisual.HitAnimation();
        yield return new WaitForSeconds(1f);

        // calculate special damage factors
        float critical = UnityEngine.Random.value * 100f <= 6.25f ? 2f : 1f;
        float effectivness = EffectivnessChart.GetEffectivness(fightMove.Base.Type, defendingPokemon.PokemonBase.mainType) * EffectivnessChart.GetEffectivness(fightMove.Base.Type, defendingPokemon.PokemonBase.secondaryType);

        float remainingHp = defendingPokemon.RemainingHp(fightMove, attackingPokemon, critical, effectivness);

        yield return new WaitForSeconds(1f);

        yield return battleText.UpdateHP(remainingHp);

        // type if critical or effective
        if(critical > 1f) yield return battleDialog.TypeText("Critical Hit!");
        if(effectivness > 1f) yield return battleDialog.TypeText("It's Very Effective!");
        else if(effectivness < 1f) yield return battleDialog.TypeText("It's Not Very Effective!");
        
      
        if(remainingHp == 0) // pokemon fainted
        {
          yield return battleDialog.TypeText($"{defendingPokemon.PokemonBase.Name} fainted");
          defendingPokemonVisual.FaintAnimation();
          yield return new WaitForSeconds(2f);
          if (isPlayerTurn)
          { 
            OnEndOfBattle(isPlayerTurn);
          }
          else
          {
            var nextPokemon = playerPokemons.GetNotFaintedPokemon();
            if(nextPokemon != null)
            {
                playerPokemon.Set(nextPokemon);
                playerPokemonText.Set(nextPokemon);

                battleDialog.SetMovesText(nextPokemon.FightMoves);
                yield return battleDialog.TypeText($"{nextPokemon.PokemonBase.Name} Tag in!");
                yield return BattleOptions();
            }
            else
            {
                OnEndOfBattle(isPlayerTurn);
            }
          }
        }
        else  
        {
          if(isPlayerTurn) StartCoroutine(OpponentMove());
          else yield return BattleOptions();
        }
    }
   
    void HandleSelection(SelectionMode selectionMode)
    {     
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) currentSelection[0] = 0;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) currentSelection[0] = 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) currentSelection[1] = 0;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) currentSelection[1] = 1;

            int selected = GetSelectedOption();

            battleDialog.UpdateSelection(selected, selectionMode);
            if (selectionMode == SelectionMode.InBattle)
            {
                try
                {
                    battleDialog.SetTypeAndPP(playerPokemon.Pokemon.FightMoves[selected]);
                }
                catch
                {
                    battleDialog.ResetTypeAndPP();
                }
            }
            try
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (selectionMode == SelectionMode.PreBattle)
                    {
                        switch (selected)
                        {
                            case 0:
                                battleDialog.fightMovesSwitch(true);
                                FightMoves();
                                break;
                            case 1: 
                                ChoosePokemonScreen();
                                break;
                            case 2:
                                print("bag");
                                break;
                            case 3: 
                                print("run");
                                break;
                            default: break;
                        }
                    }
                    else if (playerPokemon.Pokemon.FightMoves[selected].PP > 0)
                    {
                        battleDialog.fightMovesSwitch(false);
                        battleDialog.dialogSwitch(true);
                        StartCoroutine(PlayerMove());
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (selectionMode == SelectionMode.PreBattle) { }
                    else if (selectionMode == SelectionMode.InBattle)
                    {
                        battleDialog.fightMovesSwitch(false);
                        battleDialog.dialogSwitch(true);
                        StartCoroutine(BattleOptions());
                    }
                }
            }
            catch { }
        }

    }

    void ChoosePokemonScreen()
    {
        choosePokemon.Set(playerPokemons.Pokemons);
        choosePokemon.gameObject.SetActive(true);
    }

    int GetSelectedOption()
    {
        switch (currentSelection[0])
        {
            case 0:
                if (currentSelection[1] == 0) return 0;
                else return 2;
            case 1:
                if (currentSelection[1] == 0) return 1;
                else  return 3;
            default:
                return 0;
        }
    }

   
}