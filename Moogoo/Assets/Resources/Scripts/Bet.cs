using UnityEngine;

public class Bet : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    void Update()
    {
        animator = (animator == null) ? GetComponent<Animator>() : animator;
    }
}
