using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PhysicsViewer : MonoBehaviour
{
    public Collider2D[] Colliders;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPhysicsInfo()
    {
		Colliders = FindObjectsOfType<Collider2D>();

        foreach(var collider in Colliders)
        {
            Debug.Log(collider.gameObject.name);
			Debug.Log(collider.bounds.min);
            Debug.Log(collider.bounds.max); 
        }
    }
}
