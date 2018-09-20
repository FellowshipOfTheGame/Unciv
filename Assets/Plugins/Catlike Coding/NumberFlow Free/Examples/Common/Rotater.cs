// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Examples {
	
	/// <summary>
	/// Helper script used to rotate the camera, sun, and spotlight.
	/// </summary>
	public class Rotater : MonoBehaviour {
		
		/// <summary>
		/// Angle to use for the X rotation.
		/// </summary>
		public float angle;
		
		/// <summary>
		/// How fast to change the Y rotation.
		/// </summary>
		public float speed;
		
		private float rotation;
		
		private new Transform transform;
		
		void Awake () {
			// Cache the transform reference.
			transform = base.transform;
		}
		
		void Update () {
			rotation += Time.deltaTime * speed;
			transform.localRotation = Quaternion.Euler(angle, rotation, 0f);
		}
	}
}