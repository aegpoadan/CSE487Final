using UnityEngine;
using System.Collections;

public interface IPickupable
{
	/// <summary>
	/// Pick up and attach this object to a parent.
	/// </summary>
	/// <returns><c>true</c>, if it was picked up, <c>false</c> otherwise.</returns>
	/// <param name="parent">Object that should become this weapon's parent.</param>
	void GetPickedPickup();
	/// <summary>
	/// Unchild this object from its parent.
	/// </summary>
	void GetDropped();

	void SetOwner(GameObject owner);

	void SetActive(bool active);

	/// <summary>
	/// Aligns the position of the weapon on pickup.
	/// </summary>
	void AlignPositionOnPickup();

	void OnCollisionEnter(Collision other);
}

