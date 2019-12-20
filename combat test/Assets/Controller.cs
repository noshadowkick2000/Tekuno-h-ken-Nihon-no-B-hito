using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Animator _charAnimator;
    private Rigidbody _rigidbody;
    private const float MoveMult = .01f;

    private Vector3 _moveLeft;
    private Vector3 _moveRight;
    
    // Start is called before the first frame update
    void Start()
    {
        _charAnimator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponentInChildren<Rigidbody>();
        
        _moveLeft = Vector3.ClampMagnitude(Vector3.left, MoveMult);
        _moveRight = Vector3.ClampMagnitude(Vector3.right, MoveMult);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("up");
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("down");
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("back");
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("front");
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            
        }

        if (Input.GetKey(KeyCode.S))
        {

        }

        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.MovePosition(transform.position + _moveLeft);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.MovePosition(transform.position + _moveRight);
        }
    }

    private void ResetAllTriggers()
    {
        _charAnimator.ResetTrigger("up");
        _charAnimator.ResetTrigger("down");
        _charAnimator.ResetTrigger("front");
        _charAnimator.ResetTrigger("back");
    }

    public void PrintState(int state)
    {
        print(state.ToString());
    }
}
