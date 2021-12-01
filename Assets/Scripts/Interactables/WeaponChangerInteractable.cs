using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangerInteractable : MonoBehaviour, IInteractable
{
    public string GetInteractionTypeString()
    {
        return "find the weather changer";
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        ConstraintManager.Instance.SetSatisfied(ConstraintManager.GameConstraint.finishLevel);
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
