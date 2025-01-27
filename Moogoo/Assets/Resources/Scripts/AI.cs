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
    [HideInInspector]
    public int playerIndex;
    public MovingChip movingChip;
    public enum Personality { Reckless, Aggressive, Defensive};
    public Personality personality = Personality.Reckless;

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

            // Choose where to place chip
            Board selectedBoard = null;
            switch (personality)
            {
                case Personality.Aggressive:
                    // Find the board with the highest value that is not locked and not full
                    selectedBoard = availableBoards
                        .Where(board => board.value > 0 && !board.isLocked && board.bets.Count < gameManager.maxBet) // Filter valid boards
                        .OrderByDescending(board => board.value) // Sort by highest value
                        .ThenBy(board => board.CountBets(playerIndex)) // Prefer boards with fewer AI bets
                        .FirstOrDefault();

                    // Try the second highest if the highest is not available
                    if (selectedBoard == null || selectedBoard.bets.Count >= gameManager.maxBet)
                    {
                        selectedBoard = availableBoards
                            .Where(board => board.value > 0 && !board.isLocked && board.bets.Count < gameManager.maxBet)
                            .OrderByDescending(board => board.value)
                            .Skip(1) // Skip the highest and try the second highest
                            .FirstOrDefault();
                    }

                    // Default to Reckless if no suitable board is found
                    if (selectedBoard == null)
                    {
                        selectedBoard = availableBoards[UnityEngine.Random.Range(0, availableBoards.Count)];
                    }
                    break;

                case Personality.Defensive:
                    // Find the board with the highest value that is not locked and not full
                    selectedBoard = availableBoards
                        .Where(board => board.value > 0 && !board.isLocked && board.bets.Count < gameManager.maxBet) // Filter valid boards
                        .OrderByDescending(board => board.value) // Sort by highest value
                        .ThenByDescending(board => board.CountBets(playerIndex)) // Prefer boards with more AI bets
                        .FirstOrDefault();

                    // Try the second highest if the highest is not available
                    if (selectedBoard == null || selectedBoard.bets.Count >= gameManager.maxBet)
                    {
                        selectedBoard = availableBoards
                            .Where(board => board.value > 0 && !board.isLocked && board.bets.Count < gameManager.maxBet)
                            .OrderByDescending(board => board.value)
                            .Skip(1) // Skip the highest and try the second highest
                            .FirstOrDefault();
                    }

                    // Default to Reckless if no suitable board is found
                    if (selectedBoard == null)
                    {
                        Debug.Log("No options, playing Reckless");
                        selectedBoard = availableBoards[UnityEngine.Random.Range(0, availableBoards.Count)];
                    }
                    break;

                case Personality.Reckless:
                    // Choose a random board from the available ones
                    selectedBoard = availableBoards[UnityEngine.Random.Range(0, availableBoards.Count)];
                    break;
            }

            // Place chip
            selectedBoard.bets.Add(playerIndex);
            selectedBoard.betMarkers[selectedBoard.bets.Count - 1].GetComponent<Animator>().enabled = true;
        }

        // Determine which card to play
        yield return new WaitForSeconds(1.0f);
        int selection = 0;
        switch (personality)
        {
            case Personality.Defensive:
                // Focus on raising or maintaining the value of the board with the most of AI's bets
                var defensiveTargetBoard = gameManager.boards
                    .Where(board => board.gameObject.activeInHierarchy && !board.isLocked && board.CountBets(playerIndex) > 0)
                    .OrderByDescending(board => board.CountBets(playerIndex)) // Most AI bets
                    .ThenByDescending(board => board.value) // Highest value
                    .FirstOrDefault();
        
                if (defensiveTargetBoard != null)
                {
                    selection = playerData.hand.FindIndex(card =>
                        int.Parse(card.Split(":")[0]) == defensiveTargetBoard.boardNumber);
                }
                break;
        
            case Personality.Aggressive:
                // Lower the value of the board with the most bets from the first/second place players
                var topPlayers = gameManager.GetTopPlayers().Take(2).Select(player => player.playerIndex).ToList();
        
                var aggressiveTargetBoard = gameManager.boards
                    .Where(board => board.gameObject.activeInHierarchy && !board.isLocked &&
                                    topPlayers.Any(p => board.CountBets(p) > 0) && board.CountBets(playerIndex) == 0) // Avoid sabotaging own board
                    .OrderByDescending(board => board.value) // Highest value
                    .FirstOrDefault();
        
                if (aggressiveTargetBoard != null)
                {
                    selection = playerData.hand.FindIndex(card =>
                        int.Parse(card.Split(":")[0]) == aggressiveTargetBoard.boardNumber);
                }
                break;
        
            case Personality.Reckless:
                // Pick the first card in the hand
                selection = 0;
                break;
        }
        
        // Ensure selection is valid
        if (selection < 0 || selection >= playerData.hand.Count)
        {
            selection = 0; // Default to the first card if no valid card was found
        }

        // Extract the content of the card being played
        string[] cardContent = playerData.hand[selection].Split(":");

        // Play a card
        if (int.Parse(cardContent[0]) > -1)
        {
            // Get the selected board
            Board selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == int.Parse(cardContent[0]));
            while (selectedBoard == null || !selectedBoard.gameObject.activeInHierarchy)
            {
                selection++;
                if (selection >= playerData.hand.Count)
                    break; // Exit loop if no valid cards remain
                cardContent = playerData.hand[selection].Split(":");
                selectedBoard = gameManager.boards.FirstOrDefault(board => board.boardNumber == int.Parse(cardContent[0]));
            }

            // Card effects
            if (selectedBoard != null && int.Parse(cardContent[1]) > -1 && !selectedBoard.isLocked)
            {
                // Play sound
                AudioManager.Play("card1", "card2");

                // Play the card
                selectedBoard.value = (int.Parse(cardContent[1]) == 0) ? UnityEngine.Random.Range(1, 10) : int.Parse(cardContent[1]);
            }
            else if (selectedBoard != null && int.Parse(cardContent[1]) > -1 && selectedBoard.isLocked)
            {
                // Play sound
                AudioManager.Play("discard");
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
            bool nothingChanged = true;
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value > 0 && !board.isLocked)
                {
                    nothingChanged = false;
                    int oldValue = board.value;

                    while (board.value == oldValue)
                    {
                        board.value = UnityEngine.Random.Range(1, 10);
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
        else if (int.Parse(cardContent[0]) == -2)
        {
            // Fill empty boards
            bool nothingChanged = true;
            foreach (Board board in gameManager.boards)
            {
                if (board.gameObject.activeInHierarchy && board.value == 0 && !board.isLocked)
                {
                    nothingChanged = false;
                    board.value = int.Parse(cardContent[1]);
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
