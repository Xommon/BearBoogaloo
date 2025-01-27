using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;

[ExecuteAlways]
public class Board : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Image imageMask;
    public Button button;
    public GameManager gameManager;
    public int value;
    public Image[] betMarkers;
    public Image trim;
    public Unity.VectorGraphics.SVGImage iconImage;
    public List<int> bets = new List<int>();
    public int boardNumber;
    public GridLayoutGroup betsParent;
    public Image dangerImage;
    private Animator animator;
    private int oldValue;
    public RawImage cross;
    public bool isLocked;
    public GameObject lockObject;
    public int turnToUnlock;

    void Update()
    {
        // References
        gameManager = (gameManager == null) ? FindObjectOfType<GameManager>() : gameManager;
        betsParent = (betsParent == null) ? GetComponentInChildren<GridLayoutGroup>() : betsParent;
        animator = (animator == null) ? GetComponentInChildren<Animator>() : animator;

        // Update name
        gameObject.name = $"Board: {boardNumber}";

        // Change value of board
        valueText.text = (value > 0) ? value.ToString() : "";
        valueText.color = gameManager.colours[boardNumber];
        if (value != oldValue)
        {
            animator.Play("boards_number_jump", -1, 0f);
            oldValue = value;
        }

        // Update button
        button.interactable = (gameManager.turn == 0 && gameManager.bettingTime && bets.Count < gameManager.maxBet && !isLocked);

        // Update icon
        iconImage.sprite = gameManager.cardIcons[transform.GetSiblingIndex()];
        imageMask.color = new Color(gameManager.colours[transform.GetSiblingIndex()].r + 0.7f, gameManager.colours[transform.GetSiblingIndex()].g + 0.7f, gameManager.colours[transform.GetSiblingIndex()].b + 0.7f);

        // Update trim colour
        trim.color = Color.white;

        // Cross colour
        cross.color = new Color(gameManager.colours[boardNumber].r + 0.25f, gameManager.colours[boardNumber].g + 0.25f, gameManager.colours[boardNumber].b + 0.25f);

        // Update danger display
        int lowestValue = 9;
        int amountOfBoardsWithValue = 0;

        foreach (Board board in gameManager.boards)
        {
            if (board.value > 0)
                amountOfBoardsWithValue++;

            if (board.value < lowestValue && board.value > 0)
            {
                lowestValue = board.value;
            }
        }
        dangerImage.gameObject.SetActive(value == lowestValue && amountOfBoardsWithValue > 1);

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

        // Show lock when locked
        lockObject.SetActive(isLocked);

        // Unlock
        if (gameManager.totalTurns == turnToUnlock)
        {
            turnToUnlock = -1;
            isLocked = false;
        }
    }

    public void PlaceBet()
    {
        // Play sound
        AudioManager.Play("chip0", "chip1", "chip2");

        // Increase score
        if (!gameManager.betScore)
        {
            gameManager.players[0].score++;
        }

        // Place the bet
        Board selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == transform.GetSiblingIndex());
        selectedBoard.bets.Add(0);
        selectedBoard.betMarkers[selectedBoard.bets.Count - 1].GetComponent<Animator>().enabled = true;
        gameManager.bettingTime = false;
    }

    public void DeleteBoard()
    {
        animator.Play("boards_delete", -1, 0f);
        AudioManager.Play("lose");
    }
}
