using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public string playerName;
    public Color colour;
    public int score;

    void Update()
    {
        // Change name of player object
        gameObject.name = (playerName != "") ? $"Player: {playerName}" : "Player";
    }
}
