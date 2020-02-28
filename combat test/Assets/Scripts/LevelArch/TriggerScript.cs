using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private Constants.ObjectTypes detectionType;
    
    private bool _triggered;
    private List<TimingMachineEnemy> _enemies = new List<TimingMachineEnemy>();
    private BoxCollider _refCollider;

    //used for objects other than enemies
    private int _counter;

    private void Awake()
    {
        _refCollider = GetComponent<BoxCollider>();
    }

    /*private void OnTriggerEnter(Collider other)
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
    }*/

    private EnemyDefense[] OverlapEnemy()
    {
        Collider[] hitColliders = Physics.OverlapBox(_refCollider.center + _refCollider.transform.position, _refCollider.size * .5f, Quaternion.identity);

        List<EnemyDefense> enemies = new List<EnemyDefense>();
        
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Enemy"))
            {
                enemies.Add(hitColliders[i].GetComponent<EnemyDefense>());
            }
        }

        return enemies.ToArray();
    }

    private int OverlapCounter()
    {
        Collider[] hitColliders = Physics.OverlapBox(_refCollider.center, _refCollider.size, Quaternion.identity);

        int count = 0;
        
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
                count++;
        }
        
        return count;
    }

    public EnemyDefense[] GetEnemies()
    {
        return OverlapEnemy();
    }

    public int GetOther()
    {
        return OverlapCounter();
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
