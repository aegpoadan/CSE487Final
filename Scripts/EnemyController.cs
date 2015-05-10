using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour {

	public GlobalData globalData;

	public List<GameObject> moveTargets = new List<GameObject>();

	public Animator animator;

	[Range(1, 100)]
	public float maxMoveSpeed;
	[Range(1, 100)]
	public float acceleration = 10f;
	[Range(1f, 20f)]
	public float rotationSpeed;

	[Range(0, 10)]
	public float atTargetWaitTime = 3f;

	private Rigidbody enemyRbody;
	private float sqrMaxVelocity;
	private bool waitingAtTarget = false;
	private int currentTarget = 0;

	// Use this for initialization
	void Start ()
	{
		rotationSpeed = 15f;

		if (!globalData) {
			globalData = GameObject.Find ("__GM__").GetComponent<GlobalData>();
		}
		maxMoveSpeed = globalData.enemySpeed;
		// Cache value for more efficient speed magnitude comparisons.
		sqrMaxVelocity = maxMoveSpeed * maxMoveSpeed;

		enemyRbody = this.GetComponent<Rigidbody>();

		if (enemyRbody == null) {
			throw new UnityException("Attach rigidbody component to enemy!");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		maxMoveSpeed = globalData.enemySpeed;

		if (waitingAtTarget) {
			return;
		}
		float frameSpeed = maxMoveSpeed * Time.deltaTime * acceleration;	
		Vector3 movement = Vector3.zero;
		float distToTarget = Vector3.Magnitude(this.transform.position - moveTargets[currentTarget].transform.position);

		if (distToTarget < 6f) {
			currentTarget = ((currentTarget + 1) % moveTargets.Count); // Set next target as goal destination.
			StartCoroutine(WaitAtTarget());
			return;
		} else {
			print ("Dist: " + distToTarget);
		}

		Quaternion lookRot = Quaternion.LookRotation(moveTargets[currentTarget].transform.position
		                                             - this.transform.position);

		Quaternion frameRotation = Quaternion.Lerp(this.transform.rotation, lookRot, Time.deltaTime * rotationSpeed);

		Vector3 flatRot = frameRotation.eulerAngles;
		flatRot.x = 0;
		flatRot.z = 0;
		transform.rotation = Quaternion.Euler(flatRot);

		movement += (this.transform.forward * frameSpeed);

		enemyRbody.AddForce(movement, ForceMode.VelocityChange);

		float yVel = enemyRbody.velocity.y;
		Vector3 tempVel = enemyRbody.velocity;
		tempVel.y = 0;
		
		// Normalize velocity along the XZ plane, while maintaining regular Y velocity
		// for jumping and falling.
		if (tempVel.sqrMagnitude > sqrMaxVelocity) {
			tempVel = Vector3.ClampMagnitude(tempVel, maxMoveSpeed);
		}
		tempVel.y = yVel;
		enemyRbody.velocity = tempVel;
	}


	private IEnumerator WaitAtTarget() {
		if (animator) {
			animator.SetBool ("IsWalking", false);
		}
		waitingAtTarget = true;
		enemyRbody.velocity = Vector3.zero;
		enemyRbody.angularVelocity = Vector3.zero;
		yield return new WaitForSeconds(atTargetWaitTime);
		if (animator) {
			animator.SetBool ("IsWalking", true);
		}
		waitingAtTarget = false;
		yield break;
	}
}
