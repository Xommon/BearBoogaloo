using UnityEngine;

public class Bet : MonoBehaviour
{
    private Animator animator;

    void Update()
    {
        animator = (animator == null) ? GetComponent<Animator>() : animator;
    }
}
