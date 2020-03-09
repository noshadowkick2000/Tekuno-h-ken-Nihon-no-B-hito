using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _heading;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float dieTime;

    private float _deadTime;
    private Collider _collider;

    private HumanPlayer _player;

    private void Start()
    {
        _player = FindObjectOfType<HumanPlayer>();
        _heading = (_player.transform.position + new Vector3(0, .5f, 0) - transform.position).normalized;
        _deadTime = Time.time + dieTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _heading * speed;

        if (_deadTime < Time.time)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
            _player.GetHit(transform.position.y, damage);
        Destroy(gameObject);
    }
}
