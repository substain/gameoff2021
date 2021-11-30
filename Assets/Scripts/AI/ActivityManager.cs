using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class ActivityManager : MonoBehaviour
{
    [SerializeField]
    private GameObject controlledObject;

    [SerializeField]
    private bool useRandomOrder = false;

    [SerializeField]
    private int initialActivityIndex = 0;

    [SerializeField]
    private int afterPursueActivity;

    private List<AbstractActivity> orderedActivities = new List<AbstractActivity>();

    private int currentActivityIndex = -1;

    private PursuePlayerActivity pursuePlayerActivity;
    private bool pursuingPlayer = false;

    [SerializeField]
    private BugAttachment bugAttachment;

    private bool isPaused = false;

    void Awake()
    {
        AbstractActivity[] activities = GetComponents<AbstractActivity>(); 

        //search for pursue player activities
        pursuePlayerActivity = (PursuePlayerActivity) activities.FirstOrDefault(activity => activity.GetType() == typeof(PursuePlayerActivity));
        pursuePlayerActivity.Init(controlledObject);

        orderedActivities = activities.OrderBy(activity => activity.GetOrder())
                                        .Where(activity => activity.GetType() != typeof(PursuePlayerActivity))
                                        .ToList();

        orderedActivities.ForEach(x => x.Init(controlledObject));
        currentActivityIndex = Mathf.Min(initialActivityIndex, orderedActivities.Count-1);

        if (orderedActivities.Count == 0)
        {
            Debug.LogWarning("no activities to do");
            return;
        }

    }

    void Start()
    {
        InvokeRepeating("CheckActivityStatus", 0.11f, 0.11f);
        if(orderedActivities.Count == 0 || currentActivityIndex < 0)
        {
            return;
        }

        pursuePlayerActivity.CheckActivationConstraints();
        orderedActivities.ForEach(x => x.CheckActivationConstraints());

        orderedActivities[currentActivityIndex].StartActivity();
        bugAttachment.SetCurrentActivity(orderedActivities[currentActivityIndex]);
        ConstraintManager.OnChangeConstraints += CheckConstraints;
    }

    private void CheckConstraints()
    {
        pursuePlayerActivity.CheckActivationConstraints();
        orderedActivities.ForEach(oa => oa.CheckActivationConstraints());
    }

    private void CheckActivityStatus()
    {
        if (pursuingPlayer && pursuePlayerActivity.CheckIfFinished())
        {
            StopFollowingPlayer();
            return;
        }
        if (orderedActivities.Count == 0 || currentActivityIndex < 0)
        {
            return;
        }
        if (orderedActivities[currentActivityIndex].CheckIfFinished())
        {
            UpdateActiveActivity();
        }
    }

    private void UpdateActiveActivity()
    {
        orderedActivities[currentActivityIndex].StopActivity();
        UpdateToNextActivityIndex();
        orderedActivities[currentActivityIndex].StartActivity();
        bugAttachment.SetCurrentActivity(orderedActivities[currentActivityIndex]);
    }

    private void UpdateToNextActivityIndex()
    {
        List<AbstractActivity> possibleActivities = orderedActivities.Where(oa => oa.HasConstraintsSatisfied()).ToList();
        if (possibleActivities.Count == 0)
        {
            return;
        }

        List<int> possibleIndices = Enumerable.Range(0, possibleActivities.Count).ToList();

        if (useRandomOrder)
        {
            if(possibleIndices.Count > 1)
            {
                possibleIndices.RemoveAt(currentActivityIndex);
            }
            currentActivityIndex = possibleIndices[UnityEngine.Random.Range(0, possibleIndices.Count)];
        }
        else
        {
            int refIndex = possibleIndices.FindIndex(ind => ind == currentActivityIndex);
            refIndex = (refIndex + 1) % possibleIndices.Count;
            currentActivityIndex = possibleIndices[refIndex];
        }
    }

    public void UpdatePlayerPosition(Transform targetTransform)
    {
        if (!pursuePlayerActivity)
        {
            return;
        }
        pursuePlayerActivity.SetTargetPosition(targetTransform.position);
    }

    public void StartPursuePlayer(Transform targetTransform)
    {
        if(!pursuingPlayer)
        {
            pursuingPlayer = true;
            pursuePlayerActivity.SetPlayer(targetTransform);
            pursuePlayerActivity.SetTargetPosition(targetTransform.position);
            pursuePlayerActivity.StartActivity();
            bugAttachment.SetCurrentActivity(pursuePlayerActivity);
        }
    }

    public void StopFollowingPlayer()
    {
        pursuingPlayer = false;

        if (orderedActivities.Count == 0 || currentActivityIndex < 0)
        {
            return;
        }
        currentActivityIndex = afterPursueActivity;
        Debug.Log("stop following player, next activity:" + currentActivityIndex);

        orderedActivities[currentActivityIndex].StartActivity();
        bugAttachment.SetCurrentActivity(orderedActivities[currentActivityIndex]);
    }

    public void SetPaused(bool isPaused)
    {
        if (orderedActivities.Count == 0)
        {
            return;
        }
        pursuePlayerActivity.SetPaused(isPaused);
        orderedActivities[currentActivityIndex].SetPaused(isPaused);
    }

    private void OnDestroy()
    {
        ConstraintManager.OnChangeConstraints -= CheckConstraints;
    }
}
