using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomEvent : MonoBehaviour
{
    public enum Type
    {
        FirstEnter,
        SubsequentEnter,
        AnyEnter,
        FirstExit,
        SubsequentExit,
        AnyExit
    }

    [SerializeField] protected Type type;
    [SerializeField] protected bool hasCondition;
    [SerializeField] private UnityEvent roomEvent;

    protected bool triggerNextRoomChange;
    protected bool everTriggered;
    protected bool everEntered;
    protected Vector3Int room;

    private void Start()
    {
        room = RoomManager.main.GetRoom(transform);
        RoomManager.main.onPlayerMovedRoom += CheckEvent;
    }

    private void CheckEvent(Vector3Int room)
    {
        if (hasCondition && !CheckCondition())
            return;

        //if entering the room
        if (this.room.x == room.x && this.room.y == room.y)
            switch (type)
            {
                case Type.FirstEnter:
                    if (everEntered)
                        break;
                    roomEvent.Invoke();
                    everTriggered = true;
                    everEntered = true;
                    break;
                case Type.SubsequentEnter:
                    if (everEntered && !everTriggered)
                    {
                        everTriggered = true;
                        roomEvent.Invoke();
                        break;
                    }
                    everEntered = true;
                    break;
                case Type.AnyEnter:
                    roomEvent.Invoke();
                    everTriggered = true;
                    everEntered = true;
                    break;
                case Type.FirstExit:
                case Type.SubsequentExit:
                case Type.AnyExit:
                    triggerNextRoomChange = true;
                    break;
                default:
                    break;
            }
        //if exiting the room after entering
        else if (triggerNextRoomChange)
            switch (type)
            {
                case Type.FirstEnter:
                case Type.SubsequentEnter:
                case Type.AnyEnter:
                    break;
                case Type.FirstExit:
                    if (everEntered)
                        break;
                    roomEvent.Invoke();
                    everTriggered = true;
                    everEntered = true;
                    triggerNextRoomChange = false;
                    break;
                case Type.SubsequentExit:
                    if (everEntered && !everTriggered)
                    {
                        everTriggered = true;
                        triggerNextRoomChange = false;
                        roomEvent.Invoke();
                        break;
                    }
                    everEntered = true;
                    break;
                case Type.AnyExit:
                    roomEvent.Invoke();
                    everTriggered = true;
                    everEntered = true;
                    triggerNextRoomChange = false;
                    break;
                default:
                    break;
            }
    }

    protected virtual bool CheckCondition()
    {
        return true;
    }

    public void EnableGroup(Transform group)
    {
        for (int i = 0; i < group.childCount; i++)
        {
            group.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void DisableGroup(Transform group)
    {
        for (int i = 0; i < group.childCount; i++)
        {
            group.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new(1, 1, 0, 0.1f);
        Gizmos.DrawCube(transform.position, Vector3.one * 8f);
    }

}
