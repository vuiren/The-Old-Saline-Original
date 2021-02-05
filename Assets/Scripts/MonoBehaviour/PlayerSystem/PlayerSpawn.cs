using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start() 
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player is null)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = transform.position;
        }
        else
        {
            player.transform.position = transform.position;
        }
    }
}
