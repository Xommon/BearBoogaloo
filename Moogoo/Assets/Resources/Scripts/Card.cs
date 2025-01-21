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
    public TextMeshProUGUI valueDisplay;
    public int colour;
    public int value;
    public Button button;
    public Hand hand;
    public Sprite[] numberIcons;
    public Transform[] allImages;

    void Update()
    {
        // References
        gameManager = (gameManager == null) ? FindObjectOfType<GameManager>() : gameManager;
        hand = (hand == null) ? FindObjectOfType<Hand>() : hand;

        // Main colour
        Color mainColour = (colour > -1) ? gameManager.colours[colour] : Color.black;
        
        // Trim
        trimDisplay.color = mainColour;

        // Background
        backgroundDisplay.color = new Color(mainColour.r + 0.55f, mainColour.g  + 0.55f, mainColour.b + 0.55f);

        // SVG Image
        if (colour > - 1)
        {
            iconDisplay.sprite = gameManager.cardIcons[colour];
        }
        
        iconDisplay.gameObject.SetActive(colour > -1);

        // SVG TESTING
        //Scene scene = SVGParser.ImportSVG(new System.IO.StringReader(svgFile.text)).Scene;
        //Scene scene = VectorUtils.BuildScene(svgImage.sprite, 1.0f);
        //TraverseSceneNode(scene.Root, 0);
        
        // Value display
        valueDisplay.color = new Color(mainColour.r + 0.15f, mainColour.g  + 0.15f, mainColour.b + 0.15f);
        if (colour == -1)
        {
            // Randomise card
            valueDisplay.text = "<size=16>Randomise Existing Values";
            valueDisplay.alignment = TextAlignmentOptions.Center;
        }
        else if (colour == -2)
        {
            // Fill empty boards card
            valueDisplay.text = $"<size=16>Set Empty Boards' Values to {value}";
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
    }

    public void PlayCard()
    {
        // Play sound
        AudioManager.Play("card1", "card2");

        // Play card
        if (colour > - 1)
        {
            // Normal cards
            gameManager.boards.FirstOrDefault(board => board.boardNumber == colour).value = (value == 0) ? Random.Range(1,10) : value;
        }
        else if (colour == -1) // Special cards
        {
            // Randomise current values
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value > 0)
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
