using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour {
	public int tileResolution = 64;
	public float tileSize = 1.0f;

    public CameraMove cameraMove;

	void Start () {
		buildMap ();
	}

    void OnDestroy() {
        cameraMove = null;
    }
	
	public void buildMap() {
        MapData map = new MapData((int)cameraMove.mCameraWidth, (int)cameraMove.mCameraHeight);

        transform.position = new Vector3(-cameraMove.mCameraWidth / 2, -cameraMove.mCameraHeight / 2);

		int numTiles = map.width * map.height;
		int numTris = numTiles * 2;
		
		int numVertsX = 2 * (map.width - 1) + 2;
		int numVertsY = 2 * (map.height - 1) + 2;
		int numVerts = numVertsX * numVertsY;
		
		float textureStep = (float)tileResolution / GetComponent<Renderer>().sharedMaterial.mainTexture.width;
		float texelHalfWidth = (1f / GetComponent<Renderer> ().sharedMaterial.mainTexture.width) / 2f;

		Mesh mesh = new Mesh ();
		
		Vector3[] vertices = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		for (int y = 0; y < numVertsY; y++) {
			for(int x = 0; x < numVertsX; x++) {
				int i = y * numVertsX + x;
				
				Vector3 vert = new Vector3((x / 2) * tileSize, (y / 2) * tileSize, 0);
				
				// Set vertex depth
				if(y == 0 || y % 2 == 1) {
					if(x == numVertsX-1 || x % 2 == 0) {
						vert.z = 0;
						
						if(x != 0 && x != numVertsX-1 ) {
							vertices[i-1].z = vert.z;
						}
					}
				} else {
					vert.z = vertices[i-numVertsX].z;
				}
				
				Vector2 texCoord;
				
				if(x % 2 == 0 && y % 2 == 0) {
					int type = map[x/2, y/2];
					texCoord = new Vector2(texelHalfWidth + type * textureStep, 0);
				} else {
					int tileX = (x/2)*2;
					int tileY = (y/2)*2;
					int tileIndex = tileY * numVertsX + tileX;
					texCoord = uv[tileIndex];
				}
				
				if(x % 2 == 1) {
					vert.x += tileSize;
					texCoord.x += textureStep - 2f*texelHalfWidth;
				}
				
				if(y % 2 == 1) {
					vert.y += tileSize;
					texCoord.y = 1;
				}
				
				vertices[i] = vert;
				uv[i] = texCoord;
			}
		}
		
		int[] triangles = new int[numTris * 3];
		
		for (int y = 0; y < map.height; y++) {
			for (int x = 0; x < map.width; x++) {
				int i = (y * map.width + x) * 2 * 3;
				int j = y*2 * numVertsX + x*2;
				
				triangles [i] = j;
				triangles [i + 1] = j + numVertsX;
				triangles [i + 2] = j + 1;
				
				triangles [i + 3] = j + numVertsX;
				triangles [i + 4] = j + numVertsX + 1;
				triangles [i + 5] = j + 1;
			}
		}
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		
		mesh.RecalculateNormals ();
		
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		meshFilter.mesh = mesh;
		
		MeshCollider meshCollider = GetComponent<MeshCollider> ();
		meshCollider.sharedMesh = mesh;
	}
}