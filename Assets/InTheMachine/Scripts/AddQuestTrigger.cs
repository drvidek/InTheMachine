using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AddQuestTrigger : MonoBehaviour
{
    [SerializeField] private Quest quest;
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
}
