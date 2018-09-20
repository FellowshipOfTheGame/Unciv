using UnityEngine;
using System.Collections;

namespace CatlikeCoding.NumberFlow.Examples {

	public class MultisceneSwitcher : MonoBehaviour {

		public string nextSceneName;

		void Update () {
			if (Time.timeSinceLevelLoad > 2f) {
				Application.LoadLevel(nextSceneName);
			}
		}
	}
}