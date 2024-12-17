using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Respawn : MonoBehaviour
{
	PlayerMovement playerMovement;
	Vector2 checkpointPos;
	Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		playerMovement = GetComponent<PlayerMovement>();
	}

	private void Start()
	{
		checkpointPos = transform.position;
	}
	private void Update()
	{
		if(playerMovement.stuck == true)
		{
			Die();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Spike"))
		{
			Die();
		}
	}


	public void UpdateCheckpoint(Vector2 pos)
	{
		checkpointPos = pos;
	}
	void Die()
	{
		StartCoroutine(respawn(0.85f));
	}


IEnumerator respawn(float duration)
	{
		playerMovement.dead = true;
		body.simulated = false;
		body.velocity = Vector3.zero;
		//transform.localScale = new Vector3(0,0,0);
		yield return new WaitForSeconds(duration);
		transform.position = checkpointPos;
		//transform.localScale = Vector3.one;
		body.simulated = true;
		playerMovement.dead = false;
		playerMovement.stuck = false;
	}


}
