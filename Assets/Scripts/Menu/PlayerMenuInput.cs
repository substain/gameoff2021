using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInput : MonoBehaviour
{
    public static PlayerMenuInput Instance = null;
    public const float NAV_THRESHOLD = 0.5f;

    private bool isEnabled;
    private bool focusInputFinished = true;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
        }
        Instance = this;
    }

    public void SetEnabled(bool isEnabled)
    {
        this.isEnabled = isEnabled;
    }

    public void ProcessBackInput(InputAction.CallbackContext context)
    {
        if (!isEnabled || !context.performed)
        {
            return;
        }
        UIManager.Instance.UseBackInMenu();
    }

    public void ProcessUseInput(InputAction.CallbackContext context)
    {

        if (!isEnabled || !context.performed)
        {
            return;
        }
        UIManager.Instance.UseFocusedInMenu();
    }

    public void ProvideDirectionalInput(InputAction.CallbackContext context)
    {
        if (!isEnabled)
        {
            return;
        }
        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.magnitude > NAV_THRESHOLD)
        {
            if (focusInputFinished)
            {
                ProcessInputs(Util.ToDir4(direction));
                focusInputFinished = false;
            }
        }
        else
        {
            focusInputFinished = true;
        }
    }

    public void ProcessInputs(Util.Dir4 dir)
    {
        UIManager.Instance.NavigateDirectional(dir);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
