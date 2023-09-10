using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] Base pokemonBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayer;

    public Pokemon Pokemon { get; set; }

    Image pokemonSprite;
    Vector2 originalPosition;
    Color originalColor;

    private void Awake()
    {
        pokemonSprite = GetComponent<Image>();
        originalPosition = pokemonSprite.transform.localPosition;
        originalColor = pokemonSprite.color;
    }

    public void Set()
    {
        Pokemon = new Pokemon(pokemonBase, level);
        if (isPlayer) pokemonSprite.sprite = Pokemon.PokemonBase.back;
        else pokemonSprite.sprite = Pokemon.PokemonBase.front;
        EnterAnimation();
    }

    public void EnterAnimation()
    {
        if(isPlayer) pokemonSprite.transform.localPosition = new Vector2(-500f, originalPosition.y);
        else pokemonSprite.transform.localPosition = new Vector2(500f, originalPosition.y);

        pokemonSprite.transform.DOLocalMoveX(originalPosition.x, 1f);

    }

    public void AttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer) sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPosition.x + 50f, 0.25f));
        else sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPosition.x - 50f, 0.25f));

        sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPosition.x, 0.25f));
    }

    public void HitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(pokemonSprite.DOColor(originalColor, 0.1f));
        sequence.Append(pokemonSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(pokemonSprite.DOColor(originalColor, 0.1f));
        sequence.Append(pokemonSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(pokemonSprite.DOColor(originalColor, 0.1f));
    }
}

