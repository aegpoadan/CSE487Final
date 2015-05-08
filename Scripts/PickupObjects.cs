using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PickupObjects : MonoBehaviour, IPickup {

	private List<GameObject> itemsHeld;
	public Transform weaponHolder;

	public void Start()
	{
		itemsHeld = new List<GameObject>();
	}

	#region IPickup implementation

	public void Pickup (GameObject item)
	{
		item.transform.SetParent(weaponHolder, false);
		itemsHeld.Add(item);
		IPickupable ip = (IPickupable)(item.GetComponents<MonoBehaviour>().First(mb => (mb as IPickupable != null)));
		ip.SetOwner(this.gameObject);
	}

	public void Drop (GameObject item)
	{
		item.transform.SetParent(null, true);
		if (!itemsHeld.Remove(item)) {
			throw new System.Exception("Invalid program logic.  All item removals should return true.");
		}
	}
	

	bool IPickup.HasItem (GameObject item)
	{
		GameObject found = itemsHeld.SingleOrDefault( i => i == item);
		return found != null;
	}


	#endregion

}
