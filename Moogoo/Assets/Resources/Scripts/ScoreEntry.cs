using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class ScoreEntry : MonoBehaviour
{
    public Image iconDisplay;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI scoreDisplay;
    public Sprite icon;
    public string name;
    public int score;

    void Update()
    {
        iconDisplay.sprite = icon;
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();
    }
}
