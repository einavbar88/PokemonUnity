using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] SceneSwitchers sceneSwitcher;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                StartCoroutine(sceneSwitcher.StartGame());
            }
        }
    }
}
