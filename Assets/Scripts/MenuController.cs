using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    IEnumerator Start()
    {
        Screen.SetResolution(800, 600, false);
        yield return true;
    }

    void Update()
    {

    }

    private IEnumerator WaitUntilFrameLoad()
    {
        yield return new WaitForEndOfFrame();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("game", LoadSceneMode.Single);
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("howtoplay", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
