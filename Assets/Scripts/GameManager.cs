using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;

	private GameScene thisScene;

	//might be used in future for loading checkpoints
	private GameObject player; 

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

	public void SetPlayer(GameObject player)
	{
		this.player = player;
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
}
