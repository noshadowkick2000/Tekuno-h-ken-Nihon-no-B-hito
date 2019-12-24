using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//for playerscript only atm ngahgadhgasf
public class DebugDisplaySender : MonoBehaviour
{
    private Player _player;
    private DebugDisplayReceiver _debugDisplay;

    private string _header;
    private List<string> _names = new List<string>() {"state", "state name"};
    private List<string> _values = new List<string>() {"", ""};
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<Player>();
        _debugDisplay = FindObjectOfType<DebugDisplayReceiver>();
        
        _debugDisplay.AddEntry(gameObject, "Player", _names, _values);
    }

    // Update is called once per frame
    void Update()
    {
        _values = _player.GetValues();
        _debugDisplay.UpdateEntry(gameObject, _values);
    }
}
