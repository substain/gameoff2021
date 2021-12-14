using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputKeyHelper : MonoBehaviour
{
    public enum ControlType
    {
        Interact, Move, Dash, Sneak, Run, Back, ListenBug, OpenMenu
    }

    public static InputKeyHelper Instance = null;

    private PlayerInput playerInput;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There is more than one InputKeyHelper in this scene.");
        }
        Instance = this;
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
    }

    public string GetNameForKey(ControlType ctype)
    {
        //int binding = playerInput.actions[ctype.ToString()].GetBindingForControl(control).GetValueOrDefault().effectivePath;
        string bindingName =  playerInput.actions[ctype.ToString()].GetBindingDisplayString();

        return bindingName; //TODO
        //https://forum.unity.com/threads/get-name-of-mapped-key-with-new-input-system.939464/
        //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/ActionBindings.html?_gl=1*1k2d9xx*_ga*OTgyNjM5MDEzLjE1OTM1NTM3Mzg.*_ga_1S78EFL1W5*MTYyOTk5NDMxNy40NDUuMS4xNjI5OTk4MTgxLjI1&_ga=2.114602183.1394560935.1629563507-982639013.1593553738&_gac=1.214180453.1629682290.CjwKCAjw64eJBhAGEiwABr9o2AleTuUMGvEqxEWvNJkDGaee64a6xyJzHE7UgU2il9LRn20gQAdTThoCMKAQAvD_BwE#showing-current-bindings
        //var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup("Gamepad"));
        //m_RebindButton.GetComponentInChildren<Text>().text =
        //action.GetBindingDisplayString(bindingIndex);
        //InputBinding.effectivePath
    }

    private static string ToControlTypePlaceholder(ControlType ctype)
    {
        return "%" + ctype.ToString().ToLower() + "%";
    }

     public string ReplacePlaceholdersWithCurrentKeys(string text)
    {
        foreach (ControlType ctype in Enum.GetValues(typeof(ControlType)))
        {
            string currentKeyName = GetNameForKey(ctype);
            if(currentKeyName != null)
            {
                text = text.Replace(ToControlTypePlaceholder(ctype), currentKeyName);
            }
        }
        return text;
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
