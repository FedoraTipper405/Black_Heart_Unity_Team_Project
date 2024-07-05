using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _movementSpeed = 5f;
    private float _xAxis, _yAxis;

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
    public static PlayerController Instance;
    Animator animator;
    private float gravity;

    [Header("Dash Settings")]
    [SerializeField]
    private float _dashingPower;
    [SerializeField]
    private float _dashingTime;
    [SerializeField]
    private float _dashingCoolDown;
    private bool _canDash = true;
    private bool _isDashing;

    [Header("Attacking")]
    bool _Attack = false;
    float _timeBetweenAttack = 0.5f;
    float _timeSinceAttack;
    [SerializeField]
    Transform _sideAttackTransform, _upAttackTransform, _downAttackTransform;
    [SerializeField]
    Vector2 _sideAttackArea, _upAttackArea, _downAttackArea;
    [SerializeField]
    LayerMask _attackableLayer;
    [SerializeField]
    float _swordDamage;
    [SerializeField] GameObject _slashEffect;

    [Header("Recoil")]
    [SerializeField]
    private bool _recoilingX, _recoilingY;
    [SerializeField]
    private bool _lookingRight;
    [SerializeField]
    int _recoilXSteps = 5;
    [SerializeField]
    int _recoilYSteps = 5;
    [SerializeField]
    float _recoilXForce = 20;
    [SerializeField]
    float _recoilYForce = 20;
    private int _stepsXRecoiled, _stepsYRecoiled;

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
        gravity = rb.gravityScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_sideAttackTransform.position, _sideAttackArea);
        Gizmos.DrawWireCube(_upAttackTransform.position, _upAttackArea);
        Gizmos.DrawWireCube(_downAttackTransform.position, _downAttackArea);
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
        Attack();
    }

    private void FixedUpdate()
    {
        Recoil();
    }

    //Gets the directional input of the player, and sets it into a variable.
    void GetInput()
    {
        _xAxis = Input.GetAxisRaw("Horizontal");
        _yAxis = Input.GetAxisRaw("Vertical");
        _Attack = Input.GetButtonDown("Attack");
    }

    //Flips the sprite of the character based on which direction the character is moving.
    void Flip()
    {
        if (_xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            _lookingRight = false;
        }
        else if (_xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            _lookingRight = true;
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
            AudioManager.Instance.PlaySFX("DashSound");
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

    void Attack()
    {
        _timeSinceAttack += Time.deltaTime;
        if (_Attack && _timeSinceAttack >= _timeBetweenAttack)
        {
            _timeSinceAttack = 0;
            animator.SetTrigger("Attacking");
            AudioManager.Instance.PlaySFX("AttackSound");

            if (_yAxis == 0 || _yAxis < 0 && IsGrounded())
            {
                Hit(_sideAttackTransform, _sideAttackArea, ref _recoilingX, _recoilXForce);
                Instantiate(_slashEffect, _sideAttackTransform);
            }
            else if (_yAxis > 0)
            {
                Hit(_upAttackTransform, _upAttackArea, ref _recoilingY, _recoilYForce);
                SlashEffectAtAngle(_slashEffect, 90, _upAttackTransform);
            }
            else if (_yAxis < 0 && !IsGrounded())
            {
                Hit(_downAttackTransform, _downAttackArea, ref _recoilingY, _recoilYForce);
                SlashEffectAtAngle(_slashEffect, -90, _downAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, _attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(_swordDamage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (_recoilingX)
        {
            if (_lookingRight)
            {
                rb.velocity = new Vector2(-_recoilXForce, 0);
            }
            else
            {
                rb.velocity = new Vector2(_recoilXForce, 0);
            }
        }

        if (_recoilingY)
        {
            rb.gravityScale = 0;
            if (_yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, _recoilYForce);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -_recoilYForce);
            }
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (_recoilingX && _stepsXRecoiled < _recoilXSteps)
        {
            _stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (_recoilingY && _stepsYRecoiled < _recoilYSteps)
        {
            _stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (IsGrounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        _stepsXRecoiled = 0;
        _recoilingX = false;
    }
    void StopRecoilY()
    {
        _stepsYRecoiled = 0;
        _recoilingY = false;
    }


    //Checks if the player is on a object with a Ground layer.
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.20f, _groundLayer);
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
            AudioManager.Instance.PlaySFX("JumpSound");
        }
        animator.SetBool("Jumping", !IsGrounded());
    }
}
