using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDashArea : MonoBehaviour
{
	public GameObject player;
	PlayerMovement playerMovement;


	private void Start()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			print("Çýktý");
			playerMovement.isInNoDashArea = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			playerMovement.isInNoDashArea = false;
		}
	}
}
