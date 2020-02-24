using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class EnemyMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;
    private Health _health;
    private TimingMachineEnemy _timingMachineEnemy;
    private TimingMachine _player;

    [SerializeField] private TriggerScript leftCollider;
    [SerializeField] private TriggerScript rightCollider;

    private bool _engaged = false;

    [SerializeField] private Constants.MoveBehaviour freeMoveBehaviour;
    [SerializeField] private Constants.MoveBehaviour engagedMoveBehaviour;

    //should change this into single thing later, do in order of highest health first
    [SerializeField] private int[] engagedHealthBehaviourChangeTreshold;
    [SerializeField] private Constants.MoveBehaviour[] otherEngagedMoveBehaviour;

    private int _otherEngagedBehaviourCounter = 0;
    private bool _behaviourChangable = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        _timingMachineEnemy = GetComponent<TimingMachineEnemy>();
        _player = FindObjectOfType<TimingMachine>();

        if (engagedHealthBehaviourChangeTreshold.Length > 0)
        {
            _behaviourChangable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemyInRange();
        MoveEnemy();
    }
    
    //this shit is copy pasted from inputmove
    private void CheckEnemyInRange()
    {
        //maybe can change to single function later and set engaged in GetClosestEnemy
        if (leftCollider.GetOther() > 0 || rightCollider.GetOther() > 0)
        {
            _engaged = true;
        }
        else
        {
            _engaged = false;
        }
    }

    private void MoveEnemy()
    {
        if (_behaviourChangable)
        {
            if (_health.curHealth < engagedHealthBehaviourChangeTreshold[_otherEngagedBehaviourCounter])
            {
                _otherEngagedBehaviourCounter += 1;
            }
            else
            {
                MoveEnemyWithBehaviour(otherEngagedMoveBehaviour[_otherEngagedBehaviourCounter]);
            }
        }
        else if (_engaged)
        {
            MoveEnemyWithBehaviour(engagedMoveBehaviour);
            _timingMachineEnemy.SetDirection(IsPlayerRight());
        }
        else
        {
            MoveEnemyWithBehaviour(freeMoveBehaviour);
        }
    }

    private bool IsPlayerRight()
    {
        return _player.transform.position.x > transform.position.x;
    }

    private void MoveEnemyWithBehaviour(Constants.MoveBehaviour curBehaviour)
    {
        switch (curBehaviour)
        {
            
        }
    }
}
