using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointZone : MonoBehaviour
{
    [SerializeField]
    private int checkPointId;

    [SerializeField]
    private bool allowCheckpointOnExit = true;

    [SerializeField]
    private ConstraintManager.GameConstraint requiredConstraint = ConstraintManager.GameConstraint.none;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SaveCheckpoint(other.gameObject);
        }
    }

    
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && allowCheckpointOnExit)
        {
            SaveCheckpoint(other.gameObject);
        }
    }

    private void SaveCheckpoint(GameObject target)
    {
        if (requiredConstraint == ConstraintManager.GameConstraint.none || ConstraintManager.Instance.IsSatisfied(requiredConstraint))
        {
            PlayerInteraction playerInteraction = target.GetComponentInParent<PlayerInteraction>();
            HashSet<int> obtainedKeyIds = playerInteraction == null ? new HashSet<int>() : playerInteraction.GetObtainedKeyIds();
            CheckpointManager.SetCheckpointReached(checkPointId, target.transform.position, obtainedKeyIds);
        }
    }
}
