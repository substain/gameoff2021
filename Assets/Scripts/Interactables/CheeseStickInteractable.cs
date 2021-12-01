using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseStickInteractable : MonoBehaviour, IInteractable
{
    private int amountLeft = 5;

    public string GetInteractionTypeString()
    {
        return "eat a cheesestick";
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        amountLeft--;
        switch (amountLeft)
        {
            case 4:
                {
                    ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.ateCheeseStick1);
                    break;
                }
            case 3:
                {
                    ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.ateCheeseStick2);
                    break;
                }
            case 2:
                {
                    ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.ateCheeseStick3);
                    break;
                }
            case 1:
                {
                    ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.ateCheeseStick4);
                    break;
                }
            case 0:
                {
                    ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.lastCheeseStickUsed);
                    break;
                }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
