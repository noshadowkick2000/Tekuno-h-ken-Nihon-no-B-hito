using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public enum Stances
    {
        Up = 0,
        Forward = 1,
        Down = 2,
        Backward = 3,
        Free = 4,
        Null = 5
    }
    
    public enum MoveBehaviour
    {
        Stationary = 0,
        Patrol = 1,
        Chase = 2,
    }
    
    public enum ObjectTypes
    {
        Enemy = 0,
        Player = 1
    }
}
