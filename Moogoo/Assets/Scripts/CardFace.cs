using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardFace : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI valueText;
    public GameManager gameManager;
    public Sprite[] spriteBank;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
    }

    public void AttachCard(Card card)
    {
        Debug.Log(card.value);
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = spriteBank[card.boardIndex];
        valueText.text = card.value.ToString();
    }
}
