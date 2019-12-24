using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private Animator _charAnimator;
    private Rigidbody _rigidbody;
    private Renderer[] _renderer;
    private const float MoveMult = .01f;

    //holds names of all states and the amount, atm this is pretty much only for reference to figure out which int corresponds with which state
    [SerializeField] public string[] playerStates;
    //int which holds the current state, gets updated by the animation state machine override (StateDenoter)
    public int currentState;
    
    // Start is called before the first frame update
    void Start()
    {
        _charAnimator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _renderer = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if (Input.GetButtonUp("attackUp"))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("up");
        }

        if (Input.GetButtonUp("attackDown"))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("down");
        }

        if (Input.GetButtonUp("attackLeft"))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("back");
        }

        if (Input.GetKeyUp("attackRight"))
        {
            ResetAllTriggers();
            _charAnimator.SetTrigger("front");
        }

        _rigidbody.MovePosition(_rigidbody.transform.position + new Vector3(Input.GetAxis("moveHorizontal"), 0, 0));
    }

    private void ResetAllTriggers()
    {
        _charAnimator.ResetTrigger("up");
        _charAnimator.ResetTrigger("down");
        _charAnimator.ResetTrigger("front");
        _charAnimator.ResetTrigger("back");
    }

    public List<string> GetValues()
    {
        List<string> values = new List<string>() {currentState.ToString(), playerStates[currentState]};
        return values;
    }
}
