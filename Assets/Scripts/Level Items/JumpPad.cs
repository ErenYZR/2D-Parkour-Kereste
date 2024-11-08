using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

	[SerializeField] float bounce = 40f;
	/*
	SADE_NOTE: direction de�i�keni enum olarak tan�mlan�rsa kod i�erisinde do�rudan direction.Left �eklinde yaz�labilir bu da kodu daha okunakl� yapar
	enum Direction
	{
		Up = 1,
		Down = 2,
		Right = 3,
		Left = 4
	}
	int direction = 4; yerine
	Direction direction = direction.Left;
	*/

	[SerializeField] int direction;//ald��� de�ere g�re s�ras�yla yukar� a�a�� sa� sol 1 2 3 4
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;
	[SerializeField] Rigidbody2D body;
	private Animator anim;
	public bool active;


	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		body = player.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if (playerMovement.bouncing == true) print("Z�pl�yor");

		anim.SetBool("active", active);
	}
	private void OnCollisionStay2D(Collision2D collision)//enter
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			StartCoroutine(nameof(Bounce));

			switch (direction)
			{
				case 1:
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
					break;

				case 2:
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down * bounce, ForceMode2D.Impulse);
					break;

				case 3:
					body.velocity = Vector2.zero;
					//body.velocity = Vector2.right* 20;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,0) * bounce, ForceMode2D.Impulse);
					break;

				case 4:
					//body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * bounce, ForceMode2D.Impulse);
					break;
				case 5://sa� �st
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,1) * bounce, ForceMode2D.Impulse);
					break;
				case 6:
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * bounce, ForceMode2D.Impulse);
					break;
			}
								
		}
	}
	IEnumerator Bounce()
	{
		playerMovement.bouncing = true;
		active = true;
		yield return new WaitForSecondsRealtime(1f);
		active = false;
		yield return new WaitForSeconds(1f);
		playerMovement.bouncing = false;


	}

}
