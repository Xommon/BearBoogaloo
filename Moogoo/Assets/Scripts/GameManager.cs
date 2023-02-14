using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Color[] colours;
    public List<Card> deck;
    public List<Card> discardPile;
    public bool gameStarted;
    public List<GameObject> playList = new List<GameObject>();

    // Player creation
    public GameObject playerEntryPrefab;
    public Transform playerListObject;
    public GameObject playerCreationPanel;
    public TMP_InputField playerNameInputField;
    public Button playerNameOkButton;
    public PhotonView myEntry;

    private void Awake()
    {
        
    }

    private void Start()
    {
        playerCreationPanel.SetActive(true);
    }

    private void Update()
    {
        // Disable/enable name button
        if (playerNameInputField.text == "")
        {
            playerNameOkButton.interactable = false;
        }
        else
        {
            playerNameOkButton.interactable = true;
        }
    }

    public void CreateDeck()
    {
        deck.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int y = 1; y < 10; y++)
            {
                Card newCard = new Card();
                newCard.boardIndex = i;
                newCard.value = y;
                deck.Add(newCard);
            }
        }

        ShuffleDeck(deck);
    }

    void ShuffleDeck<T>(List<T> inputList)
    {
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    public void CreatePlayer()
    {
        if (!gameStarted || playerListObject.GetComponentsInChildren<NameEntry>().Length < 6)
        {
            PhotonNetwork.Instantiate(playerEntryPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
}