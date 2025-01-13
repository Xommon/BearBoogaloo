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
    public Image trimDisplay;
    public Image backgroundDisplay;
    public Image iconDisplay;
    public TextMeshProUGUI valueDisplay;
    public int colour;
    public int value;
    public Button button;
    public Hand hand;
    public Sprite[] numberIcons;

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
            // Colour and Icon
            Color mainColour = (colour > -1) ? gameManager.colours[colour] : Color.black;
            trimDisplay.color = mainColour;
            backgroundDisplay.color = new Color(mainColour.r + 0.65f, mainColour.g  + 0.65f, mainColour.b + 0.65f);
            iconDisplay.sprite = gameManager.cardIcons[colour];
            
            // Value Display
            valueDisplay.text = (value == 0) ? "?" : value.ToString();
            valueDisplay.color = mainColour;
        }
        catch
        {

        }
    }

    public void PlayCard()
    {
        // Play card and discard it from hand
        gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).value = (value == 0) ? Random.Range(1,10) : value;
        gameManager.players[0].hand.RemoveAt(transform.GetSiblingIndex());

        // Draw a new card
        gameManager.DealCards(1, 0);
        hand.canPlayCard = false;
        StartCoroutine(WaitForTurn());
    }

    IEnumerator WaitForTurn()
    {
        yield return new WaitForSeconds(1.0f);
        gameManager.NextTurn();
    }
}
