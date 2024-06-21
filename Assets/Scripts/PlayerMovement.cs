using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _movementSpeed = 5f;
    private float _xAxis;

    [Header("Ground Check Settings")]
    [SerializeField]
    private float _jumpForce = 45f;
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Coyote Time Settings")]
    [SerializeField]
    private float _coyoteTime;
    private float _coyoteTimeCounter;

    private Rigidbody2D rb;
    public static PlayerMovement Instance;
    Animator animator;

    [Header("Dash Settings")]
    [SerializeField]
    private float _dashingPower;
    [SerializeField]
    private float _dashingTime;
    [SerializeField]
    private float _dashingCoolDown;
    private bool _canDash = true;
    private bool _isDashing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (_isDashing)
        {
            return;
        }
        GetInput();
        MovePlayer();
        CoyoteTimer();
        Jump();
        Flip();
        StartDash();
    }

    //Gets the directional input of the player, and sets it into a variable.
    void GetInput()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
    }

    //Flips the sprite of the character based on which direction the character is moving.
    void Flip()
    {
        if (_xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (_xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    //Moves the player using the directional input variable and multiples it to the movement speed.
    private void MovePlayer()
    {
        rb.velocity= new Vector2(_movementSpeed * _xAxis, rb.velocity.y);
        animator.SetBool("Running", rb.velocity.x != 0 && IsGrounded());
    }

    //Executes the coroutine when the player presses the dash button and if the player can dash.
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && _canDash)
        {
            StartCoroutine(Dash());
        }
    }

    //Sets up the dash coroutine.
    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        animator.SetTrigger("Dashing");
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * _dashingPower, 0f);
        yield return new WaitForSeconds(_dashingTime);
        rb.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(_dashingCoolDown);
        _canDash = true;
    }

    //Checks if the player is on a object with a Ground layer.
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.15f, _groundLayer);
    }

    //Allows the player to still jump for a small amount of time, even if the player is not Grounded.
    void CoyoteTimer()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    //Allows the player to jump when the jump key is pressed and if other conditions are met.
    void Jump()
    {
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            _coyoteTimeCounter = 0f;
        }
        
        if(Input.GetButtonDown("Jump") && _coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, _jumpForce);     
        }
        animator.SetBool("Jumping", !IsGrounded());
    }
}
