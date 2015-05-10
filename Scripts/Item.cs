using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(AudioSource))]
public class Item : MonoBehaviour, IPickupable {

	/// <summary>
	/// Rigidbody of the owner object.
	/// </summary>
	Rigidbody ownerRbody;

	/// <summary>
	/// The rigidbody bullet.
	/// </summary>
	public Rigidbody projectile = null;
	/// <summary>
	/// The starting offset from the origin of the weapon.
	/// </summary>
	public Vector3 projectileStartPosOffset = new Vector3(0, 0, 1f);
	/// <summary>
	/// The local offset position from the parent transform.
	/// </summary>
	public Vector3 weaponPosOffset = new Vector3(0, 0, 0.4f);
	/// <summary>
	/// The local rotation of the weapon transform.
	/// </summary>
	public Vector3 weaponRotOffset = new Vector3(0, 0, 0);
	/// <summary>
	/// The name of the shoot button as defined in Input settings.
	/// </summary>
	public string fireButtonName = "Fire1";
	/// <summary>
	/// Power with which projectiles should be fired.
	/// </summary>
	[Range(0, 200)]
	public float projectileForcePower = 60f;
	/// <summary>
	/// Force applied to the character after every shot.
	/// </summary>
	[Range(0, 200)]
	public float recoilForce = 10f;
	/// <summary>
	/// Bullets per second.
	/// </summary>
	[Range(.1f, 100)]
	public float shotsPerSecond = 1f;
	/// <summary>
	/// Number of bullets created each shot.
	/// </summary>
	[Range(1, 100)]
	public float bulletsPerShot = 1f;
	/// <summary>
	/// If false, item will never need to reload.
	/// </summary>
	public bool hasClipSize = true;
	[Range(1, 1000)]
	public int ammoClipSize = 20;
	[Range(0, 10)]
	public float reloadDuration = 2f;
	/// <summary>
	/// Angle of spread cone. Zero is straight.
	/// </summary>
	[Range(0, 90)]
	public float maxSpreadAngle = 45.0f;
	/// <summary>
	/// The spread rotation vector.
	/// </summary>
	public Vector3 spreadWorldRotationAxis = new Vector3(0, 0, 1);
	/// <summary>
	/// Particle system to play at all times.
	/// </summary>
	public ParticleSystem idleParticles;
	/// <summary>
	/// Create a new particle at every shot.
	/// </summary>
	public ParticleSystem shotParticles;
	/// <summary>
	/// Sound to play when item is picked up.
	/// </summary>
	public AudioClip pickupSound;

	public AudioClip shotSound;

	public AudioClip reloadSound;

	public List<Renderer> itemRenderers;

	public List<Collider> itemColliders;
	public bool loopAudio = true;
	public Sprite itemImage;
	/// <summary>
	/// React to button input only if weapon is active (used by player).
	/// </summary>
	private bool isActiveWeapon = false;
	
	private AudioSource itemAudioSource;

	private int currentBulletsShot = 0;


	public void Awake()
	{

	}

	#region UnityMethods

	// Use this for initialization
	public void Start ()
	{
		// Setup audio components
		itemAudioSource = this.GetComponent<AudioSource>();
		itemAudioSource.clip = shotSound;
		itemAudioSource.loop = loopAudio;

		// Setup idle particle system
		if (idleParticles)
		{
			idleParticles = Instantiate(idleParticles, this.transform.position, this.transform.rotation) as ParticleSystem;
			idleParticles.transform.SetParent(this.transform);
			idleParticles.Play();
		}

		// Start shot detection coroutine
		StartCoroutine(TimedShotCoroutine());

		// Find all renderers and colliders for item
		if (itemRenderers.Count == 0) {
			itemRenderers.AddRange(this.GetComponents<Renderer>());
			MyUtilities.GameObjectUtil.IterateChildren(this.gameObject, MyUtilities.GameObjectUtil.RecurseType.RECURSE, child => {
				itemRenderers.AddRange(child.GetComponents<Renderer>());
			});
		}
		if (itemRenderers.Count == 0) {
			throw new MissingReferenceException("No renderers found for " + this.gameObject.name + ". Must manually set in inspector.");
		}

		if (itemColliders.Count == 0) {
			itemColliders.AddRange(this.GetComponents<Collider>());
			MyUtilities.GameObjectUtil.IterateChildren(this.gameObject, MyUtilities.GameObjectUtil.RecurseType.RECURSE, child => {
				itemColliders.AddRange(child.GetComponents<Collider>());
			});
		}

		if (itemColliders.Count == 0) {
			throw new MissingReferenceException("No colliders found for " + this.gameObject.name + ". Must manually set in inspector.");
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{

	}


	void OnCollisionEnter (Collision other)
	{
		(this as IPickupable).OnCollisionEnter(other);
	}

	#endregion

	#region ClassMethods

	private IEnumerator TimedShotCoroutine()
	{
		float shotWaitTime = 1 / shotsPerSecond;
		while(true)
		{
			if (isActiveWeapon) {
				if (!Input.GetButton(fireButtonName)) {
					if (loopAudio) {
						itemAudioSource.loop = false;
						//itemAudioSource.Stop();
					}
				}
				yield return null;

				if (Input.GetButton(fireButtonName))
				{
					if (loopAudio) {
						itemAudioSource.loop = true;
						if (!itemAudioSource.isPlaying) {
							itemAudioSource.Play();
						}
					} else {
						itemAudioSource.Play();
						itemAudioSource.time = 0f;
					}

					if (hasClipSize && currentBulletsShot > ammoClipSize) {
						currentBulletsShot = 0;
						yield return StartCoroutine(ReloadItem());
						continue;
					}
					currentBulletsShot++;
					ShootWeapon ();
					yield return new WaitForSeconds(shotWaitTime);
				}
			} else {
				yield return new WaitForSeconds(0.5f); // No need to check often for inactive weapons
			}
		}
	}



	public void ShootWeapon()
	{
		for (int i = 0; (i < bulletsPerShot); i++) {
			Vector3 spreadShotVector = RandomSpreadVector().normalized;
			Rigidbody localProjectile = Instantiate(projectile, 
			                                        this.transform.TransformPoint(projectileStartPosOffset),
			                                        this.transform.rotation) as Rigidbody;
			// Apply force to projectile
			localProjectile.AddForce(spreadShotVector * projectileForcePower, ForceMode.Impulse);
			
			ownerRbody.AddForce(-spreadShotVector * recoilForce, ForceMode.Impulse);
		}
		if (shotParticles) {
			ParticleSystem shotEffect = Instantiate(shotParticles, this.transform.TransformPoint(projectileStartPosOffset),
			                                        this.transform.rotation) as ParticleSystem;
			Destroy (shotEffect.gameObject, shotEffect.duration);
		}

	}


	private IEnumerator ReloadItem() {
		AudioSource.PlayClipAtPoint(reloadSound, this.transform.position);
		yield return new WaitForSeconds(reloadDuration);
		yield break;
	}

	/// <summary>
	/// Returns the weapons random spread vector, relative to its current forward vector.
	/// </summary>
	/// <returns>The unnormalized random shot vector.</returns>
	private Vector3 RandomSpreadVector()
	{
		float halfSpread = maxSpreadAngle * 0.5f;
		float randSpread = Random.Range(-halfSpread, halfSpread);
		return  Quaternion.Euler(spreadWorldRotationAxis * randSpread) * this.transform.forward;
	}

	/// <summary>
	/// Changes the physical properties such that collisions and physics
	/// do or do not take effect.
	/// </summary>
	/// <param name="removePhysics">If set to <c>true</c> remove physics.</param>
	private void ChangePhysics(bool removePhysics) {
		this.GetComponent<Rigidbody>().isKinematic = removePhysics;

		this.GetComponentInChildren<Collider>().isTrigger = removePhysics;
		this.GetComponent<Collider>().isTrigger = removePhysics;
	}
	#endregion
	


	#region IPickupable implementation

	public void GetPickedPickup ()
	{
		//isActiveWeapon = true;
		EnableItem(false);
		ChangePhysics(true);

		AlignPositionOnPickup();
		AudioSource.PlayClipAtPoint(pickupSound, this.transform.position);
	}
	

	public void GetDropped ()
	{
		//isActiveWeapon = false;
		ChangePhysics(false);
		ownerRbody = null;
	}

	public void ActivateChildren(bool active) {
		this.gameObject.GetComponentsInChildren<Transform>(true).ToList().ForEach( trans => {
			if (trans.gameObject != this.gameObject) {
				trans.gameObject.SetActive(active);
			}
		});
	}

	public void EnableItem(bool active)
	{
		isActiveWeapon = active;
		itemRenderers.ForEach(rend => rend.enabled = active);
		itemColliders.ForEach(coll => coll.enabled = active);
		ActivateChildren(active);

	}


	public void AlignPositionOnPickup ()
	{
		this.transform.localPosition = weaponPosOffset;
		this.transform.localRotation = Quaternion.Euler(weaponRotOffset);
	}


	void IPickupable.OnCollisionEnter (Collision other)
	{
		IPickup pu = (IPickup)(other.gameObject.GetComponent(typeof(IPickup)));
		if (pu != null) {
			if (pu.HasItem(this.gameObject)) {
//				print ("Getting dropped.");
//				this.GetDropped(); // Does order matter?!
//				pu.Drop(this.gameObject);
			} else {
				this.GetPickedPickup();
				pu.Pickup(this.gameObject);
			}
		}
		// else object didn't implement IPickup
	}


	public void SetOwner (GameObject owner)
	{
		Rigidbody or = owner.GetComponent<Rigidbody>();
		if (or) {
			ownerRbody = or;
		}
	}
	
	#endregion
}


