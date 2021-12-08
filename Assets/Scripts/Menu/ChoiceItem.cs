using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceItem : MenuItem
{
    private int chosenIndex = 0;

    private bool isChosen;

    public Action actionToDo;

    [SerializeField]
    private GameObject isChosenObject;

    private readonly Color selectedTextColor = HUDDialogueDisplay.GetPersonColor(Person.Sting);
    private readonly Color unselectedTextColor = Color.white;

    private ISelectableMenu parent;

    private Text buttonText;
    //private Image backgroundImage;


    void Awake()
    {
        this.buttonText = GetComponentInChildren<Text>();

        parent = transform.parent.GetComponent<ISelectableMenu>();
        buttonText.color = unselectedTextColor;
    }

    public override void SetFocused(bool isFocused)
    {
        throw new NotImplementedException();
    }

    public override void UseFocused()
    {
        throw new NotImplementedException();
    }

    public override void SelectNext()
    {
        throw new NotImplementedException();
    }

    public override void SelectPrevious()
    {
        throw new NotImplementedException();
    }

    public override UIManager.MenuSoundType GetSoundType()
    {
        throw new NotImplementedException();
    }
    /*
protected override void Start()
{
   base.Start();
}

public override void InitSelection()
{
   if (parent != null && isStartButton)
   {
       SelectControlled();
   }
   else
   {
       base.InitSelection();
   }
}

public override void Activate()
{
   actionToDo?.Invoke();
}

public override void Select(bool playSound = true)
{
   SetSelected(true);

   base.Select(playSound);
}

public override void Deselect()
{
   SetSelected(false);
   base.Deselect();
}
public void SetSelected(bool isSelected)
{
   buttonText.color = isSelected ? selectedTextColor : unselectedTextColor;
   //this.backgroundImage.color = isSelected ? selectedBackgroundColor : unselectedBackgroundColor;
}

public void SetIndex(int i)
{
   buttonIndex = i;
}

public void SelectControlled()
{
   parent?.SetIndexSelected(buttonIndex);
}

public void OnPointerEnter(PointerEventData eventData)
{
   SelectControlled();
}    */

}
