using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseJoint {
	public Node Node1;
	public Node Node2;
    public float springConstant = 100;
    public float mass = 1;
    public float dampeningModifier = 200;

    Vector2 forceNodeOne;
    Vector2 forceNodeTwo;

    Vector2 dampeningForce;

    float distanceStart;
    float distanceCurrent;

	public BaseJoint(Node _node1, Node _node2) {
		Node1 = _node1;
		Node2 = _node2;

        distanceStart = Vector2.Distance(Node1.transform.position, Node2.transform.position);
	}

	public virtual void Update() {
        dampeningForce = ((Node2.CurrentPosition - Node2.previousPosition) - (Node1.CurrentPosition - Node1.previousPosition)) * dampeningModifier;
        distanceCurrent = (Vector2.Distance(Node1.CurrentPosition, Node2.CurrentPosition));

        forceNodeOne = (1 * (distanceCurrent - distanceStart) * springConstant * (Node2.CurrentPosition - Node1.CurrentPosition).normalized) + dampeningForce;
        forceNodeTwo = -1 * forceNodeOne;

        Node1.AddAcceleration(forceNodeOne / mass);
        Node2.AddAcceleration(forceNodeTwo / mass);

	}
}
