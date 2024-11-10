using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

	[SerializeField] float bounce = 40f;
	/*
	SADE_NOTE: direction deðiþkeni enum olarak tanýmlanýrsa kod içerisinde doðrudan direction.Left þeklinde yazýlabilir bu da kodu daha okunaklý yapar
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

	[SerializeField] int direction;//aldýðý deðere göre sýrasýyla yukarý aþaðý sað sol 1 2 3 4
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;
	[SerializeField] Rigidbody2D body;
	private Animator anim;
	public bool active;
	IEnumerator bounceCoroutine;


	private void Awake()
	{		 
		playerMovement = player.GetComponent<PlayerMovement>();
		body = player.GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		bounceCoroutine = Bounce();
	}

	private void Update()
	{
		if (playerMovement.bouncing == true) print("Zýplýyor");

		anim.SetBool("active", active);
	}
	private void OnCollisionStay2D(Collision2D collision)//enter
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
			bounceCoroutine = Bounce();
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
					StartCoroutine(bounceCoroutine);
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,0) * bounce, ForceMode2D.Impulse);
					break;

				case 4:
					StartCoroutine(bounceCoroutine);
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * bounce, ForceMode2D.Impulse);
					break;
				case 5://sað üst
					StartCoroutine(bounceCoroutine);
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,1) * bounce, ForceMode2D.Impulse);
					break;
				case 6:
					StartCoroutine(bounceCoroutine);
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * bounce, ForceMode2D.Impulse);
					break;
				case 7:
					StartCoroutine(bounceCoroutine);
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0.3f) * bounce, ForceMode2D.Impulse);
					break;
			}
								
		}
	}
	IEnumerator Bounce()
	{
		print("Coroutine started" + gameObject.name);
		playerMovement.isGroundedControl = false;
		playerMovement.bouncing = true;
		active = true;
		yield return new WaitForSecondsRealtime(0.1f);
		playerMovement.isGroundedControl = true;
		yield return new WaitForSecondsRealtime(0.9f);
		active = false;
		yield return new WaitForSeconds(1f);
		playerMovement.bouncing = false;
	}

}
