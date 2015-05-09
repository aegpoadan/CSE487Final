using UnityEngine;
using System.Collections;

public class TimeSlow : MonoBehaviour {
	private GameObject gameManager;
	private GlobalData globalData;

	void Awake() {
		gameManager = GameObject.Find ("GameManager");
		globalData = gameManager.GetComponent<GlobalData> ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine ("slowEnemies");
		Destroy (gameObject, 5.1f);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator slowEnemies() {
		print ("Slowing enemies!");
		globalData.enemySpeed = .5f;	//Reduce enemy speed to half of original value
		yield return new WaitForSeconds(5.0f); //Enemies are slowed for 5 seconds
		globalData.enemySpeed = 1.0f;
		print ("Enemies are back at normal sped");
	}
}
