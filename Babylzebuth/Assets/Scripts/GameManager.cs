﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField]
	private AudioSource mySounds;

	[SerializeField]
	private AudioClip SpikesSound;
	[SerializeField]
	private AudioClip ArrowsSound;

	[SerializeField]
	private GameObject baby;
	[SerializeField]
	private GameObject bonus;
	[SerializeField]
	private List<Trap> spikes = new List<Trap>();
	[SerializeField]
	private GameObject arrowsR;
	[SerializeField]
	private GameObject arrowsL;
	[SerializeField]
	private Transform spawnArrowR;
	[SerializeField]
	private Transform spawnArrowL;
	


	private bool gameIsOver = false;

	private float timer = 0;
	private int scoreP1 = 0;
	private int scoreP2 = 0;
	private int scoreP3 = 0;
	private int scoreP4 = 0;

	[SerializeField]
	private int babiesTimeRate = 5;

	void Start ()
	{
		Instance = this;
		mySounds = GetComponent<AudioSource>();

		timer = 0;
		scoreP1 = 0;
		scoreP2 = 0;
		scoreP3 = 0;
		scoreP4 = 0;
		babiesTimeRate = 5;
		gameIsOver = false;
		MenuManager.Instance.ScoreInGame(scoreP1, scoreP2);

		if (Time.timeScale != 1)
			Time.timeScale = 1;

		//StartCoroutine("BabiesCoroutine");
		//StartCoroutine("BonusCoroutine");
		//StartCoroutine("SpikesCoroutine");
		StartCoroutine("ArrowsCoroutine");
	}
	
	void Update ()
	{
		timer += Time.deltaTime;
		MenuManager.Instance.Clock(string.Format("{0:0}:{1:00}.{2:0}", Mathf.Floor(timer / 60), Mathf.Floor(timer) % 60, Mathf.Floor((timer * 10) % 10)));

		if (Input.GetKeyDown(KeyCode.B))
		{
			babiesTimeRate -= 1;
		}

		if (timer >= 60 && !gameIsOver)
		{
			gameIsOver = true;
			timer = 60;
			Time.timeScale = 0;
			MenuManager.Instance.ChangeState(MenuManager.GameState.Score);
			MenuManager.Instance.Podium(scoreP1, scoreP2);
		}
	}

	public void AddScore(string _playerName)
	{
		switch(_playerName)
		{
			case "p1":
				scoreP1 += 1;
			break;
			case "p2":
				scoreP2 += 1;
			break;
			case "p3":
				scoreP3 += 1;
			break;
			case "p4":
				scoreP4 += 1;
			break;
			default:
				Debug.Log("Fail AddScore");
			break;
		}
		MenuManager.Instance.ScoreInGame(scoreP1, scoreP2);
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

	IEnumerator SpikesCoroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(1);

			for (int i = 0; i < spikes.Count; i++)
			{
				mySounds.PlayOneShot(SpikesSound);
				spikes[i].transform.position = new Vector3(spikes[i].transform.position.x, spikes[i].transform.position.y + 0.5f, spikes[i].transform.position.z);
				spikes[i].GetComponent<Collider>().enabled = false;
				spikes[i].canStun = true;
			}

			yield return new WaitForSeconds(3);

			for (int i = 0; i < spikes.Count; i++)
			{
				mySounds.PlayOneShot(SpikesSound);
				spikes[i].transform.position = new Vector3(spikes[i].transform.position.x, spikes[i].transform.position.y - 0.5f, spikes[i].transform.position.z);
				spikes[i].GetComponent<Collider>().enabled = true;
				spikes[i].canStun = false;
			}

			yield return new WaitForSeconds(2);
		}
	}

	IEnumerator ArrowsCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1, 3));

			mySounds.PlayOneShot(ArrowsSound);
			Instantiate(arrowsR, spawnArrowR.position, spawnArrowR.rotation);
			arrowsR.transform.position = new Vector3(arrowsR.transform.position.x, arrowsR.transform.position.y, arrowsR.transform.position.z);

			yield return new WaitForSeconds(Random.Range(2, 5));

			mySounds.PlayOneShot(ArrowsSound);
			Instantiate(arrowsL, spawnArrowL.position, spawnArrowL.rotation);

			yield return new WaitForSeconds(Random.Range(3, 7));
		}
	}
}
