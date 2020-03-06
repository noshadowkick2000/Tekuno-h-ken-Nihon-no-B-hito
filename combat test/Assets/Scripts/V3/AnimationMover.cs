using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMover : MonoBehaviour
{
    private Character _character;

    private bool _inAnimation;
    [SerializeField] private float currentValue;
    private float _baseValue;
    private bool _facingRight; //need to use value set at start of animation

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inAnimation)
        {
            Vector3 curPosition = _character.rigidBody.position;
            if (_facingRight)
            {
                _character.rigidBody.MovePosition(new Vector3(_baseValue, curPosition.y, curPosition.z) + new Vector3(currentValue, 0, 0));
            }
            else
            {
                _character.rigidBody.MovePosition(new Vector3(_baseValue, curPosition.y, curPosition.z) - new Vector3(currentValue, 0, 0));
            }
        }
    }

    public void StartAnimationMove()
    {
        _inAnimation = true;
        _baseValue = _character.rigidBody.position.x;
        _facingRight = _character.isFacingRight;
    }
    
    public void ReleaseAnimationMove()
    {
        _inAnimation = false;
    }
}
