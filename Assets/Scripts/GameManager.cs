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

	public static GameManager GameInstance;

	private bool gameOver = false;

	[SerializeField]
	private float timeToFinish = 1.0f;

	[SerializeField]
	private GameScene thisScene;	
	
	[SerializeField]
	private GameScene nextScene;

	[SerializeField]
	private string finalText;

	private const ConstraintManager.GameConstraint constraintNeededForNextLevel = ConstraintManager.GameConstraint.loadNextLevel;

	private PlayerInteraction player;

	private AudioSource musicAudioSource;

	[SerializeField]
	private AudioClip initialBackgroundClip;

	[SerializeField]
	private bool useOverlayFading = true;

	private Vector3? playerStartPos;

	private PlayerInteraction playerInteraction;

	protected override void Awake()
	{
		CheckpointManager.SetCurrentScene(thisScene);
		SetInstance();
		SetPaused(false);
		playerStartPos = CheckpointManager.GetCurrentPosition();
		base.Awake();
	}

	private void SetInstance()
	{
		if (GameInstance != null)
		{
			Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
		}
		GameInstance = this;
	}

	protected override void Start()
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

		if (useOverlayFading)
		{
			HUDManager.HUDInstance.SetOverlayVisible(true);
			HUDManager.HUDInstance.FadeOutOverlay();
		}
		else
		{
			HUDManager.HUDInstance.SetOverlayVisible(false);
		}
	}

	public PlayerInteraction GetPlayer()
	{
		return player;
	}

	public Vector3? GetPlayerStartPos()
	{
		return playerStartPos;
	}

	public int GetFadeDelay()
	{
		return (useOverlayFading && HUDManager.HUDInstance.IsFading()) ? Mathf.CeilToInt(HUDManager.FADE_DURATION) : 0;
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
		GameOverReason usedGameOverReason = player.GetComponent<PlayerMovement>().SlowedByCheese() ? GameOverReason.TooMuchCheese : gameOverReason;
		SetMenuActive(MenuController.MenuType.gameover, gameOverReason: usedGameOverReason);
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

	protected override void OnDestroy()
	{
		ConstraintManager.OnChangeConstraints -= CheckLevelFinished;
		GameInstance = null;
		base.OnDestroy();
	}

	public void CheckLevelFinished()
	{
		if (ConstraintManager.Instance.IsSatisfied(constraintNeededForNextLevel))
		{
			Timer timer = gameObject.AddComponent<Timer>();
			timer.Init(timeToFinish, LoadNextLevel);
			ConstraintManager.OnChangeConstraints -= CheckLevelFinished;
			SettingsManager.SetSceneFinished(thisScene);
			if (useOverlayFading)
			{
				HUDManager.HUDInstance.FadeInOverlay(finalText);
			}
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
