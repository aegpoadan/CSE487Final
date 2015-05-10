using UnityEngine;
using System.Collections;


public class Camera2D : MonoBehaviour {

	/// <summary>
	/// Game object that the camera should follow.
	/// </summary>
	public Transform target;
	/// <summary>
	/// Offset from the target's position where the camera should ultimately end up.
	/// </summary>
	public Vector3 permanentPositionOffset = Vector3.zero;
	/// <summary>
	/// The time in which the camera should move to its goal position.
	/// </summary>
	public float followSpeed = 1f;

	/// <summary>
	/// If false, camera's position won't change.
	/// </summary>
	public bool moveWithTarget;

	// Use this for initialization
	void Start () {
		if (target == null) {
			this.GetComponent<Camera2D>().enabled = false;
			throw new MissingReferenceException("Missing camera target.");
		}
	}
	
	// Called after all physics updates have occured.
	void LateUpdate () {
		if (moveWithTarget) {
			Vector3 goalPos = target.position + permanentPositionOffset;
			this.transform.position = Vector3.Lerp(this.transform.position, goalPos, Time.deltaTime * followSpeed);
		}
		Vector3 posDifference = target.position - this.transform.position;
		this.transform.rotation = Quaternion.LookRotation(posDifference);
	}
}
