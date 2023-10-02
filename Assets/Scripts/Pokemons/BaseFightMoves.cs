using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fight Move", menuName ="Pokemon/Create New Fight Move")]
public class BaseFightMoves : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    public string Name { get { return moveName; } }
    public string Description { get { return description; } }
    public PokemonType Type { get { return type; } }
    public int Power { get { return power; } }
    public int Accuracy { get { return accuracy; } }
    public int PP { get { return pp; } }
    public bool IsSpecial()
    {
        return type == PokemonType.Dragon || type == PokemonType.Fire || type == PokemonType.Ice || type == PokemonType.Water || type == PokemonType.Electric || type == PokemonType.Grass;
    }
}
