using UnityEngine;
using System.Collections;

public interface IPickup
{
	void Pickup(GameObject item);

	void Drop(GameObject item);

	bool HasItem(GameObject item);
}

