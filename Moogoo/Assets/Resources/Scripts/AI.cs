using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AI : MonoBehaviour
{
    public GameManager gameManager;
    [HideInInspector]
    public PlayerEntry playerData;
    private int playerIndex;
    public MovingChip movingChip;

    void Start()
    {
        playerIndex = transform.GetSiblingIndex();
        playerData = GetComponent<PlayerEntry>();
    }

    void Update()
    {
        movingChip = (movingChip == null) ? FindObjectOfType<MovingChip>() : movingChip;
    }

    public IEnumerator PlayTurn()
    {
        // Place a bet if all boards are NOT full of bets
        int selection = UnityEngine.Random.Range(0, gameManager.boards.Count);
        if (!gameManager.boards.All(board => board.bets.Count >= gameManager.maxBet))
        {
            yield return new WaitForSeconds(1.0f);
            while (!gameManager.boards[selection].gameObject.activeInHierarchy)
            {
                selection = UnityEngine.Random.Range(0, gameManager.boards.Count);
            }
            gameManager.boards[selection].bets.Add(playerIndex);
        }

        // Play a card
        yield return new WaitForSeconds(1.0f);
        selection = 0;
        string[] cardContent = playerData.hand[selection].Split(":");

        if (int.Parse(cardContent[0]) > 0)
        {
            Board selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == int.Parse(cardContent[0]));
            while (!selectedBoard.gameObject.activeInHierarchy)
            {
                selection++;
            }

            // Play the card
            selectedBoard.value = int.Parse(cardContent[1]);
        }
        else if (int.Parse(cardContent[0]) == -1)
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
                        board.value = UnityEngine.Random.Range(1, 10);
                    }
                }
            }
        }

        // Discard the card
        playerData.hand.RemoveAt(selection);

        // Draw a new card
        gameManager.DealCards(1, playerIndex);

        // End turn
        yield return new WaitForSeconds(1.0f);
        gameManager.NextTurn();
    }

    /*public IEnumerator PlayTurn()
    {
        // Analyse cards in hand
        int[] strongestBoard = new int[] { 0, 0 };
        int[] backupBoard = new int[] { 0, 0 };
        int[] lowestBoard = new int[] { 0, int.MaxValue };
        int strongestCardIndex = -1;
        int backupCardIndex = -1;
        int lowestCardIndex = -1;
        int specialCardIndex = -1; // Tracks the index of a special card

        for (int i = 0; i < playerData.hand.Count; i++)
        {
            string[] cardValues = playerData.hand[i].Split(":");
            int boardNumber = int.Parse(cardValues[0]);
            int cardValue = int.Parse(cardValues[1]);

            int[] currentBoard = new int[] { boardNumber, cardValue };

            // Identify special cards (color value -1)
            if (cardValue == -1)
            {
                specialCardIndex = i;
                continue; // Skip evaluation for strongest/lowest cards
            }

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

        // Decide whether to play a special card
        if (specialCardIndex != -1 && UnityEngine.Random.Range(0, 4) == 0) // 25% chance to play the special card
        {
            // Randomize all boards except those with value 0
            foreach (Board board in gameManager.boards)
            {
                if (board.value > 0)
                {
                    board.value = UnityEngine.Random.Range(1, 10); // Randomize values (example range: 1 to 10)
                }
            }

            // Remove the special card from the hand
            playerData.hand.RemoveAt(specialCardIndex);

            // React to the special card play
            playerData.React(1, UnityEngine.Random.Range(0.0f, 1.0f));

            // End turn after playing the special card
            yield return new WaitForSeconds(1.0f);
            gameManager.NextTurn();
            yield break;
        }

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
                try
                {
                    playerData.hand.RemoveAt(backupCardIndex);
                }
                catch { }
            }
            else
            {
                gameManager.boards.FirstOrDefault(board => board.boardNumber == strongestBoard[0]).value = strongestBoard[1];
                if (strongestBoard[1] > 7 && UnityEngine.Random.Range(0, 3) == 0)
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
    }*/
}
