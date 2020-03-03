using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    
#pragma warning disable 0649
    
    [Header("Connected Scripts")]
    [Header("Non-Serializable")]
    public Animator animator;
    public Rigidbody rigidBody;
    
    [Header("Current Health and Stamina")]
    public int curHealth;
    public int curStamina;

    [Header("Base Health and Stamina")] 
    [Header("Serializable")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int maxStamina;

    [Header("Other")]
    public bool isFacingRight = true;
    
#pragma warning restore 0649
    
    // Start is called before the first frame update
    public void Init()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        curHealth = maxHealth;
        curStamina = maxStamina;
    }

    public void Wound(int damageAmount)
    {
        curHealth -= damageAmount;
    }

    public void Heal(int healAmount)
    {
        curHealth += healAmount;
    }

    public bool UseStamina(int amount) //when called if true allow for action and drain stamina
    {
        int newStamina = curStamina - amount;
        if (newStamina < 0)
            return false;
        curStamina = newStamina;
        return true;
    }

    public void RegenerateStamina(int amount)
    {
        int newStamina = curStamina + amount;
        if (newStamina > maxStamina)
            curStamina = maxStamina;
        else
            curStamina = newStamina;
    }
}
