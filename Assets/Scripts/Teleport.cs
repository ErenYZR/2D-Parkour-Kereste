using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField] GameObject player;
    [SerializeField] Transform teleportPoint;


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
        {
            player.transform.position = teleportPoint.position;
        }
	}
}
