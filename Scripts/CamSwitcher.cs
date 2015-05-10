using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CamSwitcher : MonoBehaviour {

	public string cameraTag = "MainCamera";
	public string cameraInputButtonName = "CamSwitch";
	private List<GameObject> cameras;

	private GameObject activeCam;

	// Use this for initialization
	void Start () {
		cameras = GameObject.FindGameObjectsWithTag(cameraTag).ToList();
		setActiveCam(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(cameraInputButtonName)) {
			int currCam = cameras.FindIndex(obj => (obj == activeCam));
			int nextCam = (currCam + 1) % cameras.Count;
			setActiveCam(nextCam);
		}
	}


	public void setActiveCam(int index) {
		for (int i = 0; (i < cameras.Count); i++) {
			Camera cam = cameras[i].GetComponent<Camera>();
			AudioListener listener = cameras[i].GetComponent<AudioListener>();
			if (index == i) {
				toggleCam(true, cam, listener);
				activeCam = cameras[i];
			} else {
				toggleCam(false, cam, listener);
			}
		}
	}

	private void toggleCam(bool toggle, Camera cam, AudioListener listener) {
		cam.enabled = toggle;
		listener.enabled = toggle;
	}
}
