using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	public float maximumRange = 4f;
	public float minimumFieldStrength = 1f;
	public float maximumFieldStrength = 4f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Vector3 CalculateField(Vector3 target)
	{
		// calculate the 2D vector to the target
		Vector3 distanceVector = target - transform.position;
		distanceVector.y = 0;

		// calculate the magnitude
		float distance = distanceVector.magnitude;

		// outside of max range?
		if (distance >= maximumRange)
			return Vector3.zero;

		// return the field
		float normalisedDistance = Mathf.Clamp01(distance / maximumRange);
		normalisedDistance *= normalisedDistance;
		return distanceVector.normalized * Mathf.Lerp(minimumFieldStrength, maximumFieldStrength, 1f - normalisedDistance);
	}
}
