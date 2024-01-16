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
    public int reward;
    public bool persistent;
    public Quest(QuestID id = QuestID.Null, string name = "QuestName", string clearName = "QuestClear", int reward = 0, bool persistent = false)
    {
        this.id = id;
        this.name = name;
        this.clearName = clearName;
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
    [SerializeField] private TextMeshProUGUI questDisplay, questFullDisplay, questTitle;

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

        questTitle.text = "NEW JOB";

        Quest questToAdd = new();

        foreach (var questInList in questList.quests)
        {
            if (questInList.id == questID)
            {
                questToAdd = questInList;
            }
        }
        if (questToAdd.id == QuestID.Null)
        {

        }
        questLog.Add(questToAdd);
        AddQuestToLog(questToAdd);
        onNewQuest?.Invoke();
        string questDisplayText = $">{questToAdd.name} ({questToAdd.reward}c)";


        //AddTextToTickerQueue(questDisplayText, typeDelay);
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

    public void CompleteQuest(QuestID completedQuestID)
    {
        //check if we already have the quest in our log
        Quest questFoundInList = new();
        foreach (var q in questLog)
        {
            if (q.id == completedQuestID)
            {
                questFoundInList = q;
                break;
            }
        }

        //bool firstAdd = false;
        //if we found no match
        if (questFoundInList.id == QuestID.Null)
        {
            //add the quest to the log
            AddQuest(completedQuestID);
            questFoundInList = questList.quests[(int)completedQuestID];
           // firstAdd = true;
        }
        CashManager.main.IncreaseCashBy(questFoundInList.reward);

        //if (firstAdd)
        //    return;

        questTitle.text = "JOB DONE";

        questDisplay.text += $"\n{questFoundInList.clearName} +{questFoundInList.reward}c";
        //AddTextToTickerQueue($"\n{questFoundInList.name} +{questFoundInList.reward}c", typeDelay);

        //remove a non-persistent quest
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
    }

    private void AddQuestToLog(Quest quest)
    {
        questFullDisplay.text += "\n>" + quest.name + $" (Reward: {quest.reward}c{(quest.persistent ? " each" : "")})";
    }

    IEnumerator AddQuestToTicker(string stringToAdd, float typeTime)
    {
        //bool repeat = false;
        //if (questDisplay.text == stringToAdd)
        //{
        //    repeat = true;
        //}
        //
        //if (!repeat)
        //{
        //    string currentString = "";
        //    int position = 0;
        //
        //    while (currentString.Length < stringToAdd.Length)
        //    {
        //        yield return new WaitForSeconds(typeTime);
        //        currentString += stringToAdd[position];
        //        questDisplay.text = currentString;
        //        position++;
        //    }
        //}

        string currentString = "";
        string currentQuestLog = questDisplay.text;
        int position = 0;

        while (currentString.Length < stringToAdd.Length)
        {
            yield return new WaitForSeconds(typeTime);
            currentString += stringToAdd[position];
            questDisplay.text = currentQuestLog + currentString;
            position++;
        }

        //clear the current routine - ready for next prompt
        typewriterRoutine = null;

        //check the queue for next prompt
        if (typewriterQueue.Count > 0)
        {
            typewriterRoutine = typewriterQueue[0];
            typewriterQueue.RemoveAt(0);
            yield return null;

            StartCoroutine(typewriterRoutine);
        }
    }
}
