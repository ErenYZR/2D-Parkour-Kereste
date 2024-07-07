using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
	public float speed;
	public float jumpPower;
    private float horizontalInput;
    private float verticalInput;
    private float jumpcount;
    private float isGroundedCooldowntimer;
    private float timer;
    [SerializeField] float climbSpeed = 3f;
 

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
    private Vector2 wallJumpingPower = new Vector2(24f, 12f);





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

        if (isClimbing()) print("Týrmanýyor");
        if (onWall()) print("Duvarda");


        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

		//animatör parametrelerini ayarlama kýsmý
		anim.SetBool("run", horizontalInput != 0 && isGrounded());
		anim.SetBool("grounded", isGrounded());
		anim.SetBool("jump", !isGrounded());
		anim.SetBool("dash", whileDashing());

		//Oyuncunun saða sola dönme animasyonu
		if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

        
        //dash atmazkenki hareket
        if (!isDashing && !onWall())
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



        if (isGrounded()) //yere düþünce jumpcount sýfýrlama
            {
            // print("yerde");
            if (jumpcount < 2) jumpcount = 1;      //sonra dön, ilk zýplayýþta jumpcount eksilmiyo. zýplayýnca yerden hemen ayrýlmadýðý için isgrounded jump countu sýfýrlýyo olabilir.

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

    //zýplama kodu
    private void Jump()
    {
		if (Input.GetKeyDown(KeyCode.Space) && !(onWall() && Input.GetKey(KeyCode.Mouse0))) 
        {
			if (jumpcount > 0)
			{
				body.velocity = new Vector2(body.velocity.x, jumpPower);
				print("Zýpladý");
				jumpcount--;
			}
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

	}



    //duvara týrmanma
    private void Climb()
    {


        if (isClimbing())
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.gravityScale = 0;
            body.velocity = new Vector2(body.velocity.x, verticalInput * climbSpeed);
        }
        else
        {
            body.gravityScale = 5;
        }
	}


    //duvarda zýplama
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

        if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x*3, wallJumpingPower.y);
            wallJumpingCounter = 0f;


            print("Duvarda zýpladý");
        }

        Invoke(nameof(StopWallJumping), wallJumpingDuration);

    }


    private void StopWallJumping()
    {

        isWallJumping = false;
    }


    //yerde mi
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size*0.5f,0,Vector2.down,0.5f,groundLayer);

      
        return raycastHit.collider != null;
    }

    //duvarda mý
	private bool onWall()
	{
		RaycastHit2D raycastHit = Physics2D.BoxCast(climb.bounds.center, boxCollider.bounds.size*0.5f, 0, new Vector2(transform.localScale.x,0), 0.2f, groundLayer);//wall layer
		return raycastHit.collider != null;

	}

	private void OnDrawGizmos()
	{
        Gizmos.DrawCube(climb.bounds.center, boxCollider.bounds.size * 0.5f);
		Gizmos.DrawCube(climb.bounds.center + Vector3.down*0.5f, boxCollider.bounds.size * 0.5f);

       // Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size * 0.5f);
		//Gizmos.DrawCube(boxCollider.bounds.center + new Vector3(transform.localScale.x,0,0)*0.5f, boxCollider.bounds.size * 0.5f);
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
        return (Input.GetKey(KeyCode.Mouse0) && onWall());
    }


}
