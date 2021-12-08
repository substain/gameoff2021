using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SettingsManager
{
	private const string PP_DIFFICULTY = "CruiseNoir_Difficulty";

	private const DifficultySetting defaultDifficulty = DifficultySetting.normal;

	public enum DifficultySetting
	{
		easy, normal
	}

	public static void SetDifficulty(DifficultySetting difficulty)
	{
		PlayerPrefs.SetInt(PP_DIFFICULTY, ToDifficultyInt(difficulty));
	}

	public static DifficultySetting GetDifficulty()
	{
		if (!PlayerPrefs.HasKey(PP_DIFFICULTY))
		{
			PlayerPrefs.SetInt(PP_DIFFICULTY, ToDifficultyInt(defaultDifficulty));
			return defaultDifficulty;
		}

		return (DifficultySetting) PlayerPrefs.GetInt(PP_DIFFICULTY);
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
}
