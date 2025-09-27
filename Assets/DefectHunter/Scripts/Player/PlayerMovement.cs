using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [field: SerializeField] public float Speed { get; private set; }
    [SerializeField] private FixedJoystick _joystick;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector3(_joystick.Vertical * Speed, _rb.velocity.y, -_joystick.Horizontal * Speed);
    }

}
