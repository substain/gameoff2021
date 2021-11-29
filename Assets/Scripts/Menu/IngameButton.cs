using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngameButton: ButtonScript
{
    //private readonly Color selectedBackgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.25f);
    //private readonly Color unselectedBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    private readonly Color selectedTextColor = HUDDialogueDisplay.GetPersonColor(Person.Sting);
    private readonly Color unselectedTextColor = Color.white;

    private ISelectableMenu parent;
    private int buttonIndex = 0;

    private Text buttonText;
    //private Image backgroundImage;

    [SerializeField]
    private MenuNavigationTarget navigationTarget;

    void Awake()
    {
        this.buttonText = GetComponentInChildren<Text>();

        parent = transform.parent.GetComponent<ISelectableMenu>();
        buttonText.color = unselectedTextColor;
    }
    
    protected override void Start()
    {
        base.Start();
    }

    public override void InitSelection()
    {
        if(parent != null && isStartButton)
        {
            SelectControlled();
        }
        else
        {
            base.InitSelection();
        }
    }

    public MenuNavigationTarget GetNavigationTarget()
    {
        return this.navigationTarget;
    }

    void OnEnable()
    {


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
    }
}