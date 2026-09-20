using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
            Destroy(gameObject, clip.length);
        }
    }
}