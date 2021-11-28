using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DialogueHolder;

public class ConstraintManager : MonoBehaviour
{ 
	public enum GameConstraint
	{
		testConstraint, ch1Finished, ch2Finished, startUseTutorial, startDashTutorial
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
		return satisfiedConstraints.Contains(constraint);
	}

	public void SetSatisfied(GameConstraint constraint)
	{
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

	void OnDestroy()
	{
		Instance = null;
	}
}
