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

    void Awake()
    {
        AbstractActivity[] activities = GetComponents<AbstractActivity>();

        if (activities.Length == 0)
        {
            Debug.LogWarning("no activities to do");
            return;
        }

        orderedActivities = activities.OrderBy(activity => activity.GetOrder()).ToList();

        orderedActivities.ForEach(x => x.Init(controlledObject));
        currentActivityIndex = initialActivityIndex;
    }

    void Start()
    {
        InvokeRepeating("CheckActivityStatus", 0.11f, 0.11f);
        orderedActivities[currentActivityIndex].StartActivity();
    }

    private void CheckActivityStatus()
    {
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

    private void SetPaused(bool isPaused)
    {
        orderedActivities[currentActivityIndex].SetPaused(isPaused);
    }
}
