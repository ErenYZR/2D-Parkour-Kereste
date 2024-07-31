using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
	[SerializeField] Transform PosA, PosB;
	[SerializeField] int speed;
	Vector3 targetPos;
	private bool onPlatform;

	PlayerMovement playerMovement;
	Rigidbody2D rb;
	Vector3 moveDirection;
	[SerializeField] GameObject player;

	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		rb = GetComponent<Rigidbody2D>();
	}


	// Start is called before the first frame update
	void Start()
	{
		targetPos = PosB.position;
		DirectionCalculate();
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector2.Distance(transform.position, PosA.position) < 0.1f)
		{
			targetPos = PosB.position;
			DirectionCalculate();
		}

		if (Vector2.Distance(transform.position, PosB.position) < 0.1f)
		{
			targetPos = PosA.position;
			DirectionCalculate();
		}

	}

	private void FixedUpdate()
	{
		if (canStart())
		{
			StartCoroutine(Move());

			print("Can start");
		}

		if(Vector2.Distance(transform.position, PosB.position) < 0.1f)
		{
			rb.velocity = moveDirection * speed;
		}

		if (Vector2.Distance(transform.position, PosA.position) < 0.1f && !canStart())
		{
			rb.velocity = Vector2.zero;
		}
	}

	void DirectionCalculate()
	{
		moveDirection = (targetPos - transform.position).normalized;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerMovement.isOnPlatform = true;
			playerMovement.platformRb = rb;
			onPlatform = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerMovement.isOnPlatform = false;
			onPlatform = false;
		}
	}
	
IEnumerator Move()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		rb.velocity = moveDirection * speed;
	}

	public bool canStart()
	{
		return onPlatform && samePosition();
	}


	public bool samePosition()
	{
		//return rb.transform.position == PosA.transform.position;
		return (Vector2.Distance(transform.position, PosA.position) < 0.1f);
	}
}
