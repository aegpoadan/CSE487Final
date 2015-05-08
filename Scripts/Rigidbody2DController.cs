using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Rigidbody2DController : MonoBehaviour {

	[Range(1, 40)]
	public float forwardSpeed = 5f;
	[Range(1, 50)]
	public float acceleration = 10f;
	/// <summary>
	/// Vector to look at when moving right.
	/// </summary>
	public Vector3 rightWorldDirection = Vector3.right;
	/// <summary>
	/// Vector to look at when moving left.
	/// </summary>
	public Vector3 leftWorldDirection = (-Vector2.right);

	public string jumpButtonName = "Jump";

	// Physical force applied on jump.
	public float jumpForce = 100f;
	/// <summary>
	/// Additional "wiggle room" added to y-extents of collider height.
	/// </summary>
	public float skinWidthModifier = 0.05f;

	private Rigidbody rbody;

	private LookDirection currentDirection;

	/// <summary>
	/// Holds the distance from the center of the object to the ground,
	/// as determined by the size of the collider.
	/// </summary>
	private float distFromCenterToGround = 0f;

	private float sqrMaxVelocity;

	void Start() {
		rbody = this.gameObject.GetComponent<Rigidbody>();
		rbody.freezeRotation = true;

		distFromCenterToGround = this.GetComponent<Collider>().bounds.extents.y + skinWidthModifier;

		currentDirection = LookDirection.LEFT;
		TestLookRight(); // Force adjustment with oppposites.
		sqrMaxVelocity = forwardSpeed * forwardSpeed;
	}

	// Update is called once per frame
	void Update () {
		float frameSpeed = forwardSpeed * Time.deltaTime * acceleration;

		Vector3 movement = Vector3.zero;

		#region MovementAndTurning
		if (Input.GetKeyDown(KeyCode.A))
		{
			TestLookLeft();
		}

		if (Input.GetKey (KeyCode.A))
		{
			movement += (this.transform.forward * frameSpeed);
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			TestLookRight();
		}
		if (Input.GetKey (KeyCode.D))
		{
			movement += (this.transform.forward * frameSpeed);
		}
		#endregion


		if (Input.GetButtonDown(jumpButtonName) && IsGrounded())
		{
			rbody.AddForce(this.transform.up * jumpForce, ForceMode.Impulse);
		}

		//rbody.MovePosition(rbody.position + movement);
		rbody.AddForce(movement, ForceMode.VelocityChange);


		float yVel = rbody.velocity.y;
		Vector3 tempVel = rbody.velocity;
		tempVel.y = 0;

		// Normalize velocity along the XZ plane, while maintaining regular Y velocity
		// for jumping and falling.
		if (tempVel.sqrMagnitude > sqrMaxVelocity) {
			tempVel = Vector3.ClampMagnitude(tempVel, forwardSpeed);
		}
		tempVel.y = yVel;
		rbody.velocity = tempVel;

		//rbody.velocity = Mathf.Min (rbody.velocity, forwardSpeed);
		//rbody.velocity += movement;
	}


	/// <summary>
	/// Determines whether this object is grounded.
	/// </summary>
	/// <returns><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</returns>
	private bool IsGrounded()
	{
		Ray downRay = new Ray(this.transform.position, Vector3.down);
		Debug.DrawRay(downRay.origin, downRay.direction, Color.green, 2f);
		return (true == Physics.Raycast(downRay, distFromCenterToGround));
	}

	#region HelperMethods
	private void TestLookRight()
	{
		if (currentDirection != LookDirection.RIGHT) {
			this.transform.rotation = Quaternion.LookRotation(rightWorldDirection);
			currentDirection = LookDirection.RIGHT;
		}
	}

	private void TestLookLeft()
	{
		if (currentDirection != LookDirection.LEFT) {
			this.transform.rotation = Quaternion.LookRotation(leftWorldDirection);
			currentDirection = LookDirection.LEFT;
		}
	}
	#endregion


	private enum LookDirection {
		LEFT, RIGHT
	};
}
