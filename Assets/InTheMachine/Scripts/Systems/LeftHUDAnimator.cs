using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHUDAnimator : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        FogOfWar.onMapReveal += () => animator.SetTrigger("MapReveal");
        QuestManager.main.onNewQuest += () => animator.SetTrigger("NewQuest");
    }

}
