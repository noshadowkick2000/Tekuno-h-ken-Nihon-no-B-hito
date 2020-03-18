using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ArcherEnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float treeRange;
    [SerializeField] private float engagedRange;
    [SerializeField] private float transitionTime;
    
    private Vector3[] _inRangeTrees;
    private float _curPlayerDistance;

    private bool _moving;
    private bool _hiding;

    private float _startTime;
    private float _endTime;
    private Vector3 _startPosition;
    private Vector3 _curGoalPosition;

    private Enemy _enemy;
    private Anchor _lastAnchor;
    private bool _hasLastAnchor;

    private bool _regenerating;

    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        _curPlayerDistance = Vector3.Distance(transform.position, _enemy.player.transform.position);

        if (_hiding && !_moving)
        {
            if (_curPlayerDistance <= engagedRange)
            {
                MoveTree();
            }
            else if (_enemy.curStamina == 0)
            {
                MoveRoad();
            }
        }
        else if (!_hiding && !_moving)
        {
            if (_enemy.curStamina < _enemy.maxStamina)
            {
                if (_curPlayerDistance <= engagedRange)
                {
                    _enemy.animator.SetBool("inrange", true);
                }
                else
                {
                    _enemy.animator.SetBool("inrange", false);
                }

                //temp solution
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }
            else
            {
                if (_curPlayerDistance <= engagedRange)
                {
                    MoveTree();
                }
            }
        }
        else if (_moving)
            MovePosition();
    }

    void MoveTree()
    {
        Anchor temp = Anchor.FindRandomAnchor("ArcherAnchor", transform.position, treeRange);
        _curGoalPosition = temp.transform.position;
        _enemy.animator.SetTrigger("movetree");
        _moving = true;
        GetComponent<CapsuleCollider>().enabled = false;

        _hiding = true;

        _startTime = Time.time;
        _endTime = _startTime + transitionTime;
        _startPosition = transform.position;

        temp.occupied = true;
        
        if (_hasLastAnchor)
            _lastAnchor.occupied = false;
        else
            _hasLastAnchor = true;

        _lastAnchor = temp;
    }

    void MoveRoad()
    {
        _curGoalPosition = FindClosestSpline();
        _enemy.animator.SetTrigger("movetoroad");
        _moving = true;
        GetComponent<CapsuleCollider>().enabled = false;

        _hiding = false;

        _startTime = Time.time;
        _endTime = _startTime + transitionTime;
        _startPosition = transform.position;

        _lastAnchor.occupied = false;
        _hasLastAnchor = false;
    }

    void StopMove()
    {
        _moving = false;
        GetComponent<CapsuleCollider>().enabled = true;

        if (_hiding)
        {
            _enemy.animator.SetTrigger("arrivedtree");
        }
        else
        {
            _enemy.animator.SetTrigger("arrivedroad");
        }
    }

    void MovePosition()
    {
        float t = (Time.time - _startTime) / transitionTime;

        if (t <= 1)
            transform.position = Vector3.Lerp(_startPosition, _curGoalPosition, t);
        else
            StopMove();
    }

    //change later for spline
    Vector3 FindClosestSpline()
    {
        Vector3 newPos = transform.position;
        newPos.z = 0;

        return newPos;
    }

    //call as event in animation
    public void StaminaRegen()
    {
        if (!_regenerating)
        {
            StartCoroutine(Restamina());
            _regenerating = true;
        }
    }

    private IEnumerator Restamina()
    {
        yield return new WaitForSeconds(5);
        _enemy.RegenerateStamina(_enemy.maxHealth/4);
        _regenerating = false;
    }
}
