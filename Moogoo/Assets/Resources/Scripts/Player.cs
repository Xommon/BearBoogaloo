using UnityEngine;
using Unity.Netcode;

[ExecuteAlways]
public class Player : NetworkBehaviour
{
    public string name;
    public Color colour;
    public int points;

    void Update()
    {
        gameObject.name = name;

        if (!IsOwner)
        {
            return;
        }
    }

    // Start
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
    }
}
