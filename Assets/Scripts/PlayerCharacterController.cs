using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacterController : MonoBehaviour
{

    Rigidbody _rb;
    private Vector3 _movementVector;
    [SerializeField] private float _speed = 8f;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log(_rb.linearVelocity);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        //HandleGroundCheck();
    }

    private void HandleMovement()
    {
        _movementVector = InputManager.Instance.PlayerMovementVector * _speed;
        _rb.linearVelocity = new Vector3(_movementVector.x, _rb.linearVelocity.y, _movementVector.z);
    }

    //private void HandleGroundCheck()
    //{
    //    Ray ray = new Ray(transform.position, Vector3.down);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 5f))
    //    {
    //        //Debug.Log("hit: " + hit.collider.name);
    //    }
    //    Debug.DrawRay(ray.origin, ray.direction, Color.red);
    //}

}
