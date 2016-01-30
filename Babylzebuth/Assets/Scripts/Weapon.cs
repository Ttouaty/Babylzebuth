using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	[SerializeField]
	private AudioSource mySounds;

	[SerializeField]
	private AudioClip hitWallSound;
	[SerializeField]
	private AudioClip hitSound;

	private bool _isLaunchable = true;
	private bool _isReturning = false;
	private bool _isRetrievable = false;
	private string _targetTag;

	private Transform _playerTarget;
	private Vector3 _direction;
	[SerializeField]
	private float speed = 100;

	private Rigidbody _rigidB;

	void Start()
	{
		mySounds = GetComponent<AudioSource>();
		this._rigidB = GetComponent<Rigidbody>();
		this.GetComponent<MeshRenderer>().enabled = false;
	}

	public bool Retrieve(Transform target) 
	{
		if (!this._isRetrievable)
			return false;
		this._isReturning = true;
		this._isRetrievable = false;
		this._playerTarget = target;
		this.GetComponent<Collider>().enabled = false;
		StopAllCoroutines();
		StartCoroutine("RetrieveCoroutine");
		return true;
	}

	public bool Launch(Vector3 pos, Vector3 dir, string targetTag)
	{
		if (!this._isLaunchable)
			return false;
		this.GetComponent<MeshRenderer>().enabled = true;
		this._isRetrievable = false;
		this.GetComponent<Collider>().enabled = true;
		this.transform.parent = null;
		this.transform.position = pos;
		this._targetTag = targetTag;
		this._direction = dir.normalized;
		this._isLaunchable = false;
		this._rigidB.velocity = this._direction.normalized * speed;
		return true;
	}

	private void Stop() 
	{
		if (this._isReturning)
			return;
		this._rigidB.velocity = Vector3.zero;
		this._isRetrievable = true;
		this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
		StopAllCoroutines();
	}

	private IEnumerator RetrieveCoroutine()
	{
		float elapsedTime = 0;
		float retrieveTime = 0.2f;

		this._isReturning = true;
		Vector3 startPos = this.transform.position;
		while (elapsedTime < retrieveTime)
		{
			elapsedTime += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, this._playerTarget.position, elapsedTime / retrieveTime);
			yield return null;
		}
		this._isReturning = false;
		this.GetComponent<Collider>().enabled = false;
		this.GetComponent<MeshRenderer>().enabled = false;
		this._isLaunchable = true;
	}


	void OnTriggerEnter(Collider colli)
	{
		if (colli.tag != this._targetTag) 
		{
			if (colli.GetComponent<PlayerController>() != null)
			{
				mySounds.PlayOneShot(hitSound);
				colli.GetComponent<PlayerController>().Damage(1, this._direction.normalized * 20);
			}
			else
			{
				mySounds.PlayOneShot(hitWallSound);
			}

			this.Stop();
		}
	}

}
