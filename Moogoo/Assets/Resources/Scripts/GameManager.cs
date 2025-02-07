using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<string> deck = new List<string>();

    // Players
    public Transform playerEntriesParent;
    public GameObject addButton;
    public GameObject deleteButton;
    public Button startButton;
    public PlayerEntry[] players;
    public int turn;
    public int totalTurns;
    public Hand hand;
    public Sprite[] icons;
    public Sprite[] reactionImages;
    public AnimationClip[] reactionAnimations;
    public Sprite[] cpuIcons;
    public string[] cpuNames;

    // Cards
    public Sprite[] cardIcons;
    public Color[] colours;

    // Game settings
    public int maxBet;
    public int round;
    public List<Board> boards = new List<Board>();
    public int boardsEnabled;
    public GameObject endGameWindow;
    private bool boardsIncrease;
    public Animator boardsAnimator;
    public Animator playersAnimator;
    public Sprite[] flags;
    public Image flagDisplay;

    // Game
    public bool bettingTime;
    public List<string> discardPile;
    public bool betScore;
    public int activePlayers;
    public Board[] activeBoards;

    // Settings
    public GameObject settingsWindow;
    [Range(0.0f, 1.0f)]
    public AudioSource soundAS;
    [Range(0.0f, 1.0f)]
    public AudioSource musicAS;
    [Range(0, 3)]
    public int languageIndex;
    public TMP_InputField nameInput;
    public Slider soundSlider;
    public Slider musicSlider;
    public Button languageButton;
    public TextMeshProUGUI[] settingsLabels;

    void Start()
    {
        startButton.gameObject.SetActive(true);
        
        // Set game up
        playersAnimator.enabled = false;
        boardsAnimator.enabled = false;
        round = 3;

        // Play music
        AudioManager.PlayMusic("Music0");

        // Load settings
        maxBet = PlayerPrefs.GetInt("MaxBet", 4);
        boardsEnabled = PlayerPrefs.GetInt("BoardCount", 6);
        for (int i = 0; i < boards.Count; i++)
        {
            boards[i].gameObject.SetActive(i < boardsEnabled);
        }
        for (int i = 0; i < 6; i++)
        {
            players[i].gameObject.SetActive(i < PlayerPrefs.GetInt("PlayerCount", 4));
        }
        betScore = PlayerPrefs.GetInt("GameMode", 0) == 1;
        languageIndex = PlayerPrefs.GetInt("Language", 0);
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = Language.language[10, languageIndex];
        for (int i = 0; i < settingsLabels.Length; i++)
        {
            settingsLabels[i].text = Language.language[18 + i, languageIndex];
        }
        flagDisplay.sprite = flags[languageIndex];
        for (int i = 0; i < activePlayers; i++)
        {
            Debug.Log($"PLAYER {i}: {PlayerPrefs.GetInt($"IconIndex_{i}", 0)}");
            players[i].iconIndex = PlayerPrefs.GetInt($"IconIndex_{i}", 0);
        }
        nameInput.text = PlayerPrefs.GetString("Name", "");
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0);
    }

    void Update()
    {
        // Update settings
        players[0].name = (nameInput.text == "") ? Language.language[23, languageIndex] : nameInput.text;
        soundAS.volume = soundSlider.value;
        musicAS.volume = musicSlider.value;

        // Update active players
        int tempActivePlayers = 0;
        foreach (PlayerEntry player in players)
        {
            if (player.gameObject.activeInHierarchy)
            {
                tempActivePlayers++;
            }
        }
        activePlayers = tempActivePlayers;

        // Update active boards
        int tempActiveBoards = 0;
        foreach (Board board in boards)
        {
            if (board.gameObject.activeInHierarchy)
            {
                tempActiveBoards++;
            }
        }
        activeBoards = new Board[tempActiveBoards];
        int tempIndex = 0;
        foreach (Board board in boards)
        {
            if (board.gameObject.activeInHierarchy)
            {
                activeBoards[tempIndex] = board;
                tempIndex++;
            }
        }

        // Disable card buttons when it's not your turn
        foreach (Card card in hand.cards)
        {
            card.button.interactable = (turn == 0 && hand.canPlayCard && !bettingTime);
        }

        // Count the amount of boards enabled
        boardsEnabled = 0;
        foreach (Board board in boards)
        {
            if (board.gameObject.activeInHierarchy)
            {
                boardsEnabled++;
            }
        }

        // Update score
        if (betScore)
        {
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
        }

        // Remove disabled boards
        for (int i = boards.Count - 1; i > -1; i--)
        {
            if (!boards[i].gameObject.activeInHierarchy && !startButton.gameObject.activeInHierarchy)
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
        // Play sound
        AudioManager.Play("AddPlayer");

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].gameObject.activeInHierarchy)
            {
                players[i].gameObject.SetActive(true);

                // Find all active player names
                var activeNames = players
                    .Where(player => player.gameObject.activeInHierarchy)
                    .Select(player => player.name)
                    .ToHashSet();

                // Randomly assign a unique name from playerNames
                string randomName;
                do
                {
                    randomName = cpuNames[Random.Range(0, cpuNames.Length)];
                } while (activeNames.Contains(randomName));

                players[i].name = randomName;
                break;
            }
        }
    }

    public void DeletePlayer()
    {
        // Play sound
        AudioManager.Play("DeletePlayer");

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
        // Play sound
        AudioManager.Play("UI0");

        // Close menu buttons
        addButton.SetActive(false);
        deleteButton.SetActive(false);
        startButton.gameObject.SetActive(false);

        // Disable icon buttons that change player colour
        foreach (PlayerEntry player in players)
        {
            player.iconDisplay.GetComponent<Button>().enabled = false;
        }

        // Move the game windows
        playersAnimator.enabled = true;
        boardsAnimator.enabled = true;

        // Save the stats
        PlayerPrefs.SetInt("PlayerCount", activePlayers);
        PlayerPrefs.SetInt("MaxBet", maxBet);
        PlayerPrefs.SetInt("BoardCount", boardsEnabled);
        PlayerPrefs.SetInt("GameMode", betScore ? 1 : 0);
        PlayerPrefs.SetInt("Language", languageIndex);
        PlayerPrefs.Save();
    }

    public void SetupGame()
    {
        // Create a new deck and start dealing cards
        NewDeck();
        StartCoroutine(InitialCards());
    }

    IEnumerator InitialCards()
    {
        yield return new WaitForSeconds(0.25f);
        DealCards(1);

        if (players[0].hand.Count < 5)
        {
            StartCoroutine(InitialCards());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            turn = Random.Range(0, players.Count(obj => obj.gameObject.activeInHierarchy));
            NextTurn();
        }
    }

    public void NextTurn()
    {
        StartCoroutine(StartNextTurn());
    }

    IEnumerator StartNextTurn()
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
                    // Find the lowest board
                    deletedBoardIndex = boards[i].boardNumber;
                    Board deletedBoard = boards.FirstOrDefault(board => board.boardNumber == deletedBoardIndex);

                    // Award points to the player who deleted points
                    int addedScore = 0;
                    if (deletedBoard.bets.Count() > 0)
                    {
                        for (int i2 = 0; i2 < deletedBoard.bets.Count(); i2++)
                        {
                            addedScore += (deletedBoard.bets[i2] == turn) ? -1 : 1;
                        }
                    }

                    // Remove the lowest board
                    boards[i].DeleteBoard();
                    yield return new WaitForSeconds(1.5f);
                    players[turn].score += addedScore;
                    boards[i].gameObject.SetActive(false);
                    boards.RemoveAt(i);
                }
            }

            // Reset the remaining boards
            for (int i = 0; i < boards.Count; i++)
            {
                boards[i].value = 0;
                boards[i].isLocked = false;
            }

            // Remove all the cards that belonged to the deleted board
            for (int i = deck.Count - 1; i > -1; i--)
            {
                if (int.Parse(deck[i].Split(":")[0].ToString()) == deletedBoardIndex)
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
                    if (int.Parse(players[i].hand[i2].Split(":")[0].ToString()) == deletedBoardIndex)
                    {
                        players[i].hand.Remove(players[i].hand[i2]);
                        DealCards(1, i);
                    }
                }
            }

            // End the round
            round--;

            if (round > 0)
            {
                StartCoroutine(AIReact());
            }
        }

        if (round > 0)
        {
            // Go to the next turn
            totalTurns++;
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
                        if (!board.isLocked)
                        {
                            boardsFull = false;
                        }
                    }
                }

                bettingTime = !boardsFull;
                hand.canPlayCard = true;
            }
        }
        else
        {
            // End the game
            StartCoroutine(EndGame());
        }
    }

    public void NewDeck()
    {
        // Clear the old deck
        deck.Clear();

        // Build a new deck
        for (int i = -1; i < 10; i++)
        {
            for (int i2 = 0; i2 < boards.Count(obj => obj.gameObject.activeInHierarchy); i2++)
            {
                deck.Add(boards[i2].boardNumber.ToString() + ":" + i.ToString());
            }
        }

        // Special Cards
        for (int i = 0; i < 3; i++)
        {
            // Randomise values
            deck.Add("-1:0");

            // Set current values to #
            deck.Add($"-2:{UnityEngine.Random.Range(1, 10)}");
        }

        // Shuffle deck
        deck = deck.OrderBy(x => Random.value).ToList();
    }

    public void DealCards(int amount)
    {
        // Play sound
        if (turn < 0)
        {
            AudioManager.Play("card1", "card2");
        }

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
        // Play sound
        if (turn < 0)
        {
            AudioManager.Play("card1", "card2");
        }

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

    public void ChangeMaxBet()
    {
        // Play sound
        AudioManager.Play("UI1");

        maxBet += 2;
        if (maxBet == 8)
        {
            maxBet = 2;
        }
    }

    public void ChangeBoardCount()
    {
        // Play sound
        AudioManager.Play("UI1");

        if (boardsIncrease && boardsEnabled < 6)
        {
            boards[boardsEnabled].gameObject.SetActive(true);
        }
        else if (boardsEnabled == 6)
        {
            boards[boardsEnabled - 1].gameObject.SetActive(false);
            boardsIncrease = false;
        }
        else if (!boardsIncrease && boardsEnabled > 4)
        {
            boards[boardsEnabled - 1].gameObject.SetActive(false);
        }
        else if (boardsEnabled == 4)
        {
            boards[4].gameObject.SetActive(true);
            boardsIncrease = true;
        }
    }

    public void ChangeMode()
    {
        // Play sound
        AudioManager.Play("UI1");
        
        betScore = !betScore;
    }

    public void ChangeLanguage()
    {
        // Play sound
        AudioManager.Play("UI1");

        // Update text
        languageIndex = (languageIndex == 3) ? 0 : languageIndex + 1;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = Language.language[10, languageIndex];
        for (int i = 0; i < settingsLabels.Length; i++)
        {
            settingsLabels[i].text = Language.language[18 + i, languageIndex];
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.0f);
        endGameWindow.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public List<PlayerEntry> GetTopPlayers()
    {
        // Sort the players by score in descending order and return them
        return players
            .OrderByDescending(player => player.score)
            .ToList();
    }

    public void ToggleSettingsWindow()
    {
        // Play sound
        AudioManager.Play("UI1");

        settingsWindow.SetActive(!settingsWindow.activeInHierarchy);
        Time.timeScale = (settingsWindow.activeInHierarchy) ? 0 : 1;

        // Save settings
        PlayerPrefs.SetString("Name", nameInput.text);
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void UIButton()
    {
        // Play sound
        AudioManager.Play("UI1");
    }
}
