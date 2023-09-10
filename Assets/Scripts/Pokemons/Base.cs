using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create New Pokemon")] 

public class Base : ScriptableObject
{
    [SerializeField] string pokemonName;

    [TextArea] 
    [SerializeField] string description;
    [SerializeField] public Sprite front;
    [SerializeField] public Sprite back;
    [SerializeField] public PokemonType mainType;
    [SerializeField] public PokemonType secondaryType;

    // stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int specialAttack;
    [SerializeField] int specialDefence;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMoves> learnableFightMoves;

    public string Name { get { return pokemonName; } }
    public string Description { get { return description; } }
    public int MaxHp { get { return maxHp; } }
    public int Attack { get { return attack; } }
    public int Defence { get { return defence; } }
    public int SpecialAttack { get { return specialAttack; } }
    public int SpecialDefence { get { return specialDefence; } }
    public int Speed { get { return speed; } } 
    public List<LearnableMoves> LearnableFightMoves { get { return learnableFightMoves; } }
}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] BaseFightMoves baseFightMoves;
    [SerializeField] int level;

    public BaseFightMoves BaseFightMoves { get { return baseFightMoves; } }
    public int Level { get { return level; } }

}

public enum PokemonType
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
    None
}

public class EffectivnessChart
{
    static float[][] chart = 
    {
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f ,1f ,1f ,1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f}, // normal
        new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f ,1f ,1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f}, // fire
        new float[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f ,1f ,2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f}, // water
        new float[] { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f ,1f ,0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f}, // electric
        new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f ,0.5f ,2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f}, // grass
        new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f ,1f ,2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f}, // ice
        new float[] { 2f, 1f, 1f, 1f, 1f, 2f, 1f ,0.5f ,1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f}, // fighting
        new float[] { 1f, 1f, 1f, 1f, 2f, 1f, 1f ,0.5f ,0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f}, // poison
        new float[] { 1f, 2f, 1f, 12, 0.5f, 1f, 1f ,2f ,1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f}, // ground
        new float[] { 1f, 1f, 1f, 0.5f, 2f, 1f, 2f ,1f ,1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f}, // flying
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f ,2f ,1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f}, // psychic
        new float[] { 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f ,0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f}, // bug
        new float[] { 1f, 2f, 1f, 1f, 1f, 2f, 0.5f ,1f ,0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f}, // rock
        new float[] { 0f, 1f, 1f, 1f, 1f, 1f, 1f ,1f ,1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f}, // ghust
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f ,1f ,1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f}, // dragon
        new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f ,1f ,1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f}, // dark
        new float[] { 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f ,1f ,1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f}, // steel
        new float[] { 1f, 0.5f, 1f, 1f, 1f, 1f, 2f ,0.5f ,1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f}, // fairy
    };

    public static float GetEffectivness(PokemonType attackType, PokemonType defenceType)
    {
        if (attackType == PokemonType.None || defenceType == PokemonType.None) return 1;

        return chart[(int)attackType][(int)defenceType];
    }
}
