using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Pokemon
{

    [SerializeField] Base pokemonBase;
    [SerializeField] int level;

    public Base PokemonBase { get { return pokemonBase; } }
    public int Level { get { return level; } }
    public int HP { get; set; }
    public int XP { get; set; }
    public List<FightMove> FightMoves { get; set; }

    public void Init()
    {
        HP = GetMaxHp();
        XP = CalculateLevelXp(level);
        FightMoves = new List<FightMove>();
        foreach (var learnablefightMove in PokemonBase.LearnableFightMoves)
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

    public int GetAttack(bool isSpecial) {
        return CalculateAttribute(isSpecial ? PokemonBase.SpecialAttack : PokemonBase.Attack, 0);
    }

    public int GetDefence(bool isSpecial)
    {
        return CalculateAttribute(isSpecial ? PokemonBase.SpecialDefence : PokemonBase.Defence, 0);
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
        bool isSpecialAttack = fightMove.Base.IsSpecial();
        float modifiers = Random.Range(0.85f, 1f) * effectivness * critical;
        float attack = (2 * pokemon.Level + 10) / 250f;
        float defence = attack * (float)fightMove.Base.Power * ((float)pokemon.GetAttack(isSpecialAttack) / (float)GetDefence(isSpecialAttack)) + 2;
        int damage = Mathf.FloorToInt(defence * modifiers);

        if (fightMove.Base.Power == 0) damage = 0;
        if (damage >= HP)
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

    public LearnableMoves GetLearnableMoves()
    {
        return PokemonBase.LearnableFightMoves.Find(move => move.Level == level);
    }

    public void LearnMove(LearnableMoves fightMove)
    {
        if (FightMoves.Count > 4) return;

        FightMoves.Add(new FightMove(fightMove.BaseFightMoves));
    }

    public void LearnMove(LearnableMoves fightMove, FightMove fightMoveToReplace)
    {
        int index = FightMoves.FindIndex(move => move.Equals(fightMoveToReplace));
        
        FightMoves[index] = new FightMove(fightMove.BaseFightMoves);
    }

    public void Heal(float percentage)
    {
        HP = Mathf.FloorToInt(GetMaxHp() * percentage);
        ResetPP();
    }

    public void ResetPP()
    {
        foreach(FightMove fightMove in this.FightMoves)
        {
            fightMove.PP = fightMove.Base.PP;
        }
    }

    public bool CheckIsLevelUp()
    {
        int levelUpXp = CalculateLevelXp(level + 1);
        if(XP >= levelUpXp)
        {
            float hpPercentage = (float)HP / this.GetMaxHp();
            level++;
            HP = Mathf.FloorToInt(this.GetMaxHp() * hpPercentage);
            return true;
        }
        return false;
    }

    public int CalculateLevelXp(int level)
    {
        switch (pokemonBase.GrowthRate)
        {
            case GrowthRate.Fast:
                return Mathf.FloorToInt((4 * Mathf.Pow(level, 3)) / 5);
            case GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
            case GrowthRate.MediumSlow:
                return Mathf.FloorToInt((6 * Mathf.Pow(level, 3) / 5) - 15 * Mathf.Pow(level, 2) + 100 * level - 140);
            case GrowthRate.Slow:
                return Mathf.FloorToInt((5 * Mathf.Pow(level, 3)) / 4);
            default:
                return int.MaxValue;
        }
    }
}