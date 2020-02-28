using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefense : MonoBehaviour
{
    private Health _health;
    private TimingMachineEnemy _timingMachineEnemy;
    private EnemyAttacking _enemyAttacking;
    
    public bool defending;
    //list of cycles
    [SerializeField] private EnemyCyclesHolder[] cyclesHolders;
    private int _curCycle = 0;
    private int _curCycleState;
    
    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
        _timingMachineEnemy = GetComponent<TimingMachineEnemy>();
        _enemyAttacking = GetComponent<EnemyAttacking>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_curCycle < cyclesHolders.Length)
        {
            if (cyclesHolders[_curCycle].nextCycleHealthTreshold > _health.curHealth)
            {
                _curCycle++;
            }
        }
    }
    
    public void StartDefensiveCycle()
    {
        defending = true;
        _curCycleState = 0;
        _timingMachineEnemy.SetCurrentState(cyclesHolders[_curCycle].statesInCycle[_curCycleState]);
    }
    
    public void StopDefensiveCycle()
    {
        defending = false;
    }
    
    public TimingState GetNextState()
    {
        if (_curCycleState < cyclesHolders[_curCycle].statesInCycle.Length - 1)
            _curCycleState++;
        else
        {
            print("nextState");
            StopDefensiveCycle();
            return _timingMachineEnemy.ReturnIdle();
        }
        return cyclesHolders[_curCycle].statesInCycle[_curCycleState];
    }
    
    public bool Wound(int damage)
    {
        //this is fucked
        
        if (_enemyAttacking)
        {
            _enemyAttacking.StopAttackCycle();
        }

        if (!defending)
        {
            //print("start");
            StartDefensiveCycle();
        }

        if (_timingMachineEnemy._currentState.canDefend)
        {
            return false;
        }
        
        _health.Damage(damage);
        if (_health.curHealth <= 0) 
            GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.down * 5);
        return true;
    }
}
