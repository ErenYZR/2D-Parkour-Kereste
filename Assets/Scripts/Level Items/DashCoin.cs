using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCoin : MonoBehaviour
{
	[SerializeField] private float delay;
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;

	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
	}

    void Update()
    {
		if (playerMovement.dead == true)
		{
			transform.localScale = Vector3.one;
		}
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.transform.tag == "Player")
		{
			transform.localScale = Vector3.zero;
			playerMovement.canDashCondition = true;
			playerMovement.dashingCooldown = 1f;
		}
	}
}
