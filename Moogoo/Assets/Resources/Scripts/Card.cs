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

    void Update()
    {
        // References
        gameManager = (gameManager == null) ? FindObjectOfType<GameManager>() : gameManager;
        hand = (hand == null) ? FindObjectOfType<Hand>() : hand;

        try
        {
            // Main colour
            Color mainColour = (colour > -1) ? gameManager.colours[colour] : Color.black;
            
            // Trim
            trimDisplay.color = mainColour;

            // Background
            backgroundDisplay.color = new Color(mainColour.r + 0.55f, mainColour.g  + 0.55f, mainColour.b + 0.55f);

            // SVG Image
            iconDisplay.sprite = gameManager.cardIcons[colour];

            // SVG TESTING
            //Scene scene = SVGParser.ImportSVG(new System.IO.StringReader(svgFile.text)).Scene;
            //Scene scene = VectorUtils.BuildScene(svgImage.sprite, 1.0f);
            //TraverseSceneNode(scene.Root, 0);
            
            // Value display
            valueDisplay.text = (value == 0) ? "?" : value.ToString();
            valueDisplay.color = new Color(mainColour.r + 0.15f, mainColour.g  + 0.15f, mainColour.b + 0.15f);
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
