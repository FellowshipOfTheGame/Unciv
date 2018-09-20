// Copyright 2013, Catlike Coding, http://catlikecoding.com
using CatlikeCoding.Utilities;
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow.Examples {

	/// <summary>
	/// Script that controls the terrain example.
	/// </summary>
	public class TerrainController : MonoBehaviour {
		
		public int resolution = 64;
		
		public Diagram colorMapDiagram, heightFieldDiagram;
		
		public Rotater cameraRotater, spotRotater, sunRotater;
		
		public MeshController terrainMesh;
		
		private Value morphValue, octavesValue, persistenceValue, secondary;
		
		private float morphSpeed;
		
		private Texture2D terrainTexture;
		
		private Color[] pixels;
		
		private Light spotLight;
		
		private Material terrainMaterial, waterMaterial;
		
		void Start () {
			spotLight = spotRotater.GetComponent<Light>();
			pixels = new Color[resolution * resolution];

			// Generate color map.
			Texture2D colorMapTexture = new Texture2D(32, 1, TextureFormat.ARGB32, false);
			colorMapTexture.wrapMode = TextureWrapMode.Clamp;
			colorMapDiagram.Fill(pixels, 32, 1);
			colorMapTexture.SetPixels(pixels);
			colorMapTexture.Apply();
			
			terrainTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
			terrainTexture.wrapMode = TextureWrapMode.Repeat;
			terrainTexture.name = "Dynamic Terrain";
			terrainMesh.Generate(resolution);
			
			// Fetch materials, set textures, and read initial settings.
			terrainMaterial = terrainMesh.materials[0];
			waterMaterial = terrainMesh.materials[1];
			
			waterMaterial.mainTexture = terrainMaterial.mainTexture = terrainTexture;
			terrainMaterial.SetTexture("_ColorMap", colorMapTexture);
			
			morphValue = heightFieldDiagram.GetInputValue("Morph");
			octavesValue = heightFieldDiagram.GetInputValue("Octaves");
			persistenceValue = heightFieldDiagram.GetInputValue("Persistence");
			secondary = heightFieldDiagram.GetInputValue("Secondary");
		}
		
		void Update () {
			#if UNITY_EDITOR
				// Fetch inputs again, because caching fails when the diagram is shown in the editor window.
				morphValue = heightFieldDiagram.GetInputValue("Morph");
				octavesValue = heightFieldDiagram.GetInputValue("Octaves");
				persistenceValue = heightFieldDiagram.GetInputValue("Persistence");
				secondary = heightFieldDiagram.GetInputValue("Secondary");
			#endif

			// Recreate the texture each frame. No checks whether it is required.
			morphValue.Float += morphSpeed * Time.deltaTime;
			heightFieldDiagram.Fill(pixels, resolution, resolution);
			heightFieldDiagram.PostProcess(pixels, resolution, resolution, DiagramNormalFormat.ARGB);
			terrainMesh.SetHeight(pixels, resolution);
			terrainTexture.SetPixels(pixels);
			terrainTexture.Apply();
		}

		public void SetSunAngle (float angle) {
			sunRotater.angle = angle;
		}

		public void SetSunOrbitSpeed (float speed) {
			sunRotater.speed = speed;
		}

		public void SetSpotlightIntensity (float intensity) {
			spotLight.intensity = intensity;
		}

		public void SetSpotlightOrbitSpeed (float speed) {
			spotRotater.speed = -speed;
		}

		public void SetCameraOrbitSpeed (float speed) {
			cameraRotater.speed = speed;
		}

		public void SetTiling (bool tile) {
			terrainMesh.tile = tile;
		}

		public void SetMorphSpeed (float speed) {
			morphSpeed = speed;
		}

		public void SetOctaves (float octaves) {
			octavesValue.Int = (int)octaves;
		}

		public void SetPersistence (float persistence) {
			persistenceValue.Float = persistence;
		}

		public void SetSecondary (float strength) {
			secondary.Float = strength;
		}

		public void SetBumpDetail (float detail) {
			terrainMaterial.SetFloat("_Detail", detail);
		}

		public void SetColorDetail (float detail) {
			terrainMaterial.SetFloat("_ColorDetail", detail);
			waterMaterial.SetFloat("_ColorDetail", detail);
		}

		public void SetSlope (float slope) {
			terrainMaterial.SetFloat("_Slope", slope);
		}

		public void SetSnow (float snow) {
			terrainMaterial.SetFloat("_Snow", snow);
		}

		public void SetLighting (float lighting) {
			terrainMaterial.SetFloat("_Lighting", lighting);
			waterMaterial.SetFloat("_Lighting", lighting);
		}
	}
}