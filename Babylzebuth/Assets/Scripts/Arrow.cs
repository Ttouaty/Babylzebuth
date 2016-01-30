using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
	[SerializeField]
	private AudioSource mySounds;
	private Rigidbody myRigid;

	[SerializeField]
	private AudioClip hitWallSound;
	[SerializeField]
	private AudioClip hitSound;


	void Start()
	{
		mySounds = GetComponent<AudioSource>();
		myRigid = GetComponent<Rigidbody>();
		StartCoroutine("AttackCoroutine");
	}

	IEnumerator AttackCoroutine()
	{
		while(true)
		{
			myRigid.velocity = new Vector3(0, 0, -1) * 10;
			yield return new WaitForEndOfFrame();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerController>() != null)
		{
			mySounds.PlayOneShot(hitSound);
			other.GetComponent<PlayerController>().Damage(1, new Vector3(0, -1, 0) * 20);
		}
		else
		{
			mySounds.PlayOneShot(hitWallSound);
		}

		Destroy(this.gameObject);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<PlayerController>() != null)
		{
			mySounds.PlayOneShot(hitSound);
			other.gameObject.GetComponent<PlayerController>().Damage(1, new Vector3(0, -1, 0) * 20);
		}
		else
		{
			mySounds.PlayOneShot(hitWallSound);
		}

		Destroy(this.gameObject);
	}
}
