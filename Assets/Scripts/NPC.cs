using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, StoryObjects
{
    [SerializeField] Dialog dialog;

    public IEnumerator Interact()
    {
        yield return StartCoroutine(GameObjectsDialogs.Instance.Open(dialog));
        if(this.name == "Squirtle-NPC")
        {
            this.gameObject.SetActive(false);

        }
    }
}
