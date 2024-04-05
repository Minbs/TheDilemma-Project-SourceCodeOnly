using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemyScript : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
	}

    // Update is called once per frame
    void Update()
    {
		agent.destination = target.position;
	}
}
