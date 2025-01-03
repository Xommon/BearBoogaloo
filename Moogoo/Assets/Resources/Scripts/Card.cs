using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class Card : MonoBehaviour
{
    private GameManager gameManager;
    public Image colourDisplay;
    public TextMeshProUGUI valueDisplay;
    public int colour;
    public int value;
    public Button button;
    public Hand hand;

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (hand == null)
        {
            hand = FindObjectOfType<Hand>();
        }

        try
        {
            colourDisplay.color = gameManager.colours[colour];
            valueDisplay.text = value.ToString();
        }
        catch
        {

        }
    }

    public void PlayCard()
    {
        // Play card and discard it from hand
        gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).value = value;
        gameManager.players[0].hand.RemoveAt(transform.GetSiblingIndex());

        // Draw a new card
        gameManager.DealCards(1, 0);
        hand.canPlayCard = false;
        StartCoroutine(WaitForTurn());
    }

    IEnumerator WaitForTurn()
    {
        yield return new WaitForSeconds(1.5f);
        gameManager.NextTurn();
    }
}
