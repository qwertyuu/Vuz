using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : SceneSingleton<Conductor>
{
    event System.Action<decimal, decimal> _onBeat = delegate { };
    public static event System.Action<decimal, decimal> OnBeat
    {
        add { Instance._onBeat += value; }
        remove { Instance._onBeat -= value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
