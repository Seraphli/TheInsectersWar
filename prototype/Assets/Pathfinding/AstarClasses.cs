//    � Copyright 2009 Aron Granberg
//    AstarClasses.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com

//For documentation see http://www.arongranberg.com/unity/a-pathfinding/docs/

using UnityEngine;
using System.Collections;
using System;
//using System.Runtime.Serialization;
//using System.Reflection;

namespace AstarClasses {
	
	[System.Serializable]
	public class NodeLink : System.Object {
		public Vector3 fromVector;
		public Vector3 toVector;
		public LinkType linkType= LinkType.Link;
		public bool oneWay = false;
		public NodeLink () {
		}
	}
	
	[System.Serializable]
	public class Grid : System.Object {
		public string name = "New Grid";
		public bool showInEditor = true;
		public bool debug = true;
		public bool changed = false;
		//public int startPos = 0;
		
		public float _height = 10;
		public int _width = 10;
		public int _depth = 10;
		
		public float scale = 1;
		
		public float height {
			get {
				return _height;
			}
			set {
				_height = value;
				globalHeight = value;
			}
		}
		
		public int width {
			get {
				return _width;
			}
			set {
				_width = value;
				globalWidth = value;
			}
		}
		
		public int depth {
			get {
				return _depth;
			}
			set {
				_depth = value;
				globalDepth = value;
			}
		}
		
		//public int width = 100;
		//public int depth = 100;
		
		public Vector3 realOffset {
			get {
				return offset+globalOffset;
			}
		}
		
		public Vector3 offset = Vector3.zero;
		
		//This is the offset which some grid generators will use to calibrate the position of the bounds without changing the user set offset
		public Vector3 globalOffset = Vector3.zero;
		
		//The size of each node, in Mesh mode this is the scale
		public float nodeSize = 10;
		
		//This is the bounds of the grid in world units, it may differ from the width/depth multiplied with nodeSize.
		public float globalWidth = 100;
		public float globalDepth = 100;
		public float globalHeight = 50;
		
		//Show the physics settings?
		public bool showPhysics = false;
		
		public int ignoreLayer;
		public LayerMask physicsMask = -1;
		public PhysicsType physicsType = PhysicsType.TouchCapsule;
		public UpDown raycastUpDown = UpDown.Down;
		public float raycastLength = 1000;
		public float capsuleHeight = 20;
		public float physicsRadius = 1.0F;
		
		
		public Grid (float h) {
			height = h;
			width = 100;
			depth = 100;
			globalWidth = 100;
			globalDepth = 100;
			globalHeight = h;
		}
		
		public Grid () {
			height = 10;
			width = 15;
			depth = 15;
			globalWidth = 15;
			globalDepth = 15;
			globalHeight = 10;
			nodeSize = 1;
			offset = Vector3.zero;
			globalOffset = offset;
		}
		
		public Grid (Grid o) {
			height = o.height;
			width = o.width;
			depth = o.depth;
			offset = o.offset;
			nodeSize = o.nodeSize;
		}
		
		public bool Contains (Int3 p) {
			if (
			p.x >= realOffset.x && p.z >= realOffset.z && 
			p.x < realOffset.x+(globalWidth)*nodeSize && 
			p.z < realOffset.z+(globalDepth)*nodeSize && 
			p.y >= realOffset.y && 
			p.y < realOffset.y+globalHeight) {
				return true;
			}
			return false;
		}
		
		public bool Contains (Vector3 p) {
			if (
			p.x >= realOffset.x && 
			p.z >= realOffset.z && 
			p.x < realOffset.x+(globalWidth)*nodeSize && 
			p.z < realOffset.z+(globalDepth)*nodeSize && 
			p.y >= realOffset.y && 
			p.y < realOffset.y+globalHeight) {
				return true;
			}
			return false;
		}
		
		public void Reset () {
			globalOffset = Vector3.zero;
		}
	}
	
	//This is the node class you should use when you are creating procedural levels.
	public class SimpleNode {
		//The position of the node [Needed]
		public Vector3 vectorPos;
		
		//The angles to the neighbour nodes, the length must be equal to the neighbours array OR the variable should be set to null, then the CreateGrid function will fill in the data
		public float[] angles;
		
		//The connections to other nodes [Needed]
		public SimpleNode[] neighbours;
		
		//The cost of moving from this node to a neighbour node, the length must be equal to the neighbours array OR the variable should be set to null, then the CreateGrid function will fill in the data
		public int[] costs = null;
		
		//You don't need to change this, the CreateGrid function will use it for caching data
		public Int3 pos;
		
		public SimpleNode () {
		}
		
		//Typically you would first create all nodes in the first pass using this constructor, and then assign all connections in a second pass
		public SimpleNode (Vector3 pos) {
			vectorPos = pos;
		}
		
		public SimpleNode (Vector3 pos,float[] an,SimpleNode[] ne,int[] co) {
			vectorPos = pos;
			angles = an;
			neighbours = ne;
			costs = co;
			if (neighbours.Length != costs.Length) {
				Debug.LogError ("Neighbours and Costs arrays length's must be equal");
			}
		}
	}
	
	//Each point in the grid is represented by a Node, each node holds information about the nodes position, parent, neighbour nodes and a lot of other stuff
	public class Node {
		//Global position of the node
		public Vector3 vectorPos;
		
		//Local position of the node
		public Int3 pos;
		//The previous parent
		public Node parentx;
		//Current parent
		public Node parent;
		public int invParentDirection = -1;
		//Is the node walkable
		public bool walkable = true;
		
		//These variables used by the grid to indicate the cost of moving from this node's parent to this node, the basicCost is just the constant cost, and the extraCost is used for the angle cost
		public int basicCost = 0;
		public int extraCost = 0;
		
		//Cached G and H values
		public int _g = 0;
		public int hx = -1;
		
		public int penalty = 0;
		
		//This is the angles to each neighbour divided by 90
		public float[] angles = new float[9];
		
		//All neighbours
		public Node[] neighbours;
		
		/*The neighboursKeys array is an array of the direction numbers for all walkable neighbours
		Imagine a node with eight nodes around it, the numbers in this array represents the direction from the middle node (this node) to the other nodes (neighbours) with 0 as Top Left and 7 as Bottom Right
		
		0 1 2
		3 X 4
		5 6 7
		
		The numbers are also used as indexes for the cost and angles array, so the neighbour at the Right would use the index 4 in the angles array.
		However they are only used as direction in the Grid and Texture modes, for all other modes this array will only be a linear set of numbers (i.e 1,2,3,4,5...) since there are no predifined directions for those modes. */
		public int[] neighboursKeys;
		
		//The area the node in is, all other nodes in the same area is accesible from this node, this is used to prevent the script from searching all possible nodes when there isn't a path availible to the target
		public int area = 0;
		
		//When whas the area calculated
		public int areaTimeStamp = 0;
		
		//Previous scripts
		public AstarPath.Path scripty;
		
		public int[] costs = null;
		
		//public AstarPath.Path scriptx;
		//Current
		public AstarPath.Path script;
		
		public int g {
			get {
				if (parent == null) {//Does this node have a parent?
					return 0;
				}
				
				//@Performance, uncomment the next IF to get better performance but at a cost of lower accuracy
				
				//if (parent == parentx) {
				//	return _g;
				//} else {
					parentx = parent;
					_g = basicCost+extraCost+penalty+parent.g;
				//}
				
				//Trace back to the starting point
				return _g;
			}
		}
		
		public int h {
			get {
				
				//Has the end changed since last time? Acctually it is checking if the path calculating right now is the same as the one when the H value was calculated last time. If it is then we can just return the cached value since the end isn't going to change during one path calculation
				if (script == scripty) {
					return hx;
				} else {
					scripty = script;
					
					//If useWorldPositions is True, then the script will calculate the distance between this node and the target using world positions, otherwise it will use the array indexes (grids are made up of 2D arrays), which is a lot faster since it only uses integers instead of floats. It is recommended to use world positions when not using the Grid or Texture mode since all other modes does not place the nodes in an array which indexes can be used as positions.
					//It can also be good to use world positions when using multiple grids since the array indexes wont create a good direction when this node and the target node are in different grids.
					if (AstarPath.active.useWorldPositions) {
						
						hx = (int) (Mathf.Abs(script.end.vectorPos.x-vectorPos.x)*100
					
						//@Performance, comment out the next line if you dont use multiple grids
						+  Mathf.Abs(script.end.vectorPos.y-vectorPos.y)*100
					
						+ Mathf.Abs(script.end.vectorPos.z-vectorPos.z)*100);
						
					} else {
						
						hx = Mathf.Abs(script.end.pos.x-pos.x)*10
					
						//@Performance, comment out the next line if you dont use multiple grids
						+ Mathf.Abs((int)AstarPath.active.grids[script.end.pos.y].offset.y-(int)AstarPath.active.grids[pos.y].offset.y)*AstarPath.active.levelCost
					
						+ Mathf.Abs(script.end.pos.z-pos.z)*10;
						
						//This is another heuristic, try it if you want
						/*int xDistance = Mathf.Abs(script.end.pos.x-pos.x);
						int zDistance = Mathf.Abs(script.end.pos.z-pos.z);
						if (xDistance > zDistance) {
						     hx = 14*zDistance + 10*(xDistance-zDistance);
						} else {
						     hx = 14*xDistance + 10*(zDistance-xDistance);
						}*/
					}
					 
					 
					return hx;
				}
			}
		}
		
		//The script will follow the nodes which has the lowest F scores, read a bit about A* to get more info about the F score.
		public int f {
			get {
				//@Performance, choose the option you want, but hardcoded is faster. This switch statement could get executed several thousand times in one frame!
				switch (AstarPath.active.formula) {
					case Formula.HG:
						//This is the typical A* formula
						return h+g;
					case Formula.H:
						//The formula which only uses F is called Best First Search (I think)
						return h;
						
					case Formula.G:
						//The formula which only uses G is called Dijkstra's algorithm
						return g;
					default:
						return h+g;
				}
			}
		}
		
		public Node () {
			walkable = true;
			costs = null;
		}
		public Node (Node o) {//Copy
			walkable = o.walkable;
			vectorPos = o.vectorPos;
			pos = o.pos;
			angles = o.angles;
			neighbours = o.neighbours;
		}
		
	}
	
	
	
	public struct Int3 {
		public int x;
		public int y;
		public int z;
		
		public Int3 (int x,int y,int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		
		public Int3 (float x2,float y2) {
			x = Mathf.RoundToInt (x2);
			y = 0;
			z = Mathf.RoundToInt (y2);
		}
		
		public Int3 (float x2,float y2,float z2) {
			x = Mathf.RoundToInt (x2);
			y = Mathf.RoundToInt (y2);
			z = Mathf.RoundToInt (z2);
		}
		
		public static Int3 operator + (Int3 lhs, Int3 rhs) {
      		return new Int3 (lhs.x+rhs.x,lhs.y+rhs.y,lhs.z+rhs.z);
  	  	}
  	  	public static Int3 operator - (Int3 lhs, Int3 rhs) {
      		return new Int3 (lhs.x-rhs.x,lhs.y-rhs.y,lhs.z-rhs.z);
  	  	}
  	  	
  	  	public static bool operator == (Int3 lhs, Int3 rhs) {
      		return lhs.x==rhs.x&&lhs.y==rhs.y&&lhs.z==rhs.z;
  	  	}
  	  	
  	  	public static bool operator != (Int3 lhs, Int3 rhs) {
      		return lhs.x!=rhs.x||lhs.y!=rhs.y||lhs.z!=rhs.z;
  	  	}
  	  	
		public static implicit operator Int3 (Vector3 i)
			{
			Int3 temp = new Int3 (i.x,i.y,i.z);
			return temp;
		}
  	  	
  	  	public static implicit operator Vector3 (Int3 i)
			{
			Vector3 temp = new Vector3 (i.x,i.y,i.z);
			return temp;
		}
		
		public static implicit operator Vector2 (Int3 i)
			{
			Vector2 temp = new Vector2 (i.x,i.z);
			return temp;
		}
		
		public override bool Equals(System.Object obj) {
  	  		if (obj == null) {
  	  			return false;
  	  		}
  	  		
  	  		Int3 rhs = (Int3)obj;
  	  		
  	  		return this.x==rhs.x&&this.y==rhs.y&&this.z==rhs.z;
  	  	}
  	  	
  	  	//Returns unique values up to 300*300 grids or 90000 lists (when x = 0,1,2,3,4,5... and z is always 0)
  	  	public override int GetHashCode() {
        	return (int)(y*90000+z*300+x);
		}
		
		public override string ToString () {
  	  		return "("+x+","+y+","+z+")";
  	  	}
  	  	
	}
	
	public enum CoordinateSystem {
		LeftHanded,
		RightHanded
	}
	
	public enum LinkType {
		Link,
		NodeDisabler,
		NodeEnabler,
		Disabler
	}
	
	public enum MeshNodePosition {
		Edge,
		Center
	}
	
	public enum IsNeighbour {
		Eight,
		Four
	}
	
	public enum Formula {
		HG,
		H,
		G
	}
	
	public enum PhysicsType {
		OverlapSphere,
		TouchSphere,
		TouchCapsule,
		Raycast
	}
	
	public enum GridGenerator {
		Grid,
		Texture,
		Mesh,
		Bounds,
		List,
		Procedural
	}
	
	public enum UpDown {
		Up,
		Down
	}
	
	public enum DebugMode {
		Areas,
		Angles,
		H,
		G,
		F
	}
	
	public enum Height {
		Flat,
		Terrain,
		Raycast
	}
	
	public enum Simplify {
		None,
		Simple,
		Full
	}
	
}

//    © Copyright 2009 Aron Granberg
//    AstarClasses.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com