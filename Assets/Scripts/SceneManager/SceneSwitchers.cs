using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitchers : MonoBehaviour
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] int prevScene = -1;
    [SerializeField] Transform spawnPoint;
    Fade fade;
    Player player;

    void Start()
    {
        fade = FindObjectOfType<Fade>();
    }

    public void OnEnter(Player player)
    {
        this.player = player;
        StartCoroutine(Switch());
    }

    IEnumerator Switch()
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        yield return fade.FadeIn(0.25f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);


        var destinations = FindObjectsOfType<SceneSwitchers>();
        foreach(var destination in destinations)
        {
            if (destination.sceneToLoad == prevScene && destination.prevScene == sceneToLoad)
            {
                player.transform.position = destination.spawnPoint.position;
                break;
            }
        }
        GameController.Instance.PauseGame(false);
        yield return fade.FadeOut(0.25f);

        Destroy(gameObject);
    }
}
