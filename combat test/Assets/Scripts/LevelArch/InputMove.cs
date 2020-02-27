using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputMove : MonoBehaviour
{
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningMultiplier;
    [SerializeField] private float swordDrawnSpeed;
    [SerializeField] private float jumpHeight;

    [SerializeField] private TriggerScript leftCollider;
    [SerializeField] private TriggerScript rightCollider;

    private Rigidbody _rigidBody;
    private TimingMachine _timingMachine;

    public bool running;
    public bool drawn;
    public bool engaged;

    public float autoMove = 0;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _timingMachine = GetComponent<TimingMachine>();
    }

    private void Update()
    {
        if (running)
        {
            float input = Input.GetAxis("moveHorizontal");
            if (!engaged)
            {
                if (input < 0)
                {
                    _timingMachine.SetDirection(false);
                }
                else if (input > 0)
                {
                    _timingMachine.SetDirection(true);
                }
            }

            if (drawn)
            {
                CheckEnemyInRange();
                MoveDrawn();
            }
            else
                MoveSheathed();
        }
        else
        {
            AutoMove(_timingMachine.facingRight);
        }
    }

    private void CheckEnemyInRange()
    {
        //maybe can change to single function later and set engaged in GetClosestEnemy
        TimingMachineEnemy enemy = GetClosestEnemy();
        if (enemy == null)
        {
            engaged = false;
        }
        else
        {
            engaged = true;
        }
    }

    private void AutoMove(bool facingRight)
    {
        float x = autoMove * .004f;
        if (!facingRight)
            x *= -1;
        Vector3 newGroundPos = new Vector3(x, 0, 0);
        _rigidBody.MovePosition(_rigidBody.position + newGroundPos);
    }

    private void MoveSheathed()
    {
        Vector3 newGroundPos = new Vector3(walkingSpeed * Input.GetAxis("moveHorizontal"), 0, 0);
        //replace keys with buttons later

        if (Input.GetKey(KeyCode.LeftShift))
            newGroundPos *= runningMultiplier;
        _rigidBody.MovePosition(_rigidBody.position + newGroundPos);
        
        if (Input.GetKeyDown(KeyCode.Space))
            _rigidBody.AddForce(Vector3.up*jumpHeight);
    }

    private void MoveDrawn()
    {
        //check for correct direction depending on closest enemy
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 newGroundPos = new Vector3(swordDrawnSpeed * Input.GetAxis("moveHorizontal"), 0, 0);
            _rigidBody.MovePosition(_rigidBody.position + newGroundPos);
        }
    }

    private TimingMachineEnemy GetClosestEnemy()
    {
        TimingMachineEnemy[] leftEnemies = leftCollider.GetEnemies();
        TimingMachineEnemy[] rightEnemies = rightCollider.GetEnemies();
        
        TimingMachineEnemy closestEnemy = null;

        if (leftEnemies.Length > 0 || rightEnemies.Length > 0)
        {
            float closestDistanceLeft = 100;
            TimingMachineEnemy closestLeftEnemy = null;

            foreach (var enemy in leftEnemies)
            {
                float curDistance = Vector3.Distance(enemy.transform.position, transform.position);
                if (curDistance < closestDistanceLeft)
                {
                    closestDistanceLeft = curDistance;
                    closestLeftEnemy = enemy;
                }
            }
        
            float closestDistanceRight = 100;
            TimingMachineEnemy closestRightEnemy = null;

            foreach (var enemy in rightEnemies)
            {
                float curDistance = Vector3.Distance(enemy.transform.position, transform.position);
                if (curDistance < closestDistanceRight)
                {
                    closestDistanceRight = curDistance;
                    closestRightEnemy = enemy;
                }
            }

            if (closestDistanceLeft > closestDistanceRight)
            {
                closestEnemy = closestRightEnemy;
                _timingMachine.SetDirection(true);
            }
            else
            {
                closestEnemy = closestLeftEnemy;
                _timingMachine.SetDirection(false);
            }
        }

        return closestEnemy;
    }
}
