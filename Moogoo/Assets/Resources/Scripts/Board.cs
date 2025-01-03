using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class Board : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Image image;
    public Button button;
    public GameManager gameManager;
    public int value;
    public Image[] betMarkers;
    public List<int> bets = new List<int>();
    public int boardNumber;
    public GridLayoutGroup betsParent;

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (betsParent == null)
        {
            betsParent = GetComponentInChildren<GridLayoutGroup>();
        }

        // Update UI
        if (value > 0)
        {
            valueText.text = value.ToString();
        }
        else
        {
            valueText.text = "";
        }

        // Update button
        button.interactable = (gameManager.turn == 0 && gameManager.bettingTime && bets.Count < gameManager.maxBet);

        image.color = gameManager.colours[transform.GetSiblingIndex()];

        // Update bets UI
        for (int i = 0; i < betMarkers.Length; i++)
        {
            if (i > gameManager.maxBet - 1)
            {
                betMarkers[i].gameObject.SetActive(false);
            }
            else if (bets.Count <= i)
            {
                betMarkers[i].gameObject.SetActive(true);
                betMarkers[i].color = new Color(0, 0, 0, 0);
            }
            else
            {
                betMarkers[i].gameObject.SetActive(true);
                betMarkers[i].color = new Color(1, 1, 1, 1);
                betMarkers[i].sprite = gameManager.icons[gameManager.players[bets[i]].iconIndex];
            }
        }

        // Update bets layout
        switch (gameManager.maxBet)
        {
            case (6):
                betsParent.childAlignment = TextAnchor.UpperLeft;
                betsParent.constraintCount = 3;
                break;
            case (4):
                betsParent.childAlignment = TextAnchor.UpperCenter;
                betsParent.constraintCount = 2;
                break;
            case (2):
            betsParent.childAlignment = TextAnchor.MiddleCenter;
                betsParent.constraintCount = 2;
                break;
        }
    }

    public void PlaceBet()
    {
        gameManager.boards.FirstOrDefault(board => board.boardNumber == transform.GetSiblingIndex()).bets.Add(0);
        gameManager.bettingTime = false;
    }
}
