using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	Respawn respawn;

	public static Checkpoint activePoint;
	[SerializeField] private Sprite redCheckpoint;
	[SerializeField] private Sprite greenCheckpoint;
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<Respawn>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			respawn.UpdateCheckpoint(transform.position);

			if(activePoint != null)
			{
				activePoint.SetToRed();
			}

			activePoint = this;
			SetToGreen();
		}
	}

	void SetToGreen()
	{
		spriteRenderer.sprite = greenCheckpoint;
	}

	void SetToRed()
	{
		spriteRenderer.sprite = redCheckpoint;
	}
}