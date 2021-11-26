using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatcherNPC : MonoBehaviour {

	[SerializeField] public Slider spottedIndicator;
	[SerializeField] public GameObject alertIcon;
	public float viewRadius = 4f;
	[Range(0, 360)]
	public float viewAngle = 45f;
	public float spottingSpeed = 4;

	public LayerMask obstacleMask;

	public float watchDelay = 1f;
	private float watchCounter = 0;
	public bool playerSpotted = false;
	private float dstToTarget = 0;

	private List<Collider> targetsInViewRadius = new List<Collider>();

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	private ActivityManager activityManager;
	private Vector3 lookDirection = Vector3.zero;

	void Awake()
	{
		activityManager = GetComponentInChildren<ActivityManager>();
	}

	void Start()
	{
		GameObject canvas = gameObject.transform.Find("Canvas").gameObject;
		GameObject slider = canvas.transform.Find("Slider").gameObject;
		slider.GetComponent<Slider>().maxValue = watchDelay;
	}
	void FixedUpdate() {
		//Debug.Log("fixedUpdate watcherNPC");
		FindVisibleTargets();
		AdjustRotation();

		//if ( visibleTargets.Count != 0 ) {
		if ( visibleTargets.Count != 0 ) {
			//Debug.Log("Count down time " + watchCounter);
			watchCounter += Time.deltaTime * spottingSpeed * (1 / dstToTarget);
			spottedIndicator.value = watchCounter;

			//TODO maybe:  calculate spotting speed with dstToTarget and deltaTime

			if ( ( ! playerSpotted ) && watchCounter > watchDelay) {
				spottedPlayer();
            }
        }
    }

	public void SetLookDirection(Vector3 lookDirection)
	{
		this.lookDirection = lookDirection;
	}

	private void AdjustRotation() {
		spottedIndicator.transform.rotation = Quaternion.identity;
		alertIcon.transform.rotation = Quaternion.identity;
	}

	private void spottedPlayer()
	{
		GetComponentInChildren<ActivityManager>().StartPursuePlayer(visibleTargets[0]);

		//Debug.Log("Player spotted");
		spottedIndicator.gameObject.SetActive(false);
		alertIcon.SetActive(true);
		playerSpotted = true;
    }

	void OnTriggerEnter(Collider other) {
		if ( other.gameObject.name == "Player" ) { // Can be modified if other things need to be detected
			targetsInViewRadius.Add(other);
		}
    }

	void OnTriggerExit(Collider other) {
		targetsInViewRadius.Remove(other);
	}

	/**
	 * Checks if an Object of targetmask is within viewRadius and viewAngle, as well as not covered by Objects in obstacleMask
	 * Updates visibleTargets to all those Objects
	 *
	 * Only the Player Object is supposed to be in targetMask.
	 *
	 * For more information, view https://www.youtube.com/watch?v=rQG9aUWarwE&t=1172s&ab_channel=SebastianLague and it's according github page
	 */
	void FindVisibleTargets() {

		visibleTargets.Clear();

		// For every object in Range
		for (int i = 0; i < targetsInViewRadius.Count; i++) {

			//Get angle
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			//Compare angles
			if (Vector3.Angle(lookDirection, dirToTarget) < viewAngle / 2) {
				dstToTarget = Vector3.Distance(transform.position, target.position);

				// Check for Objects covering player from view
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) ) {
					visibleTargets.Add(target);
					//Debug.Log("See target");
				}
			}
		}

		// Reset values if nobody is in FoV
		if ( visibleTargets.Count == 0 ) {
			playerSpotted = false;
			watchCounter = 0;
			spottedIndicator.gameObject.SetActive(false);
			alertIcon.SetActive(false);
		} else { // player is in FoV
			activityManager.UpdatePlayerPosition(visibleTargets[0]);

			if (!playerSpotted) {
				spottedIndicator.gameObject.SetActive(true);
				alertIcon.SetActive(false);
			}
		}
	}


#if UNITY_EDITOR

	[SerializeField]
	private bool displayWatcherGizmo = true;
	private void OnDrawGizmos()
	{
		if (!displayWatcherGizmo)
		{
			return;
		}
		Gizmos.color = Color.red;

		foreach (Transform tr in visibleTargets)
		{
			Gizmos.DrawSphere(tr.position, 1.0f);
		}
		for (int i = 0; i < targetsInViewRadius.Count; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				Gizmos.color = Color.blue;

				Gizmos.DrawLine(transform.position, targetsInViewRadius[i].transform.position);
			}
			else
			{
				Gizmos.color = Color.grey;

				Gizmos.DrawLine(transform.position, targetsInViewRadius[i].transform.position);
			}

		}
	}
#endif
}