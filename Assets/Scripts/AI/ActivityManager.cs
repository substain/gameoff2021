using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public int currentIndex = -1;

    private PursuePlayerActivity pursuePlayerActivity;
    private SuspiciousActivity suspiciousActivity;
    private AbstractActivity currentActivity;

    private bool beingSuspicious = false;
    private bool pursuingPlayer = false;

    [SerializeField]
    private BugAttachment bugAttachment;

    private bool isPaused = false;

    void Awake()
    {
        AbstractActivity[] activities = GetComponents<AbstractActivity>(); 

        //search for pursue player activities
        pursuePlayerActivity = (PursuePlayerActivity) activities.FirstOrDefault(activity => activity.GetType() == typeof(PursuePlayerActivity));
        pursuePlayerActivity?.Init(controlledObject);

        suspiciousActivity = (SuspiciousActivity)activities.FirstOrDefault(activity => activity.GetType() == typeof(SuspiciousActivity));
        suspiciousActivity?.Init(controlledObject);

        orderedActivities = activities.OrderBy(activity => activity.GetOrder())
                                        .Where(activity => activity.GetType() != typeof(PursuePlayerActivity) && activity.GetType() != typeof(SuspiciousActivity))
                                        .ToList();

        orderedActivities.ForEach(x => x.Init(controlledObject));
        currentIndex = Mathf.Min(initialActivityIndex, orderedActivities.Count-1);

        if (orderedActivities.Count == 0)
        {
            //Debug.LogWarning("no activities to do");
            return;
        }

    }

    void Start()
    {
        InvokeRepeating("CheckActivityStatus", 0.11f, 0.11f);
        if(orderedActivities.Count == 0 || currentIndex < 0)
        {
            return;
        }

        pursuePlayerActivity?.CheckActivationConstraints();
        suspiciousActivity?.CheckActivationConstraints();
        orderedActivities.ForEach(x => x.CheckActivationConstraints());

        StartActivity(orderedActivities[currentIndex]);
        ConstraintManager.OnChangeConstraints += CheckConstraints;
    }

    private void CheckConstraints()
    {
        pursuePlayerActivity?.CheckActivationConstraints();
        suspiciousActivity?.CheckActivationConstraints();
        orderedActivities.ForEach(oa => oa.CheckActivationConstraints());
    }

    private void CheckActivityStatus()
    {
        if (pursuingPlayer && pursuePlayerActivity.CheckIfFinished())
        {
            StopFollowingPlayer();
            return;
        }
        if (beingSuspicious && suspiciousActivity.CheckIfFinished())
        {
            StopBeingSuspicious();
            return;
        }
        if (orderedActivities.Count == 0 || currentIndex < 0)
        {
            return;
        }
        if (orderedActivities[currentIndex].CheckIfFinished())
        {
            UpdateActiveActivity();
        }
    }

    private void UpdateActiveActivity()
    {
        UpdateToNextActivityIndex();
        if (currentIndex > -1)
        {
            StartActivity(orderedActivities[currentIndex]);
        }
    }

    private void UpdateToNextActivityIndex()
    {
        List<AbstractActivity> possibleActivities = orderedActivities.Where(oa => oa.HasConstraintsSatisfied()).ToList();
        if (possibleActivities.Count == 0)
        {
            currentIndex = -1;
            return;
        }

        List<int> possibleIndices = Enumerable.Range(0, possibleActivities.Count).ToList();

        if (useRandomOrder)
        {
            if (possibleIndices.Count > 1 && currentIndex > -1)
            {
                int currentPossibleIndex = possibleActivities.FindIndex(pa => pa == orderedActivities[currentIndex]);
                if(currentPossibleIndex > 0)
                {
                    possibleIndices.RemoveAt(currentPossibleIndex);
                }
            }
            int randomPossibleIndex = UnityEngine.Random.Range(0, possibleIndices.Count - 1);
            AbstractActivity randomActivity = possibleActivities[possibleIndices[randomPossibleIndex]];
            currentIndex = orderedActivities.FindIndex(oa => oa == randomActivity);
        }
        else
        {
            int refIndex = possibleIndices.FindIndex(ind => ind == currentIndex);
            refIndex = (refIndex + 1) % possibleIndices.Count;
            currentIndex = orderedActivities.FindIndex(oa => oa == possibleActivities[possibleIndices[refIndex]]);
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
        if (pursuingPlayer || pursuePlayerActivity == null)
        {
            return;
        }
        beingSuspicious = false;
        pursuingPlayer = true;
        pursuePlayerActivity.SetPlayer(targetTransform);
        pursuePlayerActivity.SetTargetPosition(targetTransform.position);
        StartActivity(pursuePlayerActivity);
    }

    public void StartBeingSuspicious(Transform targetTransform)
    {
        if (pursuingPlayer || beingSuspicious || suspiciousActivity == null)
        {
            return;
        }
        beingSuspicious = true;
        suspiciousActivity.SetTarget(targetTransform);
        StartActivity(suspiciousActivity);
    }

    public void StopFollowingPlayer()
    {
        pursuingPlayer = false;
        beingSuspicious = true;

        if (suspiciousActivity == null){
            StopBeingSuspicious();
            return;
        }

        StartActivity(suspiciousActivity);
    }

    public void StopBeingSuspicious()
    {
        beingSuspicious = false;

        if (orderedActivities.Count == 0 || currentIndex < 0)
        {
            return;
        }
        currentIndex = afterPursueActivity;
        StartActivity(orderedActivities[currentIndex]);
    }

    public void SetPaused(bool isPaused)
    {
        if (orderedActivities.Count == 0)
        {
            return;
        }
        pursuePlayerActivity.SetPaused(isPaused);
        orderedActivities[currentIndex].SetPaused(isPaused);
    }

    private void StartActivity(AbstractActivity activity)
    {

        currentActivity?.StopActivity();
        activity.StartActivity();
        bugAttachment.SetCurrentActivity(activity);
        currentActivity = activity;
    }

    public bool IsSuspicious()
    {
        return beingSuspicious;
    }

    private void OnDestroy()
    {
        ConstraintManager.OnChangeConstraints -= CheckConstraints;
    }
}
