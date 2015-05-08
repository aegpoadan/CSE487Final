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

	// Used to hold the location for the GUI texture
	private Vector2 aimLoc;



	public void LateUpdate(){

		// Expressed in ***PIXELS*** from the origin of the transform object to the pointer position on screen.
		Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));

		// Clamp the mouse pointer world point to the holding object's Z-position for 2D pointing.
		point.z = mainParent.position.z;

		Debug.DrawLine(mainParent.position, point, Color.black);
		//Debug.DrawLine(mainParent.position, mainParent.position + point.normalized * maxAimDistance, Color.black);

		// Get cursor point relative to the parent object (e.g. character)
		Vector3 localRelativePoint = mainParent.InverseTransformPoint(point);
		if(localRelativePoint.z < 0){
			print ("Behind");
			// Point is behind the character. We don't want character turning 
			// around so "reflect" the point back to the coinciding point in front of the character

			// This is the original Mechanim inverse calculation.  I believe it is incorrect but the results are very similar...
			//Vector3 mechInverseZ = transform.InverseTransformPoint(localRelativePoint.x,localRelativePoint.y,-localRelativePoint.z);
			Vector3 inverseZ = localRelativePoint;
			inverseZ.z = -localRelativePoint.z;

			//print ("My calc: " + inverseZ + "  ||  Mech: " + mechInverseZ);

			//point = mechInverseZ;
			point = inverseZ;
		}

		aimingBodyPart.LookAt(point, Vector3.up);



		Vector3 tempLocalRot = aimingBodyPart.localEulerAngles;
		// CLAMP (UP/DOWN) LOOK ROTATION WITHIN USER SPECIFIED BOUNDS
		tempLocalRot = clampEulerRotation(tempLocalRot);
		// Apply any user specified rotation locks (e.g. for 2D purposes)
		tempLocalRot = lockedEulerRotation(tempLocalRot);
		// Set the clamped and locked local rotation
		aimingBodyPart.localEulerAngles = tempLocalRot;

		// Draw ray forward from the current rotation and place the GUI cursor
		// on any target that it hits.
		RaycastHit hit;
		Vector3 rayTarget = aimingBodyPart.forward;
		if (Physics.Raycast (aimingBodyPart.position, aimingBodyPart.forward, out hit, maxAimDistance, hittableAimLayers)) {
			rayTarget *= hit.distance;
			aimLoc =  Camera.main.WorldToViewportPoint(hit.point);
		} else {
			rayTarget *= maxAimDistance;
			aimLoc = Camera.main.WorldToViewportPoint(rayTarget);
		}
		//print ("Ray start: " + aimingBodyPart.position + ", End: " + rayTarget);
		Debug.DrawRay (aimingBodyPart.position, rayTarget, Color.red);
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
