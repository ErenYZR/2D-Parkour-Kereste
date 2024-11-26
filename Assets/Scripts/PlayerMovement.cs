using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMovement : MonoBehaviour
{
	public bool bouncing;
	[SerializeField] JumpPad bounce;
    [SerializeField] JumpPad active;
    [SerializeField] JumpPad direction;
    public JumpPad jumpPad;
    public bool isGroundedControl;

    public LayerMask groundLayer;
	public LayerMask wallLayer;

	public Rigidbody2D body;
	private Animator anim;
	private BoxCollider2D boxCollider;
	private TrailRenderer trailRenderer;

    [SerializeField] private BoxCollider2D climb;
    [SerializeField] private float maxSpeed;
	[SerializeField] private float maxSpeedWallJump;
	[SerializeField] private float acceleration;
	[SerializeField] private float accelerationWallJump;

	[SerializeField] private float h�z;
	[SerializeField] private float dikeyH�z;
	public float jumpPower;
    private float horizontalInput;
    private float verticalInput;
    private float airJumpCounter;
    private float groundJumpCounter;
    private float isGroundedCooldowntimer;
    private float timer;
    [SerializeField] float climbSpeed = 3f;
    private bool doubleJump;

    private float coyoteTimeCounter;
    private float coyoteTime = 0.1f ;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
 

    //dash variables
    //[SerializeField] private float dashingVelocity;
    //[SerializeField] private float dashingTime;
    private float dashingCooldown;
	//private Vector2 dashingDir;
	private Vector2 dashDirection;
	private bool isDashing = false;
    private bool canDashCondition;
	[SerializeField] private float dashDistance = 5f;
	[SerializeField] private float dashDuration = 0.2f;


	//wall jump
	private bool isWallJumping;
    [SerializeField] Vector2 wallJumpPower;
    private float wallJumpingTime = 1f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.1f;
    private Vector2 wallJumpingPower = new Vector2(2f, 3f);
    private float wallJumpExpecter;
	private float wallJumpExpecterTime =0.05f;
	IEnumerator wallJumpBounceCoroutine;
    public float wallJumpSlownessCounter = 0f;

	//hanging
	private float hangingTime = 0.2f;
    private float hangingTimeCounter;

    private float airTime =0.54f;
    //death
    public bool dead;
    public bool stuck;

    //platform
    public bool isOnPlatform;
    public Rigidbody2D platformRb;

	public CoinManager coinManager;


	private void Awake()
	{
        //Rigidbody ve animat�r i�in referans al�yoruz.
		body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        //jumpPad = GetComponent<JumpPad>();
	}

	// Start is called before the first frame update
	void Start()
    {
		body.gravityScale = 5;
        canDashCondition = true;
        bouncing = false;
        wallJumpBounceCoroutine = WallJumpBounce();
	}

    // Update is called once per frame
    private void Update()
    {
        if (bouncing) print("Bouncing");
        if (isOnPlatform) print("Platform");
        if (wallJumpingCounter == 0.01f) print("WallJump Bitti");
        if (isClimbing()) print("T�rman�yor");
        if (onWall()) print("Duvarda");
        if (isGrounded()) print("Yerde"); //print(airTime);

        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //animat�r parametrelerini ayarlama k�sm�
        anim.SetBool("run", horizontalInput != 0 && isGrounded());
		anim.SetBool("grounded", isGrounded());
		anim.SetBool("jump", !isGrounded() && !isClimbing());
		anim.SetBool("dash", whileDashing() && !dead);
        anim.SetBool("climb", isClimbing() && verticalInput != 0 && !dead);
        anim.SetBool("hang", isClimbing() && verticalInput == 0 && !dead);
        anim.SetBool("dead", dead);


		//Oyuncunun sa�a sola d�nme animasyonu
		if (body.velocity.x > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (body.velocity.x < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

		acceleration = maxSpeed-Mathf.Abs(body.velocity.x);
        accelerationWallJump = maxSpeedWallJump-Mathf.Abs(body.velocity.x);
		h�z = body.velocity.x;
        dikeyH�z = body.velocity.y;
        
		if (horizontalInput == 0 && !isWallJumping && !isDashing && !bouncing && !isClimbing())//elini �ekince durmay� sa�l�yorrrrr
		{
            if (!isOnPlatform)
            {
				body.velocity = new Vector2(body.velocity.x / 100, body.velocity.y);//body.velocity.x 0 yap�labilir. /100d�
				if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
			}
            else if (isOnPlatform)
            {
                body.velocity = new Vector2(platformRb.velocity.x, body.velocity.y);
            }

		}


		if (Mathf.Abs(body.velocity.x) < maxSpeed && !isWallJumping && ((horizontalInput * body.velocity.x > 0) || (horizontalInput * body.velocity.x == 0 && horizontalInput != 0)) && !bouncing && !isClimbing() && !isOnPlatform && !isDashing)
        {
            body.velocity = new Vector2(body.velocity.x + (acceleration * horizontalInput) / 4, body.velocity.y);
        }
        else if (horizontalInput * body.velocity.x < 0 && !isWallJumping && !bouncing && !isClimbing() && !isOnPlatform && !isDashing)
        {
            body.velocity = new Vector2(body.velocity.x - (body.velocity.x / 2), body.velocity.y);
            if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
        }
        else if (bouncing && /*!isWallJumping &&*/ !isClimbing() && !isOnPlatform && body.velocity.x * horizontalInput >= 0 && !isDashing)//jump pad hareketi
        {
            if (MathF.Abs(body.velocity.x) < 6)
            {
				body.velocity = new Vector2(body.velocity.x + (horizontalInput * 3/2), body.velocity.y);
                print("B");
			}
		}
		else if (bouncing && !isWallJumping && !isClimbing() && !isOnPlatform && body.velocity.x * horizontalInput < 0)//jump pad hareketi
		{
            body.velocity = new Vector2(horizontalInput * 3/2, body.velocity.y);       
		}
		else if (isOnPlatform && Mathf.Abs(body.velocity.x) < maxSpeed && !isWallJumping && !bouncing && !isClimbing() && !isDashing)
		{
            body.velocity = new Vector2((horizontalInput * 10) + platformRb.velocity.x,body.velocity.y);
        }


		
		if (body.velocity.y > 17)//h�z limiti
        {
            body.velocity = new Vector2 (body.velocity.x, body.velocity.y-1);
        }
        else if(body.velocity.y < -17)
        {
			body.velocity = new Vector2(body.velocity.x, body.velocity.y + 1);
		}

		if (body.velocity.x > 14)
		{
			body.velocity = new Vector2(body.velocity.x - 1, body.velocity.y);
		}
		else if (body.velocity.x < -14)
		{
			body.velocity = new Vector2(body.velocity.x + 1, body.velocity.y);
		}


        if (isGroundedControl) print("Is grounded control true");
        else print("Is grounded control false");

	

		if (isGrounded()) //yere d���nce jumpcount s�f�rlama ve coyote time
            { 
            airJumpCounter = 1;
            groundJumpCounter = 1;
            coyoteTimeCounter = coyoteTime;
            canDashCondition = true;
            print("yerde");
			if (isGroundedControl) bouncing = false;
		}
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (onWall())
        {
			if (isGroundedControl) bouncing = false;
		}

        if (bouncing)
        {
            wallJumpSlownessCounter += Time.deltaTime;
        }

        //dash
        /*  if (Input.GetKeyDown(KeyCode.Mouse1) && canDashCondition && dashingCooldown >0.5f)
              {
              StartCoroutine(Dash());

              StartCoroutine(StopDashing());
              }*/
        dashingCooldown += Time.deltaTime;

		if (!isDashing)//dash atarken hareket y�n�n� kitler
		{
			float horizontalInput = Input.GetAxisRaw("Horizontal");
			float verticalInput = Input.GetAxisRaw("Vertical");
			// Hareket y�n�n� hesapla
			Vector2 inputDirection = new Vector2(horizontalInput, verticalInput).normalized;

			// Sa� t�k ile Dash ba�lat
			if (inputDirection != Vector2.zero && Input.GetMouseButtonDown(1) && canDashCondition && dashingCooldown > 0.5f)
			{
				dashDirection = inputDirection; // Dash y�n�n� sabitle
				StartCoroutine(Dash());
			}
		}


		if (onWall() && !isGrounded())
		{
			wallJumpExpecter = wallJumpExpecterTime;
			print("Expect");
		}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else jumpBufferCounter -= Time.deltaTime;

		wallJumpExpecter -= Time.deltaTime;



		Jump();
		airTime -= Time.deltaTime;
		Climb();
		WallJump();
	}


    private void Jump()
    {

		if (jumpBufferCounter > 0 && !isClimbing() && coyoteTimeCounter > 0 && groundJumpCounter > 0 && !isWallJumping)
        {
            airTime = 0.54f;
            jumpBufferCounter = 0;
			body.velocity = new Vector2(body.velocity.x, jumpPower);
			if (isGroundedControl) bouncing = false;
			groundJumpCounter--;
		}

		if (Input.GetKeyUp(KeyCode.Space) && airTime > 0)//yerde z�plad�ysa havada z�plamad�ysa ve yukar� do�ru gidiyorsa elini spaceden �ekince h�z� yar�ya d��ecek
		{
			body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);
		}

		if (Input.GetKeyDown(KeyCode.Space) && !isClimbing() && coyoteTimeCounter <=0 && !isGrounded() && airJumpCounter > 0 && !isWallJumping)
        {
			body.velocity = new Vector2(body.velocity.x, jumpPower);
            airJumpCounter--;
		}
	}

   /* private void Dash()
    {   
            bouncing = false;
            dashingCooldown = 0;
            isDashing = true;
            canDash = false;
            canDashCondition = false;
            trailRenderer.emitting = true;
            anim.SetBool("dash",true);
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            body.gravityScale = 0;
			if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
                
            }
            else{
                body.velocity = dashingDir.normalized*dashingVelocity;
            }       
    }*/

	public IEnumerator Dash()
	{
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
		dashingCooldown = 0;
		bouncing = false;
		isDashing = true;
		canDashCondition = false;
		trailRenderer.emitting = true;
		anim.SetBool("dash", true);

		// Dash h�z�n� hesapla
		Vector2 dashVelocity = dashDirection * (dashDistance / dashDuration);
		body.velocity = dashVelocity;    // Dash s�ras�nda h�z�n� ayarla

		yield return new WaitForSeconds(dashDuration);

		trailRenderer.emitting = false;
		isDashing = false;
		anim.SetBool("dash", false);
		body.gravityScale = 5;
	}

    private void Climb()
    {

        if (isClimbing() && !bouncing)
        {
            if (Input.GetKey(KeyCode.Mouse0)) //  if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (isGroundedControl) bouncing = false;
				body.velocity = new Vector2(0, 0);
				body.gravityScale = 0;

                if (!isOnPlatform) body.velocity = new Vector2(0, climbSpeed * verticalInput);//body.velocity = new Vector2(body.velocity.x, climbSpeed * verticalInput);
                else if(isOnPlatform) body.velocity = new Vector2(platformRb.velocity.x, platformRb.velocity.y+climbSpeed * verticalInput);
			}

		}
        else if (!isClimbing())
        {
            body.gravityScale = 5;
        }
    }



    private void WallJump()
    {
        if ((/*wallJumpExpecter > 0 ||*/ isClimbing()) && Input.GetKeyDown(KeyCode.Space) && !isGrounded())
        {
			//bouncing = false;
			//isWallJumping = true;
			StopCoroutine(WallJumpBounce());
			wallJumpBounceCoroutine = WallJumpBounce();
			StartCoroutine(wallJumpBounceCoroutine);
			//wallJumpingCounter = wallJumpingDuration;
            //wallJumpExpecter = 0;
			if (bouncing) print("iswalljumping:true");
            else print("iswalljumping:false");
        }

       /* if (isWallJumping) 
        {
			body.AddForce(new Vector2(-5, 3f), ForceMode2D.Impulse);
			//body.velocity = new Vector2(-transform.localScale.x * wallJumpingPower.x * 7, wallJumpingPower.y * 3.5f);
			bouncing = true;
            if(!bouncing) isWallJumping=false;
			//transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
			//wallJumpingCounter -= Time.deltaTime;
            //if(wallJumpingCounter < 0) isWallJumping = false;

		}*/
    }


	IEnumerator WallJumpBounce()
	{
        wallJumpSlownessCounter = 0;
		bouncing = true;//duvara �arpt���nda yere d��t���nde dash att���nda bouncing false olur.
		body.AddForce(new Vector2(-wallJumpPower.x * transform.localScale.x, wallJumpPower.y), ForceMode2D.Impulse);
		print("Walljumpbounce coroutine started");
		isGroundedControl = false;		
		yield return new WaitForSecondsRealtime(0.2f);
		isGroundedControl = true;
		yield return new WaitForSeconds(1.9f);
       // bouncing = false;
	}



    //duvara t�rmanma
   /* private void Climb()
    {
        if (isClimbing() && !(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f) && !isWallJumping)
        {
			body.velocity = new Vector2(0, 0); //b�yleydi   body.velocity = new Vector2(body.velocity.x, 0);
			body.gravityScale = 0;
			body.velocity = new Vector2(body.velocity.x, verticalInput * climbSpeed);			
		}		    
        else if (isClimbing() && (Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f))
        {
            body.gravityScale = 5;
			isWallJumping = true;
			 //  body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 10, wallJumpingPower.y * 10);			
			   //body.AddForce(new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y)*10, ForceMode2D.Impulse);
			  // print("Wall Jumping Direction:" + wallJumpingDirection * wallJumpingPower.x + "," + wallJumpingPower.y);
              //new WaitForSeconds(wallJumpingDuration);
			   wallJumpingCounter = 0f;
			   isWallJumping = false;
		}
        else
        {
            body.gravityScale = 5;
        }
	}*/


    //duvarda z�plama
   /* private void WallJump()
    {
        if (isClimbing() && !(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f) && !isWallJumping)
		{
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

           // CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (isWallJumping)
        {
			body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 10, wallJumpingPower.y * 10);
			new WaitForSeconds(wallJumpingDuration);
			wallJumpingCounter = 0f;
			isWallJumping = false;
		}


       /* if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 2, wallJumpingPower.y*3);
            wallJumpingCounter = 0f;


            print("Duvarda z�plad�");
        }

      //  Invoke(nameof(StopWallJumping), wallJumpingDuration);

    }*/


    //yerde mi
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size*0.7f,0,Vector2.down,0.5f,groundLayer);

      
        return raycastHit.collider != null;
    }

    //duvarda m�
	private bool onWall()
	{
		RaycastHit2D raycastHit = Physics2D.BoxCast(climb.bounds.center, boxCollider.bounds.size*0.7f, 0, new Vector2(transform.localScale.x,0), 0.2f, groundLayer);//wall layer
		return raycastHit.collider != null;

	}

	private void OnDrawGizmos()
	{
        Gizmos.DrawCube(climb.bounds.center, boxCollider.bounds.size * 0.5f);   //duvar kontrol
	    Gizmos.DrawCube(climb.bounds.center + new Vector3(transform.localScale.x, 0, 0) * 0.5f, boxCollider.bounds.size * 0.5f);

       // Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size * 0.5f);  //zemin kontrol
	   //Gizmos.DrawCube(boxCollider.bounds.center + new Vector3.down*0.5f, boxCollider.bounds.size * 0.5f);
	}

    public bool whileDashing()
    {
        return isDashing;
    }

    public bool isClimbing()
    {
        return (Input.GetKey(KeyCode.Mouse0) && onWall() && !isWallJumping);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Coin"))
		{
            Destroy(collision.gameObject);
			coinManager.coinCount++;

		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Stuck"))
        {
            stuck = true;
        }
	}
}
