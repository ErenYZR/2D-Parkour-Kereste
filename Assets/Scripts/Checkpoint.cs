using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	Respawn respawn;
	private void Awake()
	{
		respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<Respawn>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			respawn.UpdateCheckpoint(transform.position);
		}
	}
}