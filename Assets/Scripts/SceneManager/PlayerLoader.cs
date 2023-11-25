using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    [SerializeField] GameObject playerObjectPrefab;

    private void Awake()
    {
        var existing = FindObjectOfType<PlayerObjects>();
        if(existing == null)
        {
            Instantiate(playerObjectPrefab, new Vector3(13.5f, -7f, 0), Quaternion.identity);
        }
    }
}
