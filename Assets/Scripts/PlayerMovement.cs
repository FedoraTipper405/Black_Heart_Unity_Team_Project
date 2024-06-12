using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float _movementSpeed = 5f;
    
    private float _xAxis;
    
    [SerializeField]
    private float _jumpForce = 45f;

    private bool _isJumping;

    public static PlayerMovement Instance;

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
    }

    
    void FixedUpdate()
    {
        GetInput();
        MovePlayer();
        Jump();
    }

    void GetInput()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void MovePlayer()
    {
        rb.velocity= new Vector2(_movementSpeed * _xAxis, rb.velocity.y);
    }

    void Jump()
    {
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        
        if(Input.GetButtonDown("Jump") && !_isJumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, _jumpForce);
            _isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isJumping = false;
        }
    }
}
