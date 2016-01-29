using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private GameObject bonus;
	[SerializeField]
	private List<GameObject> traps = new List<GameObject>();

	//[SerializeField]
	private float timer;

	void Start ()
	{
		StartCoroutine("BonusCoroutine");
		StartCoroutine("TrapsCoroutine");
	}
	
	void Update ()
	{

	}

	IEnumerator BonusCoroutine()
	{
		Instantiate(bonus, new Vector3(Random.Range(-9, 0), 0.5f, Random.Range(-9, 0)), Quaternion.identity);

		yield return new WaitForSeconds(2);
		StartCoroutine("BonusCoroutine");
	}

	IEnumerator TrapsCoroutine()
	{
		while(true)
		{
			for (int i = 0; i < traps.Count; i++)
			{
				traps[i].transform.position = new Vector3(traps[i].transform.position.x, traps[i].transform.position.y + 0.5f, traps[i].transform.position.z);
			}

			yield return new WaitForSeconds(2);

			for (int i = 0; i < traps.Count; i++)
			{
				traps[i].transform.position = new Vector3(traps[i].transform.position.x, traps[i].transform.position.y - 0.5f, traps[i].transform.position.z);
			}
			yield return new WaitForSeconds(3);
		}
	}
}
