using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [field: SerializeField] public float Speed { get; private set; }
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_joystick.Vertical * Speed, _rb.linearVelocity.y, -_joystick.Horizontal * Speed);

        if (_joystick.Horizontal != 0 && _joystick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }
        _animator.SetBool("IsRunning", _joystick.Horizontal != 0 && _joystick.Vertical != 0);
    }

}
