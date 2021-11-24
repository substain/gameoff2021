using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public enum GameOverReason
	{
		CoverBlown, WentSwimming, TooMuchCheese
	}

	public static GameManager Instance = null;

	[SerializeField]
	private GameScene thisScene;

	private PlayerInteraction player; 

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one GameManager in this scene.");
		}
		Instance = this;
	}

	public void ReloadCurrentScene()
	{
		LoadScene(thisScene);
	}

	public void LoadScene(GameScene gameScene)
	{
		SceneManager.LoadScene(ToSceneName(gameScene));
	}

	public void SetPlayer(PlayerInteraction player)
	{
		this.player = player;
	}

	public void SetGameOver(GameOverReason gameOverReason)
	{
		player.SetMenuActive(true);
		HUDManager.Instance.ShowIngameMenu(IngameOverlayMenu.IngameMenuType.gameover, GameOverReasonToString(gameOverReason));
	}

	public void StartPauseMenu()
	{
		player.SetMenuActive(true);
		SetPaused(true);
		HUDManager.Instance.ShowIngameMenu(IngameOverlayMenu.IngameMenuType.pause, GetRandomPauseMenuTitle());
	}

	public void HideIngameMenu()
	{
		player.SetMenuActive(false);
		SetPaused(false);
		HUDManager.Instance.HideIngameMenu();
	}

	public void SetPaused(bool isPaused)
	{
		Time.timeScale = isPaused? 0 : 1;
	}

	void OnDestroy()
	{
		Instance = null;
	}

	//Wrapper to map game scene to unity scene name, for cases where GameScene.ToString() is not equal to the Scene names.
	public static string ToSceneName(GameScene gameScene)
	{
		return gameScene.ToString();
	}

	public static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	private readonly List<string> coverBlownTitles = new List<string>() {
		"your cover was blown!",
		"you were uncovered!",
		"you failed!",
		"they found you!",
		"you have nowhere to hide!",
		"you were detected!"
	};

	private readonly List<string> wentSwimmingTitles = new List<string>() {
		"swimming is not recommended!",
		"bad weather for a dive!",
		"you failed!",
		"the sharks were waiting!",
		"don't dare to spy on the ocean!",
		"you forgot your swimsuit!",
		"the water is polluted, you fool!",
		"there is no hope for humanity..."
	};

	private readonly List<string> tooMuchCheeseTitles = new List<string>() {
		"you were trapped like a mouse!",
		"you left too many cheese trails!",
		"cheese is bad for your health!",
		"you found the cheesy end!",
		"you don't know when to stop!"
	};

	public string GameOverReasonToString(GameOverReason gor)
	{
		switch (gor)
		{
			case GameOverReason.CoverBlown:
				{
					return Util.ChooseRandomFromList(coverBlownTitles);
				}
			case GameOverReason.WentSwimming:
				{
					return Util.ChooseRandomFromList(wentSwimmingTitles);
				}
			case GameOverReason.TooMuchCheese:
				{
					return Util.ChooseRandomFromList(tooMuchCheeseTitles);
				}
		}
		return "you failed.";
	}

	private readonly List<string> pauseMenuTitles = new List<string>() { 
		"game paused.", 
		"you needed a break.", 
		"time to sleep, agent.", 
		"you are a great listener.",
		"time to get some cheese sticks.",
		"keep it up, agent.",
		"we'll wait here."
	};

	public string GetRandomPauseMenuTitle()
	{
		return Util.ChooseRandomFromList(pauseMenuTitles);
	}

}
