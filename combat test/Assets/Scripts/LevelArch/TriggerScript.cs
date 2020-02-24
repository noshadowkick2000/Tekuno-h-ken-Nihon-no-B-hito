using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private Constants.ObjectTypes detectionType;
    
    private bool _triggered;
    private List<TimingMachineEnemy> _enemies = new List<TimingMachineEnemy>();

    //used for objects other than enemies
    private int _counter;

    private void OnTriggerEnter(Collider other)
    {
        switch (detectionType)
        {
            case Constants.ObjectTypes.Enemy:
                if (other.CompareTag("Enemy"))
                {
                    _enemies.Add(other.GetComponent<TimingMachineEnemy>());
                }
                break;
            case Constants.ObjectTypes.Player:
                if (other.CompareTag("Player"))
                {
                    _counter++;
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (detectionType)
        {
            case Constants.ObjectTypes.Enemy:
                if (other.CompareTag("Enemy"))
                {
                    _enemies.Remove(other.GetComponent<TimingMachineEnemy>());
                }
                break;
            case Constants.ObjectTypes.Player:
                if (other.CompareTag("Player"))
                {
                    _counter--;
                }
                break;
        }
    }

    public TimingMachineEnemy[] GetEnemies()
    {
        return _enemies.ToArray();
    }

    public int GetOther()
    {
        return _counter;
    }

    private void FixedUpdate()
    {
        if (_enemies.Count > 0)
        {
            _triggered = true;
        }
        else
        {
            _triggered = false;
        }
    }
}
