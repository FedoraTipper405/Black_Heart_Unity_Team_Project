using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _enemyHealth;
    [SerializeField]
    protected float _recoilDis;
    [SerializeField]
    protected float _recoilFactor;
    [SerializeField]
    protected bool _isRecoiling = false;

    protected float _recoilTimer;

    protected Rigidbody2D rb;

    [SerializeField] 
    protected PlayerController player;
    [SerializeField] 
    protected float _speed;
    [SerializeField] protected float _damage;
    

    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        
    }

    protected virtual void Update()
    {
        if (_enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
        if (_isRecoiling)
        {
            if (_recoilTimer < _recoilDis)
            {
                _recoilTimer += Time.deltaTime;
            }
            else
            {
                _isRecoiling = false;
                _recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        _enemyHealth -= _damageDone;

        if (!_isRecoiling)
        {
            rb.AddForce(-_hitForce * _recoilFactor * _hitDirection);
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !HealthManager.Instance._invincible)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        HealthManager.Instance.TakeDamage(10);
    }
}
