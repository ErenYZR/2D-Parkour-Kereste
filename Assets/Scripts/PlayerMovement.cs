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

    public LayerMask groundLayer;
	public LayerMask wallLayer;

	private Rigidbody2D body;
	private Animator anim;
	private BoxCollider2D boxCollider;
	private TrailRenderer trailRenderer;

    [SerializeField] private BoxCollider2D climb;
    [SerializeField] private float maxSpeed;
	[SerializeField] private float maxSpeedWallJump;
	[SerializeField] private float acceleration;
	[SerializeField] private float accelerationWallJump;

	[SerializeField] private float hýz;
	[SerializeField] private float dikeyHýz;
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
    [SerializeField] private float dashingVelocity;
    [SerializeField] private float dashingTime;
    private float dashingCooldown;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;
    private bool canDashCondition;


    //wall jump
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 1f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.1f;
    private Vector2 wallJumpingPower = new Vector2(2f, 3f);
    private float wallJumpExpecter;
	private float wallJumpExpecterTime =0.05f;
    private bool afterWallJumping;

	//hanging
	private float hangingTime = 0.2f;
    private float hangingTimeCounter;

    private float airTime =0.54f;
    //death
    public bool dead;

    //platform
    public bool isOnPlatform;
    public Rigidbody2D platformRb;

	public CoinManager coinManager;

	private void Awake()
	{
        //Rigidbody ve animatör için referans alýyoruz.
		body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
	}

	// Start is called before the first frame update
	void Start()
    {
		body.gravityScale = 5;
        canDashCondition = true;
	}

    // Update is called once per frame
    private void Update()
    {
        if (bouncing) print("Bouncing");
        if (isOnPlatform) print("Platform");
        if (wallJumpingCounter == 0.01f) print("WallJump Bitti");
        if (isClimbing()) print("Týrmanýyor");
        if (onWall()) print("Duvarda");
        if (isGrounded()) print("Yerde"); //print(airTime);

        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //animatör parametrelerini ayarlama kýsmý
        anim.SetBool("run", horizontalInput != 0 && isGrounded());
		anim.SetBool("grounded", isGrounded());
		anim.SetBool("jump", !isGrounded() && !isClimbing());
		anim.SetBool("dash", whileDashing() && !dead);
        anim.SetBool("climb", isClimbing() && verticalInput != 0 && !dead);
        anim.SetBool("hang", isClimbing() && verticalInput == 0 && !dead);
        anim.SetBool("dead", dead);


		//Oyuncunun saða sola dönme animasyonu
		if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

		acceleration = maxSpeed-Mathf.Abs(body.velocity.x);
        accelerationWallJump = maxSpeedWallJump-Mathf.Abs(body.velocity.x);
		hýz = body.velocity.x;
        dikeyHýz = body.velocity.y;
        
		if (horizontalInput == 0 && !isWallJumping && !isDashing && !bouncing && !isClimbing())//elini çekince durmayý saðlýyorrrrr
		{
            if (!isOnPlatform)
            {
				body.velocity = new Vector2(body.velocity.x / 100, body.velocity.y);//body.velocity.x 0 yapýlabilir.
				if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
			}
            else if (isOnPlatform)
            {
                body.velocity = new Vector2(platformRb.velocity.x, body.velocity.y);
            }

		}


		if (Mathf.Abs(body.velocity.x) < maxSpeed && !isWallJumping && ((horizontalInput * body.velocity.x > 0) || (horizontalInput * body.velocity.x == 0 && horizontalInput != 0)) && !bouncing && !isClimbing() && !isOnPlatform)
        {
            body.velocity = new Vector2(body.velocity.x + (acceleration * horizontalInput) / 4, body.velocity.y);
        }
        else if (horizontalInput * body.velocity.x < 0 && !isWallJumping && !bouncing && !isClimbing() && !isOnPlatform)
        {
            body.velocity = new Vector2(body.velocity.x - (body.velocity.x / 2), body.velocity.y);
            if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
        }
        else if (isDashing && !bouncing)//dash atarkenki hareket
        {
            body.velocity = new Vector2(0, 0);
            if (dashingDir.x == 0 && dashingDir.y > 0)
            {
                body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity * 3 / 4;
            }
            else if (dashingDir.x != 0 && dashingDir.y == 0)
            {
                body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity * 5 / 4;
            }
            else
            {
                body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity;
            }
        }
        else if (bouncing && !isWallJumping && !isClimbing() && !isOnPlatform && body.velocity.x * horizontalInput >= 0)//jump pad hareketi
        {
            if (MathF.Abs(body.velocity.x) < 6)
            {
				body.velocity = new Vector2(body.velocity.x + (horizontalInput * 3), body.velocity.y);
			}
		}
		else if (bouncing && !isWallJumping && !isClimbing() && !isOnPlatform && body.velocity.x * horizontalInput < 0)//jump pad hareketi
		{
				body.velocity = new Vector2(body.velocity.x + (horizontalInput * 2), body.velocity.y);
		}
		/* else if (bouncing && horizontalInput * body.velocity.x < 0 && !isWallJumping && !isClimbing() && !isOnPlatform)
		 {
			 //body.velocity = new Vector2(body.velocity.x - (body.velocity.x / 2), body.velocity.y);
			 //if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
		 }*/
		else if (isOnPlatform && Mathf.Abs(body.velocity.x) < maxSpeed && !isWallJumping && !bouncing && !isClimbing())
		{
            body.velocity = new Vector2((horizontalInput * 10) + platformRb.velocity.x,body.velocity.y);
        }






		

		if (body.velocity.y > 19)//hýz limiti
        {
            body.velocity = new Vector2 (body.velocity.x, body.velocity.y-1);
        }
        else if(body.velocity.y < -19)
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


		//dash atmazkenki hareket
		/*	 if (!isDashing && !onWall() && !isWallJumping)
			 {
				 body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); //sað sol hareket etme
			 }
			 else if(isDashing)//dash atarkenki hareket
			 {
				 body.velocity = new Vector2(0, 0);
				 if(dashingDir.x == 0 && dashingDir.y > 0)
				 {
					 body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity*3/4;
				 }
				 else if(dashingDir.x != 0 && dashingDir.y == 0)
				 {
					 body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity * 5/4;
				 }
				 else 
				 {
					 body.velocity = new Vector2(dashingDir.x, dashingDir.y).normalized * dashingVelocity;
				 }    

			 }
			 else if (isWallJumping)
			 {
				 new WaitForSeconds(1f);
				 body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

			 }*/

		if (isGrounded()) //yere düþünce jumpcount sýfýrlama ve coyote time
            {
            airJumpCounter = 1;
            groundJumpCounter = 1;
            coyoteTimeCounter = coyoteTime;
            bouncing = false;
             print("yerde");

			if (canDashCondition)  canDash = true;
           
            }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (onWall())
        {
            bouncing = false ;
        }

            //dash
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDash && canDashCondition && dashingCooldown >0.5f)
            {
                Dash();
 
                StartCoroutine(StopDashing());
            }
        dashingCooldown += Time.deltaTime;


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

    //zýplama kodu
  /*  private void Jump()
    {
		if (Input.GetKeyDown(KeyCode.Space) && !(onWall() && Input.GetKey(KeyCode.Mouse0))) 
        {
			if (jumpcount > 0)
			{
				body.velocity = new Vector2(body.velocity.x, jumpPower);
				print("Zýpladý");
				jumpcount--;
				print("Jump count:" + jumpcount);
			}
		}
		
    }*/


    private void Jump()
    {

		if (jumpBufferCounter > 0 && !isClimbing() && coyoteTimeCounter > 0 && groundJumpCounter > 0 && !isWallJumping)
        {
            airTime = 0.54f;
            jumpBufferCounter = 0;
			body.velocity = new Vector2(body.velocity.x, jumpPower);
            bouncing = false;
            groundJumpCounter--;
		}

		if (Input.GetKeyUp(KeyCode.Space) && airTime > 0)//yerde zýpladýysa havada zýplamadýysa ve yukarý doðru gidiyorsa elini spaceden çekince hýzý yarýya düþecek
		{
			body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);
		}

		if (Input.GetKeyDown(KeyCode.Space) && !isClimbing() && coyoteTimeCounter <=0 && !isGrounded() && airJumpCounter > 0 && !isWallJumping)
        {
			body.velocity = new Vector2(body.velocity.x, jumpPower);
            airJumpCounter--;
		}
	}

    private void Dash()
    {   
        bouncing = false ;
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
        
    }

    private IEnumerator StopDashing()
    {

		yield return new WaitForSeconds(dashingTime);
		trailRenderer.emitting = false;
		isDashing = false;
        canDashCondition=true;
        anim.SetBool("dash", false);  
        body.gravityScale = 5;

	}

    private void Climb()
    {

        if (isClimbing() && !isWallJumping)
        {
            if (Input.GetKey(KeyCode.Mouse0)) //  if (Input.GetKeyDown(KeyCode.Mouse0))
			{
                bouncing = false ;
				body.velocity = new Vector2(0, 0);
				body.gravityScale = 0;

                if (!isOnPlatform) body.velocity = new Vector2(0, climbSpeed * verticalInput);//body.velocity = new Vector2(body.velocity.x, climbSpeed * verticalInput);
                else if(isOnPlatform) body.velocity = new Vector2(platformRb.velocity.x, climbSpeed * verticalInput);
			}

		}
        else if (!isClimbing())
        {
            body.gravityScale = 5;
        }
    }



    private void WallJump()
    {
        if ((wallJumpExpecter > 0 || isClimbing()) && Input.GetKeyDown(KeyCode.Space) && !isGrounded())
        {
            bouncing = false;
            isWallJumping = true;
			wallJumpingCounter = wallJumpingDuration;
            wallJumpExpecter = 0;
			if (isWallJumping) print("iswalljumping:true");
            else print("iswalljumpingfalse");
        }

        if (isWallJumping) 
        {
			body.velocity = new Vector2(-transform.localScale.x * wallJumpingPower.x * 5, wallJumpingPower.y * 5);
            wallJumpingCounter -= Time.deltaTime;
            if(wallJumpingCounter < 0) isWallJumping = false;

		}
    }



    IEnumerator WallJumpWaiter()
    {
        afterWallJumping = true;
        yield return new WaitForSecondsRealtime(2);
        afterWallJumping = false;
    }
    IEnumerator afterWallJump()
    {
		if (Mathf.Abs(body.velocity.x) < maxSpeedWallJump && !isWallJumping && !isDashing && ((horizontalInput * body.velocity.x > 0) || (horizontalInput * body.velocity.x == 0 && horizontalInput != 0)))//wall jumpingten bir süre sonra da iþe yaramalý
		{
			body.velocity = new Vector2(body.velocity.x + (accelerationWallJump * horizontalInput) / 12, body.velocity.y);
		}
		else if (horizontalInput * body.velocity.x < 0 && !isDashing && !isWallJumping)
		{
			body.velocity = new Vector2(body.velocity.x - (body.velocity.x / 8), body.velocity.y);
			if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
		}
        yield return new WaitForSecondsRealtime(2);
	}


    //duvara týrmanma
   /* private void Climb()
    {
        if (isClimbing() && !(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f) && !isWallJumping)
        {
			body.velocity = new Vector2(0, 0); //böyleydi   body.velocity = new Vector2(body.velocity.x, 0);
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


    //duvarda zýplama
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


            print("Duvarda zýpladý");
        }

      //  Invoke(nameof(StopWallJumping), wallJumpingDuration);

    }*/


    private void StopWallJumping()
    {

        isWallJumping = false;
    }


    //yerde mi
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size*0.7f,0,Vector2.down,0.5f,groundLayer);

      
        return raycastHit.collider != null;
    }

    //duvarda mý
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


	//saldýrý
	public bool canAttack()
    {

        return horizontalInput == 0 && isGrounded() && !onWall();
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
}
