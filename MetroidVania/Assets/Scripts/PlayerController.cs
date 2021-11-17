using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  // Player Variables
  [SerializeField] float walkSpeed = 3.0f;
  [SerializeField] float runSpeed = 6.0f;
  [SerializeField] float airSpeed = 1.0f;
  [SerializeField] float jumpForce = 10.0f;
  [SerializeField] float dashForce = 10.0f;

  // Ability status
  [SerializeField] bool canWallJump = false;
  [SerializeField] float wallJumpTime = 0.2f;
  [SerializeField] bool canDoubleJump = false;
  [SerializeField] bool canAirDash = false;
  [SerializeField] float airDashTime = 10;

  // Component references
  Rigidbody2D body;
  Collider2D collider;
  Animator anim;

  [SerializeField] LayerMask groundLayer;
  [SerializeField] LayerMask wallLayer;

  // Movement states
  int direction;
  bool running;
  bool wallJumped;
  float wallJumpTimer;
  bool doubleJumped;
  bool airDashed;
  float airDashTimer;
  float gScale;



  // Start is called before the first frame update
  void Start()
  {

  }

  void Awake()
  {
    // Get references to seperate components on object
    collider = GetComponent<Collider2D>();
    body = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    direction = 1;
    running = false;
    wallJumped = false;
    wallJumpTimer = 0.0f;
    doubleJumped = false;
    airDashed = false;
    airDashTimer = 0.0f;

    // Remember base gravity scale
    gScale = body.gravityScale;
  }

  // Update is called once per frame
  void Update()
  {
    float moveX = Input.GetAxisRaw("Horizontal");

    // Move horizontally
    if(airDashTimer == 0.0f){
      if(Input.GetKey(KeyCode.LeftShift))
        body.velocity = new Vector2(moveX*runSpeed,body.velocity.y);
      else
      {
        body.velocity = new Vector2(moveX*walkSpeed,body.velocity.y);
      }

      // Sprite flipper
      if(moveX > 0.01f)
      {
        transform.localScale = Vector3.one;
        direction = 1;
      }
      if(moveX < -0.01f)
      {
        transform.localScale = new Vector3(-1, 1, 1);
        direction = -1;
      }
    }

    // Air dash
    if(airDashed)
    {
      if(airDashTimer <= 0.0f)
      {
        body.gravityScale = gScale;
        airDashTimer = 0.0f;
      }
      else
      {
        body.velocity = new Vector2(direction*dashForce,0);
        airDashTimer -= 1*Time.deltaTime;
      }
    }
    else if(Input.GetKey(KeyCode.LeftShift) && !isGrounded())
    {
      body.gravityScale = 0.0f;
      airDashed = true;
      airDashTimer = airDashTime;
    }

    // Wall Grab
    if(wallJumpTimer > 0.0f)
    {

      body.velocity = new Vector2(moveX*walkSpeed, body.velocity.y);

      if(onWall() && !isGrounded())
      {
        body.gravityScale = 0;
        body.velocity = Vector2.zero;
        wallJumpTimer -= Time.deltaTime;
      }
      else
        body.gravityScale = gScale;
    }
    if(wallJumpTimer <= 0.0f)

    // Jump
    if(Input.GetKeyDown(KeyCode.Space))
      Jump();

    // Set animator State
    anim.SetBool("run", moveX != 0);
    anim.SetBool("grounded", isGrounded());
  }

  void Jump()
  {
    if(isGrounded())
      body.velocity = new Vector2(body.velocity.x, jumpForce);
    else if(onWall())
    {
      wallJumped = true;
    }
    else if(canDoubleJump && !doubleJumped)
    {
      body.velocity = new Vector2(body.velocity.x, jumpForce);
      doubleJumped = true;
    }

    anim.SetTrigger("jump");
  }

  bool isGrounded()
  {
    RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    return  raycastHit.collider != null;
  }

  bool onWall()
  {
    RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
    return  raycastHit.collider != null;
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    if(collision.gameObject.tag == "Ground")
    {
      doubleJumped = false;
      airDashed = false;
    }
  }

}
