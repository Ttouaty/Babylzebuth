using UnityEngine;
using System.Collections;

public class Baby : MonoBehaviour
{
	[SerializeField]
	private AudioSource mySounds;

	[SerializeField]
	private AudioClip babySound;

	private Rigidbody myRigidbody;
	private Transform myTransform;
	public bool isCatchable = true;

	[SerializeField]
	private bool isCatch = false;
	[SerializeField]
	private bool isMoving = false;
	[SerializeField]
	private bool isArrived = false;
	[SerializeField]
	private bool isOnFloor = true;

	[SerializeField]
	private Vector3 targetPos;
	public Vector3 originalTargetPos;

	[SerializeField]
	private float speed;

	void Start ()
	{
		myRigidbody = GetComponent<Rigidbody>();
		myTransform = GetComponent<Transform>();
		mySounds = GetComponent<AudioSource>();
		targetPos = this.transform.position;
		this.originalTargetPos = this.targetPos;
	}
	
	void Update ()
	{
		if (!isCatch && !isMoving && !isArrived && isOnFloor)
			StartCoroutine("WalkCoroutine");
		if (isArrived)
			this.myRigidbody.velocity *= 0.95f;
	}

	public void Catch(Transform _playerTransform)
	{
		if (!this.isCatchable)
			return;
		
		isCatch = true;
		isMoving = false;
		isArrived = false;

		this.GetComponent<Renderer>().enabled = false;
		this.GetComponent<Collider>().enabled = false;
		myRigidbody.velocity = Vector3.zero;
		myRigidbody.useGravity = false;
		this.isCatchable = false;
	}

	public void Ejection(Vector3 startPosition, Vector3 direction)
	{
		myTransform.parent = null;
		this.isOnFloor = false;
		this.isCatchable = false;
		this.isCatch = false;
		this.GetComponent<Renderer>().enabled = true;
		this.transform.position = new Vector3(startPosition.x, 1, startPosition.z);
		this.gameObject.layer = LayerMask.NameToLayer("BabyAir");
		this.GetComponent<Collider>().enabled = true;
		myRigidbody.velocity = direction;
		myRigidbody.useGravity = true;
		StartCoroutine("delayAfterEject");
	}

	IEnumerator delayAfterEject()
	{
		yield return new WaitForSeconds(1);
		this.isCatchable = true;
		this.gameObject.layer = LayerMask.NameToLayer("Baby");
		yield return new WaitForSeconds(1.5f);
		this.isOnFloor = true;
	}

	public void Kill()
	{
		mySounds.PlayOneShot(babySound,1);
		myTransform.parent = null;
		Destroy(this.gameObject);
		StopAllCoroutines();
		GameManager.Instance.babiesActive.Remove(this);
	}

	public void FocusTarget(Vector3 target)
	{
		targetPos = target;
		isArrived = false;
	}

	IEnumerator WalkCoroutine()
	{
		isMoving = true;
		isArrived = false;

		while (!isCatch)
		{
			Vector3 dir = targetPos - this.transform.position;
			this.myRigidbody.velocity = dir.normalized * speed;
			if(dir.magnitude < 1)
			{
				this.isArrived = true;
				break;
			}
			yield return null;
		}
		isMoving = false;
	}
}
