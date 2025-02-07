using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using NUnit.Framework.Interfaces;

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

    // Board reading variables
    private int[] _scores;
    private BoardStatus[] _boards;
    private CardStatus[] _cards;
    [TextArea(1, 20)]
    public string _gameReview;

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
        // Define variables of the game
        _scores = new int[gameManager.activePlayers];
        _boards = new BoardStatus[gameManager.activeBoards.Length];
        _cards = new CardStatus[gameManager.players[playerIndex].hand.Count];
        _gameReview = "";
        
        // Hold the player's final choice for bet and card
        int[] finalChoice = new int[] { -1, -1 };

        // Read player scores
        for (int i = 0; i < gameManager.activePlayers; i++)
        {
            _scores[i] = gameManager.players[i].score;
            _gameReview += $"PLAYER {i}: {_scores[i]}\n";
        }

        _gameReview += "\n";

        // Read board statuses
        for (int i = 0; i < gameManager.activeBoards.Length; i++)
        {
            // Instantiate the board
            _boards[i] = new BoardStatus();
            
            // Board number
            _boards[i].boardNumber = gameManager.activeBoards[i].boardNumber;
            _gameReview += $"BOARD {_boards[i].boardNumber}: ";

            // Value
            _boards[i].value = gameManager.activeBoards[i].value;
            _gameReview += $"Value: {_boards[i].value} || ";

            // Locked
            Debug.Log($"LOCK: {i}");
            _boards[i].isLocked = gameManager.boards[i].isLocked;
            _gameReview += _boards[i].isLocked ? $"Locked || " : "Unlocked || ";

            // Bets
            _boards[i].playerBets = new int[gameManager.activePlayers];
            _gameReview += "Bets: ";
            if (gameManager.boards[i].bets.Count > 0)
            {
                for (int i2 = 0; i2 < gameManager.boards[i].bets.Count; i2++)
                {
                    _boards[i].playerBets[gameManager.boards[i].bets[i2]]++;
                    _gameReview += $"{gameManager.boards[i].bets[i2]} ";
                }
            }
            _gameReview += "\n";
        }

        _gameReview += "\n";

        // Read player cards
        for (int i = 0; i < gameManager.players[playerIndex].hand.Count; i++)
        {
            // Instantiate the card
            _cards[i] = new CardStatus();
            _cards[i].cardIndex = i;
            _gameReview += $"CARD {i}: ";

            // Board number associated with card
            _cards[i].boardNumber = int.Parse(gameManager.players[playerIndex].hand[i].Split(':')[0]);
            _gameReview += $"Board {_cards[i].boardNumber} | ";

            // Value
            _cards[i].value = int.Parse(gameManager.players[playerIndex].hand[i].Split(':')[1]);
            _gameReview += $"Value {_cards[i].value}\n";

            // Good
            _cards[i].goodChoice = 0;
        }

        // Sort boards from highest amount of the current player's bets to lowest
        _boards = OrderBoards(playerIndex);

        // Look through the cards to see which plays are good
        string[] reasoning = new string[5];
        string _cardStats = $"-- PLAYER {gameManager.turn} --\n";
        for (int i = 0; i < _cards.Length; i++)
        {
            // Get the relevant board from the card
            BoardStatus boardFromCard = _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber);

            // Determine whether a card is a good choice to play
            if (
                // Raise board value?
                _cards[i].boardNumber > -1 && // Card has a normal board value
                _cards[i].value > 0 && // Card is not a randomiser
                boardFromCard.playerBets[gameManager.turn] > 0 && // Target board has player's bets
                boardFromCard.value <= _cards[i].value // Target board has a lower value than the potential card
            ){
                _cards[i].goodChoice++;

                // If board with bets is in last place
                if (
                    LowestValue(_cards[i].boardNumber) // Board is the lowest value
                )
                {
                    _cards[i].goodChoice++;
                }
            }
            else if (
                // Lower board value?
                _cards[i].boardNumber > -1 && // Card has a normal board value
                _cards[i].value > 0 && // Card is not a randomiser
                boardFromCard.playerBets.Where(p => !(p == gameManager.turn)).ToList().Sum() > 0 && // Target board has bets from other players
                boardFromCard.value > _cards[i].value // Target board has a higher value than the potential card
            ){
                _cards[i].goodChoice++;

                // If board with bets is in first place
                if (
                    HighestValue(_cards[i].boardNumber) && // Board is the highest value
                    boardFromCard.playerBets[gameManager.turn] == 0 // Target board has NONE of the player's bets
                )
                {
                    _cards[i].goodChoice++;
                }
            }
            _cardStats += $"{_cards[i].boardNumber}:{_cards[i].value} = {_cards[i].goodChoice}\n";

            /*_cards[i].goodChoice = 

            // Raise board value?
            (
                _cards[i].boardNumber > -1 && // Card has a normal board value
                _cards[i].value > 0 && // Card is not a randomiser
                boardFromCard.playerBets[gameManager.turn] > 0 && // Target board has player's bets
                boardFromCard.value <= _cards[i].value // Target board has a lower value than the potential card
            ) 
            || // Lower board value?
            (
                _cards[i].boardNumber > -1 && // Card has a normal board value
                _cards[i].value > 0 && // Card is not a randomiser
                boardFromCard.playerBets[gameManager.turn] == 0 && // Target board has NONE of the player's bets
                boardFromCard.value > _cards[i].value // Target board has a higher value than the potential card
            ); 

            _cardStats += $"{_cards[i].boardNumber}:{_cards[i].value} = {_cards[i].goodChoice}\n";
            
            // Special cards
            /*if (_cards[i].boardNumber == -1) // Randomise values
            {
                // It's a good idea to randomise values if the highest value board contains NO bets from the player
                _cards[i].goodChoice = !(OrderBoards()[0].playerBets[playerIndex] > 0);
                reasoning[i] = (_cards[i].goodChoice) ? $"PLAYER {gameManager.turn}: Playing Randomise Values Card because the highest value board contains NO bets from me." : "BAD CHOICE";
            }
            else if (_cards[i].boardNumber == -2) // Set all empty boards to #
            {
                // It's a good idea to set all empty boards to # higher or lower to suit the needs of the player
                //Debug.Log($"{_cards[i].boardNumber}:{_cards[i].value}");
                _cards[i].goodChoice = true;
                reasoning[i] = $"Playing Set All Empties to {_cards[i].value} because DEFAULT.";
            }
            // Normal cards
            else if (_cards[i].value > boardFromCard.value) // Raise the board value?
            {
                // It's a good idea to raise the board value if the player has bets on it
                _cards[i].goodChoice = (_boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber).playerBets[playerIndex] > 0) && !boardFromCard.isLocked;
                reasoning[i] = (_cards[i].goodChoice) ? $"PLAYER {gameManager.turn}: Playing {_cards[i].boardNumber}:{_cards[i].value} because I want to raise the value of a board I have bets on." : "BAD CHOICE";
            }
            else if (_cards[i].value <= boardFromCard.value && _cards[i].value > 0) // Lower the board value?
            {
                // It's a good idea to lower the board value if other players have bets on the board
                int otherPlayerBets = 0;
                foreach (int amountOfBets in _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber).playerBets)
                {
                    otherPlayerBets += amountOfBets;
                }
                _cards[i].goodChoice = _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber).playerBets[playerIndex] == 0 && otherPlayerBets > 0 && !boardFromCard.isLocked;
                reasoning[i] = (_cards[i].goodChoice) ? $"PLAYER {gameManager.turn}: Playing {_cards[i].boardNumber}:{_cards[i].value} because I want to lower the value of a board I have no bets on." : "BAD CHOICE";
            }
            else if (_cards[i].value == -1 || _cards[i].value == 0) // Lock the board? Randomise the board value?
            {
                // It's a good idea to lock the board if the player has bets on the board AND it currently has a relatively high number
                _cards[i].goodChoice = _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber).playerBets[playerIndex] > 0 && Array.IndexOf(OrderBoards(), _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber)) / _boards.Length <= 0.5f;
                reasoning[i] = (_cards[i].goodChoice) ? $"PLAYER {gameManager.turn}: Playing {_cards[i].boardNumber}:{_cards[i].value} because I want to lock this board with my bets on it with a high value." : "BAD CHOICE";
                if (_cards[i].goodChoice)
                {
                    continue;
                }

                // It's a good idea to lock the board if the player has NO bets on the board AND it currently has a relatively low number
                _cards[i].goodChoice = _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber).playerBets[playerIndex] == 0 && Array.IndexOf(OrderBoards(), _boards.FirstOrDefault(b => b.boardNumber == _cards[i].boardNumber)) / _boards.Length >= 0.5f;
                reasoning[i] = (_cards[i].goodChoice) ? $"PLAYER {gameManager.turn}: Playing {_cards[i].boardNumber}:{_cards[i].value} because I want to lock this board with none of my bets on it with a low value." : "BAD CHOICE";
            }*/
        }

        Debug.Log(_cardStats);
        
        // Decide which card strategy to go with
        int maxValue = _cards.Max(c => c.goodChoice);
        CardStatus[] highestValueCards = _cards.Where(c => c.goodChoice == maxValue).ToArray();
        finalChoice[1] = highestValueCards[UnityEngine.Random.Range(0, highestValueCards.Length)].cardIndex;

        // Determine which boards are playable
        BoardStatus[] availableBoards = _boards.Where(b => !b.isLocked && b.playerBets.Sum() < gameManager.maxBet).ToArray();

        // Place a bet
        if (availableBoards.Any())
        {
            yield return new WaitForSeconds(1.0f);

            // Update the board with the value that will be changed by the card played
            if (_cards[finalChoice[1]].boardNumber > -1)
            {
                BoardStatus tempBoard = availableBoards.FirstOrDefault(ab => ab.boardNumber == _cards[finalChoice[1]].boardNumber);
                if (tempBoard != null)
                {
                    Debug.Log($"From {tempBoard.value}");
                    tempBoard.value = _cards[finalChoice[1]].value;
                    Debug.Log($"To {tempBoard.value}");
                }
            }

            // Determine which boards are a good choice to bet on
            string boardOptions = $"-- PLAYER {gameManager.turn} --\n";
            for (int i = 0; i < availableBoards.Length; i++)
            {
                availableBoards[i].goodChoice = (availableBoards[i].value > 0) ? 1.0f - Array.IndexOf(OrderBoards(availableBoards), availableBoards[i]) / availableBoards.Length : 0;
                boardOptions += $"BOARD {availableBoards[i].boardNumber} = {availableBoards[i].goodChoice}\n";
            }

            // Choose a board to bet on
            float threshold = 0.5f;
            BoardStatus[] highestValueBoards = availableBoards.Where(b => b.goodChoice > threshold).ToArray();
            finalChoice[0] = (highestValueBoards.Length > 0) ? highestValueBoards[UnityEngine.Random.Range(0, highestValueBoards.Length)].boardNumber: 0;
            Debug.Log(boardOptions);

            // Update board values according to the card thats going to be played
            /*if (_cards[finalChoice[1]].boardNumber > -1)
            {
                _boards.FirstOrDefault(b => b.boardNumber == _cards[finalChoice[1]].boardNumber).value = (_cards[finalChoice[1]].value > 0) ? _cards[finalChoice[1]].value : _boards.FirstOrDefault(b => b.boardNumber == _cards[finalChoice[1]].boardNumber).value;
            }

            // Determine which boards are a good choice to bet on
            for (int i = 0; i < availableBoards.Count; i++)
            {
                // Bet on boards with higher numbers unless you're going to change the value to higher
                BoardStatus[] orderByValue = OrderBoards();
                _boards[i].goodChoice = _boards[i].value > 0 && Array.IndexOf(orderByValue, availableBoards[i]) / orderByValue.Length <= 0.5f;

                // Don't bet on a board if you're lowering its value
                _boards[i].goodChoice = _boards[i].boardNumber == _cards[finalChoice[1]].boardNumber && _boards[i].value < gameManager.boards[i].value;
            }

            // Choose a board to bet on
            finalChoice[0] = UnityEngine.Random.Range(0, availableBoards.Count);
            int count = 0;
            while (!availableBoards[finalChoice[0]].goodChoice)
            {
                finalChoice[0] = UnityEngine.Random.Range(0, availableBoards.Count);
                count++;

                if (count == 100)
                {
                    Debug.Log("No good choice boards...");
                    break;
                }
            }*/

            // Play sound
            AudioManager.Play("chip0", "chip1", "chip2");

            // Increase score
            if (!gameManager.betScore)
            {
                playerData.score++;
            }

            // Place chip
            Board selectedBoard = gameManager.boards.FirstOrDefault(b => b.boardNumber == availableBoards[finalChoice[0]].boardNumber);
            selectedBoard.bets.Add(playerIndex);
            selectedBoard.betMarkers[selectedBoard.bets.Count - 1].GetComponent<Animator>().enabled = true;
        }

        // Play card and end the turn
        yield return new WaitForSeconds(1.0f);
        FindFirstObjectByType<Card>().PlayCard(_cards[finalChoice[1]].boardNumber, _cards[finalChoice[1]].value, finalChoice[1]);
    }

    BoardStatus[] OrderBoards()
    {
        // Order boards by value from highest to lowest value
        return _boards.OrderByDescending(b => b.value).ToArray();
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

    BoardStatus[] OrderBoards(int _playerIndex)
    {
        // Order boards by number of bets placed by a specific player
        return _boards.OrderByDescending(b => b.playerBets[_playerIndex]).ToArray();
    }

    bool LowestValue(int boardIndex)
    {
        int lowest = 9;
        for (int i = 0; i < _boards.Length; i++)
        {
            lowest = (_boards[i].value < lowest) ? _boards[i].value : lowest;
        }

        return _boards[boardIndex].value == lowest;
    }

    bool HighestValue(int boardIndex)
    {
        int highest = 0;
        for (int i = 0; i < _boards.Length; i++)
        {
            highest = (_boards[i].value > highest) ? _boards[i].value : highest;
        }

        return _boards[boardIndex].value == highest;
    }
}
