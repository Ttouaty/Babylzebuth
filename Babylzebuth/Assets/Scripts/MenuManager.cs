using UnityEngine;
using UnityEngine.EventSystems;
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
	public EventSystem myEventSys;

	public RectTransform panelSplash;
	public RectTransform panelMenu;
	public RectTransform panelPlay;
	public RectTransform panelScore;

	public Text timer;

	public Text scoreInGameP1;
	public Text scoreInGameP2;

	public Sprite couronne;
	public Sprite couronneP1;
	public Sprite couronneP2;

	public GameObject playButton;
	public GameObject scoreButton;

	public GameState currentState = GameState.Menu;
	public enum GameState
	{
		Splash,
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
		myEventSys = transform.GetChild(0).GetComponent<EventSystem>();
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
			panelScore.GetComponent<Image>().sprite = couronneP1;
		else if (_scoreP1 < _scoreP2)
			panelScore.GetComponent<Image>().sprite = couronneP2;
	}

	public void ToPlayState()
	{
		mySounds.PlayOneShot(buttonSound);
		ChangeState(GameState.Play);
		Application.LoadLevel("Scene2D");
		mySounds.loop = true;
		mySounds.PlayOneShot(themeSound[1], 0.5f);
	}

	public void ToMenuState()
	{
		if (currentState == GameState.Play)
		{
			Application.UnloadLevel("Scene2D");
		}
		ChangeState(GameState.Menu);
	}

	public void ChangeState(GameState _currentState)
	{
		switch (_currentState)
		{
			case GameState.Splash:
				panelSplash.gameObject.SetActive(true);
				panelMenu.gameObject.SetActive(false);
				panelPlay.gameObject.SetActive(false);
				panelScore.gameObject.SetActive(false);
				myEventSys.firstSelectedGameObject = playButton;
				break;
			case GameState.Menu:
				panelSplash.gameObject.SetActive(false);
				panelMenu.gameObject.SetActive(true);
				panelPlay.gameObject.SetActive(false);
				panelScore.gameObject.SetActive(false);
				myEventSys.firstSelectedGameObject = playButton;
				break;
			case GameState.Play:
				panelSplash.gameObject.SetActive(false);
				panelMenu.gameObject.SetActive(false);
				panelPlay.gameObject.SetActive(true);
				panelScore.gameObject.SetActive(false);
				myEventSys.firstSelectedGameObject = null;
				break;
			case GameState.Score:
				panelSplash.gameObject.SetActive(false);
				panelMenu.gameObject.SetActive(false);
				panelPlay.gameObject.SetActive(false);
				panelScore.gameObject.SetActive(true);
				myEventSys.firstSelectedGameObject = scoreButton;
				Debug.Log(myEventSys.currentSelectedGameObject);
				break;
		}
		Debug.Log(currentState);
	}
}
