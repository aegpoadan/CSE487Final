using UnityEngine;
using System.Collections;

public class BulletLifetime : MonoBehaviour {

	public float lifetime = 5f;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, lifetime);
	}
}
