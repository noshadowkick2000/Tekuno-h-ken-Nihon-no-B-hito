using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanPlayer : Character
{

#pragma warning disable 0649

    //DOWNWARDS STRIKES HIT HIGH, UPWARDS STRIKES HIT LOW
    public enum AttackHeight
    {
        DownwardsLight = 0,
        UpwardsLight = 1,
        DownHeavy = 2,
        UpwardsHeavy = 3
    }
    
    private SwordInput _swordInput;
    private MoveInput _moveInput;

    [Header("Movement and Combat")] 
    [SerializeField] private float moveSpeed;

    [Header("Stamina cost for each type of action")] 
    [SerializeField] private int[] actionCosts = new int[6];

    [Header("Amount of damage and relative coordinates and sizes of overlap box")] 
    [SerializeField] private HitHolder[] hitHolder = new HitHolder[4];

    [Header("Debug")] [SerializeField] private int curDebugHit;

    private bool _canLeftStick = true; //canMove
    private bool _canRightStick = true; //canAttackOrParry
    
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
            animator.SetBool("SwordDrawn", !animator.GetBool("SwordDrawn"));
    }

    private void LeftStickInput()
    {
        float x = _moveInput.GetMoveStick();
        
        //not working properly yet
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
        //parrying now with attacks
        /*if (_swordInput.GetDirectionDown(SwordInput.Directions.Up))
        {
            if (_canLeftStick)
            {
                ResetTriggers();
                if (UseStamina(actionCosts[(int) SwordInput.Directions.Up]))
                    animator.SetTrigger("ParryUp");
            }
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.Down))
        {
            if (_canLeftStick)
            {
                ResetTriggers();
                if (UseStamina(actionCosts[(int) SwordInput.Directions.Down]))
                    animator.SetTrigger("ParryDown");
            }
        }*/

        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftUp))
        {
            ResetTriggers();

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

    public void Hit(AttackHeight attack) //corresponds with number in list of HitHolders
    {
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

    public void GetHit(int damage, float height)
    {
        Wound(damage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireCube(hitHolder[curDebugHit].relPosition + transform.position, hitHolder[curDebugHit].halfSize*2);
    }
}
