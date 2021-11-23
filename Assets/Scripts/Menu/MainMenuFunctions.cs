using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// author: Witchhat

public class MainMenuFunctions : MonoBehaviour {
    public const float NAV_THRESHOLD = 0.5f;

    [SerializeField] private string gameScene;
    [SerializeField] private string optionsScene;

    //[SerializeField] private KeyCode upKey1 = KeyCode.W;
    //[SerializeField] private KeyCode upKey2 = KeyCode.UpArrow;

    //[SerializeField] private KeyCode downKey1 = KeyCode.S;
    //[SerializeField] private KeyCode downKey2 = KeyCode.DownArrow;

    //[SerializeField] private KeyCode selectKey1 = KeyCode.Return;
    //[SerializeField] private KeyCode selectKey2 = KeyCode.E;
    private bool selectionFinished = true;

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

    void Start() {
        foreach ( ButtonScript bs in ButtonGroup) {
            bs.Deselect();
        }
        currentButton = ButtonGroup.Length - 1;
        ButtonGroup[currentButton].Select();
    }

    public void ProvideDirectionalInput(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        if(direction.magnitude > NAV_THRESHOLD)
        {
            if (selectionFinished)
            {
                ProcessInputs(Util.ToDir4(direction));
                selectionFinished = false;
            }
        }
        else
        {
            selectionFinished = true;
        }
    }

    public void ProvideAcceptInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            select();
        }
    }

    public void ProvideBackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //back or cancel button, if needed
        }
    }

    public void ProcessInputs(Util.Dir4 dir) {
        //Debug.Log(dir.ToString());
        bool up = false;
        bool down = false;
        if (dir == Util.Dir4.North) {
            up = true;
        }

        if (dir == Util.Dir4.South)
        {
            down = true;
        }

        if (up && ! down) {
            goUp();
        } else
        if (!up && down) {
            goDown();
        }
    }

    private void select() {
        //Debug.Log("select func");
        ButtonGroup[currentButton].Activate();
    }

    private void goUp() {
        //Debug.Log("up func");
        if (currentButton != ButtonGroup.Length - 1) { 
            ButtonGroup[currentButton].Deselect();
            currentButton++;
            ButtonGroup[currentButton].Select();
        }
    }

    private void goDown() {
        //Debug.Log("down func");
        if (currentButton != 0) {
            ButtonGroup[currentButton].Deselect();
            currentButton--;
            ButtonGroup[currentButton].Select();
        }
    }

}
