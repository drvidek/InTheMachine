using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleQuestComplete : MonoBehaviour, IActivate
{
    [SerializeField] private QuestID quest;

    public void ToggleActive(bool active)
    {
        if (!active)
            return;
        QuestManager.main.CompleteQuest(quest);
    }

    public void ToggleActiveAndLock(bool active)
    {
        ToggleActive(active);
    }

}
