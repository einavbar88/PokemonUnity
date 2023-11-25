using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryObject : MonoBehaviour, StoryObjects
{
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog actionDialog;
    [SerializeField] List<string> requiredItemsIds;
    [SerializeField] string itemId = null;
    [SerializeField] bool isActionRequired;
    [SerializeField] bool destroy;
    [SerializeField] string persistense;
    [SerializeField] string persistenseDisapearForever;

    void Start()
    {
        if (DataPersistance.dp[persistenseDisapearForever])
        {
            gameObject.SetActive(false);
        }

    }

    public IEnumerator Interact(Player player)
    {
        if (DataPersistance.dp[persistense]) yield break;
        else
        {
            if (isActionRequired)
            {
                if (player.HasRequiredItems(requiredItemsIds))
                {
                    foreach (string requiredId in requiredItemsIds)
                    {
                        player.existingItemsIds.Remove(requiredId);
                    }
                    yield return StartCoroutine(GameObjectsDialogs.Instance.Open(actionDialog));
                    DataPersistance.dp[persistense] = true;
                    if (destroy) this.gameObject.SetActive(false);
                    else isActionRequired = false;
                }
                else
                {
                    yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
                }
            }
            else if (itemId != "")
            {
                yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
                player.existingItemsIds.Add(itemId);
                DataPersistance.dp[persistense] = true;
                if (destroy) this.gameObject.SetActive(false);
                else itemId = "";
            }
            else
            {
                yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
            }
        }
    }
}
