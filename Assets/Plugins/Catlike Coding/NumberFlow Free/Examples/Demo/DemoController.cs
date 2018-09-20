// Copyright 2013, Catlike Coding, http://catlikecoding.com
using CatlikeCoding.NumberFlow;
using CatlikeCoding.Utilities;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace CatlikeCoding.NumberFlow.Examples {
	
	/// <summary>
	/// Script that controls the Demo.
	/// </summary>
	public class DemoController : MonoBehaviour {
		
		private const int BL = 0, BR = 1, TL = 2, TR = 3, C = 4;
		
		public Diagram[] diagrams;
		
		public string[] descriptions;

		public Text infoText;
		
		private GUITexture[] uiTextures;

		private int currentSize, newSize;
		private Diagram diagram;
		private Value timeValue, tweakValue;
		private float tweak = 0f;
		private bool animate = true;

		private Texture2D texture;
		private Color[] pixels;

		private Thread thread;
		private AutoResetEvent textureFilled;
		private bool pixelsReady;

		void Start () {
			uiTextures = new GUITexture[5];
			for (int i = 0; i < 5; i++) {
				GameObject o = new GameObject("Texture Display");
				o.transform.localScale = Vector3.zero;
				o.transform.parent = gameObject.transform;
				uiTextures[i] = o.AddComponent<GUITexture>();
			}
			
			currentSize = newSize = 64;
			SetInsets();
			pixels = new Color[currentSize * currentSize];
			texture = new Texture2D(currentSize, currentSize, TextureFormat.RGB24, false);
			texture.SetPixels(pixels);
			texture.Apply();
			Vector3 textureCenter = new Vector3((200f * 0.5f) / Screen.width + 0.5f, 0.5f);
			for (int i = 0; i < uiTextures.Length; i++) {
				uiTextures[i].texture = texture;
				uiTextures[i].transform.localPosition = textureCenter;
			}
			SetTile(false);
			SetDiagram(0);

			PrepareMonaDiagrams();

			textureFilled = new AutoResetEvent(false);
			thread = new Thread(new ThreadStart(FillPixels));
			thread.IsBackground = true;
			thread.Start();
		}

		public void SetSize (int size) {
			newSize = size;
		}

		public void SetAnimation (bool animate) {
			this.animate = animate;
		}

		public void SetTile (bool tile) {
			uiTextures[BL].enabled = uiTextures[BR].enabled = uiTextures[TL].enabled = uiTextures[TR].enabled = tile;
			uiTextures[C].enabled = !tile;
		}

		public void SetTweak (float tweak) {
			this.tweak = tweak;
		}

		public void SetDiagram (int index) {
			diagram = diagrams[index];
			timeValue = diagram.GetInputValue("Time");
			tweakValue = diagram.GetInputValue("Tweak");
			infoText.text = descriptions[index];
		}
		
		private void SetInsets () {
			uiTextures[BL].pixelInset = new Rect(-currentSize, -currentSize, currentSize, currentSize);
			uiTextures[BR].pixelInset = new Rect(0f, -currentSize, currentSize, currentSize);
			uiTextures[TL].pixelInset = new Rect(-currentSize, 0f, currentSize, currentSize);
			uiTextures[TR].pixelInset = new Rect(0f, 0f, currentSize, currentSize);
			uiTextures[C].pixelInset = new Rect(currentSize / -2, currentSize / -2, currentSize, currentSize);
		}
		
		private void PrepareMonaDiagrams () {
			// Use color array instead of texture for Mona diagrams, so they work in a separate thread.
			int width = 0, height = 0;
			Color[] colors = null;
			for (int i = 0; i < diagrams.Length; i++) {
				if (diagrams[i].name.StartsWith("Mona ")) {
					Value value = diagrams[i].GetInputValue("Texture");
					if (colors == null) {
						Texture2D t = value.Pixels.texture;
						colors = t.GetPixels();
						width = t.width;
						height = t.height;
					}
					value.Pixels.Init(width, height, colors);
				}
			}
		}
		
		void Update () {
			if (pixelsReady) {
				// FillPixels is done and waiting.
				if (currentSize != newSize) {
					currentSize = newSize;
					SetInsets();
					texture.Resize(currentSize, currentSize);
					pixels = new Color[currentSize * currentSize];
				}
				texture.SetPixels(pixels);
				pixelsReady = false;
				texture.Apply();
				if (animate && timeValue != null) {
					timeValue.Float = Time.realtimeSinceStartup;
				}
				if (tweakValue != null) {
					tweakValue.Float = tweak;
				}
				// Signal FillPixels to continue.
				textureFilled.Set();
			}
		}

		private void FillPixels () {
			// This method runs in a separate thread.
			while (true) {
				diagram.Fill(pixels, currentSize, currentSize);
				pixelsReady = true;
				// Wait for signal from Update.
				textureFilled.WaitOne();
			}
		}
	}
}