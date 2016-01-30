using UnityEngine;
using System.Collections;

public class Baby : MonoBehaviour
{
	private Rigidbody myRigidbody;

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
		targetPos = this.transform.position;
	}
	
	void Update ()
	{
		if (!isCatch && !isMoving && !isArrived)
			StartCoroutine("WalkCoroutine");
	}

	public void Catch()
	{
		isCatch = true;
		isMoving = false;
		isArrived = false;
	}

	public void Ejection(Vector3 _force)
	{
		myRigidbody.AddForce(_force);
	}

	IEnumerator WalkCoroutine()
	{
		isMoving = true;
		isArrived = false;

		while (!isCatch)
		{
			Vector3 dir = this.transform.position - targetPos;
			this.myRigidbody.velocity = dir.normalized * -speed * Time.deltaTime;
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
