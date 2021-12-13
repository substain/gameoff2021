using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SettingsManager
{
	private const string PP_DIFFICULTY = "CruiseNoir_Difficulty";
	private const string PP_PROGRESS = "CruiseNoir_Progress";

	private const DifficultySetting defaultDifficulty = DifficultySetting.normal;
	private const ProgressSetting defaultProgress = ProgressSetting.none;

	public enum DifficultySetting
	{
		easy, normal
	}

	//use ascending order here
	public enum ProgressSetting
	{
		none, tutorialFinished, chapterOneFinished
	}

	public static void SetDifficulty(DifficultySetting difficulty)
	{
		PlayerPrefs.SetInt(PP_DIFFICULTY, ToDifficultyInt(difficulty));
	}

	public static void SetSceneFinished(GameScene finishedScene)
	{
		ProgressSetting progressReached = SceneToProgress(finishedScene);
		ProgressSetting maxProgress = GetMaxProgress(progressReached, GetProgress());
		SetProgress(maxProgress);
	}

	public static void SetProgress(ProgressSetting progress)
	{
		PlayerPrefs.SetInt(PP_PROGRESS, ToProgressInt(progress));
	}

	public static ProgressSetting GetProgress()
	{
		if (!PlayerPrefs.HasKey(PP_PROGRESS))
		{
			PlayerPrefs.SetInt(PP_PROGRESS, ToProgressInt(defaultProgress));
			return defaultProgress;
		}

		return ToProgress(PlayerPrefs.GetInt(PP_PROGRESS));
	}

	public static DifficultySetting GetDifficulty()
	{
		if (!PlayerPrefs.HasKey(PP_DIFFICULTY))
		{
			PlayerPrefs.SetInt(PP_DIFFICULTY, ToDifficultyInt(defaultDifficulty));
			return defaultDifficulty;
		}

		return ToDifficulty(PlayerPrefs.GetInt(PP_DIFFICULTY));
	}

	public static float GetSlightDifficultyModifier()
	{
		DifficultySetting difficultySetting = GetDifficulty();
		return difficultySetting == DifficultySetting.easy ? 0.8f : 1f;
	}

	public static float GetDifficultyModifier()
	{
		DifficultySetting difficultySetting = GetDifficulty();
		return difficultySetting == DifficultySetting.easy ? 0.65f : 1f;
	}

	public static DifficultySetting ToDifficulty(int difficultyInt)
	{
		return (DifficultySetting) difficultyInt;
	}
	public static int ToDifficultyInt(DifficultySetting difficulty)
	{
		return (int)difficulty;
	}
	public static ProgressSetting SceneToProgress(GameScene scene)
	{
		switch (scene)
		{
			case GameScene.tutorial:
				{
					return ProgressSetting.tutorialFinished;
				}
			case GameScene.chapterOne:
				{
					return ProgressSetting.chapterOneFinished;
				}
			default:
				{
					return ProgressSetting.none;
				}
		}
	}

	public static ProgressSetting ToProgress(int progressInt)
	{
		return (ProgressSetting)progressInt;
	}
	public static int ToProgressInt(ProgressSetting progress)
	{
		return (int)progress;
	}

	public static ProgressSetting GetMaxProgress(ProgressSetting progress1, ProgressSetting progress2)
	{
		return ToProgress(Mathf.Max(ToProgressInt(progress1), ToProgressInt(progress2)));
	}
}
