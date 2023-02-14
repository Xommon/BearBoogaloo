using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NameEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public Image image;
    public GameManager gameManager;
    public int playerIndex;
    public PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.myEntry = view;
        transform.parent = gameManager.playerListObject;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        NameEntry[] allEntries = transform.parent.GetComponentsInChildren<NameEntry>();
        for (int i = 0; i < allEntries.Length; i++)
        {
            if (allEntries[i] == this)
            {
                playerIndex = i;
            }
        }
        image.color = gameManager.colours[playerIndex];

        if (view.IsMine)
        {
            nameText.text = gameManager.playerNameInputField.text;
            gameManager.playerCreationPanel.SetActive(false);
        }
    }
}
