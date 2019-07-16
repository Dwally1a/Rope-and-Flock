using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
	public Node Previous;
	public Node Next;
	protected List<BaseJoint> joints = new List<BaseJoint>();

	public Vector2 CurrentPosition
	{
		get {
			return transform.position;
		}
		set {
			if (Previous != null)
				transform.position = value;
		}
	}
    Vector2 newPosition;
    public Vector2 previousPosition;
    Vector2 acceleration;
    public Vector2 gravity = new Vector2(0, -9.8f);

    public float timeStep = 0.0034f;
    float timeStepSquared;


    // Use this for initialization
    void Start () {
		// does this node have no previous? if so then it is the root node so create the joints
		if (Previous == null) {
			CreateJoints();
		}
        previousPosition = this.transform.position;
        gravity = new Vector2(0, -9.8f);
        timeStep = 0.034f;
        timeStepSquared = Mathf.Pow(timeStep, 2);
        acceleration = gravity;

        
       
	}
	
	// Update is called once per frame
	void Update () {
		// if this is the root node then tell the joints to update
		foreach(BaseJoint joint in joints) {
			joint.Update();
		}
	}

	void LateUpdate() {
        newPosition = (CurrentPosition + (CurrentPosition - previousPosition)) + (acceleration * timeStepSquared);
        previousPosition = CurrentPosition;
        CurrentPosition = newPosition;

        acceleration = gravity;
	}

	void CreateJoints() { 
		// traverse the chain of nodes
		Node current = this;
		while (current != null) {
			// if there is a next node then create the joint
			if (current.Next != null) {
				joints.Add(new BaseJoint(current, current.Next));
			}

			current = current.Next;
		}
	}

    public void AddAcceleration(Vector2 _springForce)
    {
        acceleration += _springForce;
    }
}
