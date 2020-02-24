using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDefense : MonoBehaviour
{
    public bool running;

    private TimingMachine _timingMachine;

    private float _startTimeBlock;
    private float _endTimeDefense;

    private bool _defending = false;

    private void Awake()
    {
        _timingMachine = GetComponent<TimingMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        
                    }

                    else if (Input.GetKey(KeyCode.A))
                    {
                    
                    }
                
                    else if (Input.GetKey(KeyCode.S))
                    {
                    
                    }
                
                    else if (Input.GetKey(KeyCode.D))
                    {
                    
                    }
                }
            }
        }

        CheckDefenseOver();
    }

    private void CheckDefense(Constants.Stances requiredPlayerStance)
    {
        if (_defending)
        {
            if (Time.time < _startTimeBlock)
            {
                //parry
            }
            else if (Time.time < _endTimeDefense)
            {
                //block
            }
        }
        else
        {
            _timingMachine.FailAction();
        }
    }

    public void StartDefenseTimer(float startParry, float endTime, Constants.Stances enemyStances)
    {
        _defending = true;
        _startTimeBlock = startParry;
        _endTimeDefense = endTime;
    }

    public void StopDefenseTimer()
    {
        _defending = false;
    }

    private void CheckDefenseOver()
    {
        if (Time.time > _endTimeDefense)
        {
            StopDefenseTimer();
        }
    }
}
