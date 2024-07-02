using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
	Rigidbody2D rb2;
	Vector2 input;
	float verticalSpeed;
	float horizontalSpeed;


	[Header("MOVEMENT SPEED")]
	[SerializeField] float maxSpeed = 5f;

	[Range(0.02f, 2f)]
	[SerializeField] float speedUpDuration = 0.1f;//maks hiza ulasma suresi

	[Range(0.02f, 2f)]
	[SerializeField] float speedDownDuration = 0.1f;// ters yone donme suresi

	[Range(0.02f, 2f)]
	[SerializeField] float stopDuration = 0.1f;//durma suresi

	[Header("JUMP VELOCITY")]
	[SerializeField] KeyCode jumpButton = KeyCode.Space;
	[SerializeField] float jumpUpSpeed;//yukari yone ziplarken yer cekimi ivmesi
	[SerializeField] float jumpDownSpeed;//asagi duserken yer cekimi ivmesi 

	[SerializeField] float jumpHeight;
	float jumpVelocity;

	float gravity = 9.8f;

	[Header("GROUND CHECK")]
	[SerializeField] float rayDistance;
	[SerializeField] float rayRadius;
	[SerializeField] LayerMask groundLayer;
	bool isGrounded;

	[SerializeField] float coyoteTime;//oyuncu ayagi yerden kesilince ziplama ek suresi
	float _coyoteTimer;

	[SerializeField] float jumpBufferTime;//oyuncu yerde degilken erkenden ziplama tusuna basma suresi
	float _jumpBufferTimer;

	[SerializeField] float variableJumpTime;//ziplama tusunu erken birakinca hiz kesme icin sure
	float _variableJumpTimer;

	[Range(0f, 1f)]
	[SerializeField] float jumpReleaseEffect;//ziplama tusunu birakinca hiz kesme miktari

	bool isJumped;
	private void Awake()
	{
		rb2 = GetComponent<Rigidbody2D>();
	}

	void GetInput()//oyuncu inputu almak icin surekli calisir
	{
		input.x = Input.GetAxisRaw("Horizontal");
		if (Input.GetKeyDown(jumpButton))
		{
			_jumpBufferTimer = jumpBufferTime;
		}
		else if (Input.GetKeyUp(jumpButton) && _variableJumpTimer >= 0f)
		{
			verticalSpeed *= jumpReleaseEffect;
		}
	}

	void Jump()
	{
		if (_coyoteTimer >= 0f && _jumpBufferTimer >= 0f && !isJumped)
		{
			_variableJumpTimer = variableJumpTime;
			isJumped = true;
			jumpVelocity = Mathf.Sqrt(2 * jumpUpSpeed * jumpHeight * gravity);//zipmala yuksekligine ve yercekimi ivmesine gore ziplama hizi hesaplama
			verticalSpeed = jumpVelocity;
		}
	}

	void CalculateCoyote()
	{
		if (!isGrounded)
		{
			_coyoteTimer -= Time.deltaTime;
		}
		else
		{
			isJumped = false;
		}
	}

	void CalculateJumpBuffer()
	{
		_jumpBufferTimer -= Time.deltaTime;
	}

	void CalculateVariableJump()
	{
		_variableJumpTimer -= Time.deltaTime;
	}

	private void OnDrawGizmos()//ekrana bir seyler cizmek icin 
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position - transform.up * rayDistance, rayRadius);
	}

	void CalculateVelocity()
	{
		if (input.x != 0)//herhangi bir yon tusuna basilinca hareket icin 
		{
			if (input.x * horizontalSpeed >= 0)//zaten gittigi yonde hareket ederken
			{
				horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, input.x * maxSpeed, maxSpeed * Time.fixedDeltaTime / speedUpDuration);
			}
			else//gittigi yonun tersine gitmeye calisirken
			{
				horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, input.x * maxSpeed, maxSpeed * Time.fixedDeltaTime / speedDownDuration);
			}
		}
		else//durdugu zaman
		{
			horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0f, maxSpeed * Time.fixedDeltaTime / stopDuration);
		}

		if (!isGrounded)
		{
			if (verticalSpeed >= 0f)//yukari yonde giderken uygulanan yer cekimi ivmesi
			{
				verticalSpeed -= jumpUpSpeed * Time.fixedDeltaTime * gravity;
			}
			else//asagi yonde giderken uygulanan yer cekimi ivmesi
			{
				verticalSpeed -= jumpDownSpeed * Time.fixedDeltaTime * gravity;
			}
		}

	}

	void GrouncCheck()
	{
		RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, rayRadius, -transform.up, rayDistance, groundLayer);

		if (hit2D)
		{
			if (!isGrounded)
			{
				isGrounded = true;
				verticalSpeed = 0;
				_coyoteTimer = coyoteTime;
			}
		}
		else
		{
			isGrounded = false;
		}

	}

	private void FixedUpdate()
	{
		CalculateVelocity();
		MovePlayer();
	}

	private void Update()
	{
		GetInput();
		GrouncCheck();
		CalculateCoyote();
		CalculateJumpBuffer();
		Jump();
		CalculateVariableJump();
	}

	void MovePlayer()
	{
		rb2.velocity = new Vector2(horizontalSpeed, verticalSpeed);
	}

}