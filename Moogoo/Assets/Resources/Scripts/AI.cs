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
        var availableBoards = gameManager.boards
            .Where(board => !board.isLocked && board.bets.Count < gameManager.maxBet && board.gameObject.activeInHierarchy)
            .ToList();

        if (availableBoards.Any())
        {
            yield return new WaitForSeconds(1.0f);

            // Play sound
            AudioManager.Play("chip0", "chip1", "chip2");

            // Increase score
            if (!gameManager.betScore)
            {
                playerData.score++;
            }

            // Place chip
            Board selectedBoard = availableBoards[UnityEngine.Random.Range(0, availableBoards.Count)];
            selectedBoard.bets.Add(playerIndex);
            selectedBoard.betMarkers[selectedBoard.bets.Count - 1].GetComponent<Animator>().enabled = true;
        }

        // Play a card
        yield return new WaitForSeconds(1.0f);
        int selection = 0;
        string[] cardContent = playerData.hand[selection].Split(":");

        // Play sound
        AudioManager.Play("card1", "card2");

        if (int.Parse(cardContent[0]) > -1)
        {
            Board selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == int.Parse(cardContent[0]) && !board.isLocked);
            while (selectedBoard == null || !selectedBoard.gameObject.activeInHierarchy)
            {
                selection++;
                if (selection >= playerData.hand.Count)
                    break; // Exit loop if no valid cards remain
                cardContent = playerData.hand[selection].Split(":");
                selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == int.Parse(cardContent[0]) && !board.isLocked);
            }

            if (selectedBoard != null && int.Parse(cardContent[1]) > -1)
            {
                // Play the card
                selectedBoard.value = (int.Parse(cardContent[1]) == 0) ? UnityEngine.Random.Range(1, 10) : int.Parse(cardContent[1]);
            }
            else if (selectedBoard != null && int.Parse(cardContent[1]) == -1)
            {
                // Lock the board
                selectedBoard.isLocked = true;
                selectedBoard.turnToUnlock = gameManager.totalTurns + gameManager.activePlayers;//(gameManager.turn - 1 == -1) ? gameManager.activePlayers - 1 : gameManager.turn - 1;
            }
        }
        else if (int.Parse(cardContent[0]) == -1)
        {
            // Randomize current values
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value > 0 && !board.isLocked)
                {
                    int oldValue = board.value;

                    while (board.value == oldValue)
                    {
                        board.value = UnityEngine.Random.Range(1, 10);
                    }
                }
            }
        }
        else if (int.Parse(cardContent[0]) == -2)
        {
            // Fill empty boards
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value == 0 && !board.isLocked)
                {
                    board.value = int.Parse(cardContent[1]);
                }
            }
        }

        // Discard the card
        playerData.hand.RemoveAt(selection);
        gameManager.discardPile.Add($"{cardContent[0]}:{cardContent[1]}");

        // Draw a new card
        gameManager.DealCards(1, playerIndex);

        // End turn
        yield return new WaitForSeconds(1.0f);
        gameManager.NextTurn();
    }
}
