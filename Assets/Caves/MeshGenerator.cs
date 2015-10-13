using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

	public SquareGrid squareGrid;
	public MeshFilter walls;
	public MeshFilter cave;

	List<Vector3> vertices;
	List<int> triangles;

	Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int,List<Triangle>>();
	List<List<int>> outlines = new List<List<int>>();
	HashSet<int> checkedVertices = new HashSet<int>(); // cool!

	//public void ClearLists() {
	//	vertices = new List<Vector3>();
	//	triangles = new List<int>();
	//	triangleDictionary.Clear();
	//	outlines.Clear();
	//	checkedVertices.Clear();
	//}

	public void GenerateMesh(int[,] map, float squareSize) {

		triangleDictionary.Clear();
		outlines.Clear();
		checkedVertices.Clear();
		//vertices.Clear();
		//triangles.Clear();
		squareGrid = new SquareGrid(map, squareSize);

		vertices = new List<Vector3>();
		triangles = new List<int>();

		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
				TriangulateSquare(squareGrid.squares[x,y]);
			}
		}

		Mesh mesh = new Mesh();
		cave.mesh.Clear();
		cave.mesh = mesh;

		//mesh.vertices.Clear();
		//mesh.triangles.Clear();
		//cave.mesh.ClearLists();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		CreateWallMesh();
	}


	void CreateWallMesh() {

		CalculateMeshOutlines();

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Mesh wallMesh = new Mesh();
		float wallHeight = 5;

		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count -1; i++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]); // top left
				wallVertices.Add(vertices[outline[i+1]]); // top right
				wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight); // bottom right

				// assign indices to wallTriangles list, in correct order
				// first triangle
				wallTriangles.Add(startIndex + 0); // top
				wallTriangles.Add(startIndex + 2); // bottom left
				wallTriangles.Add(startIndex + 3); // bottom right
				// second triangle
				wallTriangles.Add(startIndex + 3); // bottom right
				wallTriangles.Add(startIndex + 1); // top right
				wallTriangles.Add(startIndex + 0); // top left
			}
		}
		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		walls.mesh = wallMesh;

		// Wall collider
		Destroy(walls.gameObject.GetComponent(typeof(MeshCollider))); // CLEAR OLD COLLIDERS!!! YESSSSS
		MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
		wallCollider.sharedMesh = wallMesh;
	}

	void TriangulateSquare(Square square) {
		switch (square.configuration) { // 16 cases
			// 0 points:
			case 0:
				break;
			// 1 point:
			case 1:
				MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
				break;
			case 2:
				MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
				break;
			case 4:
				MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
				break;
			case 8:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
				break;
			// 2 points:
			case 3:
				MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft,
					square.centerLeft);
				break;
			case 6:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight,
					square.centerBottom);
				break;
			case 9:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom,
					square.bottomLeft);
				break;
			case 12:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight,
					square.centerLeft);
				break;
			case 5:
				MeshFromPoints(square.centerTop, square.topRight, square.centerRight,
					square.centerBottom, square.bottomLeft, square.centerLeft);
				break;
			case 10:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight,
					square.bottomRight, square.centerBottom, square.centerLeft);
				break;

			// 3 point:
			case 7:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight,
					square.bottomLeft, square.centerLeft);
				break;
			case 11:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight,
					square.bottomRight, square.bottomLeft);
				break;
			case 13:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight,
					square.centerBottom, square.bottomLeft);
				break;
			case 14:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight,
					square.centerBottom, square.centerLeft);
				break;

			// 4 point:
			case 15: // surrounded all by walls
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight,
					square.bottomLeft);
				checkedVertices.Add(square.topLeft.vertexIndex);
				checkedVertices.Add(square.topRight.vertexIndex);
				checkedVertices.Add(square.bottomRight.vertexIndex);
				checkedVertices.Add(square.bottomLeft.vertexIndex);
				break;
		}
	}

	void MeshFromPoints(params Node[] points) { // params keyword for unknown # of points
		AssignVertices(points);

		if (points.Length >= 3) { // if 3 or more points
			CreateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length >= 4) {
			CreateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length >= 5) {
			CreateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length >= 6) { // diagonal cases (5, 10)
			CreateTriangle(points[0], points[4], points[5]);
		}
	}

	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points[i].vertexIndex == -1) { // default
				points[i].vertexIndex = vertices.Count; // size of list/array
				vertices.Add(points[i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c) { // Create triangles out of points
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDict(triangle.vertexIndexA, triangle);
		AddTriangleToDict(triangle.vertexIndexB, triangle);
		AddTriangleToDict(triangle.vertexIndexC, triangle);
	}

	void AddTriangleToDict(int vertexIndexKey, Triangle triangle) {
		// if dictionary already contains, just add triangle to list already containing vertex index
		if (triangleDictionary.ContainsKey(vertexIndexKey)) {
			triangleDictionary[vertexIndexKey].Add(triangle);
		}
		else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}

	void CalculateMeshOutlines() {
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1) { // couldn't find
					checkedVertices.Add(vertexIndex); // record we tried/checked it

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count-1);
					outlines[outlines.Count-1].Add(vertexIndex);
				}
			}
		}
	}

	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex); // continue following outline
		}
	}

	int GetConnectedOutlineVertex(int vertexIndex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex[i];

			for (int j = 0; j < 3; j++) {
				int vertexB = triangle[j];			// optimization \/
				if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
					if (IsOutlineEdge(vertexIndex, vertexB)) {
						return vertexB;
					}
				}
			}
		}
		return -1; // haven't found
	}

	bool IsOutlineEdge(int vertexA, int vertexB) {
		List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i ++) {
			if (trianglesContainingVertexA[i].Contains(vertexB)) {
				sharedTriangleCount++;
				if (sharedTriangleCount > 1) {
					break; // not outline
				}
			}
		}
		return sharedTriangleCount == 1;
	}

	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;

		//Constructor
		public Triangle(int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;
			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		// Indexer
		public int this[int i] {
			get {
				return vertices[i];
			}
		}

		public bool Contains(int vertexIndex) {
			return vertexIndex == vertexIndexA ||vertexIndex == vertexIndexB ||
			vertexIndex == vertexIndexC;
		}
	}

	public class SquareGrid {
		public Square[,] squares;

		// Constructor
		public SquareGrid(int[,] map, float squareSize) { // map provided by MapGenerator class
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

			for (int x = 0; x < nodeCountX; x++) {
				for (int y = 0; y < nodeCountY; y++) {
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2,
						0, -mapHeight/2 + y * squareSize + squareSize/2);
					controlNodes[x,y] = new ControlNode(pos,map[x,y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX-1,nodeCountY-1];
			for (int x = 0; x < nodeCountX-1; x++) {
				for (int y = 0; y < nodeCountY-1; y++) {
					squares[x,y] = new Square(controlNodes[x,y+1], // topLeft
											  controlNodes[x+1,y+1], // topRight
											  controlNodes[x+1,y], // bottomRight
											  controlNodes[x,y]); // bottomLeft
				}
			}
		}

	}

	public class Square {
		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centerTop, centerRight, centerBottom, centerLeft;
		public int configuration; // 16 possible configs

		// Constructor
		public Square(ControlNode _topLeft, ControlNode _topRight,
					  ControlNode _bottomRight, ControlNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centerTop = topLeft.right;
			centerRight = bottomRight.above;
			centerBottom = bottomLeft.right;
			centerLeft = bottomLeft.above;

			if (topLeft.active) // 4th binary bit = 1000 = 8 (in base 10)
				configuration += 8;
			if (topRight.active) // 0100 = 4
				configuration += 4;
			if (bottomRight.active) // 0010 = 2
				configuration += 2;
			if (bottomLeft.active) // 0010 = 1
				configuration += 1;
		}
	}

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		// Constructor
		public Node(Vector3 _pos) {
			position = _pos;
		}
	}

	public class ControlNode : Node { // : = inherits
		public bool active;
		public Node above, right; // control nodes (corners) own 2 nodes each: top and right

		// Constructor
		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			 // base: _pos from base (parent/Node) constructor
			 active = _active;
			 above = new Node(position + Vector3.forward * squareSize/2f);
			 right = new Node(position + Vector3.right * squareSize/2f);
		}
	}
}
