using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AI : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerEntry playerData;
    private int playerIndex;
    public MovingChip movingChip;

    void Start()
    {
        playerIndex = transform.GetSiblingIndex();
    }

    void Update()
    {
        if (movingChip == null)
        {
            movingChip = FindObjectOfType<MovingChip>();
        }
    }

    public IEnumerator PlayTurn()
    {
        // Analyse cards in hand
        int[] strongestBoard = new int[] { 0, 0 };
        int[] backupBoard = new int[] { 0, 0 };
        int[] lowestBoard = new int[] { 0, int.MaxValue };
        int strongestCardIndex = -1;
        int backupCardIndex = -1;
        int lowestCardIndex = -1;

        for (int i = 0; i < playerData.hand.Count; i++)
        {
            string[] cardValues = playerData.hand[i].Split(":");
            int boardNumber = int.Parse(cardValues[0]);
            int cardValue = int.Parse(cardValues[1]);

            int[] currentBoard = new int[] { boardNumber, cardValue };

            // Evaluate the strongest and backup cards
            if (currentBoard[1] > strongestBoard[1])
            {
                backupBoard = strongestBoard;
                strongestBoard = currentBoard;

                backupCardIndex = strongestCardIndex;
                strongestCardIndex = i;
            }
            else if (currentBoard[1] > backupBoard[1])
            {
                backupBoard = currentBoard;
                backupCardIndex = i;
            }

            // Identify the lowest value card
            if (currentBoard[1] < lowestBoard[1])
            {
                lowestBoard = currentBoard;
                lowestCardIndex = i;
            }
        }

        // Evaluate board state
        Board targetBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == strongestBoard[0]);
        Board backupTargetBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == backupBoard[0]);
        Board lowestTargetBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == lowestBoard[0]);

        int initialBetCount = targetBoard != null ? targetBoard.bets.Count : 0;
        int betLossCount = 0;
        bool boardDeleted = false;

        // Skip betting if all boards are full
        if (!gameManager.boards.All(board => board.bets.Count >= gameManager.maxBet))
        {
            // Choose where to place the bet strategically
            yield return new WaitForSeconds(1.0f);
            int betIndex = strongestBoard[0];

            if (targetBoard != null && backupTargetBoard != null)
            {
                if (targetBoard.value > backupTargetBoard.value && targetBoard.value - backupTargetBoard.value <= 2)
                {
                    betIndex = backupBoard[0];
                }
            }

            // Place low-value card on board with fewer bets, respecting maxBet limit
            if (lowestTargetBoard != null && lowestTargetBoard.bets.Count < gameManager.maxBet)
            {
                betIndex = lowestBoard[0];
            }

            Board betBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == betIndex);
            if (betBoard != null && betBoard.bets.Count < gameManager.maxBet)
            {
                betBoard.bets.Add(playerIndex);
            }
            else
            {
                Board fallbackBoard = gameManager.boards.FirstOrDefault(board => board.bets.Count < gameManager.maxBet);
                if (fallbackBoard != null)
                {
                    fallbackBoard.bets.Add(playerIndex);
                }
            }

            // Show the moving chip
            movingChip.StartMoving(new Vector2(playerData.iconDisplay.GetComponent<RectTransform>().position.x, playerData.iconDisplay.GetComponent<RectTransform>().position.y), new Vector2(200, 0));
        }

        // Decide which card to play based on board conditions
        yield return new WaitForSeconds(1.0f);
        if (backupTargetBoard != null && targetBoard != null)
        {
            if (targetBoard.value - backupTargetBoard.value <= 3)
            {
                gameManager.boards.FirstOrDefault(board => board.boardNumber == backupBoard[0]).value = backupBoard[1];
                playerData.hand.RemoveAt(backupCardIndex);
            }
            else
            {
                gameManager.boards.FirstOrDefault(board => board.boardNumber == strongestBoard[0]).value = strongestBoard[1];
                if (strongestBoard[1] > 7 && UnityEngine.Random.Range(0,3) == 0)
                {
                    playerData.React(0, UnityEngine.Random.Range(0.0f, 1.0f));
                }
                playerData.hand.RemoveAt(strongestCardIndex);
            }
        }
        else
        {
            gameManager.boards.FirstOrDefault(board => board.boardNumber == strongestBoard[0]).value = strongestBoard[1];
            playerData.hand.RemoveAt(strongestCardIndex);
        }

        // Check if any boards were deleted and trigger appropriate reaction
        foreach (Board board in gameManager.boards.ToList())
        {
            if (!board.gameObject.activeSelf)
            {
                boardDeleted = true;
                if (board.bets.Contains(playerIndex))
                {
                    betLossCount++;
                }
            }
        }

        // Draw a new card
        gameManager.DealCards(1, playerIndex);

        // End turn
        yield return new WaitForSeconds(1.0f);
        gameManager.NextTurn();
    }
}
