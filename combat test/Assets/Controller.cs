using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Animator _charAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        _charAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("up");
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("down");
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("back");
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("front");
        }
    }

    void ResetAllTriggers()
    {
        _charAnimator.ResetTrigger("up");
        _charAnimator.ResetTrigger("down");
        _charAnimator.ResetTrigger("front");
        _charAnimator.ResetTrigger("back");
    }
}
