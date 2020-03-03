using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Character
{

#pragma warning disable 0649
    
    private SwordInput _swordInput;

    [Header("Movement and Combat")] 
    [SerializeField] private float moveSpeed;

    [Header("Stamina cost for each type of action, see enums in SwordInput for order")] [SerializeField]
    private int[] actionCosts = new int[6];

    public bool _canLeftStick = true; //canMove
    private bool _canRightStick = true; //canAttackOrParry
    
#pragma warning restore 0649
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
        _swordInput = FindObjectOfType<SwordInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canLeftStick)
            LeftStickInput();
        if (_canRightStick)
            RightStickInput();
        
        RegenerateStamina(5);
    }

    public void SetFree()
    {
        print("on");
        _canLeftStick = true;
    }

    public void SetAttacking()
    {
        print("off");
        _canLeftStick = false;
    }

    private void LeftStickInput()
    {
        float x = Input.GetAxis("moveHorizontal");

        rigidBody.MovePosition(rigidBody.position + new Vector3(x*moveSpeed, 0, 0));
    }

    private void RightStickInput()
    {
        if (_swordInput.GetDirectionDown(SwordInput.Directions.Up))
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
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftUp))
        {
            ResetTriggers();

            if (isFacingRight)
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

            if (isFacingRight)
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
            
            if (isFacingRight)
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
            
            if (isFacingRight)
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
}
