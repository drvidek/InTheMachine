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
    public int reward;
    public bool persistent;
    public Quest(QuestID id = QuestID.Null, string name = "QuestName", int reward = 0, bool persistent = false)
    {
        this.id = id;
        this.name = name;
        this.reward = reward;
        this.persistent = persistent;
    }
}

public enum QuestID
{
    Null,
    Clean,
    Pest,
    Fungus,
    Terminal,
    Elevator,
    FireDrone
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] private float typeDelay = 0.1f;
    [SerializeField] private TextMeshProUGUI questDisplay, questTitle;

    private WaitForSeconds typewriterDelay => new WaitForSeconds(typeDelay);

    [SerializeField] public List<Quest> questLog = new();

    private QuestList questList;

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
    }

    private void OnDisable()
    {
        if (main == this)
            _singleton = null;
    }
    #endregion


    private void Start()
    {
        questList = Resources.Load("QuestList") as QuestList;
    }

    public void AddQuest(QuestID questID)
    {
        if (questLog.Count == 0)
        {
            questTitle.text = "JOBS";
        }

        foreach (var currentQuest in questLog)
        {
            if (currentQuest.id == questID)
                return;
        }

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
        var routine = AddQuestToTicker("\n>" + questToAdd.name + $" ({questToAdd.reward}c)");
        StartCoroutine(routine);
    }

    public void CompleteQuest(QuestID quest)
    {
        Quest foundQuest = new();
        foreach (var q in questLog)
        {
            if (q.id == quest)
            {
                foundQuest = q;
                break;
            }
        }

        if (foundQuest.id == QuestID.Null)
        {
            AddQuest(quest);
            foundQuest = questList.quests[(int)quest];
        }

        CashManager.main.IncreaseCashBy(foundQuest.reward);
        if (!foundQuest.persistent)
        {
            questLog.Remove(foundQuest);
            int index = questDisplay.text.IndexOf(foundQuest.name);
            questDisplay.text.Remove(index - 1, foundQuest.name.Length + 2);
        }
    }

    IEnumerator AddQuestToTicker(string stringToAdd)
    {
        string currentString = "";
        string currentQuestLog = questDisplay.text;
        int position = 0;

        while (currentString.Length < stringToAdd.Length)
        {
            yield return typewriterDelay;
            currentString += stringToAdd[position];
            questDisplay.text = currentQuestLog + currentString;
            position++;
        }

    }
}
