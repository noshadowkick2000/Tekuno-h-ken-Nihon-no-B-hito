using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    
#pragma warning disable 0649
    
    [Header("Combat Attributes")] 
    [SerializeField] private int blockStaminaCost;
    [Header("Base Resistance to attacks, lower is stronger Resistance")]
    [Range(0.0f, 1.0f)] 
    public float highResistance; //multiplier of incoming damage depending on height
    [Range(0.0f, 1.0f)] 
    public float lowResistance;
    [Header("Prefabs for projectiles, if any")]
    [SerializeField] private GameObject[] projectiles;
    [SerializeField] private Vector3[] projectileSpawn;
    [SerializeField] private int debugSpawn;

    private HumanPlayer _player;
    
#pragma warning restore 0649

    private void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit(float height, int damage)
    {
        float waistHeight = transform.position.y;

        if (curStamina > 0)
            UseStamina(blockStaminaCost);
        else
        {
            if (height > waistHeight)
            {
                animator.SetTrigger("hurtup");
                Wound((int)(damage*highResistance));
            }
            else
            {
                animator.SetTrigger("hurtdown");
                Wound((int)(damage*lowResistance));
            }   
        }
    }

    public void ShootProjectile(int projectile)
    {
        if (isFacingForward)
            Instantiate(projectiles[projectile], transform.position + projectileSpawn[projectile], transform.rotation);
        else
            Instantiate(projectiles[projectile], transform.position - projectileSpawn[projectile], transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + projectileSpawn[debugSpawn], .2f);
    }
}
