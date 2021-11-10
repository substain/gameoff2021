using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// author: Witchhat

public class MainMenuFunctions : MonoBehaviour {

    [SerializeField] private string gameScene;
    [SerializeField] private string optionsScene;

    [SerializeField] private KeyCode upKey1 = KeyCode.W;
    [SerializeField] private KeyCode upKey2 = KeyCode.UpArrow;

    [SerializeField] private KeyCode downKey1 = KeyCode.S;
    [SerializeField] private KeyCode downKey2 = KeyCode.DownArrow;

    [SerializeField] private KeyCode selectKey1 = KeyCode.Return;
    [SerializeField] private KeyCode selectKey2 = KeyCode.E;

    [SerializeField] ButtonScript[] ButtonGroup;
    private int currentButton;

    public void QuitGame() {
        Application.Quit();
    }

    public void ChangeToGameScene() {
        SceneManager.LoadScene(gameScene);
    }

    public void ChangeToOptionsScene() {
        SceneManager.LoadScene(optionsScene);
    }

    void Update() {
        ProcessInputs();
    }

    void Start() {
        foreach ( ButtonScript bs in ButtonGroup) {
            bs.Deselect();
        }
        currentButton = ButtonGroup.Length - 1;
        ButtonGroup[currentButton].Select();
    }

    private void ProcessInputs() {

        bool up = false;
        bool down = false;
        if ( Input.GetKeyDown(upKey1) || Input.GetKeyDown(upKey2)) {
            up = true;
        }

        if (Input.GetKeyDown(downKey1) || Input.GetKeyDown(downKey2)) {
            down = true;
        }

        if (Input.GetKeyDown(selectKey1) || Input.GetKeyDown(selectKey2)) {
            select();
        }

        if (up && ! down) {
            goUp();
        } else
        if (!up && down) {
            goDown();
        }
    }

    private void select() {
        Debug.Log("select func");
        ButtonGroup[currentButton].Activate();
    }

    private void goUp() {
        Debug.Log("up func");
        if (currentButton != ButtonGroup.Length) {
            ButtonGroup[currentButton].Deselect();
            currentButton++;
            ButtonGroup[currentButton].Select();
        }
    }

    private void goDown() {
        Debug.Log("down func");
        if (currentButton != 0) {
            ButtonGroup[currentButton].Deselect();
            currentButton--;
            ButtonGroup[currentButton].Select();
        }
    }

}
