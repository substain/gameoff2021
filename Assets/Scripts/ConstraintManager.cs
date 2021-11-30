using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DialogueHolder;

public class ConstraintManager : MonoBehaviour
{
	public const string KEY_PREFIX = "gotKey";
	public enum GameConstraint
	{
		testConstraint, ch1Finished, ch2Finished, 
		startUseTutorial, startDashTutorial, startAvoidTutorial, 
		startBugTutorial, bugUsed,
		gotKey1, gotKey2, gotKey3, gotKey4, gotKey5, gotKey6, gotKey7, gotKey8,




		none
	}

	public enum ChoiceType
	{
		testChoice1, testChoice2, testChoice3,
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

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one ConstraintManager in this scene.");
		}
		Instance = this;
	}

	public bool AllConstraintsSatisfied(List<GameConstraint> constraints)
	{
		foreach(GameConstraint gc in constraints)
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

	void OnDestroy()
	{
		Instance = null;
	}

    public static string ConstraintToRewardString(GameConstraint value)
    {
		switch (value){
			default:
				{
					return "You found a key!";
				}
		}
	}
}
