// Copyright 2014, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Examples {

	public class GalleryVisitor : MonoBehaviour {

		public Transform head, body;

		public Vector2 sensitivity = new Vector2(15f, 15f);

		public float maxVerticalAngle = 60f;

		public float speed = 10f;

		private float verticalAngle;

		private CharacterController characterController;

		void Awake () {
			characterController = GetComponent<CharacterController>();
		}

		void Update () {
			Cursor.lockState = CursorLockMode.Locked;

			body.Rotate(0f, Input.GetAxis("Mouse X") * sensitivity.x, 0f);
			verticalAngle -= Input.GetAxis("Mouse Y") * sensitivity.y;
			verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);
			head.localEulerAngles = new Vector3(verticalAngle, 0f);

			Vector3 movement = body.forward * Input.GetAxis("Vertical") + body.right * Input.GetAxis("Horizontal");
			float sqrMagnitude = movement.sqrMagnitude;
			if (sqrMagnitude > 1f) {
				// Make sure diagonal movement isn't too fast when using arrows.
				movement /= Mathf.Sqrt(sqrMagnitude);
			}
			characterController.SimpleMove(movement * speed);
		}
	}
}