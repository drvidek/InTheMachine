using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[Serializable]
public struct Quest
{
    public QuestID id;
    public string name;
    public string clearName;
    public string failName;
    public int reward;
    public bool persistent;
    public Quest(QuestID id = QuestID.Null, string name = "QuestName", string clearName = "QuestClear", string failName = "QuestFail", int reward = 0, bool persistent = false)
    {
        this.id = id;
        this.name = name;
        this.clearName = clearName;
        this.failName = failName;
        this.reward = reward;
        this.persistent = persistent;
    }
}

public enum QuestID
{
    Null,
    Clean,
    Door,
    Pest,
    Fungus,
    Terminal,
    Elevator,
    FireDrone,
    Spider
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] private float typeDelay = 0.1f;
    [SerializeField] private TextMeshProUGUI cashTicker, questFullDisplay, jobTitleText, questDescriptionText;

    private WaitForSeconds multiLogDelay => new WaitForSeconds(3f);

    private IEnumerator typewriterRoutine = null;

    private List<IEnumerator> typewriterQueue = new();

    [SerializeField] public List<Quest> questLog = new();

    private QuestList questList;

    public Action onNewQuest, onQuestComplete;

    private string lastQuestName = "";

    #region Singleton + Awake
    private static QuestManager _singleton;
    public static QuestManager main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("QuestManager instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
        questList = Resources.Load("QuestList") as QuestList;

    }

    private void OnDisable()
    {
        if (main == this)
            _singleton = null;
    }
    #endregion

    public void AddQuest(QuestID questID)
    {
        //if we already have the quest
        foreach (var currentQuest in questLog)
        {
            if (currentQuest.id == questID)
                //we shouldn't be adding it
                return;
        }

        Quest questToAdd = GetQuest(questID);

        questLog.Add(questToAdd);
        AddQuestToLog(questToAdd);
        onNewQuest?.Invoke();
        string questDisplayText = $">{questToAdd.name} ({questToAdd.reward}c)";
        AddTextToTickerQueue(questDisplayText, typeDelay);
    }

    private Quest GetQuest(QuestID questID)
    {
        return questList.quests[(int)questID];
    }

    private void AddTextToTickerQueue(string text, float speed)
    {
        if (typewriterRoutine == null)
        {
            typewriterRoutine = AddQuestToTicker(text, speed);
            StartCoroutine(typewriterRoutine);
            return;
        }
        var addRoudtine = AddQuestToTicker(text, speed);
        typewriterQueue.Add(addRoudtine);
    }

    public void CompleteQuest(QuestID questID)
    {

        //if we found no match
        if (!ValidateQuestInLog(questID))
        {
            //add the quest to the log
            AddQuest(questID);
        }
        var questFoundInList = GetQuest(questID);
        CashManager.main.IncreaseCashBy(questFoundInList.reward);
        cashTicker.text += $"\n{questFoundInList.clearName} +{questFoundInList.reward}c";
        if (!questFoundInList.persistent)
        {
            questLog.Remove(questFoundInList);
            string[] quests = questFullDisplay.text.Split("\n");
            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i].Contains(questFoundInList.name))
                {
                    quests[i] = "";
                    break;
                }
            }
            questFullDisplay.text = "";

            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i] == "")
                    continue;

                questFullDisplay.text += quests[i];
                if (i == quests.Length - 1 || quests[i + 1] == "")
                    continue;
                questFullDisplay.text += "\n";

            }

        }

        TryToDisplayJobNotice("JOB DONE");
        
    }

    public void FailQuest(QuestID questID)
    {
        //check if we already have the quest in our log
        if (ValidateQuestInLog(questID))
        {
            Quest quest = GetQuest(questID);
            
            if (quest.failName == "")
                return;

            CashManager.main.ChargeCash(quest.reward);
            cashTicker.text += $"\n{quest.failName} -{quest.reward}c";
            TryToDisplayJobNotice("PENALTY");
        }
    }

    private void TryToDisplayJobNotice(string notice)
    {
        if (typewriterRoutine != null)
            return;

        jobTitleText.text = notice;
        questDescriptionText.text = "";
    }

    private bool ValidateQuestInLog(QuestID quest)
    {
        Quest questFoundInList = new();
        foreach (var q in questLog)
        {
            if (q.id == quest)
            {
                questFoundInList = q;
                break;
            }
        }

        return questFoundInList.id != QuestID.Null;
    }

    private void AddQuestToLog(Quest quest)
    {
        questFullDisplay.text += "\n>" + quest.name + $" (Reward: {quest.reward}c{(quest.persistent ? " each" : "")})";
    }

    IEnumerator AddQuestToTicker(string stringToAdd, float typeTime)
    {
        jobTitleText.enabled = true;

        jobTitleText.text = "NEW JOB";
        questDescriptionText.text = "";
        yield return new WaitForSeconds(1f);
        jobTitleText.enabled = false;

        string currentString = "";
        int position = 0;

        while (currentString.Length < stringToAdd.Length)
        {
            yield return new WaitForSeconds(typeTime);
            currentString += stringToAdd[position];
            questDescriptionText.text = currentString;
            position++;
        }

        //clear the current routine - ready for next prompt
        typewriterRoutine = null;
        jobTitleText.enabled = true;
        jobTitleText.text = "";


        //check the queue for next prompt
        if (typewriterQueue.Count > 0)
        {
            typewriterRoutine = typewriterQueue[0];
            typewriterQueue.RemoveAt(0);
            yield return multiLogDelay;

            StartCoroutine(typewriterRoutine);
        }
    }
}
