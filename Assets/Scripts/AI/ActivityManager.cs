using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivityManager : MonoBehaviour
{
    private float SUSPICIOUS_SPOTTING_SPEED = 1.25f;
    private float PURSUE_SPOTTING_SPEED = 2.5f;

    [SerializeField]
    private GameObject controlledObject;

    [SerializeField]
    private bool useRandomOrder = false;

    [SerializeField]
    private int initialActivityIndex = 0;

    [SerializeField]
    private int afterPursueActivity;

    [SerializeField]
    private List<ConstraintManager.GameConstraint> pursueActivationConstraints;

    private List<AbstractActivity> orderedActivities = new List<AbstractActivity>();

    public int currentIndex = -1;

    private PursuePlayerActivity pursuePlayerActivity;
    private SuspiciousActivity suspiciousActivity;
    private AbstractActivity currentActivity;

    private bool beingSuspicious = false;
    private bool pursuingPlayer = false;

    [SerializeField]
    private BugAttachment bugAttachment;

    private WatcherNPC watcherNPC;
    //private bool isPaused = false;
    void Awake()
    {
        watcherNPC = GetComponentInParent<WatcherNPC>();
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
        if (ConstraintManager.Instance.AnyConstraintsSatisfied(pursueActivationConstraints))
        {
            StartPursuePlayer(GameManager.GameInstance.GetPlayer().transform);
        }
    }

    private void CheckActivityStatus()
    {
        if (pursuingPlayer)
        {
            if (pursuePlayerActivity.CheckIfFinished())
            {
                StopPursuing(pursuePlayerActivity.GetLastPosition());
            }
            return;
        }

        if (beingSuspicious)
        {
            if (suspiciousActivity.CheckIfFinished())
            {
                StopBeingSuspicious();
            }
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
        watcherNPC.SetSpottingSpeedFactor(PURSUE_SPOTTING_SPEED);
        pursuePlayerActivity.SetPlayer(targetTransform);
        pursuePlayerActivity.SetTargetPosition(targetTransform.position);
        StartActivity(pursuePlayerActivity);
    }

    public void StartBeingSuspicious(Vector3 targetPos)
    {
        if (pursuingPlayer || suspiciousActivity == null)
        {
            return;
        }

        suspiciousActivity.SetTarget(targetPos);

        if (!beingSuspicious)
        {
            beingSuspicious = true; 
            watcherNPC.SetSpottingSpeedFactor(SUSPICIOUS_SPOTTING_SPEED);
            StartActivity(suspiciousActivity);
        }
    }

    public void StopPursuing(Vector3? targetPos)
    {
        pursuingPlayer = false;

        if (suspiciousActivity == null || !targetPos.HasValue){
            StopBeingSuspicious();
            return;
        }
        StartBeingSuspicious(targetPos.Value);
    }

    public void StopBeingSuspicious()
    {
        beingSuspicious = false;

        if (orderedActivities.Count == 0 || currentIndex < 0)
        {
            return;
        }
        currentIndex = afterPursueActivity;
        watcherNPC.SetSpottingSpeedFactor(1.0f);
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
