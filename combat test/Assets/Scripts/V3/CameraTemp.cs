using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTemp : MonoBehaviour
{
    private Vector3 _player;
    private Vector3 _enemy;
    private Vector3 _intermediate;
    private Vector3 _lastIntermediate;
    [SerializeField] private Vector3 _offset;

    [SerializeField] private GameObject _sub1;
    [SerializeField] private GameObject _sub2;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _lastIntermediate = _intermediate;
        _player = _sub1.transform.position;
        _enemy = _sub2.transform.position;
        _intermediate = Vector3.Lerp(_player, _enemy, .5f);
        Vector3 position = transform.position;
        Vector3 dir;
        if (_player.x < _enemy.x)
            dir = _enemy - _intermediate;
        else
            dir = _player - _intermediate;
        Quaternion rot = Quaternion.Euler(0, -90, 0) * Quaternion.LookRotation(dir);
        
        //transform.LookAt(Vector3.Lerp(_intermediate, _lastIntermediate, .2f));
        transform.position = Vector3.Lerp(_intermediate+(_offset), position, .2f);
    }
}
