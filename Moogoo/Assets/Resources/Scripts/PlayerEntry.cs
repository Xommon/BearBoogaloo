using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[ExecuteAlways]
public class PlayerEntry : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI nameDisplay;
    public Image iconDisplay;
    public TextMeshProUGUI scoreDisplay;
    public Image background;
    public AI ai;

    public string name;
    public int iconIndex;
    public int score;

    public List<string> hand = new List<string>();

    void Update()
    {
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();

        // Highlight when it's the player's turn
        if (gameManager.turn == transform.GetSiblingIndex())
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.75f);
        }
        else
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.25f);
        }
        
        iconDisplay.sprite = gameManager.icons[iconIndex];
        iconDisplay.color = Color.white;
    }

    public void ChangeIcon()
    {
        if (iconIndex < gameManager.icons.Length - 1)
        {
            iconIndex++;
            return;
        }
        iconIndex = 0;
    }

    public void ActivateAI()
    {
        StartCoroutine(ai.PlayTurn());
    }
}
