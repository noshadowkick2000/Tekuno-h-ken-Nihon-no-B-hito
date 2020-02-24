using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAttack : MonoBehaviour
{
    public bool running;

    [SerializeField] private TriggerScript leftCollider;
    [SerializeField] private TriggerScript rightCollider;

    [SerializeField] private float swordReach;

    [SerializeField] private int swordDamage;

    private TimingMachine _timingMachine;

    private void Awake()
    {
        _timingMachine = GetComponent<TimingMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            //later check with all enemies in range whether vulnerable, if so then deal damage
            if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftControl))
            {
                PrimeAttack();
            }
        }
    }

    private void PrimeAttack()
    {
        _timingMachine.attackPrimed = true;
    }

    public void Attack()
    {
        TimingMachineEnemy[] targetEnemies;
        //check if enemy available
        targetEnemies = _timingMachine.facingRight ? rightCollider.GetEnemies() : leftCollider.GetEnemies();
        //if so then send attack request to enemy
        foreach (var enemy in targetEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < swordReach)
            {
                enemy.Wound(swordDamage);
            }
        }
        //enemy function returns whether attack was successful or not if so, then
        _timingMachine.AttackSuccess();
        //else
        //_timingMachine.FailAction();
    }
}
