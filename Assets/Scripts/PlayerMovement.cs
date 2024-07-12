using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundLayer;
	public LayerMask wallLayer;

	private Rigidbody2D body;
	private Animator anim;
	private BoxCollider2D boxCollider;
	private TrailRenderer trailRenderer;

    [SerializeField] private BoxCollider2D climb;
    [SerializeField] private float maxSpeed;
	[SerializeField] private float acceleration;
	[SerializeField] private float deceleration;
	[SerializeField] private float h�z;
	public float speed;
	public float jumpPower;
    private float horizontalInput;
    private float verticalInput;
    private float airJumpCounter;
    private float groundJumpCounter;
    private float isGroundedCooldowntimer;
    private float timer;
    [SerializeField] float climbSpeed = 3f;
    private bool doubleJump;
 

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
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(2f, 3f);





	private void Awake()
	{
        //Rigidbody ve animat�r i�in referans al�yoruz.
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

        if (isClimbing()) print("T�rman�yor");
        if (onWall()) print("Duvarda");
        if (isGrounded()) print("Yerde");


        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

		//animat�r parametrelerini ayarlama k�sm�
		anim.SetBool("run", horizontalInput != 0 && isGrounded());
		anim.SetBool("grounded", isGrounded());
		anim.SetBool("jump", !isGrounded());
		anim.SetBool("dash", whileDashing());

		//Oyuncunun sa�a sola d�nme animasyonu
		if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

	/*	if (horizontalInput == 0)
		{
			body.velocity = new Vector2(body.velocity.x/100, body.velocity.y);
			if (body.velocity.x < 0.1f) body.velocity = new Vector2(0, body.velocity.y);
		}

		acceleration = maxSpeed-Mathf.Abs(body.velocity.x);
        h�z = body.velocity.x;
        if (Mathf.Abs(body.velocity.x) < maxSpeed && ((horizontalInput*body.velocity.x>0) || (horizontalInput * body.velocity.x == 0 && horizontalInput !=0)))
        {
            body.velocity = new Vector2(body.velocity.x + (acceleration* horizontalInput),body.velocity.y);
        }

        if(horizontalInput*body.velocity.x < 0)
        {
			body.velocity = new Vector2(body.velocity.x - (body.velocity.x *9/10), body.velocity.y);
            if(body.velocity.x < 0.1f) body.velocity = new Vector2(0,body.velocity.y);
		}*/








		//dash atmazkenki hareket
		 if (!isDashing && !onWall() && !isWallJumping)
		 {
			 body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); //sa� sol hareket etme
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

		 }

		if (isGrounded()) //yere d���nce jumpcount s�f�rlama
            {
            airJumpCounter = 1;
            groundJumpCounter = 1;
             print("yerde");
                   //sonra d�n, ilk z�play��ta jumpcount eksilmiyo. z�play�nca yerden hemen ayr�lmad��� i�in isgrounded jump countu s�f�rl�yo olabilir.

			if (canDashCondition)  canDash = true;
           
            }

            //dash
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDash && canDashCondition && dashingCooldown >1f)
            {
                Dash();
 
                StartCoroutine(StopDashing());
            }
        dashingCooldown += Time.deltaTime;


		Jump();
		Climb();
		WallJump();
	}

    //z�plama kodu
  /*  private void Jump()
    {
		if (Input.GetKeyDown(KeyCode.Space) && !(onWall() && Input.GetKey(KeyCode.Mouse0))) 
        {
			if (jumpcount > 0)
			{
				body.velocity = new Vector2(body.velocity.x, jumpPower);
				print("Z�plad�");
				jumpcount--;
				print("Jump count:" + jumpcount);
			}
		}
		
    }*/


    private void Jump()
    {
		if (Input.GetKeyDown(KeyCode.Space) && !isClimbing() && isGrounded() && groundJumpCounter > 0 && !isWallJumping)
        {
			body.velocity = new Vector2(body.velocity.x, jumpPower);
            groundJumpCounter--;

		}

		if (Input.GetKeyDown(KeyCode.Space) && !isClimbing() && !isGrounded() && airJumpCounter > 0 && !isWallJumping)
        {
			body.velocity = new Vector2(body.velocity.x, jumpPower);
            airJumpCounter--;
		}

		


	}

    private void Dash()
    {   
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



    //duvara t�rmanma
    private void Climb()
    {


        if (isClimbing() && !(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f) && !isWallJumping)
        {
			body.velocity = new Vector2(body.velocity.x, 0);
			body.gravityScale = 0;
			body.velocity = new Vector2(body.velocity.x, verticalInput * climbSpeed);			
		}		    
        else if (isClimbing() && (Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f))
        {
			isWallJumping = true;
			body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 25, wallJumpingPower.y * 25);
			// body.AddForce(new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y)*20, ForceMode2D.Impulse);
			print("Wall Jumping Direction:" + wallJumpingDirection * wallJumpingPower.x + "," + wallJumpingPower.y);
			wallJumpingCounter = 0f;
		}
        else
        {
            body.gravityScale = 5;
        }
	}


    //duvarda z�plama
    private void WallJump()
    {
        if (isClimbing())
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

       /* if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 2, wallJumpingPower.y*3);
            wallJumpingCounter = 0f;


            print("Duvarda z�plad�");
        }*/

        Invoke(nameof(StopWallJumping), wallJumpingDuration);

    }


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

    //duvarda m�
	private bool onWall()
	{
		RaycastHit2D raycastHit = Physics2D.BoxCast(climb.bounds.center, boxCollider.bounds.size*0.7f, 0, new Vector2(transform.localScale.x,0), 0.2f, wallLayer);//wall layer
		return raycastHit.collider != null;

	}

	private void OnDrawGizmos()
	{
       // Gizmos.DrawCube(climb.bounds.center, boxCollider.bounds.size * 0.5f);
	   //Gizmos.DrawCube(climb.bounds.center + Vector3.down*0.5f, boxCollider.bounds.size * 0.5f);

        Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size * 0.5f);
		Gizmos.DrawCube(boxCollider.bounds.center + new Vector3(transform.localScale.x,0,0)*0.5f, boxCollider.bounds.size * 0.5f);
	}


	//sald�r�
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
        return (Input.GetKey(KeyCode.Mouse0) && onWall());
    }


}
