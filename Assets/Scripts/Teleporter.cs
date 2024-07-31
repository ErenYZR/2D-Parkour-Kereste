using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;
	[SerializeField] Rigidbody2D body;
	[SerializeField] Transform destination;

	private Animator anim;

	private void Awake()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
		body = player.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}




	private void OnTriggerEnter2D(Collider2D collision)
	{
		anim.SetBool("Active", true);
	}


	private void OnTriggerExit2D(Collider2D collision)
	{
		anim.SetBool("Active", false);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			body.position = destination.position;

		}

	}


}
