using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRoomEvent : RoomEvent
{
    [SerializeField] private Transform webParent;
    [SerializeField] private EventSpider spider;

    protected override bool CheckCondition()
    {
        bool hasQuest = false;
        foreach (var item in QuestManager.main.QuestLog)
        {
            if (item.id == QuestID.Spider)
            {
                hasQuest = true;
                break;
            }
        }

        if (hasQuest)
            return false;

        int count = 0;
        for (int i = 0; i < webParent.childCount; i++)
        {
            if (!webParent.GetChild(i).gameObject.activeInHierarchy)
                count++;
        }

        return count >= webParent.childCount / 2f;
    }

    public void ResetSpider()
    {
        if (spider)
        {
            spider.ResetForEvent();
        }
    }

    public void ResetCobwebs(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject go = parent.GetChild(i).gameObject;
            go.SetActive(true);
            go.GetComponent<Cobweb>().Reset();
        }
    }
}
