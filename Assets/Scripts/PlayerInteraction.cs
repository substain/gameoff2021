using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

    [SerializeField]
    private GameObject interactionPosition;

    [SerializeField]
    private float interactionRadius = 2f;

    [SerializeField]
    private int numberOfBugs = 2;

    [SerializeField]
    private bool displayInteractionSphereGizmo = true;

    [SerializeField]
    private List<int> obtainedKeyIds;

    void Awake()
    {
        if(interactionPosition == null)
        {
            Debug.LogWarning("no interactionPosition assigned!");
        }
    }

    void Update()
    {
        ProcessInputs();
    }

    public bool HasKeyWithId(int value)
    {
        throw new NotImplementedException();
    }

    private void ProcessInputs()
    {
        if (Input.GetKeyDown(interactKey))
        {
            bool hasInteracted = TryInteract();
            if (hasInteracted)
            {
                return;
            }
        }
    }

    private bool TryInteract()
    {
        Collider[] matchingColliders = Physics.OverlapSphere(interactionPosition.transform.position, interactionRadius);

        foreach (Collider col in matchingColliders)
        {
            IInteractable interactable = col.gameObject.GetComponent<IInteractable>();
            if (interactable == null)
            {
                continue;
            }
            interactable.Interact(this);

            return true;
        }
        return false;
    }

    public void RemoveBugFrom(BugAttachment bugAttachment)
    {
        bugAttachment.RemoveBug();
        numberOfBugs++;
    }

    public void PutBugOn(BugAttachment bugAttachment)
    {
        if (numberOfBugs <= 0)
        {
            return;
        }
        bugAttachment.AddBug(transform.position);
        numberOfBugs--;
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!displayInteractionSphereGizmo)
        {
            return;
        }
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawSphere(interactionPosition.transform.position, interactionRadius);
    }

#endif
}
