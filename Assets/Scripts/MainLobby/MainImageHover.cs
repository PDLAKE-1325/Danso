using UnityEngine;

public class MainImageHover : MonoBehaviour
{

    public void MainImageMouseEnter(Animator animator)
    {
        animator.SetBool("hovering", true);
    }
    public void MainImageMouseExit(Animator animator)
    {
        animator.SetBool("hovering", false);
    }
}
