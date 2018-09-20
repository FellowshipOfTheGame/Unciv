// Copyright 2014, Catlike Coding, http://catlikecoding.com
using CatlikeCoding.Noise;
using UnityEngine;
using UnityEngine.UI;

public class NoiseExample : MonoBehaviour {

	private static string[] noiseOptions = {
		"Perlin", "Turbulence", "Value", "Voronoi"
	};

	private static string[] voronoiTypeOptions = {
		"Linear", "Squared", "Manhattan", "Chebyshev"
	};

	private static string[] voronoiOutputOptions = {
		"F1", "F2", "½(F1 + F2)", "F1 × F2", "F2 - F1", "Hash"
	};

	public int resolution = 200;

	public Transform sunRotater;

	public GameObject voronoiUI;

	public Text noiseLabel;

	private float
		frequency = 5f,
		lacunarity = 2f,
		persistence = 0.5f;

	private int octaves = 2;

	private const float height = 0.2f;
	private float zOffset = 0f;
	
	private int selectedNoise, selectedVoronoiType, selectedVoronoiOutput;

	private Mesh mesh;
	private Vector3[] vertices, voronoiBuffer;
	private float[] boxBlurBuffer;

	private bool needsUpdate;

	private Transform rotater;

	void Start () {
		rotater = transform;
		GenerateMesh();
		GenerateNoise();
		UpdateNoiseLabel();
	}

	public void SetNoiseType (int type) {
		selectedNoise = type;
		GenerateNoise();
		voronoiUI.SetActive(selectedNoise == 3);
		UpdateNoiseLabel();
	}

	public void SetVoronoiType (int type) {
		selectedVoronoiType = type;
		GenerateNoise();
		UpdateNoiseLabel();
	}

	public void SetVoronoiOutputType (int type) {
		selectedVoronoiOutput = type;
		GenerateNoise();
		UpdateNoiseLabel();
	}

	public void SetFrequency (float frequency) {
		this.frequency = frequency;
		GenerateNoise();
	}

	public void SetOctaves (float octaves) {
		this.octaves = (int)octaves;
		GenerateNoise();
	}

	public void SetLacunarity (float lacunarity) {
		this.lacunarity = lacunarity;
		GenerateNoise();
	}

	public void SetPersistence (float persistence) {
		this.persistence = persistence;
		GenerateNoise();
	}

	public void SetZOffset (float zOffset) {
		this.zOffset = zOffset;
		GenerateNoise();
	}

	void Update () {
		if (needsUpdate) {
			GenerateNoise();
			needsUpdate = false;
		}

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			if (Input.GetKey(KeyCode.RightArrow)) {
				sunRotater.Rotate(0f, -45f * Time.deltaTime, 0f);
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
				sunRotater.Rotate(0f, 45f * Time.deltaTime, 0f);
			}
		}
		else {
			if (Input.GetKey(KeyCode.RightArrow)) {
				rotater.Rotate(0f, 45f * Time.deltaTime, 0f);
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
				rotater.Rotate(0f, -45f * Time.deltaTime, 0f);
			}
		}
	}

	private void UpdateNoiseLabel () {
		string text = noiseOptions[selectedNoise];
		if (selectedNoise == 3) {
			text += " " + voronoiTypeOptions[selectedVoronoiType] + " " + voronoiOutputOptions[selectedVoronoiOutput];
		}
		noiseLabel.text = text;
	}

	private void GenerateNoise () {
		switch (selectedNoise) {
		case 0: GeneratePerlin(); break;
		case 1: GenerateTurbulence(); break;
		case 2: GenerateValue(); break;
		case 3: GenerateVoronoi(); break;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	private void GeneratePerlin () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			vertices[i].y = height * PerlinNoise.Sample3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateTurbulence () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			vertices[i].y = height * PerlinNoise.SampleTurbulence3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateValue () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			vertices[i].y = height * ValueNoise.Sample3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateVoronoi () {
		if (voronoiBuffer == null) {
			voronoiBuffer = new Vector3[vertices.Length];
		}
		switch (selectedVoronoiType) {
		case 0: GenerateVoronoiLinear(); break;
		case 1: GenerateVoronoiSquared(); break;
		case 2: GenerateVoronoiManhattan(); break;
		case 3: GenerateVoronoiChebyshev(); break;
		}
		switch (selectedVoronoiOutput) {
		case 0:
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i].y = height * voronoiBuffer[i].x;
			}
			break;
		case 1:
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i].y = height * voronoiBuffer[i].y;
			}
			break;
		case 2:
			for (int i = 0; i < vertices.Length; i++) {
				Vector3 v = voronoiBuffer[i];
				vertices[i].y = height * 0.5f * (v.x + v.y);
			}
			break;
		case 3:
			for (int i = 0; i < vertices.Length; i++) {
				Vector3 v = voronoiBuffer[i];
				vertices[i].y = height * v.x * v.y;
			}
			break;
		case 4:
			for (int i = 0; i < vertices.Length; i++) {
				Vector3 v = voronoiBuffer[i];
				vertices[i].y = height * (v.y - v.x);
			}
			break;
		case 5:
			for (int i = 0; i < vertices.Length; i++) {
				vertices[i].y = height * voronoiBuffer[i].z;
			}
			break;
		}
	}

	private void GenerateVoronoiLinear () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			voronoiBuffer[i] = VoronoiNoise.SampleLinear3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateVoronoiSquared () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			voronoiBuffer[i] = VoronoiNoise.SampleSquared3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateVoronoiManhattan () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			voronoiBuffer[i] = VoronoiNoise.SampleManhattan3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	private void GenerateVoronoiChebyshev () {
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v = vertices[i];
			v.y = zOffset;
			voronoiBuffer[i] = VoronoiNoise.SampleChebyshev3D(v, frequency, octaves, lacunarity, persistence);
		}
	}

	public void BoxBlur () {
		// Perform a box blur. Simply average 3x3 blocks.
		if (boxBlurBuffer == null) {
			boxBlurBuffer = new float[vertices.Length];
		}
		for (int i = 0, z = 0; z < resolution; z++) {
			for (int x = 0; x < resolution; x++, i++) {
				boxBlurBuffer[i] = (
					GetValue(x - 1, z - 1) + GetValue(x, z - 1) + GetValue(x + 1, z - 1) +
					GetValue(x - 1, z) + GetValue(x, z) + GetValue(x + 1, z) +
					GetValue(x - 1, z + 1) + GetValue(x, z + 1) + GetValue(x + 1, z + 1)
					) * (1f / 9f);
			}
		}
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i].y = boxBlurBuffer[i];
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	private float GetValue (int x, int z) {
		if (x < 0) {
			x = 0;
		}
		else if (x >= resolution) {
			x = resolution - 1;
		}
		if (z < 0) {
			z = 0;
		}
		else if (z >= resolution) {
			z = resolution - 1;
		}
		return vertices[z * resolution + x].y;
	}

	private void GenerateMesh () {
		// Generate a flat triangle grid.
		mesh = new Mesh();
		vertices = new Vector3[resolution * resolution];

		float delta = 1f / resolution;
		Vector3 v = Vector3.zero;
		for (int i = 0, z = 0; z < resolution; z++) {
			v.z = z * delta - 0.5f;
			for (int x = 0; x < resolution; x++, i++) {
				v.x = x * delta - 0.5f;
				vertices[i] = v;
			}
		}

		int triangleSize = resolution - 1;
		int[] triangles = new int[triangleSize * triangleSize * 6];
		for (int t = 0, i = 0, z = 0; z < triangleSize; z++) {
			for (int x = 0; x < triangleSize; x++, i++, t += 6) {
				triangles[t] = i;
				triangles[t + 1] = triangles[t + 4] = i + resolution;
				triangles[t + 2] = triangles[t + 3] = i + 1;
				triangles[t + 5] = i + resolution + 1;
			}
			i += 1;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
