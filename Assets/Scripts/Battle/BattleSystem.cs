using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState { Start, BattleOptions, ChoosePokemon, FightMoves, Opponent, Busy, LearnNewMove };

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleVisuals playerPokemon;
    [SerializeField] BattleText playerPokemonText;
    [SerializeField] BattleVisuals opponentPokemon;
    [SerializeField] BattleText opponentPokemonText;
    [SerializeField] BattleDialog battleDialog;
    [SerializeField] ChoosePokemonScript choosePokemon;
    [SerializeField] Image opponentTrainer;
    [SerializeField] LearnNewMove learnNewMove;

    public event Action<bool> OnEndOfBattle;

    PokemonsOwned playerPokemons;
    PokemonsOwned opponentPokemons;
    Pokemon wildPokemon;
    Trainer opponent;
    bool isWildPokemon;
    bool isBattleOver;
    LearnableMoves newMove;

    BattleState state;
    int[] currentSelection = { 0, 0 };
    int lastSelection = 0;
    int[] currentMoveSelection = { 0, 0 };
    int lastSelectedMove = 0;
    int[] newPokemonSelection = { 0, 0 };
    int[] forgetMoveSelection = { 0, 0 };

    private void Clear()
    {
        isBattleOver = false;
        lastSelection = 0;
        lastSelectedMove = 0;

        wildPokemon = null;
        opponentPokemons = null;
        newMove = null;

        opponentTrainer.gameObject.SetActive(false);

        currentSelection[0] = 0;
        currentSelection[1] = 0;
        currentMoveSelection[0] = 0;
        currentMoveSelection[1] = 0;
        newPokemonSelection[0] = 0;
        newPokemonSelection[1] = 0;
    }

    public void StartBattle(PokemonsOwned playerPokemons, Pokemon wildPokemon)
    {
        this.playerPokemons = playerPokemons;
        this.wildPokemon = wildPokemon;
        isWildPokemon = true;
        StartCoroutine(SetupBattle());
    }

    public void StartBattle(PokemonsOwned playerPokemons, PokemonsOwned opponentPokemons)
    {
        this.playerPokemons = playerPokemons;
        this.opponentPokemons = opponentPokemons;
        isWildPokemon = false;

        opponent = opponentPokemons.GetComponent<Trainer>();

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleState.BattleOptions:
                HandleMoveSelection(SelectionMode.PreBattle);
                break;
            case BattleState.FightMoves:
                HandleMoveSelection(SelectionMode.InBattle);
                break;
            case BattleState.ChoosePokemon:
                HandleChoosePokemon();
                break;
            case BattleState.LearnNewMove:
                HandleChooseForgetMove();
                break;
            default:
                break;
        }
    }
   
    public IEnumerator SetupBattle()
    {
        playerPokemon.Set(playerPokemons.GetNotFaintedPokemon());
        playerPokemonText.Set(playerPokemon.Pokemon);
        if (isWildPokemon)
        {
            opponentPokemon.Set(wildPokemon);
            opponentPokemonText.Set(opponentPokemon.Pokemon);
            choosePokemon.Init();

            battleDialog.dialogSwitch(true);
            battleDialog.SetMovesText(playerPokemon.Pokemon.FightMoves);
            battleDialog.SetTypeAndPP(playerPokemon.Pokemon.FightMoves[0]);

            yield return battleDialog.TypeText($"A wild {opponentPokemon.Pokemon.PokemonBase.Name} appeared!");
        } else
        {
            opponentPokemonText.gameObject.SetActive(false);
            opponentPokemon.gameObject.SetActive(false);
            opponentTrainer.gameObject.SetActive(true);

            opponentTrainer.sprite = opponent.TrainerSprite;
           
            yield return battleDialog.TypeText(opponent.PreBattleText);
            yield return OpponentSwitchPokemon();
            choosePokemon.Init();
            battleDialog.SetMovesText(playerPokemon.Pokemon.FightMoves);
            battleDialog.SetTypeAndPP(playerPokemon.Pokemon.FightMoves[0]);
        }

        yield return new WaitForSeconds(0.7f);
        yield return BattleOptions();

    }

    public IEnumerator BattleOptions()
    {
        state = BattleState.BattleOptions;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(battleDialog.TypeText("What Will You Do?"));
        yield return new WaitForSeconds(0.7f);
        battleDialog.battleOptionsSwitch(true, lastSelection);
    }

    public void FightMoves()
    {
        state = BattleState.FightMoves;
        battleDialog.battleOptionsSwitch(false, lastSelection);
        battleDialog.dialogSwitch(false);
        battleDialog.fightMovesSwitch(true, lastSelectedMove);
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

        lastSelectedMove = GetSelectedOption(currentMoveSelection);
        var fightMove = isPlayerTurn ? attackingPokemon.FightMoves[lastSelectedMove] : attackingPokemon.GetRandomFightMove();

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
                int xp = opponentPokemon.Pokemon.PokemonBase.XPGiven;
                int level = opponentPokemon.Pokemon.Level;
                float trainerBonus = isWildPokemon? 1f : 1.5f;
                int xpGained = Mathf.FloorToInt((xp * level * trainerBonus) / 7);

                attackingPokemon.XP += xpGained;

                yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} gained {xpGained}XP");
                yield return playerPokemonText.UpdateXp(false);

                while (attackingPokemon.CheckIsLevelUp())
                {
                    playerPokemonText.SetLevel();
                    yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} leveled up to level {attackingPokemon.Level}!");
                    yield return playerPokemonText.UpdateXp(true);

                    newMove = playerPokemon.Pokemon.GetLearnableMoves();

                    if (newMove != null)
                    {
                        if(playerPokemon.Pokemon.FightMoves.Count < 4)
                        {
                            playerPokemon.Pokemon.LearnMove(newMove);
                            yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} Learned {newMove.BaseFightMoves.Name}");
                        }
                        else
                        {
                            yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} is trying to learn {newMove.BaseFightMoves.Name}. Which move you wish to forget?");
                            learnNewMove.Init(playerPokemon.Pokemon);
                            learnNewMove.gameObject.SetActive(true);
                            learnNewMove.UpdateSelection(0);
                            state = BattleState.LearnNewMove;
                            yield return new WaitForSeconds(1.5f);
                            yield return WaitForKeyDown.Key(KeyCode.Space);
                            yield return battleDialog.TypeText($"{attackingPokemon.PokemonBase.Name} Learned {newMove.BaseFightMoves.Name}");
                        }
                    }
                }

                if (isWildPokemon) isBattleOver = true;
                else
                {
                    yield return OpponentSwitchPokemon();
                    if (!isBattleOver) StartCoroutine(BattleOptions());
                }
          }
          else
          {
                var nextPokemon = playerPokemons.GetNotFaintedPokemon();
                if (nextPokemon != null) ChoosePokemonScreen();
                else isBattleOver = true;
          }

        }
        else  
        {
          isBattleOver = false;
          if (isPlayerTurn) StartCoroutine(OpponentMove());
          else yield return BattleOptions();
        }

        if (isBattleOver)
        {
            if(!isWildPokemon)
            {
                opponentTrainer.gameObject.SetActive(true);
                yield return battleDialog.TypeText(opponent.PostBattleText);
                yield return WaitForKeyDown.Key(KeyCode.Space);
                if (opponent.IsCriminal) opponent.Arrest();
                Clear();
            }; 
            OnEndOfBattle(isPlayerTurn);
        }
    }
   
    void HandleMoveSelection(SelectionMode selectionMode)
    {     
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(selectionMode == SelectionMode.InBattle) currentMoveSelection[0] = 0;
                else currentSelection[0] = 0;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectionMode == SelectionMode.InBattle) currentMoveSelection[0] = 1;
                else currentSelection[0] = 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectionMode == SelectionMode.InBattle) currentMoveSelection[1] = 0;
                else currentSelection[1] = 0;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectionMode == SelectionMode.InBattle) currentMoveSelection[1] = 1;
                else currentSelection[1] = 1;
            }

            int selected = selectionMode == SelectionMode.InBattle ?
                GetSelectedOption(currentMoveSelection) :
                GetSelectedOption(currentSelection);

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
                if (battleDialog.IsTyping) return;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (selectionMode == SelectionMode.PreBattle)
                    {
                        switch (selected)
                        {
                            case 0:
                                battleDialog.fightMovesSwitch(true, lastSelectedMove);
                                FightMoves();
                                break;
                            case 1:
                                battleDialog.dialogSwitch(true);
                                ChoosePokemonScreen();
                                break;
                            case 2:
                                battleDialog.dialogSwitch(true);
                                battleDialog.Set("Your bag is Empty!");
                                StartCoroutine(BattleOptions());
                                break;
                            case 3:
                                battleDialog.dialogSwitch(true);
                                StartCoroutine(Run());
                                break;
                            default: break;
                        }
                        lastSelection = selected;
                    }
                    else if (playerPokemon.Pokemon.FightMoves[selected].PP > 0)
                    {
                        battleDialog.fightMovesSwitch(false, lastSelectedMove);
                        battleDialog.dialogSwitch(true);
                        StartCoroutine(PlayerMove());
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (selectionMode == SelectionMode.PreBattle) { }
                    else if (selectionMode == SelectionMode.InBattle)
                    {
                        battleDialog.fightMovesSwitch(false, lastSelectedMove);
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
        state = BattleState.ChoosePokemon;
        choosePokemon.Set(playerPokemons.Pokemons);
        choosePokemon.gameObject.SetActive(true);
    }

    int GetSelectedOption(int[] selection)
    {
        switch (selection[0])
        {
            case 0:
                if (selection[1] == 0) return 0;
                else return 2;
            case 1:
                if (selection[1] == 0) return 1;
                else  return 3;
            default:
                return 0;
        }
    }

    void HandleChoosePokemon()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(playerPokemons.Pokemons.Count > 2)
                {
                    newPokemonSelection[1] = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (playerPokemons.Pokemons.Count > 2)
                {
                    newPokemonSelection[1] = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                newPokemonSelection[0] = 0;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                newPokemonSelection[0] = 1;
            }

            int selected = GetSelectedOption(newPokemonSelection);
            choosePokemon.UpdateSelection(selected);

            try
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    var selectedPokemon = playerPokemons.Pokemons[GetSelectedOption(newPokemonSelection)];
                    if (selectedPokemon.HP <= 0)
                    {
                        choosePokemon.SetText("Pokemon is fainted!");
                        return;
                    }
                    if (selectedPokemon == playerPokemon.Pokemon)
                    {
                        choosePokemon.SetText("Pokemon is already out!");
                        return;
                    }

                    choosePokemon.gameObject.SetActive(false);
                    state = BattleState.Busy;
                    StartCoroutine(SwitchPokemon(selectedPokemon));
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (playerPokemon.Pokemon.HP > 0)
                    {
                        choosePokemon.gameObject.SetActive(false);
                        StartCoroutine(BattleOptions());
                    }
                    else
                    {
                        choosePokemon.SetText("You must choose a pokemon!");
                    }
                }
            }
            catch { }
        }

    }

    void HandleChooseForgetMove()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                forgetMoveSelection[0] = 0;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                forgetMoveSelection[0] = 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                forgetMoveSelection[1] = 0;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                forgetMoveSelection[1] = 1;
            }
            int selected = GetSelectedOption(forgetMoveSelection);
            learnNewMove.UpdateSelection(selected);
            try
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    FightMove selectedmove = playerPokemon.Pokemon.FightMoves[selected];
                    playerPokemon.Pokemon.LearnMove(newMove, selectedmove);
                    learnNewMove.gameObject.SetActive(false);
                }
            }
            catch { }
        }

    }

    IEnumerator SwitchPokemon(Pokemon pokemon)
    {
        if(playerPokemon.Pokemon.HP > 0)
        {
            yield return battleDialog.TypeText($"{playerPokemon.Pokemon.PokemonBase.Name} come back!");
            playerPokemon.FaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerPokemon.Set(pokemon);
        playerPokemonText.Set(pokemon);
        battleDialog.SetMovesText(pokemon.FightMoves);

        yield return battleDialog.TypeText($"Go {pokemon.PokemonBase.Name}!");

        StartCoroutine(OpponentMove());
    }

    IEnumerator OpponentSwitchPokemon()
    {
        var opponentChosenPokemon = opponentPokemons.GetNotFaintedPokemon();
        if (opponentChosenPokemon != null)
        {
            opponentTrainer.gameObject.SetActive(false);
            opponentPokemon.gameObject.SetActive(true);
            opponentPokemon.Set(opponentChosenPokemon);
            yield return battleDialog.TypeText($"{opponent.TrainerName} chose {opponentPokemon.Pokemon.PokemonBase.Name}");
            opponentPokemonText.Set(opponentPokemon.Pokemon);
            opponentPokemonText.gameObject.SetActive(true);
            battleDialog.dialogSwitch(true);
        }
        else isBattleOver = true;
    }

    IEnumerator Run()
    {
        state = BattleState.Busy;
        if (!isWildPokemon)
        {
            yield return battleDialog.TypeText("Cannot run while fighting another trainer!!");
            state = BattleState.BattleOptions;
            StartCoroutine(BattleOptions());
            yield break;
        }

        yield return battleDialog.TypeText("Ran away safetly!");
        yield return new WaitForSeconds(1f);

        Clear();
        OnEndOfBattle(true);
    }
}

