using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInput : MonoBehaviour
{
    //for delta calculations
    private float _curX;
    private float _oldX;

    //for holding stick
    private int _leftHoldCounter;
    private int _rightHoldCounter;
    private int _counterTreshold = 7;

    //for double flicking stick
    /*private float _doubleTapTime;
    private float _doubleTapInterval = .35f;
    private bool _leftDown;
    private bool _rightDown;*/
    
    //right trigger
    private float _rightTriggerLast;
    private float _rightTriggerCurrent;
    private bool _rightTriggerDown;
    
    //dodging
    private bool _dodgeLeft;
    private bool _dodgeRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _oldX = _curX;
        _curX = Input.GetAxisRaw("moveHorizontal");

        if (_curX == 1)
            _rightHoldCounter++;
        else if (_curX == -1)
            _leftHoldCounter++;
        else
        {
            _rightHoldCounter = 0;
            _leftHoldCounter = 0;
        }

        /*_leftDown = false;
        _rightDown = false;
        
        if (LeftDown())
        {
            if (Time.time < _doubleTapTime + _doubleTapInterval)
            {
                _leftDown = true;
            }
            _doubleTapTime = Time.time;
        }
        else if (RightDown())
        {
            if (Time.time < _doubleTapTime + _doubleTapInterval)
            {
                _rightDown = true;
            }
            _doubleTapTime = Time.time;
        }*/

        _rightTriggerLast = _rightTriggerCurrent;
        _rightTriggerCurrent = Input.GetAxis("Dash");
        _rightTriggerDown = false;
        _dodgeLeft = false;
        _dodgeRight = false;

        if (_curX>0 &&  RightTriggerDown())
        {
             _dodgeRight = true;
        }
        else if (_curX<0 && RightTriggerDown())
        {
            _dodgeLeft = true;
        }
    }

    public float GetMoveStick()
    {
        return _curX;
    }

    public bool HoldLeft()
    {
        return _leftHoldCounter > _counterTreshold;
    }

    public bool HoldRight()
    {
        return _rightHoldCounter > _counterTreshold;
    }

    private bool LeftDown()
    {
        return _curX == -1 && _oldX > -1;
    }
    
    private bool RightDown()
    {
        return _curX == 1 && _oldX < 1;
    }
    
    public bool RightTriggerDown()
    {
        return _rightTriggerCurrent == 1 && _rightTriggerLast != 1;
    }

    /*public bool DoubleLeft()
    {
        return _leftDown;
    }

    public bool DoubleRight()
    {
        return _rightDown;
    }*/

    public bool DodgeLeft()
    {
        return _dodgeLeft;
    }

    public bool DodgeRight()
    {
        return _dodgeRight;
    }
}
