using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatisfyConstraintZone : MonoBehaviour
{
    [SerializeField]
    private ConstraintManager.GameConstraint constraint;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ConstraintManager.Instance.SetSatisfied(constraint);
        }
    }
}
