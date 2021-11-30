using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyOptionsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.DontHideMenuOnStart();
        GameManager.Instance.StartOptionsMenu();
    }
    public void ProcessBackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HUDManager.Instance.IngameMenuUseBack();
        }
    }

    public void ProcessUseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HUDManager.Instance.IngameMenuUseSelected();
        }
    }

}
