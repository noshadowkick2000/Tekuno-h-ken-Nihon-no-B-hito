using UnityEngine;

public class TimingMachineEnemy : MonoBehaviour
{
    public bool swordDrawn;

    [SerializeField] private float masterTimeSpeed;
    [SerializeField] private float masterMoveSpeed;

    [SerializeField] private TimingState[] idleStates;
    [SerializeField] private TimingState[] blockStates;
    [SerializeField] private TimingState[] parryStates;
    [SerializeField] private TimingState hurtStates;
    [SerializeField] private TimingState missTimeStates;
    [SerializeField] private Constants.Stances _curStance;

    [SerializeField] public float enemyDamageReach;

    public TimingState _currentState;
    private float _exitTimeCounter;

    public bool facingRight = true;

    //private enemies arraylist etc

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer; 
    private EnemyAttacking _enemyAttacking;
    private EnemyDefense _enemyDefense;
    private EnemyMove _enemyMove;
    private InputDefense _playerDefense;

    private Health _health;
    
    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
        _enemyAttacking = GetComponent<EnemyAttacking>();
        _enemyDefense = GetComponent<EnemyDefense>();
        _enemyMove = GetComponent<EnemyMove>();

        _playerDefense = FindObjectOfType<InputDefense>();
        
        SetCurrentStance(Constants.Stances.Forward);
        SetCurrentState(idleStates[(int)_curStance]);
    }

    //when enemy changes stance etc
    public void SetCurrentStance(Constants.Stances defensiveStance)
    {
        _curStance = defensiveStance;
    }

    //set current state and which modules are called in update
    public void SetCurrentState(TimingState newState)
    {
        swordDrawn = true;
        _currentState = newState;
        _enemyMove.autoMove = _currentState.autoMove;
        animator.SetInteger("curState", _currentState.animatorID);
        
        SetCurrentStance(newState.curStance);

        if (_currentState.exitTime != 0f)
        {
            _exitTimeCounter = Time.time + _currentState.exitTime / masterTimeSpeed;
        }
        
        //for blocking and parrying
        if (_currentState.canAttack)
        {
            _playerDefense.StartDefenseTimer(_currentState.exitTime / masterTimeSpeed * .8f, _currentState.exitTime / masterTimeSpeed, _curStance,
                _currentState.baseAttackDamage, _enemyAttacking);
        }
    }

    void Update()
    {
        if (_health.curHealth <= 0)
            Destroy(gameObject);
        
        if (swordDrawn)
        {
            if (_exitTimeCounter <= Time.time)
            {
                if (_enemyAttacking.attacking)
                    SetCurrentState(_enemyAttacking.GetNextState());
                else if (_enemyDefense.defending)
                    SetCurrentState(_enemyDefense.GetNextState());
            }
            //print(_currentState.transform.name);
        }
    }

    public void AttackSuccess()
    {
        SetCurrentState(_currentState.nextAttackState);
    }

    public void BlockSuccess()
    {
        SetCurrentState(blockStates[(int)_curStance]);
    }

    public void ParrySuccess()
    {
        SetCurrentState(parryStates[(int)_curStance]);
    }

    public void FailAction()
    {
        SetCurrentState(missTimeStates);
    }

    public void Hurt()
    {
        SetCurrentState(hurtStates);
    }

    public TimingState ReturnIdle()
    {
        return (idleStates[(int)_curStance]);
    }

    public void SetDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;
        spriteRenderer.flipX = !facingRight;
    }
}
