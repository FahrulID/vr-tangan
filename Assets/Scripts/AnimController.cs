using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Animator animator;
    
    public void toggleHand()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            bool isOpen = animator.GetBool("isOpen");
            animator.SetBool("isOpen", !isOpen);
        }
    }

    public void openHand()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            animator.SetBool("isOpen", true);
        }
    }

    public void closeHand()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            animator.SetBool("isOpen", false);
        }
    }
}
