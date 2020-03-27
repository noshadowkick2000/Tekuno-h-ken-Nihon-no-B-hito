using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using Debug = System.Diagnostics.Debug;

public class HumanPlayer : Character
{

#pragma warning disable 0649

    private SwordInput _swordInput;
    private MoveInput _moveInput;

    [Header("Movement and Combat")] 
    [SerializeField] private float moveSpeed;

    private bool _canLeftStick = true; //canMove
    private bool _canRightStick = false; //canAttackOrParry

    private bool _defendingHigh = false;
    private bool _defendingLow = false;
    private bool _counterAttack = false;

    private SwordInput.Directions _lastDirection = SwordInput.Directions.LeftUp;
    
    private Collider[] _collider_buffer = new Collider[10];

    //temp
    [SerializeField] private AudioClip _oof;
    [SerializeField] private AudioClip _swoosh;
    [SerializeField] private AudioClip _counter;
    private AudioSource _audioSource;

#pragma warning restore 0649
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
        _swordInput = FindObjectOfType<SwordInput>();
        _moveInput = FindObjectOfType<MoveInput>();

        //temp
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canLeftStick)
            LeftStickInput();
        if (_canRightStick)
            RightStickInput();

        UniversalInput();
        
        RegenerateStamina(10);
    }
    
    public void SetFree()
    {
        _canLeftStick = true;
        //temp movinput.active shadows canleftstick
        _moveInput.active = true;
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

    public void SetAttacking(AttackType attack)
    {
        //temp
        _audioSource.clip = _swoosh;
        _audioSource.Play();
        
        _canLeftStick = false;
        //temp
        _moveInput.active = false;
        _canRightStick = false;
        
        if (attack == AttackType.LD || attack == AttackType.LU)
            SetDefense(attack);
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
        float x = _moveInput.GetMoveHorizontal();
        
        if (_moveInput.DodgeLeft())
        {
            if (UseStamina(actionCosts[(int) SwordInput.Directions.Dash]))
                animator.SetTrigger(isFacingForward ? "DashBackward" : "DashForward");
        }
        else if (_moveInput.DodgeRight())
        {
            if (UseStamina(actionCosts[(int) SwordInput.Directions.Dash]))
                animator.SetTrigger(isFacingForward ? "DashForward" : "DashBackward");
        }
        else if (_moveInput.RightTriggerDown())
        {
            if (UseStamina(actionCosts[(int) SwordInput.Directions.Dash]))
                animator.SetTrigger("DashBackward");
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

        //now is done by bezier movement
        //rigidBody.MovePosition(rigidBody.position + new Vector3(x*moveSpeed, 0, 0));

        //temp solution
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
        _canRightStick = true;

        curDebugHit = (int) attack;
        
        int dmg = hitHolder[(int) attack].baseDamage;
        if (isFacingForward)
            hitHolders.transform.localRotation = new Quaternion();
        else
            hitHolders.transform.localRotation = Quaternion.Euler(0, 180, 0);
        Vector3 hitLocation = hitHolder[(int) attack].position;
        int total = Physics.OverlapBoxNonAlloc(hitLocation, hitHolder[(int) attack].halfSize,
            _collider_buffer, Quaternion.identity, 512);

        for (int i = 0; i < total; i++)
        {
            if (_collider_buffer[i].CompareTag("Enemy"))
                _collider_buffer[i].GetComponent<Enemy>().GetHit(hitLocation.y, dmg);
        }
        
        if (attack == AttackType.HD || attack == AttackType.HU)
            SetDefense(attack);
    }

    private void SetDefense(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.LD:
                _defendingHigh = true;
                break;
            case AttackType.LU:
                _defendingLow = true;
                break;
            case AttackType.HD:
                _defendingHigh = true;
                _counterAttack = true;
                break;
            case AttackType.HU:
                _defendingLow = true;
                _counterAttack = true;
                break;
        }
    }

    public bool GetHit(float height, int damage)
    {
        float waistHeight = transform.position.y;

        if (height > waistHeight)
        {
            if (_defendingHigh)
            {
                _defendingHigh = false;
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
                    Hit(AttackType.HU);
                
                //temp
                _audioSource.clip = _counter;
                _audioSource.Play();

                return false;
            }
        }
        else
        {
            if (_defendingLow)
            {
                _defendingLow = false;
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
                    Hit(AttackType.HD);
                
                //temp
                _audioSource.clip = _counter;
                _audioSource.Play();

                return false;
            }
        }
        
        //animator.SetTrigger("hurtdown");
            Wound(damage);
            animator.SetTrigger("Hurt");
            
            switch (_lastDirection)
            {
                case SwordInput.Directions.LeftUp:
                    animator.SetTrigger(isFacingForward ? "AttackHU" : "AttackLU");
                    break;
                case SwordInput.Directions.RightUp:
                    animator.SetTrigger(isFacingForward ? "AttackLU" : "AttackHU");
                    break;
                case SwordInput.Directions.LeftDown:
                    animator.SetTrigger(isFacingForward ? "AttackHD" : "AttackLD");
                    break;
                case SwordInput.Directions.RightDown:
                    animator.SetTrigger(isFacingForward ? "AttackLD" : "AttackHD");
                    break;
            }

                //temp
            _audioSource.clip = _oof;
            _audioSource.Play();
                
            return true;
    }
}
