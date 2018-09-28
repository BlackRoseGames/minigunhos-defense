using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class PlatformController : RaycastController {

	public Spline m_MySpline;
	public float m_Duration = 2;
	private Vector3 m_Velocity;
	private Transform referenceTransform;
	private Vector3 m_OldPosition;

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform,Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

	public override void Start ()
	{
		base.Start();
		referenceTransform = transform;
		Tween.Spline (m_MySpline, referenceTransform, 0, 1, false, m_Duration, 0, Tween.EaseInOut, Tween.LoopType.PingPong);
	}

	void Update() {
		UpdateRaycastOrigins();
		m_Velocity = (referenceTransform.position - m_OldPosition);
		m_OldPosition = transform.position;

		CalculatePassengerMovement(m_Velocity);

		MovePassengers (true);
		transform.position = referenceTransform.position;
		MovePassengers (false);
	}

	void MovePassengers(bool beforeMovePlatform) {
		foreach (PassengerMovement passenger in passengerMovement) {
			if (!passengerDictionary.ContainsKey(passenger.transform)) {
				passengerDictionary.Add(passenger.transform,passenger.transform.GetComponent<Controller2D>());
			}

			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity) {
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// Vertically moving platform
		if (velocity.y != 0) {
			float rayLength = Mathf.Abs (velocity.y) + k_SkinWidth;
			
			for (int i = 0; i < m_VerticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)?m_RaycastOrigins.bottomLeft:m_RaycastOrigins.topLeft;
				rayOrigin += Vector2.right * (m_VerticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_CollisionMask);

				Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

				if (hit) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = (directionY == 1)?velocity.x:0;
						float pushY = velocity.y - (hit.distance - k_SkinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, true));
					}
				}
			}
		}

		// Horizontally moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + k_SkinWidth;
			
			for (int i = 0; i < m_HorizontalRayCount; i ++) {
				Vector2 rayOrigin = (directionX == -1)?m_RaycastOrigins.bottomLeft:m_RaycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (m_HorizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_CollisionMask);

				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

				if (hit) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x - (hit.distance - k_SkinWidth) * directionX;
						float pushY = -k_SkinWidth;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), false, true));
					}
				}
			}
		}

		// Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = k_SkinWidth * 2;
			
			for (int i = 0; i < m_VerticalRayCount; i ++) {
				Vector2 rayOrigin = m_RaycastOrigins.topLeft + Vector2.right * (m_VerticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, m_CollisionMask);
				
				if (hit) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), true, false));
					}
				}
			}
		}
	}

	struct PassengerMovement {
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}
}
