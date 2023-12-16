using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public enum Quest
    {
        Debris,
        Pest,
        Fungus,
        Elevator
    }

    public Action<Quest> onQuestComplete;
}
