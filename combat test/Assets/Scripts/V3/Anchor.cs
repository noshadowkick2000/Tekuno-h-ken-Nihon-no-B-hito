using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Anchor : MonoBehaviour
{
    public static Anchor FindRandomAnchor(string tag, Vector3 position, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, ~8);
        List<Anchor> possibleLocations = new List<Anchor>();
        
        foreach (var col in colliders)
        {
            Anchor temp = col.GetComponent<Anchor>();
            if (col.CompareTag(tag) && !temp.occupied)
            {
                possibleLocations.Add(temp);
            }
        }

        return possibleLocations[Random.Range(0, possibleLocations.Count)];
    }

    public bool occupied;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(8, 9);
    }
}
