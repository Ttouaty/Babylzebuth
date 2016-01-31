using UnityEngine;
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
	private Sprite trapSprite3;
	[SerializeField]
	private Sprite trapSprite2;
	[SerializeField]
	private Sprite trapSprite1;

	[Header("BabySpawn")]
	[SerializeField]
	private GameObject baby;
	[SerializeField]
	private GameObject[] SpawnZones;
	[SerializeField]
	private int babiesTimeRate = 5;

	[Space(5)]
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
	private Bonus[] BonusList;
	[SerializeField]
	private Transform BonusAltar;
	public bool BonusSpawned = false;
	private float bonusElapsedTime = 0;
	public List<Baby> babiesActive = new List<Baby>();

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

		StartCoroutine("BabiesCoroutine");
		StartCoroutine("BonusCoroutine");
		StartCoroutine("SpikesCoroutine");
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
		StartCoroutine(PopBaby());
		yield return new WaitForSeconds(babiesTimeRate) ;
		StartCoroutine("BabiesCoroutine");
	}

	IEnumerator PopBaby()
	{
		GameObject selectedSpawnZone = SpawnZones[Random.Range((int)0, (int)SpawnZones.Length)];
		Bounds zoneBounds = selectedSpawnZone.GetComponent<Collider>().bounds;
		GameObject newBaby = Instantiate(baby, zoneBounds.center + getPositionInZone(zoneBounds), Quaternion.identity) as GameObject;
		babiesActive.Add(newBaby.GetComponent<Baby>());
		while (Physics.OverlapSphere(newBaby.transform.position, newBaby.GetComponent<Collider>().bounds.extents.x, ~(1 << LayerMask.NameToLayer("Stage"))).Length > 2)
		{
			baby.transform.position = zoneBounds.center + getPositionInZone(zoneBounds);
			baby.transform.position = new Vector3(baby.transform.position.x, 0.5f, baby.transform.position.z);
			print("relaunch");
			yield return new WaitForEndOfFrame();
		}
	}

	Vector3 getPositionInZone(Bounds zone, int[] constraint = null)
	{ 
		
		Vector3 returnVect = new Vector3(	Random.Range(-zone.extents.x, zone.extents.x),
											Random.Range(-zone.extents.y, zone.extents.y),
											Random.Range(-zone.extents.z, zone.extents.z));

		return returnVect;
	}

	IEnumerator BonusCoroutine()
	{
		float spawnTime = 8;
		while (true)
		{
			bonusElapsedTime += Time.deltaTime;
			if (bonusElapsedTime >= spawnTime)
			{
				resetBonusTimer();
				if (!this.BonusSpawned)
				{
					Instantiate(BonusList[Random.Range((int)0, (int)BonusList.Length)], BonusAltar.position, Quaternion.identity);
					this.BonusSpawned = true;
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void resetBonusTimer() { bonusElapsedTime = 0; }

	IEnumerator SpikesCoroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(1);

			for (int i = 0; i < spikes.Count; i++)
			{
				spikes[i].GetComponent<SpriteRenderer>().sprite = trapSprite3;
				mySounds.PlayOneShot(SpikesSound);
				spikes[i].GetComponent<Collider>().enabled = true;
				spikes[i].canStun = true;
			}

			yield return new WaitForSeconds(3);

			for (int i = 0; i < spikes.Count; i++)
			{
				spikes[i].GetComponent<SpriteRenderer>().sprite = trapSprite2;
				mySounds.PlayOneShot(SpikesSound);
				spikes[i].GetComponent<Collider>().enabled = false;
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
