using System;
using System.Linq;
using UnityEngine;

namespace osgEx
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class osgMono_Geometry : osgMono_Base<osg_Geometry>
    {
        private MeshRenderer m_meshRenderer;
        public MeshRenderer meshRenderer { get { if (m_meshRenderer == null) { m_meshRenderer = this.GetOrAddComponent<MeshRenderer>(); } return m_meshRenderer; } }
        private MeshFilter m_meshFilter;
        public MeshFilter meshFilter { get { if (m_meshFilter == null) { m_meshFilter = this.GetOrAddComponent<MeshFilter>(); } return m_meshFilter; } }
        private MeshCollider m_meshCollider;
        public MeshCollider meshCollider { get { if (m_meshCollider == null) { m_meshCollider = this.GetOrAddComponent<MeshCollider>(); } return m_meshCollider; } }
        private Mesh m_mesh;
        private Texture2D m_mainTexture;
        
        private static osgManager osgManagerInstance;
        public override void Generate(osg_Geometry osgGeometry)
        {
            meshRenderer.material = osgManager.Instance.materialData.Material;
            if (meshRenderer.material != null)
            {
                var m_materialPropertyBlock = new MaterialPropertyBlock();
                m_mainTexture = (osgGeometry.stateSet?.textures?[0] as osg_Texture2D)?.Generate();
                if (!string.IsNullOrWhiteSpace(osgManager.Instance.materialData.MainTexProperty))
                {
                    m_materialPropertyBlock.SetTexture(osgManager.Instance.materialData.MainTexProperty, m_mainTexture);
                }
                var emission = osgGeometry.stateSet?.materials?[0].emission;
                var diffuse = osgGeometry.stateSet?.materials?[0].diffuse;
                var ambient = osgGeometry.stateSet?.materials?[0].ambient;
                var specular = osgGeometry.stateSet?.materials?[0].specular;
                meshRenderer.SetPropertyBlock(m_materialPropertyBlock);
            }
            var tempMesh = new Mesh();
            tempMesh.vertices = osgGeometry.vertexs.Select(v=>new Vector3(-v.x, v.y, v.z)).ToArray();
            var triangles = new int[osgGeometry.indices.Length];
            for (var i = 0; i < triangles.Length; i += 3 )
            {
                triangles[i] = osgGeometry.indices[i];
                triangles[i + 1] = osgGeometry.indices[i + 2];
                triangles[i + 2] = osgGeometry.indices[i + 1];
            }
            tempMesh.triangles = triangles;
            tempMesh.uv = osgGeometry.uv[0];
            
            if (m_mesh) { osgManagerInstance.DestroyObject(m_mesh); m_mesh = null; }
            m_mesh = tempMesh;

            meshFilter.sharedMesh = m_mesh;
            if ((osgManagerInstance ??= osgManager.Instance).colliderEnabled)
            {
                meshCollider.convex = true;
            meshCollider.sharedMesh = m_mesh;
                meshCollider.enabled = true;
            }

            if (osgGeometry.normals != null)
            {
                m_mesh.normals = osgGeometry.normals;
            }
            else
            {
                m_mesh.RecalculateNormals();
            }
            m_mesh.UploadMeshData(true);
        }

        private void OnDestroy()
        {
            if (m_mesh != null)
            {
                Destroy(m_mesh);
            }
            if (m_mainTexture != null)
            {
                Destroy(m_mainTexture);
            }
        }
    }
}
