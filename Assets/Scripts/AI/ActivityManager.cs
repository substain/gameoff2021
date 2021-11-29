using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActivityManager : MonoBehaviour
{
    [SerializeField]
    private GameObject controlledObject;

    [SerializeField]
    private bool useRandomOrder = false;

    [SerializeField]
    private int initialActivityIndex = 0;

    private List<AbstractActivity> orderedActivities = new List<AbstractActivity>();

    private int currentActivityIndex = -1;

    private PursuePlayerActivity pursuePlayerActivity;
    private bool pursuingPlayer = false;

    [SerializeField]
    private BugAttachment bugAttachment;

    void Awake()
    {
        AbstractActivity[] activities = GetComponents<AbstractActivity>(); 
        currentActivityIndex = initialActivityIndex;

        //search for pursue player activities
        pursuePlayerActivity = (PursuePlayerActivity) activities.FirstOrDefault(activity => activity.GetType() == typeof(PursuePlayerActivity));
        pursuePlayerActivity.Init(controlledObject);

        orderedActivities = activities.OrderBy(activity => activity.GetOrder())
                                        .Where(activity => activity.GetType() != typeof(PursuePlayerActivity))
                                        .ToList();

        orderedActivities.ForEach(x => x.Init(controlledObject));

        if (orderedActivities.Count == 0)
        {
            Debug.LogWarning("no activities to do");
            return;
        }

    }

    void Start()
    {
        InvokeRepeating("CheckActivityStatus", 0.11f, 0.11f);
        orderedActivities[currentActivityIndex].StartActivity();
        bugAttachment.SetCurrentActivity(orderedActivities[currentActivityIndex]);
    }

    private void CheckActivityStatus()
    {
        if (pursuingPlayer && pursuePlayerActivity.IsFinished())
        {
            StopFollowingPlayer();
            return;
        }
        if (orderedActivities[currentActivityIndex].IsFinished())
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
        if (useRandomOrder)
        {
            List<int> possibleIndices = Enumerable.Range(0, orderedActivities.Count).ToList();
            possibleIndices.RemoveAt(currentActivityIndex);
            currentActivityIndex = possibleIndices[Random.Range(0, possibleIndices.Count)];
        }
        else
        {
            currentActivityIndex = (currentActivityIndex + 1) % orderedActivities.Count;
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
        Debug.Log("stop following player");
        pursuingPlayer = false;

        /*        if (orderedActivities[currentActivityIndex].GetType() == typeof(IdleActivity))
                {
                    UpdateActiveActivity();
                }*/
        //      else
        //    {
        orderedActivities[currentActivityIndex].StartActivity();
        bugAttachment.SetCurrentActivity(orderedActivities[currentActivityIndex]);
        //  }
    }

    private void SetPaused(bool isPaused)
    {
        orderedActivities[currentActivityIndex].SetPaused(isPaused);
    }
}
