using UnityEngine;
using System.Collections;

public class MouseAim : MonoBehaviour {

	public bool lockX;
	[Range(0, 360)]
	public float xLockRot;
	public bool lockY;
	[Range(0, 360)]
	public float yLockRot;
	public bool lockZ;
	[Range(0, 360)]
	public float zLockRot;

	public Transform mainParent;
	public Transform aimingBodyPart;

	public Texture2D targetAim;

	[Range(270, 360)]
	public float maxXRotation;
	[Range(0, 90)]
	public float minXRotation;

	[Range(1, 100)]
	public float maxAimDistance;
	public LayerMask hittableAimLayers;

	public bool DEBUG = true;


	// Used to hold the location for the GUI texture
	private Vector2 aimLoc;



	public void LateUpdate(){

		// Expressed in ***PIXELS*** from the origin of the transform object to the pointer position on screen.
		//Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));

		// Calculate the mouse pointer position assuming its Z position is that of the player.
		float distFromCam = Mathf.Abs (Camera.main.transform.position.z - aimingBodyPart.position.z);
		Vector3 cursorPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCam));
		Vector3 finalPoint = cursorPoint;


		// Get cursor point relative to the mainParent
		Vector3 localRelativePoint = mainParent.InverseTransformPoint(cursorPoint);
		if(localRelativePoint.z < 0){
			// Point is behind the character. We don't want character turning 
			// around so "reflect" the point back to the coinciding point in front of the character

			Vector3 reflectedPt = localRelativePoint;
			// Reflect to opposite end of the mainParent's local forward (Z) axis.
			reflectedPt.z = -localRelativePoint.z;

			/* Point is current expressed relative the mainParent's origin.
			 * Express it relative to the world coordinate system. 
			 **/
			finalPoint = mainParent.TransformPoint(reflectedPt);
		}

		// Clamp the mouse pointer world point to the holding object's Z-position for 2D pointing.

		if (DEBUG) {
			Debug.DrawLine(aimingBodyPart.position, finalPoint, Color.black);
			Debug.DrawLine(aimingBodyPart.position, cursorPoint, Color.yellow);
		}

		// Look at point, no matter where it is.
		aimingBodyPart.LookAt(finalPoint, Vector3.up);


		// Constrain the look rotation within the desired boundaries
		Vector3 tempLocalRot = aimingBodyPart.localEulerAngles;
		// CLAMP (UP/DOWN) LOOK ROTATION WITHIN USER SPECIFIED BOUNDS
		tempLocalRot = clampEulerRotation(tempLocalRot);
		// Apply any user specified rotation locks (e.g. for 2D purposes)
		tempLocalRot = lockedEulerRotation(tempLocalRot);
		// Set the clamped and locked local rotation
		aimingBodyPart.localEulerAngles = tempLocalRot;

		// Draw ray forward from the current rotation and place the GUI cursor
		// on any target that it hits.
//		RaycastHit hit;
//		Vector3 rayTarget = aimingBodyPart.forward;
//		if (Physics.Raycast (aimingBodyPart.position, aimingBodyPart.forward, out hit, maxAimDistance, hittableAimLayers)) {
//			rayTarget *= hit.distance;
//			aimLoc =  Camera.main.WorldToViewportPoint(hit.point);
//		} else {
//			print ("No hit");
//			rayTarget *= maxAimDistance;
//			aimLoc = Camera.main.WorldToViewportPoint(rayTarget);
//		}
//		//print ("Ray start: " + aimingBodyPart.position + ", End: " + rayTarget);
//		Debug.DrawRay (aimingBodyPart.position, rayTarget, Color.red);


		// Draw ray forward from the current rotation and place the GUI cursor
		// on any target that it hits.
		RaycastHit hit;
		Vector3 rayTarget = Vector3.zero;

		float aimDist = Vector3.Distance(aimingBodyPart.position, finalPoint);
		if (Physics.Raycast (aimingBodyPart.position, aimingBodyPart.forward, out hit, aimDist, hittableAimLayers)) {
			rayTarget = aimingBodyPart.forward * hit.distance;
			aimLoc =  Camera.main.WorldToViewportPoint(hit.point);
		} else {
			rayTarget = finalPoint;
			aimLoc = Camera.main.WorldToViewportPoint(rayTarget);
		}
		
		if (DEBUG) {
			Debug.DrawLine(aimingBodyPart.position, finalPoint, Color.black);
			Debug.DrawLine(aimingBodyPart.position, cursorPoint, Color.yellow);
		}
    }



	Vector3 lockedEulerRotation(Vector3 rot) {
		if (lockX)
		{
			rot.x = xLockRot;
		}
		if (lockY)
		{
			rot.y = yLockRot;
		}
		if (lockZ)
		{
			rot.z = zLockRot;
		}
		return rot;
	}

	Vector3 clampEulerRotation(Vector3 rot) {
		if (rot.x <  180) {
			// Looking down
			rot.x = Mathf.Min (rot.x, minXRotation);
		} else {
			// Looking up
			rot.x = Mathf.Max (rot.x, maxXRotation);
		}
		return rot;
	}

	
	void OnGUI(){
		int sw = Screen.width;
		int sh = Screen.height;
		GUI.DrawTexture(new Rect(aimLoc.x * sw - 8, sh-(aimLoc.y * sh) -8, 16, 16), targetAim, ScaleMode.StretchToFill, true, 10.0F);
		}
}
