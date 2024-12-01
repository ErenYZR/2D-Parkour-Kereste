using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

	[SerializeField] float bounce = 40f;

	public enum Direction
	{
		Up, Down, Left, Right, TopRight, TopLeft, TopSlightRight
	}

	[SerializeField] private Direction directionEnum;
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
			switch (directionEnum)
			{
				case Direction.Up:
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
					break;

				case Direction.Down:
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down * bounce, ForceMode2D.Impulse);
					break;

				case Direction.Right:
					body.velocity = Vector2.zero;
					StartCoroutine(bounceCoroutine);
					//collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,0) * bounce, ForceMode2D.Impulse);
					collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0) * bounce;
					break;

				case Direction.Left:
					StartCoroutine(bounceCoroutine);
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * bounce, ForceMode2D.Impulse);
					break;
				case Direction.TopRight://sað üst
					StartCoroutine(bounceCoroutine);
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,1) * bounce, ForceMode2D.Impulse);
					break;
				case Direction.TopLeft:
					StartCoroutine(bounceCoroutine);
					body.velocity = Vector2.zero;
					collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * bounce, ForceMode2D.Impulse);
					break;
				case Direction.TopSlightRight:
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
		playerMovement.wallJumpSlownessCounter = 0;
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
