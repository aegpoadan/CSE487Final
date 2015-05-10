using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour, IPickup {

	// Call updates to GUI when number of items changes.
	public SelectedItemIconManager guiManager;

	public List<string> itemButtonNames;

	private List<GameObject> itemsHeld;
	public Transform weaponHolder;

	public void Start()
	{
		itemsHeld = new List<GameObject>();
	}

	public void Update() {
		CheckItemSwitch();
	}


	#region IPickup implementation

	public void Pickup (GameObject item)
	{
		item.transform.SetParent(weaponHolder, false);
		itemsHeld.Add(item);
		IPickupable ip = (IPickupable)(item.GetComponents<MonoBehaviour>().First(mb => (mb as IPickupable != null)));
		ip.SetOwner(this.gameObject);

		//SetActiveItem(item);

		guiManager.UpdateGUIItems(GetItemImages());
	}

	public void Drop (GameObject item)
	{
		print ("Dropping item");
		item.transform.SetParent(null, true);
		if (!itemsHeld.Remove(item)) {
			throw new System.Exception("Invalid program logic.  All item removals should return true.");
		}
	}
	

	bool IPickup.HasItem (GameObject item)
	{
		GameObject found = itemsHeld.SingleOrDefault( i => i == item);
		bool hasItem = (found != null);
		return hasItem;
	}

	public void SetActiveItem(GameObject item) {
		itemsHeld.ForEach(i => {
			IPickupable ip = i.GetComponent<Item>() as IPickupable;
			//TODO -- Change above call to find a generic "Item" class, rather than "PhysicsWeapon"
			if (ip == null) {
				throw new UnityException("Bad data...this should only be dealing with items that implement IPickupable");

			} else {
				if (i == item) {
					ip.EnableItem(true);
				} else {
					ip.EnableItem(false);
				}
			}
		});
	}

	void CheckItemSwitch()
	{
		for (int i = 0; (i < itemButtonNames.Count); i++) {
			if (Input.GetButtonDown(itemButtonNames[i])) {
				if (i < itemsHeld.Count) {
					SetActiveItem(itemsHeld[i]);
				}
			}
		}
	}

	public List<Sprite> GetItemImages() {
		List<Sprite> images = new List<Sprite>();
		itemsHeld.ForEach(item => (images.Add (item.GetComponent<Item>().itemImage)));
		return images;
	}

	public int GetNumItemsHeld() {
		return itemsHeld.Count;
	}


	#endregion

}
