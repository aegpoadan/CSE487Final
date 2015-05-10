using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour {

	public GameObject particleEffect;
	[Range(0, 20)]
	public float lifetime;

	// Use this for initialization
	void Start () {
		Instantiate(particleEffect, this.transform.position, this.transform.rotation);
		Destroy (this.gameObject);
	}

}
