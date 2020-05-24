using UnityEngine;
using System;

public class WheelDrive : MonoBehaviour
{
	public float maxAngle = 10f;
	
	[Tooltip("Maximum steering angle of the wheels")]
	public float maxRotationTorque = 300f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 60000f;

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;
	private WheelCollider[] m_Wheels;

	private Rigidbody rb;

	private bool isForward;

	private bool isVehicleForward;

    // Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		isForward = true;
		rb = GetComponent<Rigidbody>();
		m_Wheels = GetComponentsInChildren<WheelCollider>();
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);
	}

	void Update()
	{
		var vertInput = Input.GetAxis("Vertical");
		float angle = maxAngle * Input.GetAxis("Horizontal");
		float rotationTorque = maxRotationTorque * Input.GetAxis("Horizontal");
		float torque = maxTorque * vertInput;
		if (vertInput > 0)
		{
			isForward = true;
		}

		if (vertInput < 0)
		{
			isForward = false;
		}
		
		var velocity = rb.velocity;
		var localVel = transform.InverseTransformDirection(velocity);
		isVehicleForward = localVel.z > 0f;

		float handBrake = Input.GetKey(KeyCode.Space) ? brakeTorque : 0;

		foreach (WheelCollider wheel in m_Wheels)
		{
			var rightTorque = 0f;
			var leftTorque = 0f;
			
			rightTorque = torque + rotationTorque;
			leftTorque = torque - rotationTorque;
			if (rightTorque > maxTorque)
			{
				leftTorque -= rightTorque - maxTorque;
				rightTorque = maxTorque;
			}
			
			if (rightTorque < -maxTorque)
			{
				leftTorque -= rightTorque + maxTorque;
				rightTorque = -maxTorque;
			}
			
			if (leftTorque > maxTorque)
			{
				rightTorque -= leftTorque - maxTorque;
				leftTorque = maxTorque;
			}
			
			if (leftTorque < -maxTorque)
			{
				rightTorque -= leftTorque + maxTorque;
				leftTorque = -maxTorque;
			}

			if (wheel.transform.localPosition.z > 0)
			{
				wheel.steerAngle = angle;
			}
			
			if (wheel.transform.localPosition.x > 0)
			{
				wheel.motorTorque = isForward ? leftTorque : rightTorque;
			}
			
			if (wheel.transform.localPosition.x < 0)
			{
				wheel.motorTorque = isForward ? rightTorque : leftTorque;
			}

			wheel.brakeTorque = (isVehicleForward != isForward) && Math.Abs(localVel.z) >= 1 ? brakeTorque : handBrake;
		}
	}
	
	void OnGUI()
	{
		foreach (WheelCollider wheel in m_Wheels) {
			GUILayout.Label (string.Format("Torque: {0}", wheel.motorTorque));
		}
		GUILayout.Label (string.Format("Vehicle forward: {0}", isVehicleForward));
		GUILayout.Label (string.Format("Push forward: {0}", isForward));
	}
}
