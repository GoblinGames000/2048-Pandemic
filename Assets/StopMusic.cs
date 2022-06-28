using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusic : MonoBehaviour
{
    private void OnEnable()
    {
            FindObjectOfType<AudioListener>().enabled = false;

    }
    private void OnDisable()
    {
            FindObjectOfType<AudioListener>().enabled = true;

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
