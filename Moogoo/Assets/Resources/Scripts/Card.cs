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
        backgroundDisplay.color = new Color(mainColour.r + 0.35f, mainColour.g  + 0.35f, mainColour.b + 0.35f);

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
            valueDisplay.text = $"<size=16>{Language.language[16, gameManager.languageIndex]}";
            valueDisplay.alignment = TextAlignmentOptions.Center;
        }
        else if (colour == -2)
        {
            // Fill empty boards card
            valueDisplay.text = $"<size=16>{Language.language[17, gameManager.languageIndex]}{value}";
            valueDisplay.alignment = TextAlignmentOptions.Center;
        }
        else if (value == 0)
        {
            valueDisplay.text = Language.language[15, gameManager.languageIndex];
            valueDisplay.alignment = TextAlignmentOptions.TopLeft;
        }
        else if (value > 0)
        {
            valueDisplay.text = Language.language[value, gameManager.languageIndex];
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

    public void PlayCurrentCard()
    {
        PlayCard(colour, value, transform.GetSiblingIndex());
    }

    public void PlayCard(int _index, int _value, int _cardIndex)
    {
        hand.canPlayCard = false;

        // Play card
        if (_index > - 1 && _value > -1)
        {
            // Normal cards
            if (!gameManager.boards.FirstOrDefault(board => board.boardNumber == _index).isLocked)
            {
                int newValue = _value;
                while (newValue == _value)
                {
                    newValue = Random.Range(1, 10);
                }
                gameManager.boards.FirstOrDefault(board => board.boardNumber == _index).value = (_value == 0) ? newValue : _value;

                // Play sound
                AudioManager.Play("card1", "card2");
            }
            else
            {
                // Play sound
                AudioManager.Play("discard");
            }
        }
        else if (_index > - 1 && _value == -1)
        {
            // Lock board cards
            gameManager.boards.FirstOrDefault(board => board.boardNumber == _index).isLocked = true;
            gameManager.boards.FirstOrDefault(board => board.boardNumber == _index).turnToUnlock = gameManager.totalTurns + gameManager.activePlayers;

        }
        else if (_index == -1) // Special cards
        {
            // Randomise current values
            bool nothingChanged = true;
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value > 0 && !board.isLocked)
                {
                    // Make sure the new value is different from the old value
                    nothingChanged = false;
                    int oldValue = board.value;

                    while (board.value == oldValue)
                    {
                        board.value = Random.Range(1, 10);
                    }
                }
            }

            if (nothingChanged)
            {
                AudioManager.Play("discard");
            }
            else
            {
                AudioManager.Play("card1", "card2");
            }
        }
        else if (_index == -2)
        {
            // Fill empty boards
            bool nothingChanged = true;
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value == 0 && !board.isLocked)
                {
                    nothingChanged = false;
                    board.value = _value;
                }
            }

            if (nothingChanged)
            {
                AudioManager.Play("discard");
            }
            else
            {
                AudioManager.Play("card1", "card2");
            }
        }

        // Discard card from hand
        gameManager.players[gameManager.turn].hand.RemoveAt(_cardIndex);
        gameManager.discardPile.Add($"{_index}:{_value}");
        StartCoroutine(WaitForTurn());
    }

    IEnumerator WaitForTurn()
    {
        // Draw a new card
        yield return new WaitForSeconds(0.5f);
        gameManager.DealCards(1, gameManager.turn);
        hand.canPlayCard = false;

        // Go to next turn
        yield return new WaitForSeconds(0.5f);
        gameManager.NextTurn();
    }
}
