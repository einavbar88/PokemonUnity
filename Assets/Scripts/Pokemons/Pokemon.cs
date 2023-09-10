using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public Base PokemonBase { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<FightMove> FightMoves { get; set; }

    public Pokemon(Base pokemonBase, int level)
    {
        PokemonBase = pokemonBase;
        Level = level;
        HP = GetMaxHp();

        FightMoves = new List<FightMove>();
        foreach (var learnablefightMove in pokemonBase.LearnableFightMoves)
        {
            if (learnablefightMove.Level <= level)
            {
                FightMoves.Add(new FightMove(learnablefightMove.BaseFightMoves));
            }

            if (FightMoves.Count >= 4)
            {
                break;
            }
        }
    }


    public string GetName()
    {
        return PokemonBase.Name;
    }

    public int GetAttack(){
        return CalculateAttribute(PokemonBase.Attack, 0);
    }

    public int GetDefence()
    {
        return CalculateAttribute(PokemonBase.Defence, 0);
    }

    public int GetSpecialAttack()
    {
        return CalculateAttribute(PokemonBase.SpecialAttack, 0);
    }

    public int GetSpecialDefence()
    {
        return CalculateAttribute(PokemonBase.SpecialDefence, 0);
    }

    public int GetSpeed()
    {
        return CalculateAttribute(PokemonBase.Speed, 0);
    }

    public int GetMaxHp()
    {
        return CalculateAttribute(PokemonBase.MaxHp, 5);
    }


    private int CalculateAttribute(int attribute, int extra)
    {
        return Mathf.FloorToInt((attribute * Level) / 100f) + 5 + extra;
    }

    public float RemainingHp(FightMove fightMove, Pokemon pokemon, float critical, float effectivness)
    {
        float modifiers = Random.Range(0.85f, 1f) * effectivness * critical;
        float attack = (2 * pokemon.Level + 10) / 250f;
        float defence = attack * (float)fightMove.Base.Power * ((float)pokemon.GetAttack() / (float)GetDefence()) + 2;
        int damage = Mathf.FloorToInt(defence * modifiers);

        if(fightMove.Base.Power == 0) damage = 0;
        if(damage >= HP)
        {
            HP = 0;
            return 0;
        }

        HP -= damage;

        return HP;
    }

    public FightMove GetRandomFightMove()
    {
        int random = Random.Range(0, FightMoves.Count);
        return FightMoves[random];
    }
}