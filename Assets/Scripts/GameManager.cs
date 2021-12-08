using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MainManager
{
	public enum GameOverReason
	{
		CoverBlown, WentSwimming, TooMuchCheese
	}

	public static new GameManager Instance;

	private bool gameOver = false;

	[SerializeField]
	private float timeToFinish = 4.0f;

	[SerializeField]
	private GameScene thisScene;	
	
	[SerializeField]
	private GameScene nextScene;

	[SerializeField]
	private ConstraintManager.GameConstraint constraintNeededForNextLevel;

	private PlayerInteraction player;

	private AudioSource musicAudioSource;

	[SerializeField]
	private AudioClip initialBackgroundClip;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void SetInstance()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
		}
		Instance = this;
	}

	void Start()
	{
		AudioSource[] audioSources = GetComponents<AudioSource>();
		if (audioSources.Length < 1)
		{
			Debug.LogWarning("Warning: No audio sources were found.");
		}
		musicAudioSource = audioSources[0];
		musicAudioSource.clip = initialBackgroundClip;
		musicAudioSource.loop = true;
		musicAudioSource.Play();

		ConstraintManager.OnChangeConstraints += CheckLevelFinished;
	}

	public void ReloadCurrentScene()
	{
		LoadScene(thisScene);
	}

	public void LoadNextLevel()
	{
		SceneManager.LoadScene(ToSceneName(nextScene));
	}

	public void SetPlayer(PlayerInteraction player)
	{
		this.player = player;
	}

	public void SetGameOver(GameOverReason gameOverReason)
	{
		if (gameOver)
		{
			return;
		}
		gameOver = true;
		SetMenuActive(MenuController.MenuType.gameover, gameOverReason: gameOverReason);
	}

	public void StartPauseMenu()
	{
		SetMenuActive(MenuController.MenuType.pause);
	}

	public void StartOptionsMenu()
	{
		SetMenuActive(MenuController.MenuType.options);
	}

	public void HideIngameMenu()
	{
		SetMenuActive(null);
	}

	public void SetMenuActive(MenuController.MenuType? menuType, MenuController.MenuType? parentMenu = null, GameOverReason? gameOverReason = null)
	{
		bool menuActive = menuType.HasValue;
		if (menuActive)
		{
			PauseBackgroundMusic();
		}
		else
		{
			ContinueBackgroundMusic();
		}
		player?.SetMenuActive(menuActive);
		SetPaused(menuActive);
		UIManager.Instance.SetMenuActive(menuType, parentMenu, gameOverReason);
	}

	public void SetPaused(bool isPaused)
	{
		Time.timeScale = isPaused? 0 : 1;
	}

	void OnDestroy()
	{
		ConstraintManager.OnChangeConstraints -= CheckLevelFinished;
		Instance = null;
	}

	public void CheckLevelFinished()
	{
		if (ConstraintManager.Instance.IsSatisfied(constraintNeededForNextLevel))
		{
			Timer timer = gameObject.AddComponent<Timer>();
			timer.Init(timeToFinish, LoadNextLevel);
			ConstraintManager.OnChangeConstraints -= CheckLevelFinished;
		}
	}

	public void StartBackgroundMusicClip(AudioClip clip)
	{
		if(musicAudioSource.clip == clip)
		{
			return;
		}
		musicAudioSource.clip = clip;
		musicAudioSource.Play();
	}

	public void PauseBackgroundMusic()
	{
		musicAudioSource.Pause();
	}

	public void ContinueBackgroundMusic()
	{
		musicAudioSource.Play();
	}
}
