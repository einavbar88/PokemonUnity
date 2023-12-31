using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour, StoryObjects
{
    [SerializeField] Dialog dialog;
    [SerializeField] Sprite sprite;
    [SerializeField] string preBattleText;
    [SerializeField] string postBattleText;
    [SerializeField] string trainerName;
    [SerializeField] bool isCriminal;
    [SerializeField] string persistense;
    [SerializeField] string persistenseDisapearForever;
     
    void Start()
    {
        if (DataPersistance.dp[persistenseDisapearForever])
        {
            gameObject.SetActive(false);
        }

    }
    
    public string TrainerName { get { return trainerName; } }
    public Sprite TrainerSprite { get { return sprite; } }
    public string PreBattleText { get { return preBattleText; } }
    public string PostBattleText { get { return postBattleText; } }
    public bool IsCriminal { get { return isCriminal; } }

    public IEnumerator Interact(Player player)
    {
        if (DataPersistance.dp[persistense]) yield break;
        else
        {
            yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
            GameController.Instance.StartBattle(this);
        }
    }

    public void Arrest()
    {
        DataPersistance.dp[persistense] = true;
        this.gameObject.SetActive(false);
    }
}
