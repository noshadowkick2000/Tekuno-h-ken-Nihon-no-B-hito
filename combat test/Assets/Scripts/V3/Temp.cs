using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private SwordInput _swordInput;
    private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _swordInput = FindObjectOfType<SwordInput>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_swordInput.GetDirectionDown(SwordInput.Directions.Up))
        {
            ResetTriggers();
            _animator.SetTrigger("ParryUp");
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.Down))
        {
            ResetTriggers();
            _animator.SetTrigger("ParryDown");
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftUp))
        {
            ResetTriggers();
            _animator.SetTrigger("AttackHU");
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.LeftDown))
        {
            ResetTriggers();
            _animator.SetTrigger("AttackHD");
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.RightUp))
        {
            ResetTriggers();
            _animator.SetTrigger("AttackLU");
        }

        if (_swordInput.GetDirectionDown(SwordInput.Directions.RightDown))
        {
            ResetTriggers();
            _animator.SetTrigger("AttackLD");
        }
    }

    private void ResetTriggers()
    {
        _animator.ResetTrigger("ParryUp");
        _animator.ResetTrigger("ParryDown");
        _animator.ResetTrigger("AttackLU");
        _animator.ResetTrigger("AttackLD");
        _animator.ResetTrigger("AttackHU");
        _animator.ResetTrigger("AttackHD");
    }
}
