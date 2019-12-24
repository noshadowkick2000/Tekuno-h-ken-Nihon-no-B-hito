using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDisplayReceiver : MonoBehaviour
{
    struct Entry
    {
        public string Header;
        public List<string> Name;
        public List<string> Value;
        public GameObject Reference;
    }
    
    private List<Entry> _entries;

    public void AddEntry(GameObject reference, string header, List<string> name, List<string> value)
    {
        Entry newEntry = new Entry();
        newEntry.Header = header;
        newEntry.Name = name;
        newEntry.Value = value;
        newEntry.Reference = reference;
        
        _entries.Add(newEntry);
    }
    
    public void UpdateEntry(GameObject reference, List<string> value)
    {
        Entry entry = _entries.Find(x => x.Reference == reference);
        entry.Value = value;
    }

    public void RemoveEntry(GameObject reference)
    {
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        string outputTxt = "";
        
        foreach (var entry in _entries)
        {
            outputTxt += entry.Header;
            outputTxt += "\n";
            for (int i = 0; i<entry.Name.Count; i++)
            {
                outputTxt += (entry.Name[i] + ": " + entry.Value[i]);
                outputTxt += "\n";
            }
        }
    }
}
