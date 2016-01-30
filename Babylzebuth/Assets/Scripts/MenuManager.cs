using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	[SerializeField]
	private AudioSource mySounds;
	[SerializeField]
	private AudioClip buttonSound;
	[SerializeField]
	private AudioClip[] themeSound;

	public Canvas myCanvas;
	public Transform myEventSys;

	public RectTransform panelMenu;
	public RectTransform panelPlay;
	public RectTransform panelScore;

	public Text timer;

	public Text scoreInGameP1;
	public Text scoreInGameP2;

	public Text scoreP1;
	public Text scoreP2;

	public Sprite couronne;
	public Image couronneP1;
	public Image couronneP2;

	public GameState currentState = GameState.Menu;
	public enum GameState
	{
		Menu,
		Play,
		Score
	}

	public bool imTheOriginal = false;

	void Awake()
	{
		if (Instance)
		{
			if (!imTheOriginal)
				Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			imTheOriginal = true;
		}
	}

	void Start ()
	{
		mySounds = GetComponent<AudioSource>();
		mySounds.PlayOneShot(themeSound[0]);
	}

	public void Clock(string _timerToShow)
	{
		timer.text = _timerToShow;
	}

	public void ScoreInGame(int _scoreP1, int _scoreP2)
	{	
		scoreInGameP1.text = _scoreP1.ToString();
		scoreInGameP2.text = _scoreP2.ToString();
	}

	public void Podium(int _scoreP1, int _scoreP2)
	{
		if (_scoreP1 > _scoreP2)
			couronneP1.sprite = couronne;
		else if (_scoreP1 < _scoreP2)
			couronneP2.sprite = couronne;

		scoreP1.text = _scoreP1.ToString();
		scoreP2.text = _scoreP2.ToString();
	}

	public void ToPlayState()
	{
		mySounds.PlayOneShot(buttonSound);
		ChangeState(GameState.Play);
		Application.LoadLevel("FirstStage");
		mySounds.loop = true;
		mySounds.PlayOneShot(themeSound[1], 0.5f);
	}

	public void ToMenuState()
	{
		if (currentState == GameState.Play)
		{
			Application.UnloadLevel("FirstStage");
		}
		ChangeState(GameState.Menu);
	}

	public void ChangeState(GameState _currentState)
	{
		switch (_currentState)
		{
			case GameState.Menu:
				panelMenu.gameObject.SetActive(true);
				panelPlay.gameObject.SetActive(false);
				panelScore.gameObject.SetActive(false);
				break;
			case GameState.Play:
				panelMenu.gameObject.SetActive(false);
				panelPlay.gameObject.SetActive(true);
				panelScore.gameObject.SetActive(false);
				break;
			case GameState.Score:
				panelMenu.gameObject.SetActive(false);
				panelPlay.gameObject.SetActive(false);
				panelScore.gameObject.SetActive(true);
				break;
		}
	}
}
