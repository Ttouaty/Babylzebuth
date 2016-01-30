using UnityEngine;
using System.Collections;

public class Baby : MonoBehaviour
{
	private Rigidbody myRigidbody;
	private Transform myTransform;

	[SerializeField]
	private bool isCatch = false;
	[SerializeField]
	private bool isMoving = false;
	[SerializeField]
	private bool isArrived = false;

	[SerializeField]
	private Vector3 targetPos;

	[SerializeField]
	private float speed;

	void Start ()
	{
		myRigidbody = GetComponent<Rigidbody>();
		myTransform = GetComponent<Transform>();
		targetPos = this.transform.position;
	}
	
	void Update ()
	{
		if (!isCatch && !isMoving && !isArrived)
			StartCoroutine("WalkCoroutine");
	}

	public void Catch(Transform _playerTransform)
	{
		isCatch = true;
		isMoving = false;
		isArrived = false;

		myTransform.position = _playerTransform.position;
		myTransform.SetParent(_playerTransform);
		this.GetComponent<Renderer>().enabled = false;
	}

	public void Ejection(Vector3 _force)
	{
		myTransform.parent = null;
		this.GetComponent<Renderer>().enabled = true;
		Debug.Log(_force);
		myRigidbody.AddForce(_force);
	}

	IEnumerator WalkCoroutine()
	{
		isMoving = true;
		isArrived = false;

		while (!isCatch)
		{
			Vector3 dir = this.transform.position - targetPos;
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
