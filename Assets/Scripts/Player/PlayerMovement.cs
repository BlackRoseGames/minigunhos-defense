using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {

	[Range(0, 10)] [SerializeField] private float walkSpeed = 6f;
	[Range(-20, 0)] [SerializeField] private float gravity = -20f;
	protected Controller2D controller;
	protected Vector3 velocity;
	protected bool jump = false;

	void Start() {
		controller = GetComponent<Controller2D>();
	}

	void Update () {
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		velocity.x = input.x * walkSpeed;
		/*
		if (Input.GetButtonDown("Jump")) {
			jump = true;
		}
		 */

	}

	void FixedUpdate () {
		
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
	}
}
