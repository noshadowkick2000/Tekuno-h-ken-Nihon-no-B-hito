﻿using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SwordInput : MonoBehaviour
{
    public enum Directions
    {
        Up = 0,
        Down = 1,
        LeftUp = 2,
        LeftDown = 3,
        RightUp = 4,
        RightDown = 5
    }
    
    private bool[] _inputs = new bool[6];
    private bool[] _oldInputs = new bool[6];
    
    // Update is called once per frame
    void Update()
    {
        SaveReset();

        float x = Input.GetAxis("HorizontalRight");
        float y = Input.GetAxis("VerticalRight");

        Vector2 direction = new Vector2(x, y);
        float angle;

        if (x < 0)
            angle = 360 - Vector2.Angle(Vector2.up, direction);
        else
            angle = Vector2.Angle(Vector2.up, direction);

        if (angle == 0)
        {
            if (y > 0)
            {
                _inputs[(int) Directions.Up] = true;
            } 
        }
        else
        {
            if (InBetween(angle, 355, 360) || InBetween(angle, 0, 5))
                _inputs[(int) Directions.Up] = true;
            else if (InBetween(angle, 0, 90))
                _inputs[(int) Directions.RightUp] = true;
            else if (InBetween(angle, 90, 180))
                _inputs[(int) Directions.RightDown] = true;
            else if (angle == 180)
                _inputs[(int) Directions.Down] = true;
            else if (InBetween(angle, 180, 270))
                _inputs[(int) Directions.LeftDown] = true;
            else if (InBetween(angle, 270, 360))
                _inputs[(int) Directions.LeftUp] = true;
        } 
    }

    public bool GetDirectionDown(Directions direction)
    {

        if (_inputs[(int) direction] && !_oldInputs[(int) direction])
            return true;
        else
            return false;
    }

    private bool InBetween(float angle, float lower, float higher)
    {
        if (angle > lower && angle < higher)
            return true;
        else
            return false;
    }

    private void SaveReset()
    {
        for (int i = 0; i < _inputs.Length; i++)
        {
            _oldInputs[i] = _inputs[i];
            _inputs[i] = false;
        }
    }
}
