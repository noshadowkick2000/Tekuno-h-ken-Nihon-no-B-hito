using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private TextMeshPro healthDisplay;
    
    [SerializeField] private int maxHealth;
    public int curHealth;

    private void Start()
    {
        curHealth = maxHealth;
    }

    public void Damage(int dmgAmount)
    {
        curHealth -= dmgAmount;
        healthDisplay.text = curHealth.ToString();
    }

    public void Heal(int healAmount)
    {
        if (healAmount + curHealth > maxHealth)
            curHealth = maxHealth;
        curHealth += healAmount;
        healthDisplay.text = curHealth.ToString();
    }
}
