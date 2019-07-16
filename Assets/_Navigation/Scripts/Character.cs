using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : Minion {
	protected List<GameObject> markerPoints;
	protected int currentMarker = -1;

	protected List<Obstacle> allObstacles;

	protected Rigidbody rb;

	public float MarkerReachedThreshold = 0.1f;

	public float PotentialFieldContribution = 0.4f;

	public float MinSpeedPercentage = 0.1f;
	public float MaxSpeedPercentage = 1f;
	public float MinSpeedDistance = 0.5f;
	public float MaxSpeedDistance = 2f;

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

		// find all of the marker points
		markerPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Marker"));	

		// find all of the obstacles
		allObstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());

		// retrieve the rigid body
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		Update_Navigation();
	}

	protected void Update_Navigation()
	{
		// no markers?
		if ((markerPoints == null) || (markerPoints.Count == 0))
			return;

		// do we have a current marker? if not start at a random one
		if (currentMarker < 0)
			currentMarker = Random.Range(0, markerPoints.Count);

		// have we reached the current marker?
		Vector2 distanceToMarker = new Vector2(markerPoints[currentMarker].transform.position.x - transform.position.x,
											   markerPoints[currentMarker].transform.position.z - transform.position.z);
		if (distanceToMarker.magnitude < MarkerReachedThreshold)
		{
			// pick a new marker - could pick the same one but we'll take the chance
			currentMarker = Random.Range(0, markerPoints.Count);
		}

	}

	void FixedUpdate()
	{
		Update_Steering();
	}

	protected void Update_Steering()
	{
		// no markers? or current point?
		if ((markerPoints == null) || (markerPoints.Count == 0) || (currentMarker < 0))
			return;

		// calculate the vector to the marker
		Vector2 vectorToMarker = new Vector2(markerPoints[currentMarker].transform.position.x - transform.position.x,
										     markerPoints[currentMarker].transform.position.z - transform.position.z);
		
		// store distance to marker
		float distToMarker = vectorToMarker.magnitude;

		// calculate the desired velocity
		Vector3 desiredVelocity = new Vector3(vectorToMarker.x, 0, vectorToMarker.y);
		desiredVelocity.Normalize();

		// calculate the potential field
		Vector3 potentialField = CalculatePotentialField();

		Debug.DrawLine(transform.position, markerPoints[currentMarker].transform.position, Color.white);
		Debug.DrawRay(transform.position + Vector3.up * 0.5f, potentialField.normalized*2f, Color.red);
		Debug.DrawRay(transform.position + Vector3.up, desiredVelocity.normalized*2f, Color.green);

		// blend between desired velocity and potential field
		desiredVelocity = (desiredVelocity * (1f - PotentialFieldContribution)) + 
						  (potentialField * PotentialFieldContribution);

		// slew the character's rotation around into the direction of movement
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(desiredVelocity, Vector3.up), 60f * Time.deltaTime);

		// lerp the speed between our ranges
		float distancePercentage = Mathf.Clamp01((distToMarker - MinSpeedDistance) / (MaxSpeedDistance - MinSpeedDistance));
		float speedScale = Mathf.Lerp(MinSpeedPercentage, MaxSpeedPercentage, distancePercentage);

		// apply the desired velocity
		rb.velocity = transform.forward * Speed * speedScale;
		velocity = rb.velocity;
	}

	protected Vector3 CalculatePotentialField()
	{
		Vector3 calculatedField = Vector3.zero;

		// update the field based on all of the obstacles
		foreach(Obstacle obstacle in allObstacles)
		{
			calculatedField += obstacle.CalculateField(transform.position);
		}

		return calculatedField;
	}
}
