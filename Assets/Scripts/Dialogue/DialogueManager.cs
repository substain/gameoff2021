using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ConstraintManager;
using static DialogueHolder;

/// <summary>
/// Loads dialogues from the specified file key and provides them via GetDialogueTemplate(key)
/// </summary>
public class DialogueManager : MonoBehaviour 
{
	public enum FileKey
	{
		testDialogues, tutorialDialogues, gameDialogues
	}

	[SerializeField]
	private FileKey fileToLoad;

	[SerializeField]
	private bool useDebugOutput;
	public const string DIALOGUE_INDICATOR = "#";
	private const string COMMENT_INDICATOR = "//";
	private const string CONSTRAINT_INDICATOR = "?";
	private const string REPEATABLE_INDICATOR = "*";
	private const string BLOCKING_INDICATOR = "!";
	private const string CHOICE_INDICATOR = "+"; //choices are always blocking
	private const string NAME_SEPARATOR = ":";
	public const string SUBJECT_PLACEHOLDER = "%subject%";

	private Dictionary<DialogueKey, DialogueTemplate> dialogueTemps = new Dictionary<DialogueKey, DialogueTemplate>();

	public static DialogueManager Instance = null;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("There is more than one DialogueManager in this scene.");
		}
		Instance = this;
		LoadDialogues();
	}

	private void LoadDialogues()
	{
		DebugPrintln("Start loading dialogues from " + fileToLoad.ToString() + " ...");
		TextAsset dialogTextAsset = (TextAsset)Resources.Load(fileToLoad.ToString());
		if (dialogTextAsset == null)
		{
			Debug.LogWarning("Could not load dialogs file. File was not found");
			return;
		}
		if (dialogTextAsset.text.Length == 0)
		{
			Debug.LogWarning("dialogues file was empty");
			return;
		}
		
		string[] lines = dialogTextAsset.text.Split('\n');
		//do the actual processing here
		ProcessLines(lines);

		DebugPrintln("Finished loading dialogues. Added " + dialogueTemps.Count + " dialogues.");
	}

	private void ProcessLines(string[] lines) {
		DialogueKey? key = null;
		DialogueTemplate currentDialogue = null;

		foreach (string line in lines)
		{
			if (line.Trim().Length == 0 || line.StartsWith(COMMENT_INDICATOR))
			{
				continue;
			}
			if (line.StartsWith(DIALOGUE_INDICATOR))
			{
				key = ToDialogueKey(line.Substring(2).Trim());
				if (key.HasValue)
				{
					if (dialogueTemps.ContainsKey(key.Value))
					{
						Debug.LogWarning("The key " + key.Value + " is already used and will be overwritten.");
					}
					dialogueTemps[key.Value] = new DialogueTemplate();
					dialogueTemps[key.Value].SetKey(key.Value);
					currentDialogue = dialogueTemps[key.Value];
				}
				continue;
			}
			if (line.StartsWith(CONSTRAINT_INDICATOR))
			{
				GameConstraint? constraint = ToGameConstraint(line.Substring(2).Trim());
				if (constraint.HasValue)
				{
					currentDialogue.AddConstraint(constraint.Value);
				}
				else
				{
					Debug.LogWarning("could not find constraint " + line.Substring(2).Trim());
				}
				continue;
			}
			if (line.StartsWith(BLOCKING_INDICATOR))
			{
				currentDialogue.SetIsBlocking(true);
				continue;
			}
			if (line.StartsWith(REPEATABLE_INDICATOR))
			{
				currentDialogue.SetIsOneShot(false);
				continue;
			}
			if (line.StartsWith(CHOICE_INDICATOR))
			{
				string linecontent = line.Substring(2);
				int spaceIndex = linecontent.IndexOf(" ");
				if(spaceIndex < 0)
				{
					Debug.LogWarning("could not find a second entry for the key/text pair required for a choice. " +
						"This should be provided in the format [" + CHOICE_INDICATOR + " choiceKey choiceText]. Ignoring this choice.");
					continue;
				}
				ChoiceType? choiceType = ToChoiceType(linecontent.Substring(0, spaceIndex));
				if(choiceType.HasValue)
				{

					string choiceText = linecontent.Substring(spaceIndex);
					Choice choice = new Choice(choiceType.Value, choiceText);
					currentDialogue.AddChoiceToLastLine(choice);
				}
				continue;
			}
			if (key.HasValue && dialogueTemps.ContainsKey(key.Value))
			{
				AddLineTo(line, currentDialogue);
				continue;
			}
		}
	}

	private static void AddLineTo(string line, DialogueTemplate dialogue)
	{
		int separationIndex = line.IndexOf(NAME_SEPARATOR);
		if(separationIndex < 0)
		{
			dialogue.AddLine(line);
			return;
		}
		
		string subject = line.Substring(0, separationIndex).Trim();
		string content = line.Substring(separationIndex + 1, line.Length - separationIndex - 1).Trim();
		if (subject.Length == 0 || content.Length == 0)
		{
			dialogue.AddLine(line);
			return;
		}
		dialogue.AddLine(subject, content);
	}

	public DialogueTemplate GetDialogueTemplate(DialogueKey dialogueKey)
	{
		if (!dialogueTemps.ContainsKey(dialogueKey))
		{
			Debug.LogWarning("Requested dialogue with key " + dialogueKey + " does not exist");
			return null;
		}
		return dialogueTemps[dialogueKey];
	}

	public static DialogueKey? ToDialogueKey(string input)
	{
		foreach(DialogueKey dialogueKey in Enum.GetValues(typeof(DialogueKey)))
		{
			if (dialogueKey.ToString().ToLower().Equals(input.ToLower()))
			{
				return dialogueKey;
			}
		}
		Debug.LogWarning("Warning, dialogue key " + input + " unknown.");

		return null;
	}

	public static GameConstraint? ToGameConstraint(string input)
	{
		foreach (GameConstraint gameConstraint in Enum.GetValues(typeof(GameConstraint)))
		{
			if (gameConstraint.ToString().ToLower().Equals(input.ToLower()))
			{
				return gameConstraint;
			}
		}
		Debug.LogWarning("Warning, constraint " + input + " unknown.");

		return null;
	}

	public static ChoiceType? ToChoiceType(string input)
	{
		foreach (ChoiceType choice in Enum.GetValues(typeof(ChoiceType)))
		{
			if (choice.ToString().ToLower().Equals(input.ToLower()))
			{
				return choice;
			}
		}
		Debug.LogWarning("Warning, choice " + input + " unknown.");

		return null;
	}

	private void DebugPrintln(string info)
	{
		if (!useDebugOutput)
		{
			return;
		}
		Debug.Log(info);
	}

	void OnDestroy()
	{
		Instance = null;
	}
}