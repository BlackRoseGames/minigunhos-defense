using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {

	[Range(0, 10f)] [SerializeField] private float m_WalkSpeed = 6f;
	[Range(0, 1f)] [SerializeField] private float m_TimeToJumpApex = 0.4f;
	[Range(0, 10f)] [SerializeField] private float m_JumpHeight = 4;
	[Range(0, 1f)] [SerializeField] private float WalkSmoothing = 0.1f;
	[Range(0, 1f)] [SerializeField] private float m_AccelerationTimeAirborne = 0.2f;
	[Range(0, 1f)] [SerializeField] private float m_AccelerationTimeWalking = 0.1f;

	private float m_WalkVelocitySmoothing;
	private float m_Gravity;
	private float m_JumpVelocity;
	private Controller2D m_Controller;
	private Vector3 m_Velocity;

	void Start() {
		m_Controller = GetComponent<Controller2D>();
		CalculateGravity();
	}

	void CalculateGravity() {
		m_Gravity = -(2 * m_JumpHeight) / Mathf.Pow(m_TimeToJumpApex, 2);
		m_JumpVelocity = Mathf.Abs(m_Gravity * m_TimeToJumpApex);
	}

	void Update () {
		if (m_Controller.collisions.above || m_Controller.collisions.below) {
			m_Velocity.y = 0;
		}

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetButtonDown("Jump") && m_Controller.collisions.below) {
			m_Velocity.y = m_JumpVelocity;
		}
		float targetVelocity = input.x * m_WalkSpeed;
		m_Velocity.x = Mathf.SmoothDamp(m_Velocity.x, targetVelocity, ref m_WalkVelocitySmoothing, (m_Controller.collisions.below?m_AccelerationTimeWalking:m_AccelerationTimeAirborne));
		m_Velocity.y += m_Gravity * Time.deltaTime;
		m_Controller.Move(m_Velocity * Time.deltaTime);
	}
}
