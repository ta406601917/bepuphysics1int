﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BEPUphysics.ResourceManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using Microsoft.Xna.Framework.Graphics;

namespace BEPUphysics.DataStructures
{
    ///<summary>
    /// Data structure containing triangle mesh data and its associated bounding box tree.
    ///</summary>
    public class TriangleMesh
    {
        private TriangleMeshBoundingBoxTreeData data;
        ///<summary>
        /// Gets or sets the bounding box data used in the mesh.
        ///</summary>
        public TriangleMeshBoundingBoxTreeData Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                tree.Reconstruct(data);
            }
        }

        private TriangleMeshBoundingBoxTree tree;
        ///<summary>
        /// Gets the bounding box tree that accelerates queries to this triangle mesh.
        ///</summary>
        public TriangleMeshBoundingBoxTree Tree
        {
            get
            {
                return tree;
            }
        }

        ///<summary>
        /// Constructs a new triangle mesh.
        ///</summary>
        ///<param name="data">Data to use to construct the mesh.</param>
        public TriangleMesh(TriangleMeshBoundingBoxTreeData data)
        {
            this.data = data;
            tree = new TriangleMeshBoundingBoxTree(data);
        }

        ///<summary>
        /// Constructs a new triangle mesh.
        ///</summary>
        ///<param name="data">Data to use to construct the mesh.</param>
        ///<param name="margin">Margin to expand the bounding boxes of the triangles by.</param>
        public TriangleMesh(TriangleMeshBoundingBoxTreeData data, float margin)
        {
            this.data = data;
            tree = new TriangleMeshBoundingBoxTree(data, margin);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        ///<param name="hitCount">Number of hits between the ray and the mesh.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, out int hitCount)
        {
            var rayHits = Resources.GetRayHitList();
            bool toReturn = RayCast(ray, rayHits);
            hitCount = rayHits.Count;
            Resources.GiveBack(rayHits);
            return toReturn;
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        ///<param name="rayHit">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, out RayHit rayHit)
        {
            return RayCast(ray, float.MaxValue, TriangleSidedness.DoubleSided, out rayHit);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
        ///<param name="rayHit">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, TriangleSidedness sidedness, out RayHit rayHit)
        {
            return RayCast(ray, float.MaxValue, sidedness, out rayHit);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        ///<param name="hits">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, IList<RayHit> hits)
        {
            return RayCast(ray, float.MaxValue, TriangleSidedness.DoubleSided, hits);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
        ///<param name="hits">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, TriangleSidedness sidedness, IList<RayHit> hits)
        {
            return RayCast(ray, float.MaxValue, sidedness, hits);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        ///<param name="rayHit">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
        {
            return RayCast(ray, maximumLength, TriangleSidedness.DoubleSided, out rayHit);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        /// <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
        ///<param name="rayHit">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
        {
            var rayHits = Resources.GetRayHitList();
            bool toReturn = RayCast(ray, maximumLength, sidedness, rayHits);
            if (toReturn)
            {
                rayHit = rayHits[0];
                for (int i = 1; i < rayHits.Count; i++)
                {
                    RayHit hit = rayHits[i];
                    if (hit.T < rayHit.T)
                        rayHit = hit;
                }
            }
            else
                rayHit = new RayHit();
            Resources.GiveBack(rayHits);
            return toReturn;
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        ///<param name="hits">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, float maximumLength, IList<RayHit> hits)
        {
            return RayCast(ray, maximumLength, TriangleSidedness.DoubleSided, hits);
        }

        ///<summary>
        /// Tests a ray against the triangle mesh.
        ///</summary>
        ///<param name="ray">Ray to test against the mesh.</param>
        /// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
        /// <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
        ///<param name="hits">Hit data for the ray, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, IList<RayHit> hits)
        {
            var hitElements = Resources.GetIntList();
            tree.RayCast(ray, hitElements);
            for (int i = 0; i < hitElements.Count; i++)
            {
                Vector3 v1, v2, v3;
                data.GetTriangle(hitElements[i], out v1, out v2, out v3);
                RayHit hit;
                if (Toolbox.FindRayTriangleIntersection(ref ray, maximumLength, sidedness, ref v1, ref v2, ref v3, out hit))
                {
                    hits.Add(hit);
                }
            }
            Resources.GiveBack(hitElements);
            return hits.Count > 0;
        }

        #region Saving/loading
        //TODO: This section could use some improvements.  It's not very robust, especially on WP7.  Remember the VertexBuffer.GetData bug on WP7.
#if WINDOWS_PHONE
        /// <summary>
        /// Gets an array of vertices and indices from the provided model.
        /// </summary>
        /// <param name="collisionModel">Model to use for the collision shape.</param>
        /// <param name="vertices">Compiled set of vertices from the model.</param>
        /// <param name="indices">Compiled set of indices from the model.</param>
        public static void GetVerticesAndIndicesFromModel(Model collisionModel, out Vector3[] vertices, out int[] indices)
        {
            var verticesList = new List<Vector3>();
            var indicesList = new List<int>();
            var transforms = new Matrix[collisionModel.Bones.Count];
            collisionModel.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix transform;
            foreach (ModelMesh mesh in collisionModel.Meshes)
            {
                if (mesh.ParentBone != null)
                    transform = transforms[mesh.ParentBone.Index];
                else
                    transform = Matrix.Identity;
                AddMesh(mesh, transform, verticesList, indicesList);
            }

            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();


        }

        internal static void AddMesh(ModelMesh collisionModelMesh, Matrix transform, List<Vector3> vertices, List<int> indices)
        {
            foreach (ModelMeshPart meshPart in collisionModelMesh.MeshParts)
            {
                int startIndex = vertices.Count;
                VertexElement[] elements = meshPart.VertexBuffer.VertexDeclaration.GetVertexElements();
                Vector3[] meshPartVertices = new Vector3[meshPart.NumVertices];

                //Check for the built in vertex types.
                //This could be generalized to support any kind of vertex, but the GetData(... vertexStride) bug makes it a little too annoying.
                if (elements.Length == 3 &&
                    elements[0].VertexElementUsage == VertexElementUsage.Position &&
                    elements[0].VertexElementFormat == VertexElementFormat.Vector3 &&
                    elements[1].VertexElementUsage == VertexElementUsage.Normal &&
                    elements[1].VertexElementFormat == VertexElementFormat.Vector3 &&
                    elements[2].VertexElementUsage == VertexElementUsage.TextureCoordinate &&
                    elements[2].VertexElementFormat == VertexElementFormat.Vector2)
                {
                    var verts = new VertexPositionNormalTexture[meshPart.VertexBuffer.VertexCount];
                    meshPart.VertexBuffer.GetData(verts);
                    for (int i = meshPart.VertexOffset; i < meshPart.VertexOffset + meshPart.NumVertices; i++)
                    {
                        Vector3.Transform(ref verts[i].Position, ref transform, out meshPartVertices[i - meshPart.VertexOffset]);
                    }
                }
                else if (elements.Length == 2 &&
                    elements[0].VertexElementUsage == VertexElementUsage.Position &&
                    elements[0].VertexElementFormat == VertexElementFormat.Vector3 &&
                    elements[1].VertexElementUsage == VertexElementUsage.Color &&
                    elements[1].VertexElementFormat == VertexElementFormat.Color)
                {
                    var verts = new VertexPositionColor[meshPart.VertexBuffer.VertexCount];
                    meshPart.VertexBuffer.GetData(verts);
                    for (int i = meshPart.VertexOffset; i < meshPart.VertexOffset + meshPart.NumVertices; i++)
                    {
                        Vector3.Transform(ref verts[i].Position, ref transform, out meshPartVertices[i - meshPart.VertexOffset]);
                    }
                }
                else if (elements.Length == 3 &&
                   elements[0].VertexElementUsage == VertexElementUsage.Position &&
                   elements[0].VertexElementFormat == VertexElementFormat.Vector3 &&
                   elements[1].VertexElementUsage == VertexElementUsage.Color &&
                   elements[1].VertexElementFormat == VertexElementFormat.Color &&
                   elements[2].VertexElementUsage == VertexElementUsage.TextureCoordinate &&
                   elements[2].VertexElementFormat == VertexElementFormat.Vector2)
                {
                    var verts = new VertexPositionColorTexture[meshPart.VertexBuffer.VertexCount];
                    meshPart.VertexBuffer.GetData(verts);
                    for (int i = meshPart.VertexOffset; i < meshPart.VertexOffset + meshPart.NumVertices; i++)
                    {
                        Vector3.Transform(ref verts[i].Position, ref transform, out meshPartVertices[i - meshPart.VertexOffset]);
                    }
                }
                else if (elements.Length == 2 &&
                  elements[0].VertexElementUsage == VertexElementUsage.Position &&
                  elements[0].VertexElementFormat == VertexElementFormat.Vector3 &&
                  elements[1].VertexElementUsage == VertexElementUsage.TextureCoordinate &&
                  elements[1].VertexElementFormat == VertexElementFormat.Vector2)
                {
                    var verts = new VertexPositionTexture[meshPart.VertexBuffer.VertexCount];
                    meshPart.VertexBuffer.GetData(verts);
                    for (int i = meshPart.VertexOffset; i < meshPart.VertexOffset + meshPart.NumVertices; i++)
                    {
                        Vector3.Transform(ref verts[i].Position, ref transform, out meshPartVertices[i - meshPart.VertexOffset]);
                    }
                }
                else
                    throw new FormatException("Unsupported vertex type in mesh.");


                //Transform it so its vertices are located in the model's space as opposed to mesh part space.
                vertices.AddRange(meshPartVertices);

                if (meshPart.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
                {
                    var meshIndices = new int[meshPart.PrimitiveCount * 3];
                    meshPart.IndexBuffer.GetData(meshPart.StartIndex * 4, meshIndices, 0, meshPart.PrimitiveCount * 3);
                    for (int k = 0; k < meshIndices.Length; k++)
                    {
                        indices.Add(startIndex + meshIndices[k]);
                    }
                }
                else
                {
                    var meshIndices = new ushort[meshPart.PrimitiveCount * 3];
                    meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, meshIndices, 0, meshPart.PrimitiveCount * 3);
                    for (int k = 0; k < meshIndices.Length; k++)
                    {
                        indices.Add(startIndex + meshIndices[k]);
                    }


                }
            }
        }
#else

        /// <summary>
        /// Gets an array of vertices and indices from the provided model.
        /// </summary>
        /// <param name="collisionModel">Model to use for the collision shape.</param>
        /// <param name="vertices">Compiled set of vertices from the model.</param>
        /// <param name="indices">Compiled set of indices from the model.</param>
        public static void GetVerticesAndIndicesFromModel(Model collisionModel, out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> verticesList = Resources.GetVectorList();
            List<int> indicesList = Resources.GetIntList();
            var transforms = new Matrix[collisionModel.Bones.Count];
            collisionModel.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix transform;
            foreach (ModelMesh mesh in collisionModel.Meshes)
            {
                if (mesh.ParentBone != null)
                    transform = transforms[mesh.ParentBone.Index];
                else
                    transform = Matrix.Identity;
                AddMesh(mesh, transform, verticesList, indicesList);
            }

            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();

            Resources.GiveBack(verticesList);
            Resources.GiveBack(indicesList);

        }

        internal static void AddMesh(ModelMesh collisionModelMesh, Matrix transform, List<Vector3> vertices, List<int> indices)
        {
            foreach (ModelMeshPart meshPart in collisionModelMesh.MeshParts)
            {
                int startIndex = vertices.Count;
                var meshPartVertices = new Vector3[meshPart.NumVertices];
                //Grab position data from the mesh part.
                int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                meshPart.VertexBuffer.GetData(
                        meshPart.VertexOffset * stride,
                        meshPartVertices,
                        0,
                        meshPart.NumVertices,
                        stride);

                //Transform it so its vertices are located in the model's space as opposed to mesh part space.
                Vector3.Transform(meshPartVertices, ref transform, meshPartVertices);
                vertices.AddRange(meshPartVertices);

                if (meshPart.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
                {
                    var meshIndices = new int[meshPart.PrimitiveCount * 3];
                    meshPart.IndexBuffer.GetData(meshPart.StartIndex * 4, meshIndices, 0, meshPart.PrimitiveCount * 3);
                    for (int k = 0; k < meshIndices.Length; k++)
                    {
                        indices.Add(startIndex + meshIndices[k]);
                    }
                }
                else
                {
                    var meshIndices = new ushort[meshPart.PrimitiveCount * 3];
                    meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, meshIndices, 0, meshPart.PrimitiveCount * 3);
                    for (int k = 0; k < meshIndices.Length; k++)
                    {
                        indices.Add(startIndex + meshIndices[k]);
                    }


                }
            }




        }
#endif

        #endregion



    }
}
