using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform PosA, PosB;
    [SerializeField] int speed;
    Vector3 targetPos;

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
		transform.position = PosA.position;
        targetPos = PosB.position;
		DirectionCalculate();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position,PosA.position) < 0.1f)
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
		rb.velocity = moveDirection * speed;
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
        }
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerMovement.isOnPlatform = false;
		}
	}
}
