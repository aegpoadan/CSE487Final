using UnityEngine;
using System.Collections;
using System.Linq;


[RequireComponent(typeof(Rigidbody))]
public class PhysicsWeapon : MonoBehaviour, IPickupable {

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
	public Vector3 projectileStartPosOffset;
	/// <summary>
	/// The local offset position from the parent transform.
	/// </summary>
	public Vector3 weaponPosOffset = new Vector3(0, 0, 2);
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
	public float projectileForcePower = 60f;
	/// <summary>
	/// Force applied to the character after every shot.
	/// </summary>
	public float recoilForce = 10f;
	/// <summary>
	/// Bullets per second.
	/// </summary>
	public float bulletsPerSecond = 1f;
	/// <summary>
	/// Angle of spread cone. Zero is straight.
	/// </summary>
	public float maxSpreadAngle = 45.0f;
	/// <summary>
	/// The spread rotation vector.
	/// </summary>
	public Vector3 spreadRotationAxis = new Vector3(0, 0, 1);
	/// <summary>
	/// Particle system to play at all times.
	/// </summary>
	public ParticleSystem idleParticles;
	/// <summary>
	/// Sound to play when item is picked up.
	/// </summary>
	public AudioClip pickupSound;

	/// <summary>
	/// React to button input only if weapon is active (used by player).
	/// </summary>
	private bool isActiveWeapon = false;
	private Renderer itemRenderer;


	public void Awake()
	{

	}

	#region UnityMethods

	// Use this for initialization
	public void Start ()
	{
		if (idleParticles)
		{
			idleParticles = Instantiate(idleParticles, this.transform.position, this.transform.rotation) as ParticleSystem;
			idleParticles.transform.SetParent(this.transform);
			idleParticles.Play();
		}
		StartCoroutine(TimedShotCoroutine());

		if (!itemRenderer) {
			itemRenderer = GetComponent<Renderer>();
		}
		if (!itemRenderer) {
			itemRenderer = GetComponentInChildren<Renderer>();
		}
		if (!itemRenderer) {
			throw new MissingReferenceException("Must specify the primary " + this.gameObject.name + " renderer.");
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
		float shotWaitTime = 1 / bulletsPerSecond;
		while(true)
		{
			if (Input.GetButton(fireButtonName) && isActiveWeapon)
			{
				ShootWeapon ();
				yield return new WaitForSeconds(shotWaitTime);
			}
			yield return null;
		}
	}


	public void ShootWeapon()
	{
		Vector3 spreadShotVector = RandomSpreadVector().normalized;
		Rigidbody localProjectile = Instantiate(projectile, 
		                                        this.transform.position + projectileStartPosOffset, 
		                                        this.transform.rotation) as Rigidbody;
		// Apply force to projectile
		localProjectile.AddForce(spreadShotVector * projectileForcePower, ForceMode.Impulse);

		ownerRbody.AddForce(-spreadShotVector * recoilForce, ForceMode.Impulse);
	}

	/// <summary>
	/// Returns the weapons random spread vector, relative to its current forward vector.
	/// </summary>
	/// <returns>The unnormalized random shot vector.</returns>
	private Vector3 RandomSpreadVector()
	{
		float halfSpread = maxSpreadAngle * 0.5f;
		float randSpread = Random.Range(-halfSpread, halfSpread);
		return  Quaternion.Euler(spreadRotationAxis * randSpread) * this.transform.forward;
	}

	/// <summary>
	/// Changes the physical properties such that collisions and physics
	/// do or do not take effect.
	/// </summary>
	/// <param name="removePhysics">If set to <c>true</c> remove physics.</param>
	private void ChangePhysics(bool removePhysics) {
		this.GetComponent<Rigidbody>().isKinematic = removePhysics;

		this.GetComponentInChildren<Collider>().isTrigger = removePhysics;
		//this.GetComponent<Collider>().isTrigger = removePhysics;
	}
	#endregion
	


	#region IPickupable implementation

	public void GetPickedPickup ()
	{
		ChangePhysics(true);
		isActiveWeapon = true;

		AlignPositionOnPickup();
		AudioSource.PlayClipAtPoint(pickupSound, this.transform.position);
	}
	

	public void GetDropped ()
	{
		isActiveWeapon = false;
		ChangePhysics(false);
		ownerRbody = null;
	}

	public void SetActive(bool active)
	{
		isActiveWeapon = active;
		itemRenderer.enabled = active;

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


