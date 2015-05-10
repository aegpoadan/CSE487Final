using UnityEngine;
using System.Collections;

public class CharacterBoundsResetting : MonoBehaviour {

	public float minYPos;
	public Vector3 resetPos;
	[Range(0.1f, 5)]
	public float checkTime;

	// Use this for initialization
	void Start () {
		StartCoroutine(resetIfOutOfBounds());
	}
	

	public IEnumerator resetIfOutOfBounds() {
		while (true) {
			if (this.transform.position.y < minYPos) {
				print ("Resetting");
				this.transform.position = resetPos;
			}
			
			yield return new WaitForSeconds(checkTime);
		}
	}
}
