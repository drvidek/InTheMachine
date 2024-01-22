using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRoomEvent : RoomEvent
{
    [SerializeField] private Transform webParent;
    
    protected override bool CheckCondition()
    {
        int count = 0;
        for (int i = 0; i < webParent.childCount; i++)
        {
            if (!webParent.GetChild(i).gameObject.activeInHierarchy)
                count++;
        }

        return count >= webParent.childCount / 2f;
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
