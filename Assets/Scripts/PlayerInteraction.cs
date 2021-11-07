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
    private int numberOfBugs = 3;

    [SerializeField]
    private bool displayInteractionSphereGizmo = true;

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

            if(interactable.GetType() == typeof(BugAttachment))
            {
                InteractWithBugAttachment((BugAttachment)interactable);
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Put bug on person or take it off, if it already is bugged
    /// </summary>
    private void InteractWithBugAttachment(BugAttachment bugAttachment)
    {
        if (bugAttachment.HasBugAttached())
        {
            bugAttachment.RemoveBug();
            numberOfBugs++;
        }
        else
        {
            if (numberOfBugs <= 0)
            {
                return;
            }
            bugAttachment.AddBug(transform.position);
            numberOfBugs--;
        }
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
