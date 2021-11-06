using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// author: Witchhat

public class MainMenuFunctions : MonoBehaviour {

    [SerializeField] private string gameScene;
    [SerializeField] private string optionsScene;

    public void QuitGame() {
        Application.Quit();
    }

    public void ChangeToGameScene() {
        SceneManager.LoadScene(gameScene);
    }

    public void ChangeToOptionsScene() {
        SceneManager.LoadScene(optionsScene);
    }


}
