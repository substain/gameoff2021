using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherNPC : MonoBehaviour {

	public float viewRadius = 4f;
	[Range(0, 360)]
	public float viewAngle = 45f;

	public LayerMask obstacleMask;

	public float watchDelay = 1f;
	private float watchCounter = 0;
	private bool playerSpotted = false;
	private float dstToTarget = 0;

	private List<Collider> targetsInViewRadius = new List<Collider>();

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();


	void FixedUpdate() {
		//Debug.Log("fixedUpdate watcherNPC");
		FindVisibleTargets();

		//if ( visibleTargets.Count != 0 ) {
		if ( visibleTargets.Count != 0 ) {
			//Debug.Log("Count down time " + watchCounter);
			watchCounter += Time.deltaTime;

			//TODO maybe:  calculate spotting speed with dstToTarget and deltaTime

			if ( ( ! playerSpotted ) && watchCounter > watchDelay) {
				spottedPlayer();
            }
        }
    }
	
	private void spottedPlayer() {
		Debug.Log("Player spotted");
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
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) {
				dstToTarget = Vector3.Distance(transform.position, target.position);
				
				// Check for Objects covering player from view
				if ( Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) ) {
					visibleTargets.Add(target);
					//Debug.Log("See target");
				}
			}
		}

		// Reset values if nobody is detected
		if ( visibleTargets.Count == 0 ) {
			playerSpotted = false;
			watchCounter = 0;
        }
	}
}