// Copyright 2013, Catlike Coding, http://catlikecoding.com
using CatlikeCoding.NumberFlow;
using UnityEngine;
using UnityEngine.UI;

namespace CatlikeCoding.NumberFlow.Examples {
	
	/// <summary>
	/// Script that controls the Game of Life example.
	/// </summary>
	public class GameOfLifeController : MonoBehaviour {
		
		private const int resolution = 128;
		
		public Diagram lifeDiagram, seedDiagram;
		
		public Texture2D gun, growth;

		public Text infoText;
		
		private Texture2D texture;
		
		private Color[] pixels;
		
		private static string[] texts = {
			"The Gosper Glider Gun produces gliders, until it gets destroyed by them.",
			"Infinite growth spawned from a single partially-living row, until it wraps around and interferes with itself.",
			"The initial generation is created with perlin noise."
		};
		
		public void InitializeGosperGliderGun () {
			texture.SetPixels32(gun.GetPixels32());
			texture.Apply();
			infoText.text = texts[0];
		}

		public void InitializeGrowth () {
			texture.SetPixels32(growth.GetPixels32());
			texture.Apply();
			infoText.text = texts[1];
		}

		public void InitializeRandom () {
			seedDiagram.GetInputValue("Offset").Float = Time.realtimeSinceStartup;
			seedDiagram.Fill(pixels, resolution, resolution);
			texture.SetPixels(pixels);
			texture.Apply();
			infoText.text = texts[2];
		}
		
		void Start () {
			texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
			texture.filterMode = FilterMode.Point;
			pixels = new Color[resolution * resolution];
			gameObject.GetComponent<GUITexture>().texture = texture;
			
			Value cells = lifeDiagram.GetInputValue("Cells");
			cells.Pixels.texture = texture;
			InitializeRandom();
		}
		
		void Update () {
			lifeDiagram.Fill(pixels, resolution, resolution);
			texture.SetPixels(pixels);
			texture.Apply();
		}
	}
}