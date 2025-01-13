using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class ScoreEntry : MonoBehaviour
{
    public Image iconDisplay;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI scoreDisplay;
    public int icon;
    public string name;
    public int score;
    private GameManager gameManager;

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        iconDisplay.sprite = gameManager.icons[icon];
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();
    }
}
