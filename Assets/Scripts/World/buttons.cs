using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag != "GameController") return;
        gameObject.transform.Translate(0, 0, (float)(0.025));
	}
	void OnTriggerExit(Collider other) {
        if(other.gameObject.tag != "GameController") return;
        gameObject.transform.Translate(0, 0, (float)(-0.025));
	}
}
