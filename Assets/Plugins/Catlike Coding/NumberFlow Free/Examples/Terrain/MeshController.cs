// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Examples {
	
	/// <summary>
	/// Helper script to take care of the meshes, tiling, and materials.
	/// </summary>
	public class MeshController : MonoBehaviour {
		
		/// <summary>
		/// The materials used for the mesh.
		/// </summary>
		public Material[] materials;
		
		private Mesh mesh;
		private Vector3[] vertices, normals;
		
		private Bounds  meshBounds = new Bounds(new Vector3(0.5f, 0f, 0.5f), Vector3.one);
		
		private Renderer[] renderers;
		
		void Awake () {
			// Clone materials to prevent adjusting asset settings.
			for (int i = 0; i < materials.Length; i++) {
				materials[i] = new Material(materials[i]);
			}
		}
		
		/// <summary>
		/// Get or set whether the terrain should be tiled in a 2x2 grid.
		/// </summary>
		public bool tile {
			get {
				return _tile;
			}
			set {
				if (_tile == value) {
					return;
				}
				renderers[1].enabled = renderers[2].enabled = renderers[3].enabled = _tile = value;
				if (value) {
					transform.localPosition -= new Vector3(0.5f, 0f, 0.5f);
				}
				else {
					transform.localPosition += new Vector3(0.5f, 0f, 0.5f);
				}
			}
		}
		private bool _tile;
		
		/// <summary>
		/// Set the height of the terrain based on the alpha channel.
		/// </summary>
		/// <param name='pixels'>
		/// Colors to use.
		/// </param>
		/// <param name='resolution'>
		/// Resolution of the data. Must match the mesh resolution.
		/// </param>
		public void SetHeight (Color[] pixels, int resolution) {
			int i = 0;
			// Set the grid.
			for (int v = 0; v < resolution; v++) {
				for (int u = 0; u < resolution; u++, i++) {
					vertices[i].y = pixels[u + v * resolution].a;
				}
				i += 1;
			}
			// Wrap the last row.
			for (int u = 0; u < resolution; u++, i++) {
				vertices[i].y = pixels[u].a;
			}
			// Wrap the last column.
			for (int v = 0; v < resolution; v++) {
				vertices[v * (resolution + 1) + resolution].y = pixels[v * resolution].a;
			}
			// Wrap the last vertex.
			vertices[vertices.Length - 1].y = pixels[0].a;
			mesh.vertices = vertices;
		}
		
		/// <summary>
		/// Generate the terrain mesh and prepare four tiles.
		/// </summary>
		/// <param name='resolution'>
		/// Resolution to use.
		/// </param>
		public void Generate (int resolution) {
			mesh = new Mesh();
			mesh.name = "Terrain";
			renderers = new Renderer[4];
			for (int i = 0; i < 4; i++) {
				GameObject o = new GameObject("Terrain Renderer");
				o.transform.parent = transform;
				o.AddComponent<MeshFilter>().mesh = mesh;
				Renderer r = o.AddComponent<MeshRenderer>();
				r.materials = materials;
				r.enabled = false;
				renderers[i] = r;
			}
			renderers[0].enabled = true;
			renderers[1].transform.localPosition = Vector3.right;
			renderers[2].transform.localPosition = Vector3.forward;
			renderers[3].transform.localPosition = new Vector3(1f, 0f, 1f);
			
			float delta = 1f / resolution;
			transform.localPosition = new Vector3((1f + delta) * -0.5f, -0.5f,  (1f + delta) * -0.5f);
			resolution += 1;
			vertices = new Vector3[resolution * resolution];
			normals = new Vector3[vertices.Length];
			int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
			int t = 0;
			Vector3 p;
			p.y = 0f;
			for (int y = 0, i = 0; y < resolution; y++) {
				p.z = (y + 0.5f) * delta;
				for (int x = 0; x < resolution; x++, i++) {
					p.x = (x + 0.5f) * delta;
					vertices[i] = p;
					normals[i] = Vector3.up;
					if (x < resolution - 1 && y < resolution - 1) {
						triangles[t] = i;
						triangles[t + 1] = i + resolution;
						triangles[t + 2] = i + 1;
						triangles[t + 3] = i + 1;
						triangles[t + 4] = i + resolution;
						triangles[t + 5] = i + resolution + 1;
						t += 6;
					}
				}
			}
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = triangles;
			mesh.bounds = meshBounds;
		}
	}
}