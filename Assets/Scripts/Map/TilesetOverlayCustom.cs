//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno
{

    /// <summary>
    /// north: 37.97679,
    /// south: 37.97324,
    /// east:  23.72500,
    /// west:  23.72060,
    /// </summary>
	public class TilesetOverlayCustom : MonoBehaviour
	{
        public int renderQueue;

        [Space]
        // Overlay texture in mercator projection
        public Texture texture;

        /// <summary>
        /// Border coordinates of the overlay
        /// </summary>
        [Header("North")]
        public double topLatitude;
        [Space(-10f)]
        [Header("South")]
        public double bottomLatitude;
        [Space(-10f)]
        [Header("East")]
        public double rightLongitude;
        [Space(-10f)]
        [Header("West")]
        public double leftLongitude;


        [Space]
        // Overlay transparency
        [Range(0, 1)]
        public float alpha = 1;

        private Mesh overlayMesh;
        public Material material;

        GameObject overlayContainer;

        private void Start()
        {
            // Create overlay container
            overlayContainer = new GameObject("OverlayContainer");
            overlayContainer.transform.parent = transform;

            // Init overlay material
            MeshRenderer meshRenderer = overlayContainer.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = overlayContainer.AddComponent<MeshFilter>();
            //material = new Material(Shader.Find("Transparent/Diffuse"));
            material.mainTexture = texture;
            material.renderQueue += renderQueue;
            meshRenderer.sharedMaterial = material;

            overlayMesh = meshFilter.sharedMesh = new Mesh();
            overlayMesh.name = "Overlay Mesh";
            overlayMesh.MarkDynamic();
            overlayMesh.vertices = new Vector3[4];

            // Subscribe to events
            OnlineMapsTileSetControl.instance.OnMeshUpdated += UpdateMesh;


            // Init mesh
            UpdateMesh();
        }

        public void OnTopographicsToggle()
        {
            overlayContainer.SetActive(!overlayContainer.activeSelf);
        }

        private void UpdateMesh()
        {
            overlayMesh.Clear();
            OnlineMapsTileSetControl control = OnlineMapsTileSetControl.instance;

            Vector3 p1 = control.GetWorldPosition(leftLongitude, topLatitude);
            Vector3 p2 = control.GetWorldPosition(rightLongitude, bottomLatitude);

            Vector2 size = control.sizeInScene;
            size.x *= -1;

            if (p1.x > 0 && p2.x > 0) return;
            if (p1.x < size.x && p2.x < size.x) return;
            if (p1.z < 0 && p2.z < 0) return;
            if (p1.z > size.y && p2.z > size.y) return;

            Vector2 uv0 = new Vector2(0, 1);
            Vector2 uv1 = new Vector2(1, 0);

            Vector3 v1 = p1;
            Vector3 v2 = p2;

            if (p1.x > 0)
            {
                float m = p1.x / (p1.x - p2.x);
                uv0.x = m;
                v1.x = 0;
            }

            if (p1.z < 0)
            {
                float m = p1.z / (p1.z - p2.z);
                uv0.y = 1 - m;
                v1.z = 0;
            }

            if (p2.x < size.x)
            {
                float m = (p1.x - size.x) / (p1.x - p2.x);
                uv1.x = m;
                v2.x = size.x;
            }

            if (p2.z > size.y)
            {
                float m = (size.y - p2.z) / (p1.z - p2.z);
                uv1.y = m;
                v2.z = size.y;
            }

            overlayMesh.vertices = new[]
            {
            new Vector3(v1.x, 0.1f, v1.z),
            new Vector3(v2.x, 0.1f, v1.z),
            new Vector3(v2.x, 0.1f, v2.z),
            new Vector3(v1.x, 0.1f, v2.z),
        };

            overlayMesh.normals = new[]
            {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };

            overlayMesh.uv = new[]
            {
            uv0,
            new Vector2(uv1.x, uv0.y),
            uv1,
            new Vector2(uv0.x, uv1.y),
        };

            // Init triangles
            overlayMesh.SetTriangles(new[]
            {
            0, 1, 2,
            0, 2, 3
        }, 0);

            overlayMesh.RecalculateBounds();
            overlayMesh.RecalculateNormals();
        }

        private void Update()
        {
            if (Math.Abs(material.color.a - alpha) > float.Epsilon)
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }

}
