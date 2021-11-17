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
		testConstraint, ch1Finished, ch2Finished
	}

	private List<GameConstraint> satisfiedConstraints = new List<GameConstraint>();

	public static ConstraintManager Instance = null;

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

	public void SetSatisfied(GameConstraint constraint)
	{
		satisfiedConstraints.Add(constraint);
	}
}
