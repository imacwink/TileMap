using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapData {
	public int width;
	public int height;
	private int[,] map;

	public int this[int x, int y] {
		get { return map[x, y]; }
		set { map[x, y] = value; }
	}

	public MapData(int width, int height) {
		this.width = width;
		this.height = height;
		map = new int[width,height];
	}

	public bool isConnectedMap (int tileType) {
		bool result = true;

		int startX = -1;
		int startY = -1;
		
		int count = 0;
		
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				if(map[x, y] == tileType) {
					count++;
					
					if(count == 1) {
						startX = x;
						startY = y;
					}
				}
			}
		}

		if (count > 0) {
			int connectedCount = numberOfConnectedTiles (startX, startY);

			result = (count == connectedCount);
		}

		return result;
	}
        
	public int numberOfConnectedTiles(int startX, int startY) {
		int tileType = map [startX, startY];

		bool[,] visited = new bool[width, height];

		List<int> list = new List<int> ();
		list.Add (startX * height + startY);

		int count = 0;

		while (list.Count > 0) {
			int index = list[list.Count-1];
			list.RemoveAt(list.Count-1);

			int x = index / height;
			int y = index % height;

			if(!visited[x, y]) {
				visited[x, y] = true;

				if(map[x,y] == tileType) {
					count++;

					if (x > 0 && !visited[x - 1, y]) {
						list.Add ((x - 1) * height + y);
					}

					if (x < this.width - 1 && !visited[x + 1, y]) {
						list.Add ((x + 1) * height + y);
					}

					if (y > 0 && !visited[x, y - 1]) {
						list.Add (x * height + (y - 1));
					}
					
					if (y < this.height - 1 && !visited[x, y + 1]) {
						list.Add (x * height + (y + 1));
					}
				}
			}
		}

		return count;
	}
}
