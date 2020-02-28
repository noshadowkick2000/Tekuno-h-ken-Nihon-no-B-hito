using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDefense : MonoBehaviour
{
    public bool running;

    private TimingMachine _timingMachine;
    private Health _health;

    private float _startTimeBlock;
    private float _endTimeDefense;

    public bool _defending = false;
    private Constants.Stances _defensiveStance = Constants.Stances.Free;
    private int _damage;
    private EnemyAttacking _enemy;

    private void Awake()
    {
        _timingMachine = GetComponent<TimingMachine>();
        _health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _endTimeDefense)
            CheckDefenseOver();
        if (running && _defending)
        {
            CheckDefense();
        }
    }

    private void CheckDefense()
    {
        Constants.Stances inputStance = Constants.Stances.Null;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.W))
            {
                inputStance = Constants.Stances.Up;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                inputStance = _timingMachine.facingRight ? Constants.Stances.Backward : Constants.Stances.Forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                inputStance = Constants.Stances.Down;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                inputStance = _timingMachine.facingRight ? Constants.Stances.Forward : Constants.Stances.Backward;
            }
        }

        if (inputStance == _defensiveStance)
        {
            if (Time.time < _startTimeBlock)
            {
                //parry
                //print("parry");
                _timingMachine.ParrySuccess();
                //_enemy.StopAttackCycle();
            }
            else if (Time.time < _endTimeDefense)
            {
                //block
                //print("block");
                _timingMachine.BlockSuccess();
                //_enemy.StopAttackCycle();
            }
            StopDefenseTimer();
        }
        else if (inputStance != Constants.Stances.Null)
        {
            //print("oof");
            _timingMachine.FailAction();
        }
    }

    public void StartDefenseTimer(float startBlock, float endTime, Constants.Stances enemyStances, int damage, EnemyAttacking enemy)
    {
        CheckDefenseOver();
        //print("start");
        _defending = true;
        _startTimeBlock = Time.time + startBlock;
        _endTimeDefense = Time.time + endTime;
        _defensiveStance = enemyStances;
        _timingMachine.SetCurrentStance(enemyStances);
        _damage = damage;
        _enemy = enemy;
    }

    public void StopDefenseTimer()
    {
        //print("stopped");
        _defending = false;
        _defensiveStance = Constants.Stances.Free;
    }

    private void CheckDefenseOver()
    {
        if (_defending)
        {
            print("fakoff");
            StopDefenseTimer();
            _health.Damage(_damage);
            _timingMachine.Hurt();
        }
    }
}
