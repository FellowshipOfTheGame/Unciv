// Copyright 2014, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System.Collections.Generic;

namespace CatlikeCoding.Utilities {

	/// <summary>
	/// Makes sure the game object isn't destroyed when another scene is loaded.
	/// Can be configured to make sure no two such objects with the same name exist.
	/// </summary>
	/// <description>
	/// To guarantee that duplicates are destroyed, place this script at the top of the script execution order.
	/// </description>
	public class ScenePersistentObject : MonoBehaviour {

		/// <summary>
		/// Whether only one version of this object should exist.
		/// </summary>
		public bool onlyOnce;

		private static List<string> currentObjectNames;

		void Awake () {
			if (onlyOnce) {
				if (currentObjectNames == null) {
					currentObjectNames = new List<string>();
				}
				if (!currentObjectNames.Contains(name)) {
					currentObjectNames.Add(name);
				}
				else {
					onlyOnce = false;
					gameObject.SetActive(false);
					Destroy(gameObject);
					return;
				}
			}
			DontDestroyOnLoad(gameObject);
		}

		void OnDestroy () {
			if (onlyOnce) {
				currentObjectNames.Remove(name);
			}
		}

#if UNITY_EDITOR
		void Reset () {
			onlyOnce = true;
		}
#endif

	}
}