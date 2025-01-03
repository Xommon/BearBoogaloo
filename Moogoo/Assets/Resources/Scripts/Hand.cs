using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Hand : MonoBehaviour
{
    public PlayerEntry mainPlayer;
    public GameManager gameManager;
    public Card[] cards;
    public bool canPlayCard;

    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Show the cards that are in the hand
        for (int i = 0; i < mainPlayer.hand.Count; i++)
        {
            if (mainPlayer.hand.Count > i)
            {
                cards[i].gameObject.SetActive(true);
                string[] cardContents = mainPlayer.hand[i].Split(":");
                cards[i].colour = int.Parse(cardContents[0]);
                cards[i].value = int.Parse(cardContents[1]);
            }
            else
            {
                //cards[i].GetComponent<Button>().interactable = false;
                cards[i].gameObject.SetActive(false);
            }
        }
    }
}
