using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AddQuestTrigger : MonoBehaviour
{
    [SerializeField] private QuestID quest;
    [SerializeField] private bool destroyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            QuestManager.main.AddQuest(quest);
            if (destroyObject)
                Destroy(gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new(0, 1, 0, 0.1f);
        foreach (var collider in GetComponentsInChildren<BoxCollider2D>())
        {
            Vector3 offset = new(transform.localScale.x * collider.offset.x, transform.localScale.y * collider.offset.y);
            Gizmos.DrawCube(transform.position + offset, transform.localScale * 0.95f) ;
        }
    }
}
