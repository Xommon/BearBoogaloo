using UnityEngine;
using TMPro;

[ExecuteAlways]
public class PlayerEntry : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Update()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            return;
        }

        gameObject.name = "PlayerEntry: " + text.text;
    }
}
