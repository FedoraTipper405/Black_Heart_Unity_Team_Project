using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float _followSpeed = 0.1f;
    [SerializeField]
    private Vector3 _offSet;


    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerMovement.Instance.transform.position + _offSet, _followSpeed); //updates camera position.
    }
}

