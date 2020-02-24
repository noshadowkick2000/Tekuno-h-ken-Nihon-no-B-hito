using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingState : MonoBehaviour
{
    [SerializeField] public int animatorID; //which number corresponds to the transition in the animator component
    [SerializeField] public bool canMove; //whether movement controls are locked in this state
    [SerializeField] public float autoMove; //if cannot move, how much the player moves by itself, positive to the right, 0 if canMove or stationary
    [SerializeField] public bool canAttack; //whether attack controls can be queued in this state
    [SerializeField] public TimingState nextAttackState;
    [SerializeField] public bool canDefend; //false if misstimed block/parry
    [SerializeField] public float baseAttackDamage; //0 if not attacking state
    [SerializeField] public float exitTime; //0 if no auto-exit
}
