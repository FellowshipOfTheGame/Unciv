// Copyright 2014, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;
using System.Collections;
using System.Threading;

namespace CatlikeCoding.NumberFlow {

	/// <summary>
	/// Manager for diagram and material links.
	/// </summary>
	[ExecuteInEditMode]
	public sealed class DiagramMaterialManager : MonoBehaviour {

		/// <summary>
		/// Whether to use a thread to generate the textures, instead of doing it all in Start on the main thread.
		/// </summary>
		public bool useThread;

		/// <summary>
		/// Links between diagrams and materials.
		/// </summary>
		public DiagramMaterialLink[] links;

		/// <summary>
		/// Whether the manager is finished generating its textures.
		/// </summary>
		[NonSerialized]
		public bool isFinishedGenerating;

		private Color[][] buffers = new Color[0][];

		private Thread thread;
		private bool threadFinished;
		private DiagramMaterialLink currentLink;
		private AutoResetEvent threadShouldBeWorking;

		/// <summary>
		/// Occurs when all textures have been generated.
		/// Subscribers are automatically removed.
		/// Will immediately call back on add if textures have already been generated.
		/// </summary>
		public event Action onFinishedGenerating {
			add {
				if (isFinishedGenerating) {
					value();
				}
				else {
					onFinished += value;
				}
			}
			remove {
				if (onFinished != null) {
					onFinished -= value;
				}
			}
		}

		private Action onFinished;

		private void Start () {
			#if UNITY_EDITOR
				if (!Application.isPlaying) {
					return;
				}
			#endif

			if (useThread) {
				StartCoroutine(FillTexturesThreaded());
			}
			else {
				for (int i = 0; i < links.Length; i++) {
					links[i].AllocateTextures();
					links[i].Process(ref buffers);
					links[i].ApplyTextures(buffers);
					links[i].AssignTexturesToMaterials();
				}
				FinishGenerating();
			}
		}

#if UNITY_EDITOR
		private void Reset () {
			useThread = true;
			links = new DiagramMaterialLink[0];
		}

		private void OnEnable () {
			if (Application.isPlaying) {
				return;
			}
			editorUpdater = EditorUpdate();
			UnityEditor.EditorApplication.update += EditorApplicationUpdate;
		}

		private void EditorApplicationUpdate () {
			if (!editorUpdater.MoveNext() &&
			    links.Length > 0 &&
			    links[0].materials.Length > 0 &&
			    links[0].materials[0] != null &&
			    links[0].materials[0].mainTexture == null) {
				// Materials lost textures, most likely due to a scene save action.
				for (int i = 0; i < links.Length; i++) {
					links[i].AssignTexturesToMaterials();
				}
			}
		}

		private IEnumerator editorUpdater;

		private IEnumerator EditorUpdate () {
			yield return null;
			for (int i = 0; i < links.Length; i++) {
				DiagramMaterialLink link = links[i];
				if (link.diagram != null) {
					int rowIndex = 0, rowCount = 0;
					link.AllocateTextures();
					link.InitBuffers(ref buffers);
					yield return null;
					float previousTime = 0f;
					while (!link.FillRows(buffers, ref rowIndex, rowCount)) {
						float now = Time.realtimeSinceStartup;
						float deltaTime = now - previousTime;
						previousTime = now;
						if (rowCount <= 0) {
							// We took a break or just started.
							rowCount = 1;
						}
						else if (deltaTime < 0.05f) {
							// We can speed up.
							rowCount *= 2;
						}
						else if (deltaTime > 0.1f) {
							// Emergency stop.
							rowCount = 0;
						}
						else if (deltaTime > 0.05f) {
							// Slow down.
							rowCount /= 2;
						}
						yield return null;
					}
					link.PostProcess(buffers);
					link.ApplyTextures(buffers);
					link.AssignTexturesToMaterials();
				}
				yield return null;
			}
		}
#endif

		private void OnDisable () {
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				UnityEditor.EditorApplication.update -= EditorApplicationUpdate;
			}
			else if (thread != null) {
				// Make sure the thread is no longer alive before textures are filled in edit mode.
				thread.Abort();
				thread.Join();
			}
#endif
			for (int i = 0; i < links.Length; i++) {
				links[i].OnDestroy();
			}
		}

		private IEnumerator FillTexturesThreaded () {
			threadShouldBeWorking = new AutoResetEvent(false);
			thread = new Thread(FillTextures);
			thread.IsBackground = true;
			thread.Start();
			for (int i = 0; i < links.Length; i++) {
				currentLink = links[i];
				currentLink.AllocateTextures();
				threadFinished = false;
				threadShouldBeWorking.Set();
				while (!threadFinished) {
					yield return null;
				}
				currentLink.ApplyTextures(buffers);
				currentLink.AssignTexturesToMaterials();
			}
			currentLink = null;
			threadShouldBeWorking.Set();
			FinishGenerating();
		}

		private void FinishGenerating () {
			buffers = null;
			isFinishedGenerating = true;
			if (onFinished != null) {
				onFinished();
				onFinished = null;
			}
		}

		private void FillTextures () {
			while (true) {
				threadShouldBeWorking.WaitOne();
				if (currentLink == null) {
					threadFinished = true;
					return;
				}
				currentLink.Process(ref buffers);
				threadFinished = true;
			}
		}
	}
}