using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField]
    private float _followSpeed = 0.1f;
    [SerializeField]
    private Vector3 _offSet;

    //Follows the player by using the players current position and follows based on a set follow speed.
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + _offSet, _followSpeed); //updates camera position.
    }
}

