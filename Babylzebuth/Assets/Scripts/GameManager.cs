using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private GameObject baby;
	[SerializeField]
	private GameObject bonus;
	[SerializeField]
	private List<Trap> traps = new List<Trap>();

	[SerializeField]
	private float timer = 0;

	[SerializeField]
	private int scoreP1 = 0;
	[SerializeField]
	private int scoreP2 = 0;
	[SerializeField]
	private int scoreP3 = 0;
	[SerializeField]
	private int scoreP4 = 0;

	[SerializeField]
	private int babiesTimeRate = 5;

	void Start ()
	{
		StartCoroutine("BabiesCoroutine");
		StartCoroutine("BonusCoroutine");
		StartCoroutine("TrapsCoroutine");
	}
	
	void Update ()
	{
		timer += Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			Instantiate(baby, new Vector3(Random.Range(-9, 9), 0.5f, Random.Range(-9, 9)), Quaternion.identity);
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			Debug.Log(string.Format("{0:0}:{1:00}.{2:0}", Mathf.Floor(timer / 60), Mathf.Floor(timer) % 60, Mathf.Floor((timer * 10) % 10)));
			Debug.Log(timer);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			babiesTimeRate -= 1;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			baby.GetComponent<Baby>().Ejection(new Vector3(0, 0, 1));
		}

		if(timer >= 60)
		{
			Debug.Log("Game Over !");
			timer = 60;
			//Pause du jeu et Affichage des Scores
		}
	}

	public void AddScore(string _playerName)
	{
		switch(_playerName)
		{
			case "Player_1":
				scoreP1 += 1;
			break;
			case "Player_2":
				scoreP2 += 1;
			break;
			case "Player_3":
				scoreP3 += 1;
			break;
			case "Player_4":
				scoreP4 += 1;
			break;
			default:
				Debug.Log("Fail AddScore");
			break;
		}
	}

	IEnumerator BabiesCoroutine()
	{
		Instantiate(baby, new Vector3(Random.Range(-9, 9), 0.5f, Random.Range(-9, 9)), Quaternion.identity);
		yield return new WaitForSeconds(babiesTimeRate);
		StartCoroutine("BabiesCoroutine");
	}

	IEnumerator BonusCoroutine()
	{
		Instantiate(bonus, new Vector3(Random.Range(-9, 0), 0.5f, Random.Range(-9, 0)), Quaternion.identity);
		yield return new WaitForSeconds(8);
		StartCoroutine("BonusCoroutine");
	}

	IEnumerator TrapsCoroutine()
	{
		while(true)
		{
			for (int i = 0; i < traps.Count; i++)
			{
				traps[i].transform.position = new Vector3(traps[i].transform.position.x, traps[i].transform.position.y + 0.5f, traps[i].transform.position.z);
				traps[i].canStun = true;
			}

			yield return new WaitForSeconds(2);

			for (int i = 0; i < traps.Count; i++)
			{
				traps[i].transform.position = new Vector3(traps[i].transform.position.x, traps[i].transform.position.y - 0.5f, traps[i].transform.position.z);
				traps[i].canStun = false;
			}

			yield return new WaitForSeconds(3);
		}
	}
}
