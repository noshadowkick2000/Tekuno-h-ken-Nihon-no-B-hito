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
    private float _doubleTapTime;
    private float _doubleTapInterval = .35f;
    private bool _leftDown;
    private bool _rightDown;

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

        _leftDown = false;
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

    public bool DoubleLeft()
    {
        return _leftDown;
    }

    public bool DoubleRight()
    {
        return _rightDown;
    }
}
