using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
{

	PlayerMovement playerMovement;
	[SerializeField] GameObject player;
	[SerializeField] Rigidbody2D body;
	[SerializeField] GameObject portalToTeleport;
	Transform transformOfPortal;
	[SerializeField]Transform portalToTeleportArrow;
	Vector2 lookingDir;
	float playerSpeed;


	private Animator anim;

	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		body = player.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		transformOfPortal = portalToTeleport.GetComponent<Transform>();
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
			lookingDir = new Vector2(portalToTeleportArrow.position.x - transformOfPortal.position.x, portalToTeleportArrow.position.y - transformOfPortal.position.y)*(5);
			body.position = new Vector2(portalToTeleport.transform.position.x+lookingDir.x, portalToTeleport.transform.position.y+lookingDir.y);

			playerSpeed = Mathf.Sqrt(playerMovement.body.velocity.x * playerMovement.body.velocity.x + playerMovement.body.velocity.y * playerMovement.body.velocity.y);

			//playerMovement.body.velocity = new Vector2(playerSpeed*lookingDir.x, playerSpeed*lookingDir.y);
			//playerMovement.body.velocity = playerSpeed * lookingDir.normalized;
			print(lookingDir.normalized.x + "  " + lookingDir.normalized.y);

			playerMovement.body.velocity = Vector2.zero;
			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(lookingDir * playerSpeed, ForceMode2D.Impulse);
		}

	}

	private void Update()
	{
		//print(playerMovement.body.velocity.x +"  "+playerMovement.body.velocity.y);
	}
}
