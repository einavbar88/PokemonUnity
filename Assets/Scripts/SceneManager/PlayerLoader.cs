using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    [SerializeField] GameObject playerObjects;
    Vector3 location;

    void Awake()
    {
        location = gameObject.transform.position;
        PlayerObjects instance = FindObjectOfType<PlayerObjects>();
        if (instance == null)
        {
            Instantiate(playerObjects, location, Quaternion.identity);
        }
    }
}
