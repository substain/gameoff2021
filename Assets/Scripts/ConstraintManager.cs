using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstraintManager : MonoBehaviour
{
	public const string KEY_PREFIX = "gotKey";
	public enum GameConstraint
	{
		none, finishLevel, 
		gotKey1, gotKey2, gotKey3, gotKey4, gotKey5, gotKey6, gotKey7, gotKey8,
		startUseTutorial, startDashTutorial, startAvoidTutorial, startBugTutorial, bugUsed,
		cantUseStairs, finishLevelAlt, upperStairsReached, upperLevelReached, weaponXLocationRevealed,
		lastCheeseStickUsed, ateCheeseStick1, ateCheeseStick2, ateCheeseStick3, ateCheeseStick4,
		loadNextLevel
	}

	public enum ChoiceType
	{
		doShowTutorial, dontShowTutorial
	}

	public struct Choice
	{
		public readonly ChoiceType choiceType;
		public readonly string choiceText;

		public Choice(ChoiceType choiceType, string choiceText)
		{
			this.choiceType = choiceType;
			this.choiceText = choiceText;
		}
	}

	private HashSet<GameConstraint> satisfiedConstraints = new HashSet<GameConstraint>();


	public static ConstraintManager Instance = null;

	public delegate void ConstraintsChanged();
	public static event ConstraintsChanged OnChangeConstraints;

	private AudioSource playerAudioSource;
	[SerializeField]
	private AudioClip rewardSoundClip;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one ConstraintManager in this scene.");
		}
		Instance = this;
		satisfiedConstraints = new HashSet<GameConstraint>(CheckpointManager.GetSavedSatisfiedConstraints());
	}

	void Start()
	{
		playerAudioSource = FindObjectOfType<PlayerInteraction>().GetAudioSource();
	}

	public bool AllConstraintsSatisfied(List<GameConstraint> constraints)
	{
		foreach (GameConstraint gc in constraints)
		{
			if (!satisfiedConstraints.Contains(gc))
			{
				return false;
			}
		}
		return true;
	}

	public bool AnyConstraintsSatisfied(List<GameConstraint> constraints)
	{
		foreach (GameConstraint gc in constraints)
		{
			if (satisfiedConstraints.Contains(gc))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSatisfied(GameConstraint constraint)
	{
		if(constraint == GameConstraint.none)
		{
			Debug.LogWarning("'none' constraint should not be used.");
		}

		return satisfiedConstraints.Contains(constraint);
	}

	public void PlayRewardSound()
	{
		playerAudioSource.PlayOneShot(rewardSoundClip);
	}

	public void SetSatisfied(GameConstraint constraint)
	{
		if (constraint == GameConstraint.none)
		{
			Debug.LogWarning("'none' constraint should not be used.");
		}

		if (satisfiedConstraints.Contains(constraint))
		{
			return;
		}
		satisfiedConstraints.Add(constraint);
		OnChangeConstraints?.Invoke();
	}

	public void ApplyChoice(Choice choice)
	{
		Debug.Log("Used choice '" + choice.choiceText + "' (" + choice.choiceType.ToString() + "). Choices are not fully implemented yet.");
	}

	public List<GameConstraint> GetKeyConstraints()
	{
		return satisfiedConstraints.Where(sc => sc.ToString().StartsWith("gotKey")).ToList();		
	}
	public HashSet<GameConstraint> GetSatisfiedConstraints()
	{
		return satisfiedConstraints;
	}

	void OnDestroy()
	{
		Instance = null;
	}

    public static string ConstraintToRewardString(GameConstraint value)
    {
		if (value.ToString().StartsWith("gotKey"))
		{
			return "I've got a key!";
		}
		switch (value){
			case GameConstraint.weaponXLocationRevealed:
				{
					return "The weapon is somewhere in the upper level of the ship!";
				}
			default:
				{
					return "";
				}
		}
	}
}
