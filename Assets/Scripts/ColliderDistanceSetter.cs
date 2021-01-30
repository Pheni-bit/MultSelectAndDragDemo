using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDistanceSetter : MonoBehaviour
{
    Animator animator;
    public bool hover;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetBoolForAnimation()
    {
        if (animator.GetBool("Hover") == hover)
        {
            hover = !hover;
        }
        animator.SetBool("Hover", hover);
    }
    public void SetBoolForAnimation2(bool isHover)
    {
        animator.SetBool("Hover", isHover);
        hover = isHover;
    }
}
