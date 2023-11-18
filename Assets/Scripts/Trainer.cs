using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] Sprite sprite;
    [SerializeField] string preBattleText;
    [SerializeField] string trainerName;

    public string TrainerName { get {return trainerName; } }
    public Sprite TrainerSprite { get { return sprite; } }
    public string PreBattleText { get { return preBattleText; } }


    public IEnumerator Interact()
    {
        yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
        
    }
}
