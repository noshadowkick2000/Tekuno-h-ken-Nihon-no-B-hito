using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacking : MonoBehaviour
{
    private Health _health;
    private TimingMachineEnemy _timingMachineEnemy;
    
    public bool attacking;
    //list of cycles
    [SerializeField] private EnemyCyclesHolder[] cyclesHolders;
    private int _curCycle = 0;
    private int _curCycleState;

    private void Start()
    {
        _health = GetComponent<Health>();
        _timingMachineEnemy = GetComponent<TimingMachineEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        //update curCycle
        if (_curCycle < cyclesHolders.Length)
        {
            if (cyclesHolders[_curCycle].nextCycleHealthTreshold > _health.curHealth)
            {
                _curCycle++;
            }
        }
    }

    public void StartAttackCycle()
    {
        attacking = true;
        _curCycleState = 0;
        _timingMachineEnemy.SetCurrentState(cyclesHolders[_curCycle].statesInCycle[_curCycleState]);
    }
    
    public void StopAttackCycle()
    {
        attacking = false;
    }

    public TimingState GetNextState()
    {
        if (_curCycleState < cyclesHolders[_curCycle].statesInCycle.Length - 1)
            _curCycleState++;
        else
        {
            //print("getnextstateStop");
            StopAttackCycle();
            return _timingMachineEnemy.ReturnIdle();
        }
        return cyclesHolders[_curCycle].statesInCycle[_curCycleState];
    }
}
