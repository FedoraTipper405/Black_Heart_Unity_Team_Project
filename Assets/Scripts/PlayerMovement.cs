using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float _movementSpeed;
    
    private float _xAxis;
    
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private LayerMask _groundLayer;

    [SerializeField]
    private float _coyoteTime;
    private float _coyoteTimeCounter;
    public static PlayerMovement Instance;
    Animator anim;

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
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        GetInput();
        MovePlayer();
        CoyoteTimer();
        Jump();
        Flip();
    }

    void GetInput()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
    }

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

    private void MovePlayer()
    {
        rb.velocity= new Vector2(_movementSpeed * _xAxis, rb.velocity.y);
        anim.SetBool("Running", rb.velocity.x != 0 && IsGrounded());
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

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
        anim.SetBool("Jumping", !IsGrounded());
    }
}
