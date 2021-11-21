using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDDialogueDisplay : MonoBehaviour
{
    private const float FADEOUT_DISPLAY_RANGE = 4;
    private const float MAX_DISPLAY_RANGE = 8;

    [SerializeField]
    private Text contentText;

    [SerializeField]
    private Text subjectText;

    [SerializeField]
    private Image subjectPicture;

    [SerializeField]
    private List<Sprite> allSubjectPictures;

    private CanvasGroup canvasGroup;
    private Transform refPosition;
    private Transform targetPosition = null;
    private bool hasActiveDialogue;
    void Awake()
    {
        hasActiveDialogue = false;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (hasActiveDialogue && refPosition != null && targetPosition != null)
        {
            ComputeAlphaByDistance();
        }
    }

    private void ComputeAlphaByDistance()
    {
        float dist = Vector3.Distance(refPosition.position, targetPosition.position);
        float alphaByDist = 1 - Mathf.Clamp01(dist - FADEOUT_DISPLAY_RANGE / MAX_DISPLAY_RANGE - FADEOUT_DISPLAY_RANGE);
        canvasGroup.alpha = alphaByDist;
    }

    public void ShowDialogue(DialogueLine dialogueLine, Transform playerPosition, Transform targetPosition)
    {
        Person subjectPerson = GetPerson(dialogueLine.subject);
        this.refPosition = playerPosition;
        this.targetPosition = targetPosition;
        hasActiveDialogue = true;
        contentText.text = InputKeyHelper.Instance.ReplacePlaceholdersWithCurrentKeys(dialogueLine.content);
        subjectText.text = dialogueLine.subject;
        subjectText.color = GetPersonColor(subjectPerson);
        //subjectPicture.sprite = GetPersonImage(subjectPerson);
    }

    public void CloseDialogue()
    {
        hasActiveDialogue = false;
        contentText.text = "";
        subjectText.text = "";
        canvasGroup.alpha = 0;
        //subjectPicture.sprite = null;
    }

    private static Person GetPerson(string name)
    {
        if(name.Contains("Piller") || name.Contains("Pillar") || name == "Sting") 
        {
            return Person.Sting;
        }
        if(name.Contains("Iris"))
        {
            return Person.Iris;
        }

        return Person.Unknown;
    }

    private Sprite GetPersonImage(Person person)
    {
        return allSubjectPictures[(int)person];
    }

    private Color GetPersonColor(Person person)
    {
        switch(person){
            case Person.Sting:
                {
                    return new Color(0.3607843f, 0.9019608f, 0.7372549f);
                }
            case Person.Iris:
                {
                    return new Color(0.9019608f, 0.3607844f, 0.8564534f);
                }

            case Person.Unknown:
            default:
                {
                    return new Color(0.81f, 0.81f, 0.81f);
                }

        }
    }
}
