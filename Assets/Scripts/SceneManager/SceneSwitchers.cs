using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchers : MonoBehaviour
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] int sceneLoadedFrom = -1;
    [SerializeField] Transform spawnPoint;

    Fade fade;

    void Start()
    {
        fade = FindObjectOfType<Fade>();    
    }

    public void OnEnter(Player player)
    {
        StartCoroutine(Switch(player));
    }

    IEnumerator Switch(Player player)
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        yield return fade.FadeIn(0.25f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        SceneSwitchers[] destinations = FindObjectsOfType<SceneSwitchers>();
        foreach(SceneSwitchers destination in destinations)
        {
            if(destination.sceneToLoad == this.sceneLoadedFrom && this.sceneToLoad == destination.sceneLoadedFrom)
            {
                player.transform.position = destination.spawnPoint.position;
                break;
            }
        }
        yield return fade.FadeOut(0.25f);

        GameController.Instance.PauseGame(false);

        Destroy(gameObject);
    }

    public IEnumerator StartIntro()
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(7);
        Destroy(gameObject);
    }

    public IEnumerator StartGame()
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(1);
        Destroy(gameObject);
    }

    public IEnumerator SpawnInPokeCenter(Player player)
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        yield return fade.FadeIn(0.25f);

        yield return SceneManager.LoadSceneAsync(3);
       
        SceneSwitchers[] destinations = FindObjectsOfType<SceneSwitchers>();
        foreach (SceneSwitchers destination in destinations)
        {
            if (destination.sceneToLoad == 2 && destination.sceneLoadedFrom == 3)
            {
                player.transform.position = destination.spawnPoint.position;
                break;
            }
        }

        List<Pokemon> pokemonsToHeal = player.GetComponent<PokemonsOwned>().Pokemons;
        foreach (Pokemon pokemon in pokemonsToHeal)
        {
            pokemon.Heal(1);
        }

        yield return fade.FadeOut(0.25f);
        GameController.Instance.PauseGame(false);

        Destroy(gameObject);
    }
}
