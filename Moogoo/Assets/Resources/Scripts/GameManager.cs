using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Color[] colours;
    public List<string> deck = new List<string>();

    // Players
    public Transform playerEntriesParent;
    public GameObject addButton;
    public GameObject deleteButton;
    public Button startButton;
    public PlayerEntry[] players;
    public int turn;
    public Hand hand;
    public Sprite[] icons;
    public Sprite[] reactionImages;
    public AnimationClip[] reactionAnimations;

    // Game settings
    public int maxBet;

    public List<Board> boards = new List<Board>();

    // Game
    public bool bettingTime;

    void Start()
    {
        startButton.gameObject.SetActive(true);
    }

    void Update()
    {
        // Disable card buttons when it's not your turn
        foreach (Card card in hand.cards)
        {
            card.button.interactable = (turn == 0 && hand.canPlayCard && !bettingTime);
        }

        // Update score
        foreach (PlayerEntry player in players)
        {
            player.score = 0;
        }
        foreach (Board board in boards)
        {
            foreach (int bet in board.bets)
            {
                players[bet].score++;
            }
        }

        // Remove disabled boards
        for (int i = boards.Count - 1; i > -1; i--)
        {
            if (!boards[i].gameObject.activeInHierarchy)
            {
                boards.RemoveAt(i);
            }
        }

        for (int i = 1; i < 4; i++)
        {
            // Check if the number key is pressed
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + i)))
            {
                players[0].React(i - 1);
            }
        }
    }

    public void AddPlayer()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].gameObject.activeInHierarchy)
            {
                players[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    public void DeletePlayer()
    {
        for (int i = players.Length - 1; i > 0; i--)
        {
            if (players[i].gameObject.activeInHierarchy)
            {
                players[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    public void StartButton()
    {
        addButton.SetActive(false);
        deleteButton.SetActive(false);
        startButton.gameObject.SetActive(false);
        NewDeck();
        DealCards(5);
        turn = Random.Range(0, players.Count(obj => obj.gameObject.activeInHierarchy));
        NextTurn();
    }

    public void NextTurn()
    {
        // Check for win/lose conditions
        bool allBoardsHaveValue = true;
        int[] boardValues = new int[boards.Count];
        for (int i = 0; i < boardValues.Length; i++)
        {
            boardValues[i] = -1;  // Initialize with -1
        }
        
        // Check if all of the boards have a value
        for (int i = 0; i < boards.Count; i++)
        {
            if (boards[i].value == 0)
            {
                allBoardsHaveValue = false;
            }
            else
            {
                boardValues[i] = boards[i].value;
            }
        }

        // Check if there's NOT a tie
        System.Array.Sort(boardValues);
        System.Array.Reverse(boardValues);
        if (allBoardsHaveValue && boardValues[boardValues.Length - 1] < boardValues[boardValues.Length - 2])
        {
            // Find in the difference in scores
            foreach (PlayerEntry player in players)
            {
                player.oldScore = player.score;
            }

            int deletedBoardIndex = -1;
            for (int i = boards.Count - 1; i > - 1; i--)
            {
                if (boards[i].value == boardValues[boardValues.Length - 1])
                {
                    // Remove the lowest board
                    deletedBoardIndex = boards[i].boardNumber;
                    boards[i].gameObject.SetActive(false);
                    boards.RemoveAt(i);
                }
            }

            // Reset the remaining boards
            for (int i = 0; i < boards.Count; i++)
            {
                boards[i].value = 0;
            }

            // Remove all the cards that belonged to the deleted board
            for (int i = deck.Count - 1; i > -1; i--)
            {
                if (int.Parse(deck[i][0].ToString()) == deletedBoardIndex)
                {
                    deck.RemoveAt(i);
                }
            }

            // Delete the cards in the hands of the players
            for (int i = 0; i < players.Length; i++)
            {
                for (int i2 = players[i].hand.Count - 1; i2 > -1; i2--)
                {
                    // Replace the card if its board has been deleted
                    if (int.Parse(players[i].hand[i2][0].ToString()) == deletedBoardIndex)
                    {
                        players[i].hand.Remove(players[i].hand[i2]);
                        DealCards(1, i);
                    }
                }
            }

            StartCoroutine(AIReact());
        }

        // Go to the next turn
        if (turn == players.Count(obj => obj.gameObject.activeInHierarchy) - 1)
        {
            turn = 0;
        }
        else
        {
            turn++;
        }

        // Activate AI
        if (turn > 0)
        {
            players[turn].ActivateAI();
        }
        else
        {
            // Start the player's turn
            bool boardsFull = true;
            foreach (Board board in boards)
            {
                if (board.bets.Count < maxBet)
                {
                    boardsFull = false;
                }
            }

            bettingTime = !boardsFull;
            hand.canPlayCard = true;
        }
    }

    public void NewDeck()
    {
        // Clear the old deck
        deck.Clear();

        for (int i = 1; i < 10; i++)
        {
            for (int i2 = 0; i2 < boards.Count; i2++)
            {
                deck.Add(boards[i2].boardNumber.ToString() + ":" + i.ToString());
            }
        }

        // Shuffle deck
        deck = deck.OrderBy(x => Random.value).ToList();
    }

    public void DealCards(int amount)
    {
        // Create a new deck if the previous cards run out
        if (deck.Count <= 0)
        {
            NewDeck();
        }

        for (int i = 0; i < amount; i++)
        {
            foreach (PlayerEntry player in players)
            {
                if (player.gameObject.activeInHierarchy)
                {
                    player.hand.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
        }
    }

    public void DealCards(int amount, int playerIndex)
    {
        // Create a new deck if the previous cards run out
        if (deck.Count <= 0)
        {
            NewDeck();
        }

        if (players[playerIndex].gameObject.activeInHierarchy)
        {
            players[playerIndex].hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        else
        {
            Debug.LogWarning("Trying to deal cards to player " + playerIndex + ", who is not active.");
        }
    }

    IEnumerator AIReact()
    {
        yield return new WaitForSeconds(0.1f);
        // AI react to scores changing
        for (int i = 1; i < players.Length; i++)
        {
            if (!players[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            int previousTurn = turn - 1;
            if (turn < 0)
            {
                previousTurn = 0;
            }

            if (i == previousTurn)
            {
                players[turn].React(0, Random.Range(0.0f, 1.0f));
            }
            else if (players[i].score < players[i].oldScore)
            {
                players[i].React(1, Random.Range(0.0f, 1.0f));
            }
            else if (players[i].score == players[i].oldScore)
            {
                //players[i].React(2, Random.Range(0.0f, 1.0f));
            }
        }
    }
}
