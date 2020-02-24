using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TimingMachineEnemy : MonoBehaviour
{
    public bool swordDrawn;

    [SerializeField] private float masterTimeSpeed;
    [SerializeField] private float masterMoveSpeed;

    [SerializeField] private TimingState[] idleStates;
    [SerializeField] private TimingState[] blockStates;
    [SerializeField] private TimingState[] parryStates;
    [SerializeField] private TimingState hurtStates;
    [SerializeField] private TimingState missTimeStates;
    private Constants.Stances _curStance = Constants.Stances.Forward;

    [SerializeField] public float enemyDamageReach;

    private TimingState _currentState;
    private float _exitTimeCounter;

    public bool facingRight = true;

    //private enemies arraylist etc

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Health _health;
    
    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
    }

    public void Wound(int damage)
    {
        _health.Damage(damage);
    }

    //when enemy changes stance etc
    public void SetCurrentStance(Constants.Stances defensiveStance)
    {
        _curStance = defensiveStance;
    }

    //set current state and which modules are called in update
    private void SetCurrentState(TimingState newState)
    {
        swordDrawn = true;
        _currentState = newState;
        animator.SetInteger("curState", _currentState.animatorID);

        if (_currentState.exitTime != 0f)
        {
            _exitTimeCounter = Time.time + _currentState.exitTime / masterTimeSpeed;
        }
    }

    void Update()
    {
        if (swordDrawn)
        {
            if (_exitTimeCounter <= Time.time)
            {
                SetCurrentState(idleStates[(int)_curStance]); //change curstance when exittime over
            }
            //print(_currentState.transform.name);
        }
    }

    public void AttackSuccess()
    {
        SetCurrentState(_currentState.nextAttackState);
    }

    public void BlockSuccess()
    {
        SetCurrentState(blockStates[(int)_curStance]);
    }

    public void ParrySuccess()
    {
        SetCurrentState(parryStates[(int)_curStance]);
    }

    public void FailAction()
    {
        SetCurrentState(missTimeStates);
    }

    public void Hurt()
    {
        SetCurrentState(hurtStates);
    }

    public void SetDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;
        spriteRenderer.flipX = !facingRight;
    }
}
