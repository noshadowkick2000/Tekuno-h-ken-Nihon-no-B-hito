using UnityEngine;

public class TimingMachine : MonoBehaviour
{
    public bool swordDrawn;

    [SerializeField] private float masterTimeSpeed;
    [SerializeField] private float masterMoveSpeed;

    [SerializeField] private InputMove inputMove;
    [SerializeField] private InputAttack inputAttack;
    [SerializeField] private InputDefense inputDefense;

    //should change this into single thing later
    [SerializeField] private TimingState[] idleStates;
    [SerializeField] private TimingState[] blockStates;
    [SerializeField] private TimingState[] parryStates;
    [SerializeField] private TimingState hurtStates;
    [SerializeField] private TimingState missTimeStates;
    private Constants.Stances _curStance = Constants.Stances.Backward;

    private TimingState _currentState;
    private float _exitTimeCounter;

    public bool facingRight = true;

    public bool attackPrimed;
    
    //private enemies arraylist etc

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private int nonArmed;

    // Start is called before the first frame update
    void Start()
    {
        SetFree();
    }

    //when enemy changes stance etc
    public void SetCurrentStance(Constants.Stances defensiveStance)
    {
        _curStance = defensiveStance;
        SetCurrentState(idleStates[(int)_curStance]);
    }

    //set current state and which modules are called in update
    private void SetCurrentState(TimingState newState)
    {
        //print(newState.name);
        swordDrawn = true;
        _currentState = newState;
        animator.SetInteger("curState", _currentState.animatorID); 
        inputMove.running = _currentState.canMove; 
        inputMove.drawn = swordDrawn;
        inputMove.autoMove = _currentState.autoMove * masterMoveSpeed;
        inputDefense.running = _currentState.canDefend; 
        inputAttack.running = _currentState.canAttack;
        inputAttack._stateDamage = _currentState.baseAttackDamage;

        if (_currentState.exitTime != 0f)
        {
            _exitTimeCounter = Time.time + _currentState.exitTime / masterTimeSpeed;
        }
    }

    private void SetFree()
    {
        animator.SetInteger("curState", nonArmed);
        swordDrawn = false;
        inputMove.running = true; 
        inputMove.drawn = swordDrawn;
        inputMove.engaged = false;
        inputMove.autoMove = 0;
        inputDefense.running = false; 
        inputAttack.running = false;
    }

    void Update()
    {
        SetSword();

        if (swordDrawn)
        {
            if (_exitTimeCounter <= Time.time)
            {
                if (attackPrimed)
                {
                    inputAttack.Attack();
                    attackPrimed = false;
                }
                else
                    SetCurrentState(idleStates[(int)_curStance]); //change curstance when exittime over
            }
            //print(_currentState.animatorID);
            //print(_currentState.transform.name);
        }
        //else
            //print("nosword");
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
        inputAttack.Parry();
    }

    public void FailAction()
    {
        attackPrimed = false;
        SetCurrentState(missTimeStates);
    }

    public void Hurt()
    {
        attackPrimed = false;
        SetCurrentState(hurtStates);
    }

    void SetSword()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!swordDrawn)
                SetCurrentState(idleStates[(int)_curStance]);
            else
                SetFree();
        }
    }

    public void SetDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;
        spriteRenderer.flipX = !facingRight;
    }
}
