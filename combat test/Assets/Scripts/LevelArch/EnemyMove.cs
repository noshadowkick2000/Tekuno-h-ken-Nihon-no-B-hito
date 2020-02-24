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

    [SerializeField] private Transform[] patrolPoints;
    //need to copy transforms if patrolPoints are children of enemy object, otherwise the positions will move with the enemy object
    private Vector3[] _pPs;
    //not implemented yet, use later when making patrolling do more
    [SerializeField] private float patrolWaitTime;

    [SerializeField] private float normalSpeed;
    [SerializeField] private float engagedSpeed;
    
    private int _patrolPointCounter;

    private int _otherEngagedBehaviourCounter = 0;
    private bool _behaviourChangable = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        _timingMachineEnemy = GetComponent<TimingMachineEnemy>();
        _player = FindObjectOfType<TimingMachine>();

        _pPs = new Vector3[patrolPoints.Length];

        for (int i = 0; i < _pPs.Length; i++)
        {
            _pPs[i] = patrolPoints[i].position;
        }

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

    private bool IsMovementToRight(Vector3 origin, Vector3 goal)
    {
        return (origin.x < goal.x);
    }

    private void MoveEnemyWithBehaviour(Constants.MoveBehaviour curBehaviour)
    {
        float curSpeed = _engaged ? engagedSpeed : normalSpeed;

        switch (curBehaviour)
        {
            case Constants.MoveBehaviour.Patrol:
                if (Vector3.Distance(transform.position, _pPs[_patrolPointCounter]) < .1f)
                {
                    _patrolPointCounter++;
                    if (_patrolPointCounter == _pPs.Length)
                        _patrolPointCounter = 0;
                }
                else
                {
                    _timingMachineEnemy.SetDirection(IsMovementToRight(transform.position, _pPs[_patrolPointCounter]));
                    _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, _pPs[_patrolPointCounter], curSpeed));
                }
                break;
            case Constants.MoveBehaviour.Chase:
                if (Vector3.Distance(transform.position, _player.transform.position) > _timingMachineEnemy.enemyDamageReach)
                    _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, _player.transform.position, curSpeed));
                _timingMachineEnemy.SetDirection(IsPlayerRight());
                break;
            case Constants.MoveBehaviour.Stationary:
                _timingMachineEnemy.SetDirection(IsPlayerRight());
                break;
        }
    }
}
