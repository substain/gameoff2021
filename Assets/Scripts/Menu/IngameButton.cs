using UnityEngine;
using UnityEngine.EventSystems;

public class IngameButton: ButtonScript
{
    private ISelectableMenu parent;
    private int buttonIndex = 0;

    [SerializeField]
    private MenuNavigationTarget navigationTarget;

    void Start()
    {
        parent = transform.parent.GetComponent<ISelectableMenu>();
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

    public MenuNavigationTarget GetNavigationTarget()
    {
        return this.navigationTarget;
    }
}