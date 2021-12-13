using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class CheckpointManager
{
    private static Vector3? currentPosition = null;
    private static GameScene currentScene;
    private static int currentCheckPointId = 0;
    private static HashSet<ConstraintManager.GameConstraint> satisfiedConstraints = new HashSet<ConstraintManager.GameConstraint>();
    private static HashSet<DialogueHolder.DialogueKey> finishedDialogues = new HashSet<DialogueHolder.DialogueKey>();
    private static HashSet<int> obtainedKeyIds = new HashSet<int>();

    public static void SetCurrentScene(GameScene newScene)
    {
        if(currentScene != newScene)
        {
            Reset();
        }
        currentScene = newScene;
    }

    public static void Reset()
    {
        currentPosition = null;
        currentCheckPointId = 0;
        satisfiedConstraints = new HashSet<ConstraintManager.GameConstraint>();
        finishedDialogues = new HashSet<DialogueHolder.DialogueKey>();
        obtainedKeyIds = new HashSet<int>();
    }

    public static HashSet<ConstraintManager.GameConstraint> GetSavedSatisfiedConstraints()
    {
        return satisfiedConstraints;
    }

    public static HashSet<DialogueHolder.DialogueKey> GetFinishedDialogues()
    {
        return finishedDialogues;
    }

    public static void SetCheckpointReached(int checkPointId, Vector3 checkpointPos, HashSet<int> obtainedKeys)
    {
        if(checkPointId <= currentCheckPointId)
        {
            return;
        }
        currentCheckPointId = checkPointId;
        finishedDialogues = new HashSet<DialogueHolder.DialogueKey>(DialogueManager.Instance.GetFinishedDialogues());
        satisfiedConstraints = new HashSet<ConstraintManager.GameConstraint>(ConstraintManager.Instance.GetSatisfiedConstraints());
        currentPosition = checkpointPos;
        obtainedKeyIds = new HashSet<int>(obtainedKeys);
    }

    public static HashSet<int> GetObtainedKeyIds()
    {
        return obtainedKeyIds;
    }

    public static Vector3? GetCurrentPosition()
    {
        return currentPosition;
    }
}