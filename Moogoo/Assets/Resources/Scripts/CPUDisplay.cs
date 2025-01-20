using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

[ExecuteAlways]
public class CPUDisplay : MonoBehaviour
{
    public GameManager gameManager;

    // CPU Display
    public Image cpuImageDisplay;
    public TextMeshProUGUI cpuNameDisplay;
    public Image cpuColourDisplay;

    void Update()
    {
        cpuImageDisplay.gameObject.SetActive(gameManager.turn > 0);
        cpuNameDisplay.gameObject.SetActive(gameManager.turn > 0);
        cpuColourDisplay.gameObject.SetActive(gameManager.turn > 0);

        if (gameManager.turn > 0)
        {
            //cpuImageDisplay.sprite = gameManager.cpuIcons[gameManager.players[gameManager.turn].iconIndex];
            cpuImageDisplay.sprite = gameManager.cpuIcons[System.Array.IndexOf(gameManager.cpuNames, gameManager.players[gameManager.turn].name)];
            cpuNameDisplay.text = gameManager.players[gameManager.turn].name;
            //cpuColourDisplay.color = gameManager.colours[gameManager.turn];
            cpuColourDisplay.color = gameManager.colours[gameManager.players[gameManager.turn].iconIndex];
            cpuNameDisplay.color = (cpuColourDisplay.color == gameManager.colours[2] || cpuColourDisplay.color == gameManager.colours[9]) ? Color.black : Color.white;
        }
    }
}
