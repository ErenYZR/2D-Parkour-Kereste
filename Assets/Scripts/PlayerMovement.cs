using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundLayer;
	public LayerMask wallLayer;
	public float speed;
	public float jumpPower;
	private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private TrailRenderer trailRenderer;
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

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

		//Oyuncunun saða sola dönme animasyonu
		if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

        

        //animatör parametrelerini ayarlama kýsmý
        anim.SetBool("run",horizontalInput !=0 && isGrounded());
        anim.SetBool("grounded", isGrounded());
        anim.SetBool("jump", !isGrounded());
        anim.SetBool("dash",whileDashing());

         //   body.gravityScale = 5; //yer çekimi ayarlama


        //dash atmazkenki hareket
        if (!isDashing)
        {
			body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); //sað sol hareket etme
		}
        else if(isDashing)//dash atarkenki hareket
        {
            body.velocity = new Vector2(0, 0);
            body.velocity = new Vector2(dashingDir.x,dashingDir.y).normalized*dashingVelocity;
        }



		if (Input.GetKeyDown(KeyCode.Space) && !(onWall() && Input.GetKey(KeyCode.Mouse0))) // zýplama
			{ 
                Jump();				
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


       // if (onWall()) print("S");
        if (Input.GetKey(KeyCode.Mouse0) && onWall())
        {
            Climb();
        }
        else
        {
            body.gravityScale = 5;
        }
	}

    //zýplama kodu
    private void Jump()
    {
        if (jumpcount > 0)
        {
			body.velocity = new Vector2(body.velocity.x, jumpPower);
			jumpcount--;  
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
		body.velocity = new Vector2(body.velocity.x, 0);
		body.gravityScale = 0;
		body.velocity = new Vector2 (body.velocity.x, verticalInput*climbSpeed);

		/*if (Input.GetKeyDown(KeyCode.Space))
        {
            body.velocity = new Vector2(-Mathf.Sign(transform.localRotation.x)*30,50);//YARDIM
        }
        */

	}


    //yerde mi
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size,0,Vector2.down,0.1f,groundLayer);
        return raycastHit.collider != null;
    }

    //duvarda mý
	private bool onWall()
	{
		RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, groundLayer);//wall layer
		return raycastHit.collider != null;

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


}
