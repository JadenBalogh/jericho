using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOffScreen : MonoBehaviour {
    
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
