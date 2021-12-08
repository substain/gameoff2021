using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
	public const string GAME_NAME = "Cruise Noir";

	public static MainManager Instance = null;

	protected virtual void Awake()
	{
		SetInstance();
	}

	protected virtual void SetInstance()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one " + this.GetType().ToString() + " in this scene.");
		}
		Instance = this;
	}

	public void LoadScene(GameScene gameScene)
	{
		SceneManager.LoadScene(ToSceneName(gameScene));
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