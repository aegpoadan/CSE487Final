using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectedItemIconManager : MonoBehaviour {

	public List<GameObject> itemGUIObjects;
	public List<GameObject> bgGUIObjects;	
	public List<string> itemButtonNames;


	// Cache references to important GUI components.
	private List<RectTransform> itemRects = new List<RectTransform>();
	private List<Image> itemImages = new List<Image>();
	// Cache references to important GUI components.
	private List<RectTransform> bgRects = new List<RectTransform>();
	private List<Image> bgImages = new List<Image>();

	/// <summary>
	/// Icon to be placed over the currently selected item image.
	/// </summary>
	public RectTransform highlightIcon;

	private int currentNumItems = 0;
	private Vector2 offset;
	private Vector2 defaultAnchorMinMax;
	private Vector2 default3DAnchorPos;
	private Image temp;
	private int MAX_ITEMS = 0;

	// Use this for initialization
	void Start () {

		// Cache GUI object data
		InitItems();

		highlightIcon.gameObject.SetActive(false);
		defaultAnchorMinMax = bgRects[0].anchorMin;
		default3DAnchorPos = bgRects[0].anchoredPosition3D;

		if (itemButtonNames.Count != bgGUIObjects.Count) {
			throw new UnityException("Must specify equal number of button names and GUI rects.");
		}
		if (MAX_ITEMS > 1) {
			highlightIcon.anchoredPosition = bgRects[0].anchoredPosition;
			offset = bgRects[1].anchorMin - bgRects[0].anchorMin;
		}
	}

	public void UpdateGUIItems(List<Sprite> passedImages) {
		if (itemImages.Count > MAX_ITEMS) {
			throw new UnityException("Insufficient number of icons for desired item count.");
		}

		currentNumItems = passedImages.Count;
		for (int i = 0; (i < currentNumItems); i++) {
			itemImages[i].sprite = passedImages[i];
		}

		//SetHighlightIcon(currentNumItems);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; (i < currentNumItems); i++) {
			if (Input.GetButtonDown(itemButtonNames[i])) {
				SetHighlightIcon(i);
			}
		}
	}

	private void SetHighlightIcon(int idx) {
		highlightIcon.gameObject.SetActive(true);
		Vector2 anchorPos = defaultAnchorMinMax + idx * offset;
		highlightIcon.anchorMin = anchorPos;
		highlightIcon.anchorMax = anchorPos;
		highlightIcon.anchoredPosition3D = default3DAnchorPos;
	}


	private void InitItems() {
		MAX_ITEMS = bgGUIObjects.Count;

		foreach(GameObject itemObj in itemGUIObjects) {
			itemRects.Add(itemObj.GetComponent<RectTransform>());
			itemImages.Add(itemObj.GetComponent<Image>());
		}
		foreach(GameObject bgObj in bgGUIObjects) {
			bgRects.Add(bgObj.GetComponent<RectTransform>());
			bgImages.Add(bgObj.GetComponent<Image>());
		}

		highlightIcon.position = bgRects[0].anchoredPosition3D;
	}
}
