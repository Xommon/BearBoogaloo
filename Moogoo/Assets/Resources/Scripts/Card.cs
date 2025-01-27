using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using System.IO;

[ExecuteAlways]
public class Card : MonoBehaviour
{
    private GameManager gameManager;
    public Image trimDisplay;
    public Image backgroundDisplay;
    public SVGImage iconDisplay;
    public SVGImage shadowDisplay;
    public TextMeshProUGUI valueDisplay;
    public int colour;
    public int value;
    public Button button;
    public Hand hand;
    public Sprite[] numberIcons;
    public Transform[] allImages;
    public GameObject lockIcon;
    private CanvasGroup cg;

    void Update()
    {
        // References
        gameManager = (gameManager == null) ? FindObjectOfType<GameManager>() : gameManager;
        hand = (hand == null) ? FindObjectOfType<Hand>() : hand;
        cg = (cg == null) ? GetComponent<CanvasGroup>() : cg;

        // Main colour
        Color mainColour = (colour > -1) ? gameManager.colours[colour] : Color.black;
        
        // Trim
        trimDisplay.color = Color.white;

        // Background
        backgroundDisplay.color = new Color(mainColour.r + 0.15f, mainColour.g  + 0.15f, mainColour.b + 0.15f);

        // SVG Image
        iconDisplay.sprite = (colour > -1) ? gameManager.cardIcons[colour] : iconDisplay.sprite;
        iconDisplay.gameObject.SetActive(colour > -1);
        shadowDisplay.sprite = iconDisplay.sprite;
        shadowDisplay.gameObject.SetActive(iconDisplay.gameObject.activeInHierarchy);

        // Display lock or number
        lockIcon.SetActive(colour > -1 && value == -1);
        valueDisplay.gameObject.SetActive(!lockIcon.activeInHierarchy);
        
        // Value display
        valueDisplay.color = Color.white; //new Color(mainColour.r + 0.15f, mainColour.g  + 0.15f, mainColour.b + 0.15f);
        if (colour == -1)
        {
            // Randomise card
            valueDisplay.text = "<size=16>Randomise Existing Values";
            valueDisplay.alignment = TextAlignmentOptions.Center;
        }
        else if (colour == -2)
        {
            // Fill empty boards card
            valueDisplay.text = $"<size=16>Set Empty Board Values to {value}";
            valueDisplay.alignment = TextAlignmentOptions.Center;
        }
        else if (value == 0)
        {
            valueDisplay.text = "?";
            valueDisplay.alignment = TextAlignmentOptions.TopLeft;
        }
        else if (value > 0)
        {
            valueDisplay.text = value.ToString();
            valueDisplay.alignment = TextAlignmentOptions.TopLeft;
        }

        // Get all images
        if (allImages.Length < 1)
        {
            allImages = GetComponentsInChildren<Transform>();
        }

        // Make the card transparent if its not playable yet
        if (Application.isPlaying)
        {
            cg.alpha = (hand.canPlayCard && gameManager.turn == 0 || gameManager.turn == -1) ? 1 : 0.5f;
        }
    }

    public void PlayCard()
    {
        

        // Play card
        if (colour > - 1 && value > -1)
        {
            // Normal cards
            if (!gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).isLocked)
            {
                gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).value = (value == 0) ? Random.Range(1,10) : value;

                // Play sound
                AudioManager.Play("card1", "card2");
            }
            else
            {
                // Play sound
                AudioManager.Play("discard");
            }
        }
        else if (colour > - 1 && value == -1)
        {
            // Lock board cards
            gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).isLocked = true;
            gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).turnToUnlock = gameManager.totalTurns + gameManager.activePlayers;

        }
        else if (colour == -1) // Special cards
        {
            // Randomise current values
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value > 0 && !board.isLocked)
                {
                    // Make sure the new value is different from the old value
                    int oldValue = board.value;

                    while (board.value == oldValue)
                    {
                        board.value = Random.Range(1, 10);
                    }
                }
            }
        }
        else if (colour == -2)
        {
            // Fill empty boards
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value == 0)
                {
                    board.value = value;
                }
            }
        }

        // Discard it from hand
        gameManager.players[0].hand.RemoveAt(transform.GetSiblingIndex());
        gameManager.discardPile.Add($"{colour}:{value}");

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
