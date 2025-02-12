using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardStatus
{
    public int boardNumber = -1;
    public int value = -1;
    public int[] playerBets = new int[0];
    public bool isLocked = false;
    public float goodChoice = 0;

    public int TotalBets()
    {
        int total = 0;

        if (playerBets.Length == 0)
        {
            return 0;
        }

        for (int i = 0; i < playerBets.Length; i++)
        {
            total += playerBets[i];
        }

        return total;
    }
}

public class CardStatus
{
    public int cardIndex;
    public int boardNumber = -1;
    public int value = -1;
    public int goodChoice = 0;
}

public class AI : MonoBehaviour
{
    public GameManager gameManager;
    [HideInInspector]
    public PlayerEntry playerData;
    [HideInInspector]
    public int playerIndex;
    public MovingChip movingChip;

    [Range(0, 10)] public int strategy = 5; // Slider for AI's strategy level

    void Start()
    {
        playerIndex = transform.GetSiblingIndex();
        playerData = GetComponent<PlayerEntry>();
    }

    void Update()
    {
        movingChip = (movingChip == null) ? FindFirstObjectByType<MovingChip>() : movingChip;
    }

    public IEnumerator PlayTurn()
    {
        // Prep the final choices
        int[] finalChoice = {-1, -1};

        // Choose a card
        bool goodChoice = false;
        int count = 0;
        int cardBoardNumber = 0;
        Board cardBoard = null;
        int cardValue = 0;

        while (!goodChoice)
        {
            // Choose a random card
            finalChoice[1] = UnityEngine.Random.Range(0, playerData.hand.Count);

            // Read the card
            cardBoardNumber = int.Parse(playerData.hand[finalChoice[1]].Split(":")[0]);
            cardValue = int.Parse(playerData.hand[finalChoice[1]].Split(":")[1]);

            if (cardBoardNumber > 0 && cardBoardNumber < 5)
            {
                cardBoard = gameManager.boards.FirstOrDefault(b => b.boardNumber == cardBoardNumber);
            }

            // Go with random card if difficulty is easy
            if (gameManager.totalPoints < gameManager.pointMaxes[0])
            {
                goodChoice = true;
                break;
            }

            // Assess if it's a good choice
            if ( // Raise
                cardBoardNumber > 0 && cardBoardNumber < 5 && // Normal card
                BoardPlayerBets(playerIndex, cardBoard) > 0 && // Board has player's bets
                cardBoard.value <= cardValue && // The card will raise the board's value
                !cardBoard.isLocked // Board is not locked
            )
            {
                goodChoice = true;
            }
            else if ( // Lower
                cardBoardNumber > 0 && cardBoardNumber < 5 && // Normal card
                BoardPlayerBets(playerIndex, cardBoard) == 0 && // Board has NO player bets
                (cardBoard.value > cardValue || cardValue == 0) && // The card will lower the board's value OR randomise it
                cardBoard.bets.Count > 0 && // There are other bets
                !cardBoard.isLocked // Board is not locked
            )
            {
                goodChoice = true;
            }
            else if ( // Lock
                cardBoardNumber > 0 && cardBoardNumber < 5 && // Normal card
                BoardPlayerBets(playerIndex, cardBoard) > 0 && // Board has player's bets
                !LowestValueBoard(cardBoardNumber) && // Not the lowest board number
                cardValue == -1 && // Card locks the board
                !cardBoard.isLocked // Board is not locked
            )
            {
                goodChoice = true;
            }
            else if ( // Randomise
                cardBoardNumber == -1 && // Randomise card
                BoardPlayerBets(playerIndex, GetHighestBoard()) == 0 // Highest board has NO player bets
            )
            {
                goodChoice = true;
            }
            else if ( // Set All Valueless
                cardBoardNumber == -2 && // Set all card
                //AmountOfEmptyBoards() == 1 && // There is only ONE valueless board
                GetLowestBoard().value > 0 && // Lowest board is NOT blank
                GetLowestBoard().value < cardValue && // Playing this card may delete the lowest value
                BoardPlayerBets(playerIndex, GetLowestBoard()) == 0 // Board has NO player bets
            )
            {
                goodChoice = true;
            }

            if (count < 20)
            {
                count++;
            }
            else
            {
                Debug.Log("No good choices...");
                break;
            }
        }

        // Determine which boards are playable
        if (gameManager.boards.Where(b => !b.isLocked && b.bets.Count < gameManager.maxBet).ToArray().Length > 0)
        {
            yield return new WaitForSeconds(1.0f);

            // Pick a board
            Board selectedBoard = gameManager.boards[UnityEngine.Random.Range(0, gameManager.boards.Count)];
            int selectedBoardValue = (selectedBoard.boardNumber == cardBoardNumber) ? cardValue : selectedBoard.value;
            int errorCount = 0;
            bool searchingTop = true; // First 10 iterations focus on top boards

            while (errorCount < 20)
            {
                // Update the chosen board value
                selectedBoardValue = (selectedBoard.boardNumber == cardBoardNumber) ? cardValue : selectedBoard.value;

                // Check if the board is valid
                bool isValid = false;
                if (gameManager.totalPoints < gameManager.pointMaxes[0])
                {
                    Debug.Log("EASY");
                    isValid = searchingTop 
                    ? BoardInTop(6, selectedBoardValue) && !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet // First 10 tries: only pick from the top
                    : !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet; // After 10 tries: pick any valid board
                }
                else if (gameManager.totalPoints < gameManager.pointMaxes[1])
                {
                    // Normal difficulty
                    Debug.Log("NORMAL");
                    isValid = searchingTop 
                    ? BoardInTop(2, selectedBoardValue) && !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet // First 10 tries: only pick from the top
                    : !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet; // After 10 tries: pick any valid board
                }
                else if (gameManager.totalPoints < gameManager.pointMaxes[3])
                {
                    Debug.Log("HARD");
                    // Hard difficulty
                    isValid = searchingTop 
                    ? BoardInTop(1, selectedBoardValue) && !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet // First 10 tries: only pick from the top
                    : !selectedBoard.isLocked && selectedBoard.bets.Count < gameManager.maxBet; // After 10 tries: pick any valid board
                }

                if (isValid)
                    break; // Exit loop when a valid board is found

                // Pick a new board randomly
                selectedBoard = gameManager.boards[UnityEngine.Random.Range(0, gameManager.boards.Count)];
                errorCount++;

                // After failed attempts, allow picking any valid board
                if (errorCount == 10) // Switch to checking any valid board after 10 attempts
                {
                    searchingTop = false;
                    Debug.Log("Switching to any available board.");
                }
            }

            // Fatal error handling if no board is found after 20 tries
            if (errorCount >= 20)
            {
                Debug.LogError("ERROR: While loop fatal error! No valid board found.");
            }

            // Assign final choice
            finalChoice[0] = selectedBoard.boardNumber;

            // Play sound
            AudioManager.Play("chip0", "chip1", "chip2");

            // Increase score
            if (!gameManager.betScore)
            {
                playerData.score++;
            }

            // Place chip
            Debug.Log($"FINAL CHOICE: {finalChoice[0]}");
            selectedBoard = (selectedBoard == null) ? gameManager.boards[UnityEngine.Random.Range(0, gameManager.boards.Count)] : selectedBoard;
            selectedBoard.bets.Add(playerIndex);
            selectedBoard.betMarkers[selectedBoard.bets.Count - 1].GetComponent<Animator>().enabled = true;
        }

        // Play card and end the turn
        yield return new WaitForSeconds(1.0f);
        FindFirstObjectByType<Card>().PlayCard(cardBoardNumber, cardValue, finalChoice[1]);
    }

    BoardStatus[] OrderBoards(BoardStatus[] list)
    {
        // Order boards by value from highest to lowest value
        string newOrder = "NEW ORDER: ";
        for (int i = 0; i < list.Length; i++)
        {
            newOrder += list.OrderByDescending(b => b.value).ToArray()[i].boardNumber + " ";
        }
        Debug.Log(newOrder);
        return list.OrderByDescending(b => b.value).ToArray();
    }

    bool LowestValueBoard(int boardIndex)
    {
        if (boardIndex < 0 && boardIndex > 5)
        {
            return false;
        }

        int lowest = 9;
        for (int i = 0; i < gameManager.boards.Count; i++)
        {
            lowest = (gameManager.boards[i].value < lowest) ? gameManager.boards[i].value : lowest;
        }

        Debug.Log($"Lowest Value: {lowest}");
        return gameManager.boards.FirstOrDefault(b => b.boardNumber == boardIndex).value == lowest;
    }

    bool LowestPlayerScore(int playerIndex)
    {
        int lowest = 100;
        foreach (PlayerEntry player in gameManager.players)
        {
            lowest = (player.gameObject.activeInHierarchy && player.score < lowest) ? player.score : lowest;
        }

        return gameManager.players[playerIndex].score == lowest;
    }

    int BoardPlayerBets(int playerIndex, Board board)
    {
        int count = 0;

        if (board.bets.Count == 0)
        {
            return 0;
        }

        foreach (int bet in board.bets)
        {
            count += bet == playerIndex ? 1 : 0;
        }

        return count;
    }

    Board GetHighestBoard()
    {
        Board result = null;

        foreach (Board board in gameManager.boards)
        {
            result = ((result == null || result.value < board.value) && board.gameObject.activeInHierarchy) ? board : result;
        }

        return result;
    }

    Board GetLowestBoard()
    {
        Board result = null;

        foreach (Board board in gameManager.boards)
        {
            if (board.value == 0)
            {
                continue;
            }

            result = ((result == null || result.value > board.value) && board.gameObject.activeInHierarchy) ? board : result;
        }

        if (result == null)
        {
            result = gameManager.boards[0];
        }

        return result;
    }

    bool BoardInTop(int inTop, int boardNumber)
    {
        // Get the values
        Board[] tempBoards = gameManager.boards.Where(b => b.gameObject.activeInHierarchy).OrderByDescending(b => b.value).ToArray();

        for (int i = 0; i < inTop; i++)
        {
            if (i >= tempBoards.Length - 1)
            {
                return false;
            }

            if (tempBoards[i].boardNumber == boardNumber)
            {
                return true;
            }
        }

        return false;
    }

    int AmountOfEmptyBoards()
    {
        int result = 0;

        foreach (Board board in gameManager.boards)
        {
            result += board.value > 0 ? 1 : 0;
        }

        return result;
    }
}
