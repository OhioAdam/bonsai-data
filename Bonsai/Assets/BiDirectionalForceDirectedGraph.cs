﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiDirectionalForceDirectedGraph : MonoBehaviour {
    public float desiredConnectedNodeDistance=1;
    public float connectedNodeForce = 1;
    public float disconnectedNodeForce = 1;
    public List<Node> nodes;
	// Use this for initialization
	void Start () {
        nodes = new List<Node>();
		for (int i = 0; i < 20; i++)
        {
            nodes.Add(new Node() {
                children = nodes.Where(node => Random.value > 0.5f).ToList(),
                position = Random.insideUnitSphere*10,
velocity=Vector3.zero});
        }
	}
	
	// Update is called once per frame
	void Update () {
        ApplyGraphForce();
        foreach(var node in nodes)
        {
            node.position += node.velocity * Time.deltaTime;
        }
	}
    private void ApplyGraphForce()
    {
        foreach(var node in nodes)
        {
            var disconnectedNodes = nodes.Except(node.children);
            foreach(var connectedNode in node.children)
            {
                var difference = node.position - connectedNode.position;
                var distance = (difference).magnitude;
                var appliedForce = connectedNodeForce *  Mathf.Log10(distance / desiredConnectedNodeDistance);
                connectedNode.velocity += appliedForce * Time.deltaTime * difference.normalized;
            }
            foreach (var disconnectedNode in disconnectedNodes)
            {
                var difference = node.position - disconnectedNode.position;
                var distance = (difference).magnitude;
                if (distance != 0)
                {
                    var appliedForce = -disconnectedNodeForce / Mathf.Pow(distance, 2);
                    disconnectedNode.velocity -= appliedForce * Time.deltaTime * difference.normalized;
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        foreach(var node in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(node.position, 0.125f);
            Gizmos.color = Color.green;
            foreach(var connectedNode in node.children)
            {
                Gizmos.DrawLine(node.position, connectedNode.position);
            }
        }
    }
}

