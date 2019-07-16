using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour {
	protected Vector3 velocity = Vector3.zero;

	public float Speed = 5f;

	protected List<Minion> minions;

	// Define the priority of each flocking rule
	protected float CohesionStrength = 0.5f;
	protected float CohesionRange = 3f;

	protected float SeparationStrength = 1.5f;
	protected float SeparationDistance = 2f;

	protected float AlignmentStrength = 1f;
	protected float AlignmentRange = 5f;

	public virtual Vector3 Velocity {
		get {
			return velocity;
		}
		set {
			velocity = value;
		}
	}

	// Use this for initialization
	protected virtual void Start () {
		// retreive all minions
		minions = new List<Minion>(FindObjectsOfType<Minion>());	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		// Calculate and apply the flocking vector
		velocity = (velocity.normalized + Flocking_Update()).normalized * Speed;

		// Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + velocity, Color.white);

		// apply the velocity
		transform.position += velocity * Time.deltaTime;
	}

	protected Vector3 Flocking_Update() {
		// Calculate each of the vectors
		Vector3 cohesionVector = Flocking_CalculateCohesion(minions) * CohesionStrength;
		Vector3 separationVector = Flocking_CalculateSeparation(minions) * SeparationStrength;
		Vector3 alignmentVector = Flocking_CalculateAlignment(minions) * AlignmentStrength;

		//Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + cohesionVector, Color.red);
		Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + separationVector, Color.green);
		//Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + alignmentVector, Color.blue);

		// Calculate the flocking vector
		Vector3 flockingVector = cohesionVector + separationVector + alignmentVector;

		return flockingVector.normalized;
	}

	Vector3 Flocking_CalculateCohesion(List<Minion> flock)
    {
        Vector3 cohesionVector = Vector3.zero;
        Vector3 centreOfMass = Vector3.zero;

        for(int i = 0; i < flock.Count; i++)
        {
            centreOfMass += flock[i].transform.position;   
        }
        centreOfMass = centreOfMass / flock.Count;

        
        cohesionVector = (centreOfMass -this.transform.position).normalized;

		return cohesionVector;
	}

	Vector3 Flocking_CalculateSeparation(List<Minion> flock)
    {
        Vector3 separation = Vector3.zero;

        for (int i = 0; i < flock.Count; i++)
        {
            if (flock[i] != this)
            {
                if (Vector3.Distance(this.transform.position, flock[i].transform.position) < SeparationDistance)
                {
                    separation += (this.transform.position - flock[i].transform.position).normalized;
                }
            }
        }

        separation.y = 0;
        return separation.normalized;
        //return Vector3.zero;
	}

	Vector3 Flocking_CalculateAlignment(List<Minion> flock)
    {
        Vector3 alignment = Vector3.zero;

        foreach(Minion minion in flock)
        {
            alignment = minion.velocity;
        }

        alignment = alignment / flock.Count;

		return alignment;
	}
}
