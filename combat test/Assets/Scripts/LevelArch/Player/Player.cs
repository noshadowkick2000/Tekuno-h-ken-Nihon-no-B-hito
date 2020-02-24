using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator charAnimator;
    private Rigidbody _rigidbody;
    private const float MoveMult = .01f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
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
            charAnimator.SetTrigger("up");
        }

        if (Input.GetButtonUp("attackRight"))
        {
            ResetAllTriggers();
            charAnimator.SetTrigger("front");
        }

        if (Input.GetButtonUp("attackDown"))
        {
            ResetAllTriggers();
            charAnimator.SetTrigger("down");
        }

        if (Input.GetButtonUp("attackLeft"))
        {
            ResetAllTriggers();
            charAnimator.SetTrigger("back");
        }

        _rigidbody.MovePosition(_rigidbody.transform.position + new Vector3(Input.GetAxis("moveHorizontal") * MoveMult, 0, 0));
    }

    private void ResetAllTriggers()
    {
        charAnimator.ResetTrigger("up");
        charAnimator.ResetTrigger("down");
        charAnimator.ResetTrigger("front");
        charAnimator.ResetTrigger("back");
    }
}
