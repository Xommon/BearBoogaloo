using UnityEngine;
using UnityEngine.UI;

public class MovingChip : MonoBehaviour
{
    public Vector2 start;          // Starting position
    public Vector2 end;            // Ending position
    public RectTransform rt;       // RectTransform to move
    public float moveSpeed = 1.0f; // Speed of movement

    private float t = 0f;          // Normalized time (0 to 1)

    void Update()
    {
        // Increment the t value over time based on moveSpeed
        t += Time.deltaTime * moveSpeed;

        // Clamp t between 0 and 1 to prevent overshooting
        t = Mathf.Clamp01(t);

        // Lerp the position of the RectTransform
        rt.anchoredPosition = Vector2.Lerp(start, end, t);

        // Disable when it reaches its end point
        if (t >= 1f)
        {
            //rt.gameObject.SetActive(false);
        }
    }

    public void StartMoving(Vector2 _start, Vector2 _end)
    {
        start = _start;
        end = _end;
        rt.gameObject.SetActive(true);
        rt.anchoredPosition = start;
    }
}
