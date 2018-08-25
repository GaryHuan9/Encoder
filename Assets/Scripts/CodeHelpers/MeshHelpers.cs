using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using System.Linq;
using System;

namespace CodeHelpers.MeshHelpers
{
	public static class MeshHelper
	{
		const int mesh16BitSize = 65530; //65536, left 6 for safey

		/// <summary>Combine meshes given. It is wise to pass in List or Array for the IEnumerable.</summary>
		public static MeshAndMaterials CombineMeshes(IEnumerable<MeshAndMaterials> models, IEnumerable<Matrix4x4> matrices, Mesh baseMesh = null, bool overrideCheck = false)
		{
			if (models.Count() != matrices.Count() && !overrideCheck) throw new ArgumentOutOfRangeException("Different length of array for mesh combine.\n theseRenderMaterials.Length = " +
																											models.Count() + " transforms.Length = " + matrices.Count());

			var meshListFromMaterial = new Dictionary<Material, ListWithVertexCount>();
			Func<ListWithVertexCount> listWithCountFactory = () => new ListWithVertexCount { list = new List<CombineInstance>() };

			models.ForEach((thisModel, index) =>
			{
				//Loop through all the materials
				for (int i = 0; i < thisModel.materials.Length; i++)
				{
					Mesh subMesh = thisModel.GetSubMesh(i);
					var listAndCount = meshListFromMaterial.GetOrAdd(thisModel.materials[i], listWithCountFactory);

					listAndCount.vertexCount += subMesh.vertexCount;
					listAndCount.list.Add(new CombineInstance
					{
						mesh = subMesh,
						transform = matrices.Count() > index ? matrices.ElementAtOrDefault(index) : matrices.Last()
					});
				}
			});

			CombineInstance[] combineInstances = new CombineInstance[meshListFromMaterial.Count];
			int vertexCount = 0;

			meshListFromMaterial.Values.ForEach((thisListWithCount, index) =>
			{
				Mesh subMesh = new Mesh { indexFormat = thisListWithCount.vertexCount >= mesh16BitSize ? IndexFormat.UInt32 : IndexFormat.UInt16 };

				subMesh.CombineMeshes(thisListWithCount.list.ToArray(), true);
				vertexCount += subMesh.vertexCount;

				combineInstances[index] = new CombineInstance
				{
					mesh = subMesh,
					subMeshIndex = 0
				};
			});

			baseMesh = baseMesh ?? new Mesh();

			baseMesh.indexFormat = vertexCount >= mesh16BitSize ? IndexFormat.UInt32 : IndexFormat.UInt16;
			baseMesh.CombineMeshes(combineInstances, false, false);

			return new MeshAndMaterials(baseMesh, meshListFromMaterial.Keys.ToArray());
		}

		struct ListWithVertexCount { public List<CombineInstance> list; public int vertexCount; };

		public static Mesh CombineMeshes(IEnumerable<Mesh> meshes, IEnumerable<Matrix4x4> matrices, Mesh baseMesh = null, bool overrideCheck = false)
		{
			if (meshes.Count() != matrices.Count() && !overrideCheck) throw new ArgumentOutOfRangeException("Different length of array for mesh combine.\n theseRenderMaterials.Length = " +
																											meshes.Count() + " transforms.Length = " + matrices.Count());

			CombineInstance[] instances = new CombineInstance[meshes.Count()];
			int vertexCount = 0;

			meshes.ForEach((thisMesh, index) =>
			{
				vertexCount += thisMesh.vertexCount;
				instances[index] = new CombineInstance
				{
					mesh = thisMesh,
					transform = matrices.ElementAtOrDefault(index),
				};
			});

			baseMesh = baseMesh ?? new Mesh();

			baseMesh.indexFormat = vertexCount >= mesh16BitSize ? IndexFormat.UInt32 : IndexFormat.UInt16;
			baseMesh.CombineMeshes(instances, true);

			return baseMesh;
		}

		public static MeshThreadedData ThreadedCombineMeshes(IEnumerable<MeshAndMatrix> meshes, bool combineUVs = true, bool combineNormals = false)
		{
			var vertices = new List<Vector3>();
			var triangles = new Dictionary<int, List<int>>();

			var uvs = combineUVs ? new List<Vector2>() : null;
			var normals = combineNormals ? new List<Vector3>() : null;

			//Prevent allocation
			var theseVertices = new List<Vector3>();
			var theseTriangles = new List<int>();

			var theseUVs = combineUVs ? new List<Vector2>() : null;
			var theseNormals = combineNormals ? new List<Vector3>() : null;

			meshes.ForEach((thisMesh, index) =>
			{
				Mesh targetMesh = thisMesh.mesh;
				Matrix4x4 thisMatrix = thisMesh.matrix;

				//Vertices and normals
				int oldVertexCount = vertices.Count;
				int vertexCount = targetMesh.vertexCount;

				vertices.Capacity += vertexCount;

				targetMesh.GetVertices(theseVertices);

				if (combineNormals)
				{
					normals.Capacity += vertexCount;

					targetMesh.GetNormals(theseNormals);

					for (int i = 0; i < vertexCount; i++)
					{
						vertices.Add(thisMatrix.MultiplyPoint3x4(theseVertices[i]));
						normals.Add(thisMatrix.MultiplyVector(theseNormals[i]));
					}
				}
				else
				{
					for (int i = 0; i < vertexCount; i++)
					{
						vertices.Add(thisMatrix.MultiplyPoint3x4(theseVertices[i]));
					}
				}

				//UVs
				if (combineUVs)
				{
					targetMesh.GetUVs(0, theseUVs);
					uvs.AddRange(theseUVs);
				}

				//Triangles
				for (int i = 0; i < targetMesh.subMeshCount; i++)
				{
					int subMeshIndex = i + thisMesh.subMeshOffset;

					if (!triangles.ContainsKey(subMeshIndex)) triangles.Add(subMeshIndex, new List<int>());

					var subMeshTriangles = triangles[subMeshIndex];
					targetMesh.GetTriangles(theseTriangles, i);

					int trianglesCount = theseTriangles.Count;
					subMeshTriangles.Capacity += trianglesCount;

					for (int j = 0; j < trianglesCount; j++)
					{
						subMeshTriangles.Add(theseTriangles[j] + oldVertexCount);
					}
				}
			});

			return new MeshThreadedData(vertices, triangles, uvs, normals);
		}

		/// <summary>This applys a transform (matrix) to a mesh, this does not create a new mesh.</summary>
		public static Mesh ApplyMatrix(Mesh thisMesh, Matrix4x4 thisMatrix)
		{
			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();

			thisMesh.GetVertices(vertices);
			thisMesh.GetNormals(normals);

			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i] = thisMatrix.MultiplyPoint3x4(vertices[i]);
				normals[i] = thisMatrix.MultiplyVector(normals[i]);
			}

			thisMesh.SetVertices(vertices);
			thisMesh.SetNormals(normals);

			return thisMesh;
		}

		/// <summary>Creates a new mesh with the data of this old mesh.</summary>
		public static Mesh Clone(this Mesh mesh) => new Mesh { vertices = mesh.vertices, triangles = mesh.triangles, uv = mesh.uv, uv2 = mesh.uv2, normals = mesh.normals, tangents = mesh.tangents, colors = mesh.colors, bounds = mesh.bounds };
	}

	public struct MeshAndMaterials
	{
		#region Constructors

		public MeshAndMaterials(Mesh mesh, Material[] materials = null, bool calculateSubMesh = false)
		{
			this.mesh = mesh;
			this.materials = materials ?? new Material[1];

			subMeshes = null;

			if (calculateSubMesh) CalculateAllSubMeshes();
		}

		public MeshAndMaterials(Mesh mesh, Material material, bool calculateSubMesh = false)
		{
			this.mesh = mesh;
			materials = new Material[] { material };

			subMeshes = null;

			if (calculateSubMesh) CalculateAllSubMeshes();
		}

		public MeshAndMaterials(GameObject thisGameObject, bool calculateSubMesh = false)
		{
			MeshFilter thisFilter = thisGameObject.GetComponent<MeshFilter>();
			MeshRenderer thisRenderer = thisGameObject.GetComponent<MeshRenderer>();

			if (thisFilter != null) mesh = thisFilter.sharedMesh; else mesh = null;
			if (thisRenderer != null) materials = thisRenderer.sharedMaterials; else materials = new Material[1];

			subMeshes = null;

			if (calculateSubMesh) CalculateAllSubMeshes();
		}

		void CalculateAllSubMeshes()
		{
			if (mesh == null) return;
			for (int i = 0; i < mesh.subMeshCount; i++) GetSubMesh(i);
		}

		#endregion

		public readonly Mesh mesh;
		public readonly Material[] materials;

		public Material Material { get { return materials[0]; } set { materials[0] = value; } }

		Mesh[] subMeshes;
		public Mesh GetSubMesh(int index)
		{
			//Create the subMeshes array if hasn't
			if (subMeshes == null) subMeshes = new Mesh[mesh.subMeshCount];

			if (subMeshes[index] == null)
			{
				int[] triangles = mesh.GetTriangles(index);
				List<Vector3> vertices = new List<Vector3>();

				mesh.GetVertices(vertices);

				//Get all the useful vertices
				var changeTriangles = new Dictionary<int, int>();
				for (int i = 0; i < triangles.Length; i++)
				{
					int thisIndex = triangles[i];
					if (!changeTriangles.ContainsKey(thisIndex)) changeTriangles.Add(thisIndex, changeTriangles.Count);
					triangles[i] = changeTriangles[thisIndex];
				}

				//Remove all the useless vertices and uvs
				Vector3[] usefulVertices = new Vector3[changeTriangles.Count];
				foreach (int thisIndex in changeTriangles.Keys) usefulVertices[changeTriangles[thisIndex]] = vertices[thisIndex];

				//Create a new mesh
				Mesh thisMesh = new Mesh
				{
					vertices = usefulVertices,
					triangles = triangles,
					uv = mesh.uv
				};

				thisMesh.RecalculateNormals();
				thisMesh.RecalculateBounds();
				thisMesh.RecalculateTangents();

				subMeshes[index] = thisMesh;
			}

			return subMeshes[index];
		}

		public static implicit operator Mesh(MeshAndMaterials thisMeshAndMaterials) => thisMeshAndMaterials.mesh;
		public static implicit operator Material(MeshAndMaterials thisMeshAndMaterials) => thisMeshAndMaterials.Material;
		public static implicit operator Material[] (MeshAndMaterials thisMeshAndMaterials) => thisMeshAndMaterials.materials;
	}

	public struct ModelInfo
	{
		public ModelInfo(MeshAndMaterials thisMeshAndMaterials, Matrix4x4 thisMatrix)
		{
			this.thisMeshAndMaterials = thisMeshAndMaterials;
			this.thisMatrix = thisMatrix;
		}

		public MeshAndMaterials thisMeshAndMaterials;
		public Matrix4x4 thisMatrix;
	}

	/// <summary> This is just a struct that stores variables of a mesh, used for threading </summary>
	public struct MeshThreadedData
	{
		public MeshThreadedData(List<Vector3> vertices, Dictionary<int, List<int>> triangles, List<Vector2> uvs = null, List<Vector3> normals = null)
		{
			this.vertices = vertices;
			this.triangles = triangles;
			this.uvs = uvs;
			this.normals = normals;
		}

		//Info
		List<Vector3> vertices;
		Dictionary<int, List<int>> triangles;

		List<Vector2> uvs;
		List<Vector3> normals;

		public void Apply(Mesh thisMesh, bool betterRecalculateNormals = true)
		{
			thisMesh.Clear();

			thisMesh.SetVertices(vertices);
			foreach (var thisPair in triangles) thisMesh.SetTriangles(thisPair.Value, thisPair.Key);

			if (uvs != null) thisMesh.SetUVs(0, uvs);

			if (normals != null) thisMesh.SetNormals(normals); //Not that fast but not slow
			else if (betterRecalculateNormals) thisMesh.RecalculateNormals(30f); //Slowest
			else thisMesh.RecalculateNormals(); //Fastest

			thisMesh.RecalculateBounds();
		}
	}

	[Serializable]
	public struct RendererAndFilter
	{
		public RendererAndFilter(MeshRenderer thisRenderer, MeshFilter thisFilter)
		{
			this.thisRenderer = thisRenderer;
			this.thisFilter = thisFilter;
		}

		public RendererAndFilter(MonoBehaviour thisObject)
		{
			thisRenderer = thisObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault();
			thisFilter = thisObject.GetComponentsInChildren<MeshFilter>().FirstOrDefault();
		}

		public RendererAndFilter(GameObject thisObject)
		{
			thisRenderer = thisObject.GetComponentsInChildren<MeshRenderer>().FirstOrDefault();
			thisFilter = thisObject.GetComponentsInChildren<MeshFilter>().FirstOrDefault();
		}

		public MeshRenderer thisRenderer;
		public MeshFilter thisFilter;

		public Mesh Mesh
		{
			get { return thisFilter?.mesh; }
			set { if (thisFilter != null) thisFilter.mesh = value; }
		}

		public Material Material
		{
			get { return thisRenderer?.material; }
			set { if (thisRenderer != null) thisRenderer.material = value; }
		}

		public Material[] Materials
		{
			get { return thisRenderer?.materials; }
			set { if (thisRenderer != null) thisRenderer.materials = value; }
		}

		public void Apply(MeshAndMaterials model, bool duplicateMaterial = true, bool duplicateMesh = true)
		{
			if (duplicateMaterial) thisRenderer.materials = model.materials; else thisRenderer.sharedMaterials = model.materials;
			if (duplicateMesh) thisFilter.mesh = model.mesh; else thisFilter.sharedMesh = model.mesh;
		}
	}

	public struct MeshAndMatrix
	{
		public MeshAndMatrix(Mesh mesh, Matrix4x4 matrix, byte subMeshOffset = 0)
		{
			this.mesh = mesh;
			this.matrix = matrix;

			this.subMeshOffset = subMeshOffset;
		}

		public readonly Mesh mesh;
		public readonly Matrix4x4 matrix;

		public readonly byte subMeshOffset;
	}
}