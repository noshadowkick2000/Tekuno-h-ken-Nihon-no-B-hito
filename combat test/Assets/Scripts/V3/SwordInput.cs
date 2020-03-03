using System.Collections;
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
    
    // Update is called once per frame
    void Update()
    {
        Reset();

        float x = Input.GetAxis("HorizontalRight");
        float y = Input.GetAxis("VerticalRight");

        Vector2 direction = new Vector2(x, y);
        float angle;

        if (x < 0)
            angle = Vector2.SignedAngle(Vector2.up, direction) + 360;
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
            if (InBetween(angle, 337.5f, 360) || InBetween(angle, 0, 22.5f))
                _inputs[(int) Directions.Up] = true;
            else if (InBetween(angle, 22.5f, 67.5f))
                _inputs[(int) Directions.RightUp] = true;
            else if (InBetween(angle, 112.5f, 157.5f))
                _inputs[(int) Directions.RightDown] = true;
            else if (InBetween(angle, 157.5f, 202.5f))
                _inputs[(int) Directions.Down] = true;
            else if (InBetween(angle, 202.5f, 247.5f))
                _inputs[(int) Directions.LeftDown] = true;
            else if (InBetween(angle, 292.5f, 337.5f))
                _inputs[(int) Directions.LeftUp] = true;
        } 
    }

    public bool GetDirection(Directions direction)
    {
        return _inputs[(int) direction];
    }

    private bool InBetween(float angle, float lower, float higher)
    {
        if (angle > lower && angle < higher)
            return true;
        else
            return false;
    }

    private void Reset()
    {
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = false;
        }
    }
}
