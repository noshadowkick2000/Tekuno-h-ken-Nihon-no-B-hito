using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    
#pragma warning disable 0649
    
    public enum AttackType
    {
        LU = 0,
        LD = 1,
        HU = 2,
        HD = 3
    }
    
    [Header("Connected Scripts")]
    [Header("Non-Serializable")]
    public Animator animator;
    public Rigidbody rigidBody;
    public SpriteRenderer spriteRenderer;
    
    [Header("Current Health and Stamina")]
    public int curHealth;
    public int curStamina;

    [Header("Base Health and Stamina")] 
    [Header("Serializable")]
    [SerializeField] public int maxHealth;
    [SerializeField] public int maxStamina;
    
    [Header("Stamina cost for each type of action")] 
    [SerializeField] public int[] actionCosts = new int[6];

    [Header("Amount of damage and relative coordinates and sizes of overlap box")] 
    [SerializeField] public HitHolder[] hitHolder = new HitHolder[4];
    [SerializeField] public GameObject hitHolders;

    [Header("Other")]
    public bool isFacingForward = true;
    
    [Header("Debug")] [SerializeField] public int curDebugHit;
    
#pragma warning restore 0649
    
    // Start is called before the first frame update
    public void Init()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

    public void OverrideUseStamina(int amount)
    {
        curStamina -= amount;
        if (curStamina < 0)
            curStamina = 0;
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
