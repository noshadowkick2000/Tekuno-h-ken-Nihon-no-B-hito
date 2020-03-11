using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class HumanPlayer : Character
{

#pragma warning disable 0649

    private SwordInput _swordInput;
    private MoveInput _moveInput;

    [Header("Movement and Combat")] 
    [SerializeField] private float moveSpeed;

    private bool _canLeftStick = true; //canMove
    private bool _canRightStick = true; //canAttackOrParry

    private bool _defendingHigh = false;
    private bool _defendingLow = false;
    private bool _counterAttack = false;

    private SwordInput.Directions _lastDirection;
    
    private Collider[] _collider_buffer = new Collider[10];

#pragma warning restore 0649
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
        _swordInput = FindObjectOfType<SwordInput>();
        _moveInput = FindObjectOfType<MoveInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canLeftStick)
            LeftStickInput();
        if (_canRightStick)
            RightStickInput();

        UniversalInput();
        
        RegenerateStamina(2);
    }
    
    public void SetFree()
    {
        _canLeftStick = true;
        _canRightStick = true;

        _defendingHigh = false;
        _defendingLow = false;
        _counterAttack = false;
        
        animator.ResetTrigger("DashForward");
        animator.ResetTrigger("DashBackward");
        animator.ResetTrigger("MoveForward");
        animator.ResetTrigger("MoveBackward");
        animator.ResetTrigger("StopMove");
    }

    public void SetAttacking()
    {
        _canLeftStick = false;
    }

    public void SetRolling()
    {
        _canLeftStick = false;
        _canRightStick = false;
    }

    private void UniversalInput()
    {
        if (Input.GetButtonDown("DrawSword"))
        {
            bool swordDrawn = animator.GetBool("SwordDrawn");
            animator.SetBool("SwordDrawn", !swordDrawn);
            SetFree();
            if (swordDrawn)
                _canRightStick = false;
        }
    }

    private void LeftStickInput()
    {
        float x = _moveInput.GetMoveStick();
        
        if (_moveInput.DoubleLeft())
        {
            if (UseStamina(actionCosts[(int) SwordInput.Directions.Dash]))
                animator.SetTrigger(isFacingForward ? "DashBackward" : "DashForward");
        }
        else if (_moveInput.DoubleRight())
        {
            if (UseStamina(actionCosts[(int) SwordInput.Directions.Dash]))
                animator.SetTrigger(isFacingForward ? "DashForward" : "DashBackward");
        }
        
        if (x == 0)
            animator.SetTrigger("StopMove");
        else
        {
            if (x > 0)
            {
                if (isFacingForward)
                    animator.SetTrigger("MoveForward");
                else
                    animator.SetTrigger("MoveBackward");
            }
            else if (x < 0)
            {
                if (isFacingForward)
                    animator.SetTrigger("MoveBackward");
                else
                    animator.SetTrigger("MoveForward");
            }
        }

        if (_moveInput.HoldRight())
        {
            isFacingForward = true; 
        }
        else if (_moveInput.HoldLeft())
        {
            isFacingForward = false;
        }

        if (!isFacingForward)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        rigidBody.MovePosition(rigidBody.position + new Vector3(x*moveSpeed, 0, 0));
    }

    private void RightStickInput()
    {
        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftUp))
        {
            ResetTriggers();
            _lastDirection = SwordInput.Directions.LeftUp;

            if (!isFacingForward)
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.RightUp])) 
                    animator.SetTrigger("AttackLU");
            }
            else
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.LeftUp]))
                    animator.SetTrigger("AttackHU");
            }
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftDown))
        {
            ResetTriggers();
            _lastDirection = SwordInput.Directions.LeftDown;

            if (!isFacingForward)
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.RightDown])) 
                    animator.SetTrigger("AttackLD");
            }
            else
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.LeftDown]))
                    animator.SetTrigger("AttackHD");
            }
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.RightUp))
        {
            ResetTriggers();
            _lastDirection = SwordInput.Directions.RightUp;
            
            if (!isFacingForward)
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.LeftUp])) 
                    animator.SetTrigger("AttackHU");
            }
            else
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.RightUp]))
                    animator.SetTrigger("AttackLU");
            }
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.RightDown))
        {
            ResetTriggers();
            _lastDirection = SwordInput.Directions.RightDown;
            
            if (!isFacingForward)
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.LeftDown])) 
                    animator.SetTrigger("AttackHD");
            }
            else
            {
                if (UseStamina(actionCosts[(int) SwordInput.Directions.RightDown]))
                    animator.SetTrigger("AttackLD");
            }
        }

        /*if (Input.GetButtonDown("Roll"))
        {
            animator.SetTrigger("Roll");
            print("oy");
        }*/
    }
    
    private void ResetTriggers()
    {
        animator.ResetTrigger("ParryUp");
        animator.ResetTrigger("ParryDown");
        animator.ResetTrigger("AttackLU");
        animator.ResetTrigger("AttackLD");
        animator.ResetTrigger("AttackHU");
        animator.ResetTrigger("AttackHD");
    }

    public void Hit(AttackType attack) //corresponds with number in list of HitHolders
    {
        SetDefense(attack);

        curDebugHit = (int) attack;
        
        int dmg = hitHolder[(int) attack].baseDamage;
        Vector3 hitLocation;
        if (isFacingForward)
            hitLocation = hitHolder[(int) attack].relPosition + transform.position;
        else
            hitLocation = transform.position - hitHolder[(int) attack].relPosition;
        int total = Physics.OverlapBoxNonAlloc(hitLocation, hitHolder[(int) attack].halfSize,
            _collider_buffer, Quaternion.identity);

        for (int i = 0; i < total; i++)
        {
            if (_collider_buffer[i].CompareTag("Enemy"))
                _collider_buffer[i].GetComponent<Enemy>().GetHit(hitLocation.y, dmg);
        }
    }

    private void SetDefense(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.UpwardsLight:
                _defendingHigh = true;
                break;
            case AttackType.DownwardsLight:
                _defendingLow = true;
                break;
            case AttackType.UpwardsHeavy:
                _defendingHigh = true;
                _counterAttack = true;
                break;
            case AttackType.DownHeavy:
                _defendingLow = true;
                _counterAttack = true;
                break;
        }
    }

    public void GetHit(float height, int damage)
    {
        float waistHeight = transform.position.y;

        if (height > waistHeight)
        {
            if (_defendingHigh)
            {
                print("salsa");
                animator.SetTrigger("ParryUp");
                
                if (isFacingForward)
                {
                    animator.SetTrigger(_lastDirection == SwordInput.Directions.LeftUp ? "AttackHU" : "AttackLU");
                }
                else
                {
                    animator.SetTrigger(_lastDirection == SwordInput.Directions.RightUp ? "AttackLU" : "AttackHU");
                }
                
                if (_counterAttack)
                    Hit(AttackType.DownHeavy);
            }
            else
            {
                //animator.SetTrigger("hurtup");
                Wound((int)(damage));
            }
        }
        else
        {
            if (_defendingLow)
            {
                print("tango");
                animator.SetTrigger("ParryDown");
                
                if (isFacingForward)
                {
                    animator.SetTrigger(_lastDirection == SwordInput.Directions.LeftDown ? "AttackHD" : "AttackLD");
                }
                else
                {
                    animator.SetTrigger(_lastDirection == SwordInput.Directions.RightDown ? "AttackLD" : "AttackHD");
                }
                
                if (_counterAttack)
                    Hit(AttackType.UpwardsHeavy);
            }
            else
            {
                //animator.SetTrigger("hurtdown");
                Wound((int) (damage));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireCube(hitHolder[curDebugHit].relPosition + transform.position, hitHolder[curDebugHit].halfSize*2);
    }
}
