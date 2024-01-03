using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRoomEvent : RoomEvent
{
    

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
