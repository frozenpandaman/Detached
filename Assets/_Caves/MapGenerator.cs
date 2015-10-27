using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;

	void Start() {
		//UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
		GenerateMap();
	}

	void Update() {
		if (Input.GetKeyDown("space")) { // on click: Input.GetMouseButtonDown(0)
			GenerateMap();
			//meshGen.ClearMeshes();
		}
	}

	void GenerateMap() {
		map = new int[width,height];
		RandomFillMap();

		for (int i = 0; i < 4; i++) { // smooth map
			SmoothMap();
		}

		// remove small islands
		ProcessMap();

		// border generation
		int borderSize = 0;
		int[,] borderedMap = new int[width+borderSize*2,height+borderSize*2];

		for (int x = 0; x < borderedMap.GetLength(0); x++) {
			for (int y = 0; y < borderedMap.GetLength(1); y++) {
				if (x >= borderSize && x < width+borderSize &&
					y >= borderSize && y < height+borderSize) {
					borderedMap[x,y] = map[x-borderSize,y-borderSize];
					// reverse! create islands
					//borderedMap[x,y] = (map[x-borderSize,y-borderSize] == 1)? 0: 1;
				}
				else { // inside bordered area
					borderedMap[x,y] = 1;
				}
			}
		}

		//int clearBorderSize = 3;
		//for (int x = 0; x < borderedMap.GetLength(0); x++) {
		//	for (int y = 0; y < borderedMap.GetLength(1); y++) {
		//		if (x <= clearBorderSize ||
		//			y <= clearBorderSize ||
		//			x >= borderedMap.GetLength(0)-clearBorderSize ||
		//			y >= borderedMap.GetLength(1)-clearBorderSize) {
		//				borderedMap[x,y] = 0;
		//		}
		//	}
		//}

		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		//meshGen.ClearLists();
		meshGen.GenerateMesh(borderedMap, 1); // default square size

		//meshGen.CreateWallMesh();
	}

	void ProcessMap() {
		List<List<Coord>> wallRegions = GetRegions(1); // get walls
		int wallThresholdSize = 10; // any region < 20 tiles, remove from map

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Coord tile in wallRegion) {
					map[tile.tileX,tile.tileY] = 0; // set to empty space
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions(0); // get rooms/spaces
		int roomThresholdSize = 10; // any region < 20 tiles, remove from map
		List<Room> survivingRooms = new List<Room>();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) { // if under threshhold size
				foreach (Coord tile in roomRegion) {
					map[tile.tileX,tile.tileY] = 1; // set to walls
				}
			}
			else { // otherwise, add to surviving rooms list
				survivingRooms.Add(new Room(roomRegion, map));
			}
		}

		survivingRooms.Sort();
		// note: get size of each room in survivingRooms with .roomSize
		survivingRooms[0].isMainRoom = true; // make the largest the main room
		survivingRooms[0].isAccessibleFromMainRoom = true; // of course

		ConnectClosestRooms(survivingRooms);
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMain = false) {

		List<Room> roomListA = new List<Room>();
		List<Room> roomListB = new List<Room>();

		if (forceAccessibilityFromMain) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add(room);
				}
				else {
					roomListA.Add(room);
				}
			}
		}
		else { // go through all rooms in both loops
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord();
		Coord bestTileB = new Coord();
		Room bestRoomA = new Room();
		Room bestRoomB = new Room();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMain) {
				possibleConnectionFound = false; // reset, before loop looks at next room
				// ^ why is this in the if? in order to look at other rooms force to
				// find best connection. still in process of considering other rooms.

				if (roomA.connectedRooms.Count > 0) { // room already has connection
					continue; // skip to next room
				}
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
				// skip same rooms (avoid case of room connected to self)
					continue;
				}
				// commented out b/c rooms can have mult. connections
				//if (roomA.IsConnected(roomB)) { // if roomA connected to roomB, it already has a connection
				//	possibleConnectionFound = false; // to avoid if loop at bottom, avoid creating passageway
				//	break;
				//}
				// instead, we say... line 130 (after "if (!forceAccessibilityFromMain) {")

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA ++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB ++) {
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX-tileB.tileX,2) + Mathf.Pow (tileA.tileY-tileB.tileY,2)); // Mathf.Pow ,2 = square (returns float)

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			if (possibleConnectionFound && !forceAccessibilityFromMain) {
				//                         ^ same reason as comment above
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				// only create passage when forcing accessibility from main room
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMain) { // if we ARE forcing access.
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if (!forceAccessibilityFromMain) {
			ConnectClosestRooms(allRooms, true); // any rooms still not connected to main room, force
		}
	}

	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms(roomA,roomB);
		//Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

		List<Coord> line = GetLine(tileA,tileB);
		foreach (Coord c in line) {
			MakeEmptyCircle(c,1);
		}
	}

	void MakeEmptyCircle(Coord c, int r) { // r for radius
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x*x + y*y <= r*r) { // x^2 + y^2 <= r^2, inside the circle
					int realX = c.tileX + x; // or "drawX"
					int realY = c.tileY + y;
					if (IsInMapRange(realX, realY)) {
						map[realX,realY] = 0; // set to open area
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord to) {
		List<Coord> line = new List<Coord>();
		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX; // delta/change in x
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign(dx); // + or -
		int gradientStep = Math.Sign(dy);

		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs(dy); // switch
			shortest = Mathf.Abs(dx);
			step = Math.Sign(dy); // increment y, not x each step
			gradientStep = Math.Sign(dx);
		}

		int gradientAccumulation = longest/2;
		for (int i = 0; i < longest; i ++) {
			line.Add(new Coord(x,y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) { // x increases
					x += gradientStep;
				}
				else { // otherwise, y increases
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3 (-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
	}

	List<List<Coord>> GetRegions(int tileType) {
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[width,height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
					List<Coord> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion) { // mark looked at
						mapFlags[tile.tileX,tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles(int startX, int startY) { // flood fill
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[width,height];
		int tileType = map[startX,startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX,startY));
		mapFlags[startX,startY] = 1; // have now looked at this tile

		while (queue.Count > 0) { // stuff left in the queue
			Coord tile = queue.Dequeue(); // removes & returns
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) { // && not diagonal
						if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}

		return tiles;
	}

	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	void RandomFillMap() {
		if (useRandomSeed) {
			seed = Time.time.ToString() + new System.Random().Next(0, 60);
			//print(seed);
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x >= width-2 || y == 0 || y >= height-2) {
					map[x,y] = 1; // walls on edges
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0; // if then 1 else 0
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int neighborWallTiles = GetSurroundingWallCount(x,y);

				// rules
				if (neighborWallTiles > 4) {
					map[x,y] = 1;
				}
				else if (neighborWallTiles < 4) {
					map[x,y] = 0;
				}
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		// go through neighbors. counts corners
		for (int neighborX = gridX-1; neighborX <= gridX+1; neighborX++) {
			for (int neighborY = gridY-1; neighborY <= gridY+1; neighborY++) {
				if (IsInMapRange(neighborX,neighborY)) { // safely within bounds
					if (neighborX != gridX || neighborY != gridY) { // not self tile
						wallCount += map[neighborX,neighborY];
					}
				}
				else {
					wallCount++; // encourage growth of walls around edge
				}
			}
		}

		return wallCount;
	}

	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}

	class Room : IComparable<Room> { // IComparable interface to be able to sort list of main rooms
		public List<Coord> roomTiles; // list of all the tiles in the room
		public List<Coord> edgeTiles; // borders of room
		public List<Room> connectedRooms; // rooms that share a common passage - directly connected
		public int roomSize;
		public bool isMainRoom;
		public bool isAccessibleFromMainRoom;

		// Empty constructor
		public Room() {
		}

		// Constructor
		public Room(List<Coord> roomTiles, int[,] map) {
			this.roomTiles = roomTiles;
			roomSize = roomTiles.Count;
			connectedRooms = new List<Room>();
			edgeTiles = new List<Coord>();

			// go through tiles in room
			foreach (Coord tile in roomTiles) {
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x == tile.tileX || y == tile.tileY) { // excluding diagonal neighbors
							if (map[x,y] == 1) { // if wall tile
								edgeTiles.Add(tile); // add to the edgeTiles list
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room room in connectedRooms) { // each of its connected rooms
					room.SetAccessibleFromMainRoom();
				}
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB) {
			// update accessibility when 2 rooms are connected
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom();
			}
			if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom();
			}

			// add roomB to roomA's list of connected rooms, and vice versa
			 roomA.connectedRooms.Add(roomB);
			 roomB.connectedRooms.Add(roomA);
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo(roomSize);
		}

	}

}
