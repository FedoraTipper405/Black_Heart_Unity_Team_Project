using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : Enemy
{
    [SerializeField]
    public GameObject _PointA;
    [SerializeField]
    public GameObject _PointB;
    private Transform _currentPoint;

    void Start()
    {
        rb.gravityScale = 12f;
        _currentPoint = _PointB.transform;
    }

    protected override void Awake()
    {
        base.Awake();   
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!_isRecoiling)
        {
            Vector2 point = _currentPoint.position - transform.position;
            if (_currentPoint == _PointB.transform)
            {
                rb.velocity = new Vector2(_speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-_speed, 0);
            }
            if (Vector2.Distance(transform.position, _currentPoint.position) < 0.5f && _currentPoint == _PointB.transform)
            {
                flip();
                _currentPoint = _PointA.transform;
            }
            if (Vector2.Distance(transform.position, _currentPoint.position) < 0.5f && _currentPoint == _PointA.transform)
            {
                flip();
                _currentPoint = _PointB.transform;
            }
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
