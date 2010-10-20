//    ?Copyright 2009 Aron Granberg
//    AstarPath.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com

//For documentation see http://www.arongranberg.com/unity/a-pathfinding/docs/

using UnityEngine;
using System.Collections;
using AstarClasses;
using AstarMath;

[AddComponentMenu("Pathfinding/A* Pathfinding")]
public class AstarPath : MonoBehaviour
{

    //MAIN

    //There nodes is the nodes all paths will use
    public static int totalNodeAmount = 0;
    public Node[][,] staticNodes;
    public Grid[] grids = new Grid[1] { new Grid(20) };
    public Grid textureGrid = new Grid(20);
    public Grid meshGrid = new Grid(20);
    public NodeLink[] links = new NodeLink[0];

    public GridGenerator gridGenerator = GridGenerator.Grid;

    public BinaryHeap binaryHeap;

    public int levelCost = 40;

    public bool calculateOnStartup = true;

    //Is there any paths calculating currently?
    public static bool activePath = false;

    //The previous calculated path
    public static Path prevPath = null;

    public static Path[] cache;
    public bool cachePaths = false;
    public int cacheLimit = 3;
    public float cacheTimeLimit = 2.0F;


    public bool showGrid = false;

    public bool showGridBounds = true;

    public bool showLinks = true;

    //This script
    public static new AstarPath active;


    public static int[] costs = new int[9] { 14, 10, 14, 10, 10, 14, 10, 14, 20 };

    [System.NonSerialized]
    public int area = -1;

    [System.NonSerialized]
    public Color[] areaColors;

    [System.NonSerialized]
    public static Color[] presetAreaColors = new Color[8] {
		Color.green,
		Color.yellow,
		Color.blue,
		Color.red,
		new Color (1,0.5F,0), //Orange
		Color.cyan,
		Color.white,
		new Color (0.5F,0,1) //Purple
	};

    //See docs http://www.arongranberg.com/unity/a-pathfinding/docs/
    //For explanations about what the variables mean

    //The max time the script can use per frame
    public float maxFrameTime = 0.01F;
    public int maxPathsPerFrame = 3;
    public int pathsThisFrame = 3;
    public int lastPathFrame = -9999;

    public Simplify simplify = Simplify.None;

    public float staticMaxAngle = 45F;
    public bool useNormal = true;

    public float heapSize = 1.0F;

    //Debuging
    public DebugMode debugMode = DebugMode.Areas;
    public float debugModeRoof = 300F;
    public bool showParent = false;
    public bool showUnwalkable = true;
    public bool onlyShowLastPath = false;
    public Path lastPath = null;

    //Non interactive debuging
    [System.NonSerialized]
    public bool anyPhysicsChanged = false;
    [System.NonSerialized]
    public int physicsChangedGrid = -1;

    // Height


    public Height heightMode = Height.Flat;
    public LayerMask groundLayer;

    // 

    public bool dontCutCorners = false;
    public bool testStraightLine = false;
    public float lineAccuracy = 0.5F;

    public Formula formula = Formula.HG;
    public IsNeighbour isNeighbours = IsNeighbour.Eight;
    public float heightRaycast = 100;

    public bool useWorldPositions = false;


    //Physics

    public int ignoreLayer;
    public LayerMask physicsMask = -1;
    public PhysicsType physicsType = PhysicsType.TouchCapsule;
    public UpDown raycastUpDown = UpDown.Down;
    public float raycastLength = 1000;
    public float capsuleHeight = 20;
    public float physicsRadius = 1.0F;


    public float boundsMargin = 0;
    // ----- Texture Scanning ----

    public Texture2D navTex;
    public float threshold = 0.1F;
    public int penaltyMultiplier = 20;

    // ------ Bounds Scanning ----

    public string boundsTag = "";
    public float neighbourDistanceLimit = Mathf.Infinity;
    public float boundMargin = 1;
    public LayerMask boundsRayHitMask = 1;

    //This is used for list scanning too
    public float yLimit = 10;

    // -------- Mesh Scanning ------

    public Vector3 navmeshRotation = Vector3.zero;
    public Mesh navmesh;
    public CoordinateSystem meshCoordinateSystem = CoordinateSystem.RightHanded;
    //public bool removeDownFacingPolys = true;
    [System.NonSerialized]
    public Matrix rotationMatrix = new Matrix();
    public MeshNodePosition meshNodePosition = MeshNodePosition.Edge;

    //This little function generates a rotation matrix for the Mesh mode, it takes rotation, scale and offset into account
    public void GenerateRotationMatrix(Grid grid)
    {
        rotationMatrix =
        (Matrix.RotateX(navmeshRotation.x) *
        Matrix.RotateY(navmeshRotation.y) *
        Matrix.RotateZ(navmeshRotation.z) *
        Matrix.Scale(grid.nodeSize * (meshCoordinateSystem == CoordinateSystem.LeftHanded ? -1.0F : 1.0F)))
        .translate(grid.offset.x, grid.offset.y, grid.offset.z);
    }

    // --------- List Scanning -------

    public Transform listRootNode;


    public void OnDrawGizmos()
    {
        //Draw the grid
        active = this;

        for (int y = 0; y < grids.Length; y++)
        {//Height
            Grid grid = grids[y];
            if (showGridBounds)
            {
                float w = (grid.globalWidth) * grid.nodeSize;
                float d = (grid.globalDepth) * grid.nodeSize;
                Gizmos.color = Color.white;

                //This part draws a 3d wire box around the grid
                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z), new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z));
                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z), new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z + d));

                Gizmos.DrawLine(new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z), new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z + d));
                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z + d), new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z + d));


                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z), new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z));
                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z), new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d));
                Gizmos.DrawLine(new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z), new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d));
                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d), new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d));

                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z), new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z));

                Gizmos.DrawLine(new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z), new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z));

                Gizmos.DrawLine(new Vector3(grid.realOffset.x, grid.realOffset.y, grid.realOffset.z + d), new Vector3(grid.realOffset.x, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d));

                Gizmos.DrawLine(new Vector3(grid.realOffset.x + w, grid.realOffset.y, grid.realOffset.z + d), new Vector3(grid.realOffset.x + w, grid.realOffset.y + grid.globalHeight, grid.realOffset.z + d));
                //End box
            }



            if (staticNodes == null || !showGrid || !grid.debug)
            {
                continue;
            }

            //Loop through all nodes in this grid
            foreach (Node node in staticNodes[y])
            {
                if (!showUnwalkable && !node.walkable)
                {
                    continue;
                }

                if (onlyShowLastPath && node.script != lastPath)
                {
                    continue;
                }

                Color c;
                switch (debugMode)
                {
                    case DebugMode.Areas:
                        c = Color.white;

                        c = (Color)areaColors[node.area];

                        c.a = 0.3F;
                        Gizmos.color = c;
                        break;
                    case DebugMode.Angles:

                        float max = 0;
                        for (int i = 0; i < node.angles.Length; i++)
                        {
                            max = node.angles[i] > max ? node.angles[i] : max;
                        }

                        c = Color.Lerp(Color.green, Color.red, max);
                        Gizmos.color = c;
                        break;
                    case DebugMode.H:
                        c = Color.Lerp(Color.green, Color.red, node.h / debugModeRoof);
                        Gizmos.color = c;
                        break;
                    case DebugMode.G:
                        c = Color.Lerp(Color.green, Color.red, node.g / debugModeRoof);
                        Gizmos.color = c;
                        break;
                    case DebugMode.F:
                        c = Color.Lerp(Color.green, Color.red, node.f / debugModeRoof);
                        Gizmos.color = c;
                        break;
                }

                if (!node.walkable)
                {
                    c = Color.red;
                    c.a = 0.5F;
                    Gizmos.color = c;

                    Gizmos.DrawCube(node.vectorPos, Vector3.one * grid.nodeSize * 0.3F);
                    continue;
                }

                if (node.parent != null && showParent)
                {
                    Gizmos.DrawLine(node.vectorPos, node.parent.vectorPos);
                }
                else
                {
                    for (int i = 0; i < node.neighbours.Length; i++)
                    {
                        Gizmos.DrawLine(node.vectorPos, node.neighbours[i].vectorPos);
                        //Gizmos.DrawRay (node.vectorPos,(node.neighbours[i].vectorPos-node.vectorPos)*0.5F);
                    }
                }

                /*if (anyPhysicsChanged && grid == grids[physicsChangedGrid]) {
                    Gizmos.color = Color.red;
				
                    Vector3 scPos = Camera.current.WorldToScreenPoint (node.vectorPos);
                    if (Mathf.Abs (scPos.x-Screen.width/2) < 30 && Mathf.Abs (scPos.y-Screen.height/2) < 30) {
                        Gizmos.DrawWireSphere (node.vectorPos,grid.nodeSize*grid.physicsRadius);
                    }
                }*/
            }

            if (gridGenerator == GridGenerator.Mesh && navmesh != null)
            {
                Color c = Color.grey;
                c.a = 0.7F;
                Gizmos.color = c;
                Vector3[] verts = navmesh.vertices;
                int[] tris = navmesh.triangles;
                for (int i = 0; i < tris.Length / 3; i++)
                {

                    Vector3 p1 = rotationMatrix.TransformVector(verts[tris[i * 3]]);
                    Vector3 p2 = rotationMatrix.TransformVector(verts[tris[i * 3 + 1]]);
                    Vector3 p3 = rotationMatrix.TransformVector(verts[tris[i * 3 + 2]]);

                    Gizmos.DrawLine(p1, p2);

                    Gizmos.DrawLine(p1, p3);

                    Gizmos.DrawLine(p2, p3);

                    /*Gizmos.DrawLine (
                    RotatePoint (verts[tris[i*3]]*meshGrid.nodeSize)+meshGrid.offset,
                    RotatePoint (verts[tris[i*3+1]]*meshGrid.nodeSize)+meshGrid.offset);
					
                    Gizmos.DrawLine (
                    RotatePoint (verts[tris[i*3]]*meshGrid.nodeSize)+meshGrid.offset,
                    RotatePoint (verts[tris[i*3+2]]*meshGrid.nodeSize)+meshGrid.offset);
					
                    Gizmos.DrawLine (
                    RotatePoint (verts[tris[i*3+1]]*meshGrid.nodeSize)+meshGrid.offset,
                    RotatePoint (verts[tris[i*3+2]]*meshGrid.nodeSize)+meshGrid.offset);*/
                }


            }


        }

        if (staticNodes == null || !showLinks)
        {
            return;
        }

        //Draw all the links
        for (int i = 0; i < links.Length && grids.Length > 0; i++)
        {
            NodeLink link = links[i];
            Int3 from = ToLocal(link.fromVector);
            Node fromNode = null;

            Int3 to = ToLocal(link.toVector);
            //Vector3 toPos = link.toVector;
            Node toNode = null;
            Gizmos.color = Color.green;
            if (from != new Int3(-1, -1, -1) && !grids[from.y].changed)
            {
                fromNode = GetNode(from);//.vectorPos;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            if (to != new Int3(-1, -1, -1) && !grids[to.y].changed)
            {
                //Debug.Log (to + " "+grids.Length +" "+grids[to.y].width+grids[to.y].width);
                toNode = GetNode(to);//.vectorPos;

            }
            else
            {
                Gizmos.color = Color.red;
            }

            switch (link.linkType)
            {
                case LinkType.Link:
                    Gizmos.DrawLine(fromNode == null ? link.fromVector : fromNode.vectorPos, toNode == null ? link.toVector : toNode.vectorPos);
                    break;
                case LinkType.NodeDisabler:
                    if (fromNode != null)
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.6F);
                        Gizmos.DrawSphere(fromNode.vectorPos, grids[from.y].nodeSize * 0.4F);
                    }
                    else
                    {
                        Gizmos.color = new Color(1, 0.5F, 0.5F, 0.6F);
                        Gizmos.DrawSphere(link.fromVector, grids[0].nodeSize * 0.4F);
                    }
                    break;
                case LinkType.NodeEnabler:
                    if (fromNode != null)
                    {
                        Gizmos.color = new Color(0, 1, 0, 0.6F);
                        Gizmos.DrawSphere(fromNode.vectorPos, grids[from.y].nodeSize * 0.4F);
                    }
                    else
                    {
                        Gizmos.color = new Color(1, 0.5F, 0.5F, 0.6F);
                        Gizmos.DrawSphere(link.fromVector, grids[0].nodeSize * 0.4F);
                    }
                    break;
            }
        }

    }

    public int GetUnwalkableNodeAmount()
    {
        int c = 0;
        for (int i = 0; i < grids.Length; i++)
        {
            foreach (Node node in staticNodes[i])
            {
                if (!node.walkable)
                {
                    c++;
                }
            }
        }
        return c;
    }

    public int GetWalkableNodeAmount()
    {
        int c = 0;
        for (int i = 0; i < grids.Length; i++)
        {
            foreach (Node node in staticNodes[i])
            {
                if (node.walkable)
                {
                    c++;
                }
            }
        }
        return c;
    }

    //Call this function to calculate a path
    public static IEnumerator StartPathYield(Path p, Seeker s)
    {
        if (!p.error)
        {

            while (activePath || active.pathsThisFrame >= active.maxPathsPerFrame)
            {
                if (active.lastPathFrame != Time.frameCount && !activePath)
                {
                    active.pathsThisFrame = 0;
                    active.lastPathFrame = Time.frameCount;
                    break;
                }
                yield return 0;
            }
            active.lastPathFrame = Time.frameCount;
            active.pathsThisFrame++;
            //Debug.Log ("Pathfinding started, calculating...\nPathfinding started at frame "+Time.frameCount +"  "+active.pathsThisFrame);
            activePath = true;
            active.lastPath = p;
            if (!p.error)
            {
                p.Init();
            }

            //The error can turn up in the Init function
            if (!p.error && !p.foundEnd)
            {
                p.Calc();//Comment this if you want a bit higher framerate @Performance
                while (!p.foundEnd && !p.error)
                {
                    yield return 0;
                    active.pathsThisFrame = 1;
                    p.Calc();
                }

            }
            activePath = false;
        }
        s.OnComplete(p);
    }

    //Scan the map at startup
    public void Awake()
    {
        active = this;
        if (cachePaths)
        {
            cache = new Path[cacheLimit];
        }
        else
        {
            cache = new Path[1];
        }

        if (calculateOnStartup)
        {
            Scan(true, 0);
            //Debug.Log ("calc");
        }
    }


    //From global position to local position, i.e index in the node array

    //NOTE: This function is a bit unprecise since grid.offset is a Vector2 value and can therefore include decimals
    //Use the ToLocal (Vector3) function for greater precision
    public static Int3 ToLocal(Int3 pos)
    {
        if (active.gridGenerator == GridGenerator.Bounds || active.gridGenerator == GridGenerator.Mesh || active.gridGenerator == GridGenerator.List || active.gridGenerator == GridGenerator.Procedural)
        {
            return ToLocalTest(pos);
        }
        for (int i = 0; i < active.grids.Length; i++)
        {
            Grid grid = active.grids[i];
            if (grid.Contains(pos))
            {
                pos -= new Int3(grid.offset.x, grid.offset.z);
                pos.x = Mathf.RoundToInt(pos.x / grid.nodeSize);
                pos.z = Mathf.RoundToInt(pos.z / grid.nodeSize);
                pos.y = i;
                return pos;
            }
        }
        return new Int3(-1, -1, -1);
    }

    public static Int3 ToLocalTest(Vector3 pos)
    {
        Node nearest = null;
        float shortest = Mathf.Infinity;

        for (int y = 0; y < active.grids.Length; y++)
        {
            Grid grid = active.grids[y];
            if (grid.Contains(pos))
            {
                for (int z = 0; z < grid.depth; z++)
                {
                    for (int x = 0; x < grid.width; x++)
                    {
                        Node node = GetNode(x, y, z);
                        float dist = (pos - node.vectorPos).sqrMagnitude;
                        if (dist < shortest)
                        {
                            nearest = node;
                            shortest = dist;
                        }
                    }
                }

            }
        }
        if (nearest == null)
        {
            return new Int3(-1, -1, -1);
        }
        return nearest.pos;

    }


    public static Int3 ToLocal(Vector3 Vpos, int gridIndex)
    {
        if (active.gridGenerator == GridGenerator.Bounds || active.gridGenerator == GridGenerator.Mesh || active.gridGenerator == GridGenerator.List || active.gridGenerator == GridGenerator.Procedural)
        {
            return ToLocalTest(Vpos);
        }

        Grid grid = active.grids[gridIndex];
        if (grid.Contains(Vpos))
        {
            Vpos -= new Vector3(grid.offset.x, 0, grid.offset.z);
            Int3 pos = Vpos;
            pos.x = Mathf.RoundToInt(Vpos.x / grid.nodeSize);
            pos.z = Mathf.RoundToInt(Vpos.z / grid.nodeSize);
            pos.y = gridIndex;
            return pos;
        }

        return new Int3(-1, -1, -1);
    }

    public static Int3 ToLocal(Vector3 Vpos)
    {
        if (active.gridGenerator == GridGenerator.Bounds || active.gridGenerator == GridGenerator.Mesh || active.gridGenerator == GridGenerator.List || active.gridGenerator == GridGenerator.Procedural)
        {
            return ToLocalTest(Vpos);
        }

        for (int i = 0; i < active.grids.Length; i++)
        {
            Grid grid = active.grids[i];
            if (grid.Contains(Vpos))
            {
                Vpos -= new Vector3(grid.offset.x, 0, grid.offset.z);
                Int3 pos = Vpos;
                pos.x = Mathf.RoundToInt(Vpos.x / (float)grid.nodeSize);
                pos.z = Mathf.RoundToInt(Vpos.z / (float)grid.nodeSize);
                pos.y = i;
                return pos;
            }
        }
        return new Int3(-1, -1, -1);
    }

    //NOTE: This function is a bit unprecise since grid.offset.x is a float value
    //Use the ToLocalX (float) function for greater precision
    public static int ToLocalX(int pos, int level)
    {
        pos -= (int)active.grids[level].offset.x;
        pos = Mathf.RoundToInt(pos / active.grids[level].nodeSize);
        return pos;
    }

    //NOTE: This function is a bit unprecise since grid.offset.z is a float value
    //Use the ToLocalZ (float) function for greater precision
    public static int ToLocalZ(int pos, int level)
    {
        pos -= (int)active.grids[level].offset.z;
        pos = Mathf.RoundToInt(pos / active.grids[level].nodeSize);
        return pos;
    }

    public static float ToLocalX(float pos, int level)
    {
        pos -= active.grids[level].offset.x;
        pos = pos / active.grids[level].nodeSize;
        return pos;
    }
    public static float ToLocalZ(float pos, int level)
    {
        pos -= active.grids[level].offset.z;
        pos = pos / active.grids[level].nodeSize;
        return pos;
    }

    public class Path
    {
        public float pathStartTime = 0;
        //Start/End node
        private Node start;
        public Node end;

        //The open list
        private BinaryHeap open;

        //The node we are currently processing
        private Node current;
        //Have we found the end?
        public bool foundEnd = false;
        private float maxFrameTime = 0.002F;
        private float maxAngle = 20;
        private float angleCost = 2F;
        private bool stepByStep = true;
        //private float unitRadius = 0.5F;//BETA, this variable don't do anything
        public Node[] path;
        public bool error = false;//Has there occured an error while calculating?

        //Debug--
        private float t = 0;//The time the script has calculated
        private int frames = 1;//The number of frames the calculation has taken so far
        private int closedNodes = 0;//The number of searched nodes

        public Path(Vector3 newstart, Vector3 newend, float NmaxAngle, float NangleCost, bool NstepByStep)
        {
            float startTime = Time.realtimeSinceStartup;
            pathStartTime = startTime;
            maxFrameTime = AstarPath.active.maxFrameTime;
            maxAngle = NmaxAngle / 90.0F;
            angleCost = NangleCost;
            stepByStep = NstepByStep;
            //unitRadius = 0;//BETA, Not used
            Int3 startPos = ToLocal(newstart);
            Int3 endPos = ToLocal(newend);
            PostNew(startPos, endPos);
        }

        public Path(Vector3 newstart, Vector3 newend, float NmaxAngle, float NangleCost, bool NstepByStep, int grid)
        {
            float startTime = Time.realtimeSinceStartup;
            pathStartTime = startTime;
            maxFrameTime = AstarPath.active.maxFrameTime;
            maxAngle = NmaxAngle / 90.0F;
            angleCost = NangleCost;
            stepByStep = NstepByStep;
            //unitRadius = 0;//BETA, Not used
            Int3 startPos = ToLocal(newstart, grid);
            Int3 endPos = ToLocal(newend, grid);
            t += Time.realtimeSinceStartup - startTime;
            PostNew(startPos, endPos);
        }

        public void PostNew(Int3 startPos, Int3 endPos)
        {
            float startTime = Time.realtimeSinceStartup;

            if (startPos == new Int3(-1, -1, -1))
            {
                Debug.LogWarning("Start is not inside any grids");
                error = true;
                return;
            }

            if (endPos == new Int3(-1, -1, -1))
            {
                Debug.LogWarning("Target is not inside any grids");
                error = true;
                return;
            }


            start = GetNode(startPos);
            end = GetNode(endPos);
            if (!start.walkable)
            {
                if (start.neighbours.Length > 0)
                {

                    start = start.neighbours[0];
                    Debug.LogWarning("Start point is not walkable, setting a node close to start as start");
                }
                else
                {
                    Debug.LogWarning("Starting from non walkable node");
                    error = true;
                    return;
                }
            }
            current = start;

            if (!end.walkable)
            {
                if (end.neighbours.Length > 0)
                {
                    end = end.neighbours[0];
                    Debug.LogWarning("Target point is not walkable, setting a node close to target as target");
                }
                else
                {
                    Debug.LogWarning("Target point is not walkable");
                    error = true;
                    return;
                }
            }

            if (end.area != start.area)
            {
                Debug.LogWarning("We can't walk from start to end, differend areas");
                error = true;
                return;
            }

            t += Time.realtimeSinceStartup - startTime;
        }

        public void Init()
        {
            //Make a new binary heap (like an array but faster)
            //open = new BinaryHeap (Mathf.CeilToInt (totalNodeAmount));
            open = active.binaryHeap;
            open.numberOfItems = 1;


            float startTime = Time.realtimeSinceStartup;
            start.script = this;
            start.parent = null;

            if (active.cachePaths)
            {
                for (int i = 0; i < cache.Length; i++)
                {
                    Path p = cache[i];
                    if (p != null)
                    {
                        if (Time.realtimeSinceStartup - p.pathStartTime < active.cacheTimeLimit)
                        {
                            if (p.start == start && p.end == end)
                            {
                                path = p.path;
                                foundEnd = true;
                                Debug.Log("A* Pathfinding Completed Succesfully, a cashed path was used");
                                return;
                            }
                        }
                        else
                        {
                            cache[i] = null;
                        }
                    }
                }
            }

            if (active.testStraightLine && CheckLine(start, end, maxAngle))
            {

                foundEnd = true;
                path = new Node[2] { start, end };
                Debug.Log("A* Pathfinding Completed Succesfully, a straight path was used");
                return;
            }
            t += Time.realtimeSinceStartup - startTime;
        }

        //Calculate the path until we find the end or an error occured or we have searched all nodes/the maxFrameTime was exceded
        public void Calc()
        {
            float startTime = Time.realtimeSinceStartup;

            start.script = this;
            start.parent = null;

            //Continue to search while there hasn't ocurred an error and the end hasn't been found
            while (!foundEnd && !error)
            {
                //Close the current node, if the current node is the target node then the path is finnished
                if (current == end)
                {
                    foundEnd = true;
                    break;
                }

                //@Performance Just for debug info
                closedNodes++;

                //Loop through all walkable neighbours of the node
                for (int i = 0; i < current.neighbours.Length; i++)
                {
                    //Debug.DrawLine (current.vectorPos,current.neighbours[i].vectorPos,Color.red); //Uncomment for debug

                    //We shouldn't test the start node
                    if (current.neighbours[i] != start)
                    {
                        Open(current.neighbours[i], i, current.angles[current.neighboursKeys[i]]);
                    }
                }

                //No nodes left to search?
                if (open.numberOfItems <= 0)
                {
                    Debug.LogWarning("No open points, whole area searched");
                    error = true;
                    return;
                }

                //Select the node with the smallest F score and remove it from the open array
                current = open.Remove();

                //Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
                if (stepByStep || Time.realtimeSinceStartup - startTime >= maxFrameTime)
                {//@Performance remove that step By Step thing in the IF, if you don't use in the seeker component

                    t += Time.realtimeSinceStartup - startTime;
                    frames++;

                    //A class can't hold a coroutine, so a separate function need to handle the yield (StartPathYield)
                    return;
                }

            }

            if (!error)
            {


                if (end == start)
                {
                    path = new Node[1] { start };


                    t += Time.realtimeSinceStartup - startTime;

                    //@Performance Debug calls cost performance
                    Debug.Log("A* Pathfinding Completed Succesfully\nTime: " + t + " Seconds\nFrames " + frames + "\nAverage Seconds/Frame " + (t / frames) + "\nPoints:" + path.Length + "\nSearched Nodes" + closedNodes + "\nPath Length (G score) Was " + end.g);

                }
                else if (AstarPath.active.simplify == Simplify.Simple)
                {
                    if (active.gridGenerator != GridGenerator.Grid && active.gridGenerator != GridGenerator.Texture)
                    {
                        Debug.LogError("Simplification can not be used with grid generators other than 'Texture' and 'Grid', excpect weird results");
                    }

                    Node c = end;
                    int p = 0;
                    //Follow the parents of all nodes to the start point, but only add nodes if there is a change in direction
                    int preDir = c.invParentDirection;
                    ArrayList a = new ArrayList();
                    a.Add(c);
                    while (c.parent != null)
                    {

                        if (c.parent.invParentDirection != preDir)
                        {
                            a.Add(c.parent);
                            preDir = c.parent.invParentDirection;
                        }

                        c = c.parent;
                        p++;

                        if (c == start)
                        {

                            break;
                        }

                        if (p > 300)
                        {
                            Debug.LogError("Preventing possible infinity loop");
                            break;
                        }
                    }

                    //Then reverse it so the start node gets the first place in the array
                    a.Reverse();

                    path = a.ToArray(typeof(Node)) as Node[];

                    t += Time.realtimeSinceStartup - startTime;

                    //@Performance Debug calls cost performance
                    Debug.Log("A* Pathfinding Completed Succesfully\nTime: " + t + " Seconds\nFrames " + frames + "\nAverage Seconds/Frame " + (t / frames) + "\nPoints:" + path.Length + " (simplified)" + "\nSearched Nodes" + closedNodes + "\nPath Length (G score) Was " + end.g);

                }
                else if (AstarPath.active.simplify == Simplify.Full)
                {
                    if (active.gridGenerator != GridGenerator.Grid && active.gridGenerator != GridGenerator.Texture)
                    {
                        Debug.LogError("Simplification can not be used with grid generators other than 'Texture' and 'Grid' excpect weird results");
                    }

                    Node c = end;
                    ArrayList a = new ArrayList();
                    a.Add(c);
                    int p = 0;

                    //Follow the parents of all nodes to the start point
                    while (c.parent != null)
                    {
                        a.Add(c.parent);

                        c = c.parent;
                        p++;
                        if (c == start)
                        {

                            break;
                        }

                        //@Performance this IF is almost completely unnecessary
                        if (p > 300)
                        {
                            Debug.LogError("Preventing possible infinity loop, remove this code if you have very long paths");
                            break;
                        }
                    }

                    for (int i = 2; i < a.Count; i++)
                    {
                        if (i >= a.Count)
                        {
                            break;
                        }

                        if (CheckLine((Node)a[i], (Node)a[i - 2], maxAngle))
                        {
                            a.RemoveAt(i - 1);
                            i--;
                        }
                    }
                    //Then reverse it so the start node gets the first place in the array
                    a.Reverse();

                    path = a.ToArray(typeof(Node)) as Node[];


                    t += Time.realtimeSinceStartup - startTime;

                    //@Performance Debug calls cost performance
                    Debug.Log("A* Pathfinding Completed Succesfully\nTime: " + t + " Seconds\nFrames " + frames + "\nAverage Seconds/Frame " + (t / frames) + "\nPoints:" + path.Length + " (simplified)" + "\nSearched Nodes" + closedNodes + "\nPath Length (G score) Was " + end.g);

                    //We have now found the end and filled the "path" array
                    //The next update the Seeker script will find that this is done and send a message with the data
                }
                else
                {
                    Node c = end;
                    ArrayList a = new ArrayList();
                    a.Add(c);
                    int p = 0;

                    //Follow the parents of all nodes to the start point
                    while (c.parent != null)
                    {
                        a.Add(c.parent);

                        c = c.parent;
                        p++;
                        if (c == start)
                        {

                            break;
                        }

                        //@Performance this IF is almost completely unnecessary
                        if (p > 300)
                        {
                            Debug.LogError("Preventing possible infinity loop, remove this code if you have very long paths (i.e more than 300 nodes)");
                            break;
                        }
                    }
                    //Then reverse it so the start node gets the first place in the array
                    a.Reverse();

                    path = a.ToArray(typeof(Node)) as Node[];


                    t += Time.realtimeSinceStartup - startTime;

                    //@Performance Debug calls cost performance
                    Debug.Log("A* Pathfinding Completed Succesfully\nTime: " + t + " Seconds\nFrames " + frames + "\nAverage Seconds/Frame " + (t / frames) + "\nPoints:" + path.Length + "\nSearched Nodes" + closedNodes + "\nPath Length (G score) Was " + end.g);

                    //We have now found the end and filled the "path" array
                    //The next frame the Seeker script will find that this is done and send a message with the path data

                }
            }

            //@Performance Remove the next four lines if you don't use caching
            for (int i = cache.Length - 1; i > 0; i--)
            {
                cache[i] = cache[i - 1];
            }
            cache[0] = this;
            //t += Time.realtimeSinceStartup-startTime;

        }

        public void Open(Node node, int i, float angle)
        {
            //Debug.DrawLine (current.vectorPos,current.neighbours[i].vectorPos,Color.red); //Uncomment for debug
            //If the nodes script variable isn't refering to this path class the node counts as "not used yet" and can then be used
            if (node.script != this)
            {
                //Test if the angle from the current node to this one has exceded the angle limit
                if (angle >= maxAngle)
                {
                    return;
                }

                node.script = this;
                node.invParentDirection = current.neighboursKeys[i];
                node.parent = current;

                node.basicCost = (current.costs == null || costs.Length == 0 ? costs[node.invParentDirection] : current.costs[node.invParentDirection]);
                //Calculate the extra cost of moving in a slope
                node.extraCost = Mathf.RoundToInt(node.basicCost * angle * angleCost);
                //Add the node to the open array
                //Debug.DrawLine (current.vectorPos,current.neighbours[i].vectorPos,Color.green); //Uncomment for @Debug
                open.Add(node);

            }
            else
            {
                //If not we can test if the path from the current node to this one is a better one then the one already used
                int cost = (current.costs == null || current.costs.Length == 0 ? costs[current.neighboursKeys[i]] : current.costs[current.neighboursKeys[i]]);

                int extraCost = Mathf.RoundToInt(node.basicCost * angle * angleCost);

                if (current.g + cost + extraCost + node.penalty < node.g)
                {
                    node.basicCost = cost;
                    node.extraCost = extraCost;
                    node.invParentDirection = current.neighboursKeys[i];
                    node.parent = current;

                    //open.Add (node);//@Quality, uncomment for better quality (I think).

                    //Debug.DrawLine (current.vectorPos,current.neighbours[i].vectorPos,Color.cyan); //Uncomment for @Debug
                }

                else if (node.g + cost + extraCost + current.penalty < current.g)
                {//Or if the path from this node ("node") to the current ("current") is better
                    bool contains = false;

                    //Make sure we don't travel along the wrong direction of a one way link now, make sure the Current node can be accesed from the Node.
                    for (int y = 0; y < node.neighbours.Length; y++)
                    {
                        if (node.neighbours[y] == current)
                        {
                            contains = true;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        return;
                    }

                    current.parent = node;
                    current.basicCost = cost;
                    current.extraCost = extraCost;
                    current.invParentDirection = 7 - current.neighboursKeys[i];
                    //Debug.DrawLine (current.vectorPos,current.neighbours[i].vectorPos,Color.blue); //Uncomment for @Debug
                    open.Add(current);
                }
            }
        }
    }

    public static Node GetNode(Int3 pos)
    {
        return GetNode(pos.x, pos.y, pos.z);//Width, Height, Depth
    }

    public static Node GetNode(int x, int y, int z)
    {
        return AstarPath.active.staticNodes[y][x, z];
    }

    public IEnumerator SetNodes(bool walkable, Bounds bounds, bool fullPhysicsCheck, float t)
    {
        yield return new WaitForSeconds(t);
        SetNodes(walkable, bounds, fullPhysicsCheck, false, -1);
    }

    public IEnumerator SetNodes(bool walkable, Bounds bounds, bool fullPhysicsCheck, bool allGrids, float t)
    {
        yield return new WaitForSeconds(t);
        SetNodes(walkable, bounds, fullPhysicsCheck, allGrids, -1);
    }

    public void SetNodes(bool walkable, Bounds bounds, bool fullPhysicsCheck)
    {
        SetNodes(walkable, bounds, fullPhysicsCheck, false, -1);
    }

    public void SetNodes(bool walkable, Bounds bounds, bool fullPhysicsCheck, bool allGrids)
    {
        SetNodes(walkable, bounds, fullPhysicsCheck, allGrids, -1);
    }

    public void SetNodes(bool walkable, Vector3 point, int gridIndex, bool allGrids)
    {
        if (gridGenerator != GridGenerator.Grid && gridGenerator != GridGenerator.Texture)
        {
            Debug.LogError("The SetNodes function can not be used with grid generators other than 'Texture' and 'Grid'");
            return;
        }

        if (allGrids)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                Grid grid = grids[i];
                Int3 p = ToLocal(point, i);
                if (p != new Int3(-1, -1, -1))
                {
                    Node node = GetNode(p);
                    node.walkable = walkable;
                    RecalcNeighbours(node);
                    for (int z = Mathf.Clamp(p.z - 1, 0, grid.depth); z <= p.z + 1; z++)
                    {
                        for (int x = Mathf.Clamp(p.x - 1, 0, grid.width); x <= p.x + 1; x++)
                        {
                            if (x >= grid.width)
                            {
                                continue;
                            }
                            if (z >= grid.depth)
                            {
                                continue;
                            }
                            Node nodeNeighbour = GetNode(new Int3(x, i, z));
                            RecalcNeighbours(nodeNeighbour);


                        }
                    }
                }
            }
        }
        else
        {
            Int3 p = ToLocal(point, gridIndex);
            if (p != new Int3(-1, -1, -1))
            {
                Grid grid = grids[p.y];
                Node node = GetNode(p);
                node.walkable = walkable;
                RecalcNeighbours(node);

                for (int z = Mathf.Clamp(p.z - 1, 0, grid.depth); z <= p.z + 1; z++)
                {
                    for (int x = Mathf.Clamp(p.x - 1, 0, grid.width); x <= p.x + 1; x++)
                    {
                        if (x >= grid.width)
                        {
                            continue;
                        }
                        if (z >= grid.depth)
                        {
                            continue;
                        }
                        Node nodeNeighbour = GetNode(new Int3(x, gridIndex, z));
                        RecalcNeighbours(nodeNeighbour);


                    }
                }

            }


        }

    }

    public void SetNodes(bool walkable, Bounds bounds, bool fullPhysicsCheck, bool allGrids, LayerMask extraMask)
    {

        if (gridGenerator != GridGenerator.Grid && gridGenerator != GridGenerator.Texture)
        {
            Debug.LogError("The SetNodes function can not be used with grid generators other than 'Texture' and 'Grid'");
            return;
        }

        Vector3 min = bounds.min;
        Vector3 width = bounds.max - min;
        Rect rect = new Rect(min.x, min.z, width.x, width.z);
        Debug.Log("Changing the Grid...");


        if (allGrids)
        {
            bool any = false;
            for (int i = 0; i < grids.Length; i++)
            {
                Int3 p = ToLocal(bounds.center, i);
                if (p != new Int3(-1, -1, -1))
                {
                    SetNodesWorld(walkable, rect, p.y, fullPhysicsCheck, extraMask);
                    any = true;
                }
            }

            if (!any)
            {
                Debug.LogWarning("Can't set nodes, area center is outside all grids");
            }
        }
        else
        {
            Int3 p = ToLocal(bounds.center);
            if (p != new Int3(-1, -1, -1))
            {
                SetNodesWorld(walkable, rect, p.y, fullPhysicsCheck, extraMask);
            }
            else
            {
                Debug.LogWarning("Can't set nodes, area center is outside all grids");
            }
        }
    }

    //Rect is defined in world coordinates
    public void SetNodesWorld(bool walkable, Rect rect, int level, bool fullPhysicsCheck, LayerMask extraMask)
    {

        if (gridGenerator != GridGenerator.Grid && gridGenerator != GridGenerator.Texture)
        {
            Debug.LogError("The SetNodes function can not be used with grid generators other than 'Texture' and 'Grid'");
            return;
        }

        rect.x = Mathf.Floor(ToLocalX(rect.x, level));
        rect.y = Mathf.Floor(ToLocalZ(rect.y, level));
        rect.width = Mathf.Ceil(rect.width / grids[level].nodeSize);
        rect.height = Mathf.Ceil(rect.height / grids[level].nodeSize);

        if (fullPhysicsCheck)
        {
            RecalculateArea(rect, level, extraMask);
        }
        else
        {
            SetNodesLocal(walkable, rect, level);
        }
    }



    //Rect is defined in local coordinates, i.e array index
    public void SetNodesLocal(bool walkable, Rect rect, int level)
    {
        Grid grid = grids[level];
        rect = new Rect(
        Mathf.Clamp(rect.x + 1, 0, grid.width - 1),
        Mathf.Clamp(rect.y + 1, 0, grid.depth - 1),
        Mathf.Clamp(rect.width, 0, grid.width - 1),
        Mathf.Clamp(rect.height, 0, grid.depth - 1));

        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int xMax = (int)rect.xMax;
        int yMax = (int)rect.yMax;

        for (int z = ry; z < yMax; z++)
        {
            for (int x = rx; x < xMax; x++)
            {
                GetNode(x, level, z).walkable = walkable;
            }
        }

        bool anyUnWalkable = false;

        if (walkable)
        {
            for (int z = ry - 1; z < yMax + 1; z++)
            {
                for (int x = rx - 1; x < xMax + 1; x++)
                {
                    if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width)
                    {
                        Node node = GetNode(x, level, z);
                        if (node.walkable)
                        {
                            RecalcNeighbours(node);
                        }
                        else
                        {
                            anyUnWalkable = true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int z = ry - 1; z < yMax + 1; z++)
            {
                for (int x = rx - 1; x < xMax + 1; x++)
                {
                    if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width && (z < ry || z > yMax - 1))
                    {
                        Node node = GetNode(x, level, z);
                        if (node.walkable)
                        {
                            RecalcNeighbours(node);
                        }
                        else
                        {
                            anyUnWalkable = true;
                        }
                    }
                    else if (x >= 0 && x < grid.width && z >= 0 && z < grid.depth && (x < rx || x > xMax - 1))
                    {
                        Node node = GetNode(x, level, z);
                        if (node.walkable)
                        {
                            RecalcNeighbours(node);
                        }
                        else
                        {
                            anyUnWalkable = true;
                        }
                    }
                }
            }
        }

        //This is a random number specifying when a node was flood filled
        int areaTimeStamp = Mathf.RoundToInt(Random.Range(0, 10000));

        //If the whole area outside the area is walkable, don't recalculate flood fill, do it only if we are going to change them to walkable
        if (anyUnWalkable && !walkable)
        {

            for (int z = ry - 1; z < yMax + 1; z++)
            {
                for (int x = rx - 1; x < xMax + 1; x++)
                {
                    if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width && ((z < ry || z > yMax - 1) || (x < rx || x > xMax - 1)))
                    {
                        Node node = GetNode(x, level, z);
                        if (node.walkable)
                        {
                            FloodFill(node, areaTimeStamp);
                        }
                    }
                }
            }
        }
        else
        {
            FloodFill(GetNode(rx, level, ry), areaTimeStamp);
        }
    }

    public void RecalculateArea(Rect rect, int level, LayerMask extraMask)
    {
        Grid grid = grids[level];
        rect = new Rect(
        Mathf.Clamp(rect.x + 1, 0, grid.width - 1),
        Mathf.Clamp(rect.y + 1, 0, grid.depth - 1),
        Mathf.Clamp(rect.width, 0, grid.width - 1),
        Mathf.Clamp(rect.height, 0, grid.depth - 1));

        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int xMax = (int)rect.xMax;
        int yMax = (int)rect.yMax;
        int physRad = Mathf.CeilToInt(physicsRadius);

        for (int z = ry - physRad; z < yMax + physRad; z++)
        {
            for (int x = rx - physRad; x < xMax + physRad; x++)
            {
                if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width)
                {
                    FullPhysicsCheck(GetNode(x, level, z), grids[level], extraMask);
                }
            }
        }

        for (int z = ry - physRad - 1; z < yMax + physRad + 1; z++)
        {
            for (int x = rx - physRad - 1; x < xMax + physRad + 1; x++)
            {
                if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width)
                {
                    Node node = GetNode(x, level, z);
                    if (node.walkable)
                    {
                        RecalcNeighbours(node);
                    }
                }
            }
        }
        //This is a random number specifying when a node was flood filled
        int areaTimeStamp = Mathf.RoundToInt(Random.Range(0, 10000));

        //If the whole area outside the area is walkable, don't recalculate flood fill, do it only if we are going to change them to walkable
        for (int z = ry - physRad - 1; z < yMax + physRad + 1; z++)
        {
            for (int x = rx - physRad - 1; x < xMax + physRad + 1; x++)
            {
                if (z >= 0 && z < grid.depth && x >= 0 && x < grid.width)
                {
                    Node node = GetNode(x, level, z);
                    if (node.walkable)
                    {
                        FloodFill(node, areaTimeStamp);
                    }
                }
            }
        }
    }

    public void RecalcNeighbours(Node node)
    {

        ArrayList keys = new ArrayList();
        ArrayList neighbours = new ArrayList();

        int x = node.pos.x;
        int y = node.pos.y;
        int z = node.pos.z;

        Grid grid = grids[y];

        int topLeftCut = 0;
        int topRightCut = 0;
        int bottomLeftCut = 0;
        int bottomRightCut = 0;

        float staticMaxAngle2 = staticMaxAngle / 90.0F;


        //Add the node left of this node to it's neighbourlist if it is walkable
        if (x != 0 && GetNode(x - 1, y, z).walkable && (isNeighbours == IsNeighbour.Eight || isNeighbours == IsNeighbour.Four) && node.angles[3] < staticMaxAngle2)
        {
            neighbours.Add(GetNode(x - 1, y, z));
            keys.Add(3);
            topLeftCut++;
            bottomLeftCut++;
        }

        if (x != grid.width - 1 && GetNode(x + 1, y, z).walkable && (isNeighbours == IsNeighbour.Eight || isNeighbours == IsNeighbour.Four) && node.angles[4] < staticMaxAngle2)
        {
            neighbours.Add(GetNode(x + 1, y, z));
            keys.Add(4);
            topRightCut++;
            bottomRightCut++;
        }

        if (z != 0)
        {
            if (GetNode(x, y, z - 1).walkable && (isNeighbours == IsNeighbour.Eight || isNeighbours == IsNeighbour.Four) && node.angles[6] < staticMaxAngle2)
            {
                neighbours.Add(GetNode(x, y, z - 1));
                keys.Add(6);
                bottomLeftCut++;
                bottomRightCut++;
            }

            if (x != 0)
            {
                if ((bottomLeftCut == 2 || !dontCutCorners) && GetNode(x - 1, y, z - 1).walkable && isNeighbours == IsNeighbour.Eight && node.angles[5] < staticMaxAngle2)
                {
                    neighbours.Add(GetNode(x - 1, y, z - 1));
                    keys.Add(5);
                }
            }

            if (x != grid.width - 1)
            {
                if ((bottomRightCut == 2 || !dontCutCorners) && GetNode(x + 1, y, z - 1).walkable && isNeighbours == IsNeighbour.Eight && node.angles[7] < staticMaxAngle2)
                {
                    neighbours.Add(GetNode(x + 1, y, z - 1));
                    keys.Add(7);
                }
            }
        }

        if (z != grid.depth - 1)
        {

            if (GetNode(x, y, z + 1).walkable && (isNeighbours == IsNeighbour.Eight || isNeighbours == IsNeighbour.Four) && node.angles[1] < staticMaxAngle2)
            {
                neighbours.Add(GetNode(x, y, z + 1));
                keys.Add(1);
                topLeftCut++;
                topRightCut++;
            }

            if (x != 0 && (topLeftCut == 2 || !dontCutCorners) && GetNode(x - 1, y, z + 1).walkable && isNeighbours == IsNeighbour.Eight && node.angles[0] < staticMaxAngle2)
            {
                neighbours.Add(GetNode(x - 1, y, z + 1));
                keys.Add(0);
            }

            if (x != grid.width - 1 && (topRightCut == 2 || !dontCutCorners) && GetNode(x + 1, y, z + 1).walkable && isNeighbours == IsNeighbour.Eight && node.angles[2] < staticMaxAngle2)
            {
                neighbours.Add(GetNode(x + 1, y, z + 1));
                keys.Add(2);
            }
        }

        node.neighbours = neighbours.ToArray(typeof(Node)) as Node[];
        node.neighboursKeys = keys.ToArray(typeof(int)) as int[];

    }

    public void FullPhysicsCheck(Node node, Grid grid, LayerMask extraMask)
    {

        bool noPhysTest = false;
        switch (heightMode)
        {
            case Height.Flat:
                node.vectorPos.y = grid.offset.y;
                break;
            case Height.Terrain:
                if (Terrain.activeTerrain)
                {
                    node.vectorPos.y = Terrain.activeTerrain.SampleHeight(node.vectorPos);
                }
                break;
            case Height.Raycast:
                RaycastHit hit;
                node.vectorPos.y = grid.offset.y;
                if (Physics.Raycast(node.vectorPos + new Vector3(0, grid.height, 0), -Vector3.up, out hit, grid.height + 0.001F, groundLayer))
                {
                    node.vectorPos.y = hit.point.y;

                    if (useNormal)
                    {
                        Vector3 normal = hit.normal;
                        if (Vector3.Angle(Vector3.up, normal) > staticMaxAngle)
                        {
                            node.walkable = false;
                            noPhysTest = true;
                        }
                    }
                }
                else
                {
                    node.walkable = false;
                    noPhysTest = true;
                }
                break;
        }
        if (noPhysTest)
        {
            return;
        }
        //Test if the node is walkable
        switch (grid.physicsType)
        {
            case PhysicsType.OverlapSphere:

                Collider[] collisions = Physics.OverlapSphere(node.vectorPos, 0.5F * grid.nodeSize * grid.physicsRadius);
                if (collisions.Length > 0)
                {
                    for (int i = 0; i < collisions.Length; i++)
                    {
                        if (collisions[i].gameObject.layer != grid.ignoreLayer)
                        {
                            node.walkable = false;
                        }
                    }
                }
                else
                {
                    node.walkable = true;
                }
                break;
            case PhysicsType.TouchSphere:
                if (Physics.CheckSphere(node.vectorPos, 0.5F * grid.nodeSize * grid.physicsRadius, grid.physicsMask | extraMask))
                {
                    node.walkable = false;
                }
                else
                {
                    node.walkable = true;
                }
                break;
            case PhysicsType.TouchCapsule:
                if (Physics.CheckCapsule(node.vectorPos, node.vectorPos + Vector3.up * grid.capsuleHeight, 0.5F * grid.nodeSize * grid.physicsRadius, grid.physicsMask))
                {
                    node.walkable = false;
                }
                else
                {
                    node.walkable = true;
                }
                break;
            case PhysicsType.Raycast:
                Ray ray = new Ray();
                float l = 0;
                if (grid.raycastUpDown == UpDown.Up)
                {
                    ray = new Ray(node.vectorPos, Vector3.up);
                    l = grid.raycastLength;
                }
                else
                {
                    ray = new Ray(node.vectorPos + Vector3.up * grid.raycastLength, Vector3.down);
                    l = grid.raycastLength - 0.001F;
                }
                //RaycastHit hit;
                if (Physics.Raycast(ray, l, grid.physicsMask))
                {
                    node.walkable = false;
                }
                else
                {
                    node.walkable = true;
                }
                break;
        }
    }

    public static bool CheckLine(Node from, Node to, float maxAngle)
    {
        if (from.pos.y != to.pos.y)
        {
            return false;
        }

        Vector3 dir = (Vector3)(to.pos - from.pos);
        Vector3 dir2 = dir.normalized;

        Vector3 prePos = -Vector3.one;
        for (float i = 0; i < dir.magnitude; i += active.lineAccuracy)
        {
            Int3 pos = from.pos + (Int3)(dir2 * i);
            Node node = GetNode(pos);

            if (i > 0 && prePos != node.vectorPos)
            {
                Vector3 adir = node.vectorPos - prePos;
                Vector3 adir2 = adir;
                adir2.y = 0;
                if (Vector3.Angle(adir, adir2) >= maxAngle)
                {
                    return false;
                }
            }
            prePos = node.vectorPos;

            if (node.walkable == false)
            {
                //Debug.DrawRay (node.vectorPos,Vector3.up,Color.red);
                //Debug.DrawLine (from.vectorPos,to.vectorPos,Color.red);
                return false;
            }
            //Debug.DrawRay (node.vectorPos,Vector3.up,Color.green);
        }
        //Debug.DrawLine (from.vectorPos,to.vectorPos,Color.green);
        return true;
    }

    public void FloodFill(Node node, int areaTimeStamp)
    {

        if (node.areaTimeStamp == areaTimeStamp || !node.walkable)
        {
            return;
        }


        area++;
        ArrayList areaColorsArr = new ArrayList(areaColors);
        areaColorsArr.Add(new Color(Random.value, Random.value, Random.value));
        areaColors = areaColorsArr.ToArray(typeof(Color)) as Color[];
        int searched = 0;
        ArrayList open = new ArrayList();
        Node current = null;

        open.Add(node);
        while (open.Count > 0)
        {
            searched++;
            if (searched > totalNodeAmount)
            {
                Debug.Log("Infinity Loop");
            }
            current = open[0] as Node;
            current.area = area;
            open.Remove(current);
            for (int i = 0; i < current.neighbours.Length; i++)
            {
                if (current.neighbours[i].areaTimeStamp != areaTimeStamp)
                {
                    current.neighbours[i].areaTimeStamp = areaTimeStamp;
                    open.Add(current.neighbours[i]);
                }
            }
        }
        Debug.Log("Flood Filled " + searched + " Nodes, The Grid now contains " + area + " Areas");
    }

    [ContextMenu("Scan Map")]
    public void Scan()
    {
        Scan(true, -1);
    }

    //The Scan function is called when we want to calculate the grid/navmesh (mostly on startup or in the editor).
    public void Scan(bool calcAll, int pass)
    {
        active = this;

        for (int i = 0; i < grids.Length; i++)
        {//Depth
            grids[i].Reset();
        }

        if (gridGenerator == GridGenerator.Texture)
        {
            if (calcAll)
            {
                ScanTexture();
            }
            else
            {
                Debug.LogWarning("The Texture Mode don't use passes, calculate everything once instead");
            }
            binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
            return;
        }

        if (gridGenerator == GridGenerator.Bounds)
        {
            if (calcAll)
            {
                ScanBounds();
            }
            else
            {
                Debug.LogWarning("The Bounds Mode don't use passes, calculate everything once instead");
            }
            binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
            return;

        }

        if (gridGenerator == GridGenerator.List)
        {
            if (calcAll)
            {
                ScanList();
            }
            else
            {
                Debug.LogWarning("The List Mode don't use passes, calculate everything once instead");
            }
            binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
            return;

        }

        if (gridGenerator == GridGenerator.Mesh)
        {
            if (calcAll)
            {
                ScanNavmesh();
            }
            else
            {
                Debug.LogWarning("The Mesh Mode don't use passes, calculate everything once instead");
            }
            binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
            return;

        }

        if (gridGenerator == GridGenerator.Procedural)
        {
            return;
        }

        if (pass == 1 || calcAll)
        {
            staticNodes = new Node[grids.Length][,];
            totalNodeAmount = 0;
            for (int i = 0; i < grids.Length; i++)
            {//Depth
                staticNodes[i] = new Node[grids[i].width, grids[i].depth];
                totalNodeAmount += grids[i].width * grids[i].depth;
                grids[i].globalWidth = grids[i].width - 1;
                grids[i].globalDepth = grids[i].depth - 1;
                grids[i].changed = false;
            }
        }



        //Debug.Log ("Pass 1");
        bool anyWalkable = false;
        for (int y = 0; y < grids.Length && (pass == 1 || calcAll); y++)
        {//Height
            Grid grid = grids[y];
            for (int z = 0; z < grid.depth; z++)
            {//Depth
                for (int x = 0; x < grid.width; x++)
                {//Width
                    //Calculate the position of the node
                    Node node = staticNodes[y][x, z] = new Node();
                    node.pos = new Int3(x, y, z);
                    node.vectorPos = new Vector3(x * grid.nodeSize + grid.offset.x, grid.offset.y, z * grid.nodeSize + grid.offset.z);

                    FullPhysicsCheck(node, grid, 0);
                    if (node.walkable)
                    {
                        anyWalkable = true;
                    }
                }
            }
        }

        //Pass 2
        //Debug.Log ("Pass 2");
        for (int y = 0; y < grids.Length && (pass == 2 || calcAll); y++)
        {//Height
            Grid grid = grids[y];
            for (int z = 0; z < grid.depth; z++)
            {//Depth
                for (int x = 0; x < grid.width; x++)
                {//Width
                    Node node = GetNode(x, y, z);
                    /*Keys:
                    0:Top Left
                    1:Top
                    2:Top Right
                    3:Left
                    4:Right
                    5:Bottom Left
                    6:Bottom
                    7:Bottom Right
                    */

                    //The if's look a bit strange because I wanted to minimize the amount of if's in the code
                    Vector3 vector;
                    if (x != 0)
                    {

                        //The direction to another node
                        vector = GetNode(x - 1, y, z).vectorPos - node.vectorPos;
                        //We can set the other nodes vector angle to -this angle because the direction is the opposite
                        //GetNode(x-1,y,z).vectorAngles[4] = -node.vectorAngles[3];

                        node.angles[3] = (Vector3.Angle(vector, -Vector3.right) / 90.0F);
                        GetNode(x - 1, y, z).angles[4] = node.angles[3];

                    }

                    if (z != 0)
                    {


                        vector = GetNode(x, y, z - 1).vectorPos - node.vectorPos;
                        //GetNode(x,y,z-1).vectorAngles[1] = -node.vectorAngles[6];

                        node.angles[6] = (Vector3.Angle(vector, -Vector3.forward) / 90.0F);
                        GetNode(x, y, z - 1).angles[1] = node.angles[6];

                        if (x != 0)
                        {

                            vector = GetNode(x - 1, y, z - 1).vectorPos - node.vectorPos;
                            //GetNode(x-1,y,z-1).vectorAngles[2] = -node.vectorAngles[5];

                            node.angles[5] = (Vector3.Angle(vector, -Vector3.right - Vector3.forward) / 90.0F);
                            GetNode(x - 1, y, z - 1).angles[2] = node.angles[5];
                        }

                        if (x != grid.width - 1)
                        {

                            vector = GetNode(x + 1, y, z - 1).vectorPos - node.vectorPos;

                            node.angles[7] = (Vector3.Angle(vector, Vector3.right - Vector3.forward) / 90.0F);
                            GetNode(x + 1, y, z - 1).angles[0] = node.angles[7];
                        }
                    }
                }
            }
        }

        if (pass == 3 || calcAll)
        {
            ApplyEnablerLinks();
        }

        //Debug.Log ("Pass 3");
        for (int y = 0; y < grids.Length && (pass == 3 || calcAll); y++)
        {//Height
            Grid grid = grids[y];
            for (int z = 0; z < grid.depth; z++)
            {//Depth
                for (int x = 0; x < grid.width; x++)
                {//Width
                    Node node = GetNode(x, y, z);

                    RecalcNeighbours(node);
                }
            }
        }

        if (pass == 3 || calcAll)
        {
            ApplyLinks();
        }




        if ((pass == 4 || calcAll))
        {

            //Debug.Log ("Pass 4");
            //Pass 4
            FloodFillAll();
        }
        if ((pass == 1 || calcAll) && !anyWalkable)
        {
            Debug.LogError("No nodes are walkable (maybe you should change the layer mask)");
        }

        binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
        //Debug.Log ("Length is "+nodes.Length);
    }

    public void FloodFillAll()
    {

        area = 0;
        int areaTimeStamp = Mathf.RoundToInt(Random.Range(0, 10000));
        int searched = 0;
        ArrayList open = new ArrayList();
        Node current = null;
        ArrayList areaColorsArr = new ArrayList();
        areaColorsArr.Add(new Color(Random.value, Random.value, Random.value));

        int totalWalkableNodeAmount = 0;//The amount of nodes which are walkable

        for (int y = 0; y < grids.Length; y++)
        {//Height
            Grid grid = grids[y];
            for (int z = 0; z < grid.depth; z++)
            {//Depth
                for (int x = 0; x < grid.width; x++)
                {//Depth
                    Node node = GetNode(x, y, z);
                    if (node.walkable)
                    {
                        totalWalkableNodeAmount++;
                    }
                }
            }
        }

        while (searched < totalWalkableNodeAmount)
        {
            area++;

            areaColorsArr.Add(area <= presetAreaColors.Length ? presetAreaColors[area - 1] : new Color(Random.value, Random.value, Random.value));
            if (area > 400)
            {
                Debug.Log("Preventing possible Infinity Loop (Searched " + searched + " nodes in the flood fill pass)");
                break;
            }
            for (int y = 0; y < grids.Length; y++)
            {//Height
                Grid grid = grids[y];
                for (int z = 0; z < grid.depth; z++)
                {//Depth
                    for (int x = 0; x < grid.width; x++)
                    {//Depth
                        Node node = GetNode(x, y, z);
                        if (node.walkable && node.areaTimeStamp != areaTimeStamp && node.neighbours.Length > 0)
                        {
                            node.areaTimeStamp = areaTimeStamp;
                            //searched++;
                            open.Add(node);
                            z = grid.depth;
                            x = grid.width;
                            y = grids.Length;
                        }
                    }
                }
            }

            if (open.Count == 0)
            {
                searched = totalWalkableNodeAmount;
                area--;
                break;
            }

            while (open.Count > 0)
            {
                searched++;


                if (searched > totalWalkableNodeAmount)
                {
                    Debug.LogError("Infinity Loop, can't flood fill more than the total node amount (System Failure)");
                    break;
                }
                current = open[0] as Node;
                current.area = area;
                current.areaTimeStamp = areaTimeStamp;
                open.Remove(current);

                for (int i = 0; i < current.neighbours.Length; i++)
                {
                    if (current.neighbours[i].areaTimeStamp != areaTimeStamp)
                    {
                        current.neighbours[i].areaTimeStamp = areaTimeStamp;
                        open.Add(current.neighbours[i]);

                    }
                }
            }
            open.Clear();
        }

        areaColors = areaColorsArr.ToArray(typeof(Color)) as Color[];

        Debug.Log("Grid contains " + (area) + " Area(s)");
    }

    public void ScanTexture()
    {
        Color[] pixels = navTex.GetPixels(0);

        Grid grid = textureGrid;
        grids = new Grid[1] { grid };
        grid.width = navTex.width;
        grid.depth = navTex.height;

        grid.globalWidth = grid.width - 1;//The first node is placed at position 0, so the actual width of the grid will be one less than the number of nodes in one direction
        grid.globalDepth = grid.depth - 1;

        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];
        totalNodeAmount = grid.depth * grid.width;

        for (int z = 0; z < grid.depth; z++)
        {//Depth/Height
            for (int x = 0; x < grid.width; x++)
            {//Width
                Node node = new Node();
                node.pos = new Int3(x, 0, z);

                node.vectorPos = new Vector3(x * grid.nodeSize, 0, z * grid.nodeSize) + grid.offset;

                if (pixels[z * grid.width + x].grayscale <= threshold)
                {
                    node.walkable = false;
                }

                node.penalty = (int)(pixels[z * grid.width + x].r * penaltyMultiplier);

                node.angles = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                staticNodes[0][x, z] = node;
            }
        }

        ApplyEnablerLinks();

        for (int z = 0; z < grid.depth; z++)
        {//Depth
            for (int x = 0; x < grid.width; x++)
            {//Width
                Node node = GetNode(x, 0, z);
                RecalcNeighbours(node);
            }
        }

        ApplyLinks();

        FloodFillAll();
    }

    public void ScanTilemap(bool[] array, int width, int depth)
    {

        if (width * depth != array.Length)
        {
            Debug.LogError("The array length and width*depth values must match");
            return;
        }
        if (gridGenerator != GridGenerator.Texture)
        {
            Debug.LogError("Only use this grid generator with the Texture mode");
            return;
        }

        if (!calculateOnStartup)
        {
            Debug.LogWarning("To prevent that other grids gets generated at startup just to be replaced by this grid you should switch Calculate Grid On Startup to FALSE");
        }

        Grid grid = textureGrid;
        grids = new Grid[1] { grid };
        grid.width = width;
        grid.depth = depth;

        //The first node is placed at position 0, so the actual width of the grid will be one less than the number of nodes in one direction
        grid.globalWidth = grid.width - 1;
        grid.globalDepth = grid.depth - 1;

        //grid.nodeSize = 1;
        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];
        totalNodeAmount = grid.depth * grid.width;

        for (int z = 0; z < grid.depth; z++)
        {//Depth
            for (int x = 0; x < grid.width; x++)
            {//Width
                Node node = new Node();
                node.pos = new Int3(x, 0, z);

                node.vectorPos = new Vector3(x * grid.nodeSize, 0, z * grid.nodeSize) + grid.offset;
                //new Vector3 (x,0,z) + grid.offset;
                if (!array[z * grid.depth + x])
                {
                    node.walkable = false;
                }
                //node.penalty = (int)(pixels[z*grid.depth+x].r*10);

                node.angles = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                staticNodes[0][x, z] = node;
            }
        }

        ApplyEnablerLinks();

        for (int z = 0; z < grid.depth; z++)
        {//Depth
            for (int x = 0; x < grid.width; x++)
            {//Width
                Node node = GetNode(x, 0, z);
                RecalcNeighbours(node);
            }
        }

        ApplyLinks();

        FloodFillAll();
    }

    public void ScanBounds()
    {

        Collider[] all = FindObjectsOfType(typeof(Collider)) as Collider[];
        ArrayList allArr = new ArrayList();
        for (int i = 0; i < all.Length; i++)
        {
            if (!all[i].isTrigger && all[i].gameObject.tag == boundsTag)
            {
                allArr.Add(all[i]);
            }
        }
        all = allArr.ToArray(typeof(Collider)) as Collider[];

        Vector3[] points = new Vector3[all.Length * 4];
        for (int i = 0; i < all.Length; i++)
        {
            Collider obj = all[i];

            Bounds b = obj.bounds;
            points[i * 4 + 0] = new Vector3(b.extents.x + boundMargin, 0, b.extents.z + boundMargin) + b.center;

            points[i * 4 + 1] = new Vector3(-b.extents.x - boundMargin, 0, -b.extents.z - boundMargin) + b.center;

            points[i * 4 + 2] = new Vector3(b.extents.x + boundMargin, 0, -b.extents.z - boundMargin) + b.center;
            points[i * 4 + 3] = new Vector3(-b.extents.x - boundMargin, 0, b.extents.z + boundMargin) + b.center;


        }

        ArrayList pointArr = new ArrayList();
        for (int i = 0; i < points.Length; i++)
        {
            bool similarity = false;
            for (int y = 0; y < points.Length; y++)
            {
                if (points[i] == points[y] && i != y)
                {
                    similarity = true;
                }
            }
            if (!similarity)
            {
                pointArr.Add(points[i]);
            }
        }

        points = pointArr.ToArray(typeof(Vector3)) as Vector3[];
        GenerateNavmesh(points);
    }

    public void ScanNavmesh()
    {
        if (meshNodePosition == MeshNodePosition.Edge)
        {
            ScanNavmeshEdge();
        }
        else
        {
            ScanNavmeshCenter();
        }
    }

    public void ScanNavmeshEdge()
    {

        Vector3[] meshPoints = navmesh.vertices;
        int[] tris = navmesh.triangles;
        Grid grid = meshGrid;

        GenerateRotationMatrix(grid);

        if (meshPoints.Length <= 3)
        {
            Debug.LogError("Mesh Scanner : Make sure the mesh does contains at least three vertices");
            return;
        }

        for (int i = 0; i < meshPoints.Length; i++)
        {
            //Apply rotation, scale and offset to the points
            meshPoints[i] = rotationMatrix.TransformVector(meshPoints[i]);//f
            //meshPoints[i] = RotatePoint (meshPoints[i])* meshGrid.nodeSize;
        }

        //Calculate all edge points
        Vector3[] points = new Vector3[tris.Length];
        for (int i = 0; i < tris.Length / 3; i++)
        {
            //Vector3 point = ((meshPoints[tris[i*3]] + meshPoints[tris[i*3+1]] + meshPoints[tris[i*3+2]]) / 3);
            Vector3 p1 = meshPoints[tris[i * 3]];
            Vector3 p2 = meshPoints[tris[i * 3 + 1]];
            Vector3 p3 = meshPoints[tris[i * 3 + 2]];
            points[i * 3] = (p1 + p2) * 0.5F;
            points[i * 3 + 1] = (p1 + p3) * 0.5F;
            points[i * 3 + 2] = (p2 + p3) * 0.5F;
        }

        //These arrays store in which triangles the node exists in, at most there will be two triangles for each node and therefore only two arrays
        int[] triConn1 = new int[points.Length];
        int[] triConn2 = new int[points.Length];

        //The counter of how many nodes have been added, some nodes are ignored (see below)
        int c = 0;

        Vector3[] points2 = new Vector3[points.Length];

        //Loop through all points and remove all doubles, store which triangles the node exists in, at most it will be two triangles
        for (int i = 0; i < points.Length; i++)
        {

            if (triConn1[i] == -1)
            {
                continue;
            }

            triConn1[c] = Mathf.FloorToInt(i / 3.0F);
            triConn2[c] = -i;
            points2[c] = points[i];

            //Check for duplicates
            for (int x = i + 1; x < points.Length; x++)
            {
                if (triConn1[x] != -1 && points[x] == points[i])
                {
                    triConn1[x] = -1;//Don't use this node anymore since we have got two copies of it
                    triConn2[c] = Mathf.FloorToInt(x / 3.0F);
                    break;
                }
            }

            //Remove the node if it does only exist in one triangle.
            if (triConn2[c] != -i)
            {
                c++;
            }
        }

        //Make sure there is at least one connection
        if (c == 0)
        {
            Debug.LogError("Mesh Scanner : Make sure there is at least one connection");
            return;
        }

        //Trim the array to only include the nodes which we should use
        Vector3[] points3 = new Vector3[c + 1];
        for (int i = 0; i <= c; i++)
        {
            points3[i] = points2[i];
        }

        points = points3;

        //Create the bounds of the grid
        Bounds b = new Bounds(points[0], Vector3.zero);
        for (int i = 0; i < points.Length; i++)
        {
            b.Encapsulate(points[i]);
        }

        b.extents += Vector3.one * boundsMargin;
        //Debug.Log (""+b.ToString ());
        grids = new Grid[1] { grid };
        grid.width = points.Length;
        grid.depth = 1;
        grid.globalWidth = b.size.x / grid.nodeSize;

        grid.globalDepth = b.size.z / grid.nodeSize;
        grid.globalHeight = b.size.y;

        grid.globalOffset = (b.center - b.extents) - grid.offset;
        //grid.nodeSize = 1;
        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];
        totalNodeAmount = grid.width * grid.depth;

        //Initialize all nodes
        for (int x = 0; x < grid.width; x++)
        {//Width
            Node node = new Node();
            node.pos = new Int3(x, 0, 0);
            node.vectorPos = points[x];

            staticNodes[0][x, 0] = node;
        }

        ApplyEnablerLinks();

        //Create the connections between the nodes
        for (int x = 0; x < grid.width; x++)
        {
            Node node = staticNodes[0][x, 0];

            if (!node.walkable)
            {
                continue;
            }

            ArrayList keys = new ArrayList();
            ArrayList neighbours = new ArrayList();
            ArrayList costs = new ArrayList();
            ArrayList angles = new ArrayList();

            int triConnection1 = triConn1[x];
            int triConnection2 = triConn2[x];

            for (int y = 0; y < grid.width; y++)
            {
                if (y == x)
                {
                    continue;
                }
                int triConnection12 = triConn1[y];
                int triConnection22 = triConn2[y];

                //If another node exists in the same triangle as this one, they should make a connection to each other
                if (triConnection12 == triConnection1 || triConnection12 == triConnection2 || triConnection22 == triConnection1 || triConnection22 == triConnection2)
                {

                    Node otherNode = staticNodes[0][y, 0];

                    if (!otherNode.walkable)
                    {
                        continue;
                    }

                    float dist = (otherNode.vectorPos - node.vectorPos).sqrMagnitude;
                    neighbours.Add(otherNode);
                    costs.Add(Mathf.RoundToInt(Mathf.Sqrt(dist) * 100));
                    keys.Add(keys.Count);
                    Vector3 dir = (otherNode.vectorPos - node.vectorPos);
                    Vector3 dir2 = dir;
                    dir2.y = 0;
                    float a = Vector3.Angle(dir.normalized, dir2.normalized) / 90.0F;
                    angles.Add(a);

                }
            }

            node.neighbours = neighbours.ToArray(typeof(Node)) as Node[];
            node.neighboursKeys = keys.ToArray(typeof(int)) as int[];
            node.costs = costs.ToArray(typeof(int)) as int[];
            node.angles = angles.ToArray(typeof(float)) as float[];
        }

        ApplyLinks();
        FloodFillAll();
    }


    public void ScanNavmeshCenter()
    {


        Vector3[] meshPoints = navmesh.vertices;
        int[] tris = navmesh.triangles;
        Grid grid = meshGrid;

        GenerateRotationMatrix(grid);

        if (meshPoints.Length <= 2)
        {
            Debug.LogError("Make sure the mash does contains at least two vertices");
            return;
        }

        for (int i = 0; i < meshPoints.Length; i++)
        {
            meshPoints[i] = rotationMatrix.TransformVector(meshPoints[i]);
        }

        Vector3[] points = new Vector3[(int)(tris.Length / 3)];
        for (int i = 0; i < tris.Length / 3; i++)
        {
            points[i] = ((meshPoints[tris[i * 3]] + meshPoints[tris[i * 3 + 1]] + meshPoints[tris[i * 3 + 2]]) / 3);
        }

        Bounds b = new Bounds(points[0], Vector3.zero);
        for (int i = 0; i < points.Length; i++)
        {
            b.Encapsulate(points[i]);
        }

        b.extents += Vector3.one * boundsMargin;
        //Debug.Log (""+b.ToString ());
        grids = new Grid[1] { grid };
        grid.width = points.Length;
        grid.depth = 1;
        grid.globalWidth = b.size.x / grid.nodeSize;

        grid.globalDepth = b.size.z / grid.nodeSize;
        grid.globalHeight = b.size.y;

        grid.globalOffset = (b.center - b.extents) - grid.offset;
        //grid.nodeSize = 1;
        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];
        totalNodeAmount = grid.width * grid.depth;


        for (int x = 0; x < grid.width; x++)
        {//Width
            Node node = new Node();
            node.pos = new Int3(x, 0, 0);
            node.vectorPos = points[x];

            staticNodes[0][x, 0] = node;
        }

        ApplyEnablerLinks();

        for (int x = 0; x < grid.width; x++)
        {
            Node node = staticNodes[0][x, 0];

            if (!node.walkable)
            {
                continue;
            }

            ArrayList keys = new ArrayList();
            ArrayList neighbours = new ArrayList();
            ArrayList costs = new ArrayList();
            ArrayList angles = new ArrayList();

            Vector3[] verts = new Vector3[3] {
				meshPoints[tris[x*3]],
				meshPoints[tris[x*3+1]],
				meshPoints[tris[x*3+2]]
			};

            for (int y = 0; y < grid.width; y++)
            {
                if (y == x)
                {
                    continue;
                }

                int similarities = 0;
                //int[] verts2 = new int[3] {tris[y*3],tris[y*3+1],tris[y*3+2]};

                Vector3[] verts2 = new Vector3[3] {
					meshPoints[tris[y*3]],
					meshPoints[tris[y*3+1]],
					meshPoints[tris[y*3+2]]
				};

                foreach (Vector3 vert in verts)
                {
                    foreach (Vector3 vert2 in verts2)
                    {
                        if (vert == vert2)
                        {
                            similarities++;
                        }
                    }
                }

                if (similarities >= 2)
                {

                    Node otherNode = staticNodes[0][y, 0];

                    if (!otherNode.walkable)
                    {
                        continue;
                    }

                    float dist = (otherNode.vectorPos - node.vectorPos).sqrMagnitude;
                    neighbours.Add(otherNode);
                    costs.Add(Mathf.RoundToInt(Mathf.Sqrt(dist) * 100));
                    keys.Add(keys.Count);
                    Vector3 dir = (otherNode.vectorPos - node.vectorPos);
                    Vector3 dir2 = dir;
                    dir2.y = 0;
                    float a = Vector3.Angle(dir.normalized, dir2.normalized) / 90.0F;
                    //Debug.Log ("Angle : "+a);
                    //Debug.DrawRay (node.vectorPos,dir2,Color.yellow);
                    //Debug.Log (a);
                    angles.Add(a);

                }
            }

            node.neighbours = neighbours.ToArray(typeof(Node)) as Node[];
            node.neighboursKeys = keys.ToArray(typeof(int)) as int[];
            node.costs = costs.ToArray(typeof(int)) as int[];
            node.angles = angles.ToArray(typeof(float)) as float[];
        }

        ApplyLinks();
        FloodFillAll();
    }

    public void ScanList()
    {
        if (listRootNode == null)
        {
            Debug.LogError("No Root Node Was Assigned");
            return;
        }

        Transform[] listNodes = GetChildren(listRootNode);//listRootNode.GetComponentsInChildren(typeof(Transform)) as Transform[];
        Vector3[] points = new Vector3[listNodes.Length];
        for (int i = 0; i < listNodes.Length; i++)
        {
            points[i] = listNodes[i].position;
        }
        GenerateNavmesh(points);
    }

    Transform[] GetChildren(Transform parent)
    {
        Transform[] childs = new Transform[parent.childCount];
        int i = 0;
        foreach (Transform child in parent)
        {
            childs[i] = child;
            i++;
        }
        return childs;
    }

    public void ApplyEnablerLinks()
    {
        for (int i = 0; i < links.Length; i++)
        {
            NodeLink link = links[i];

            Int3 from = ToLocal(link.fromVector);
            Node fromNode = null;
            if (from != new Int3(-1, -1, -1))
            {
                fromNode = GetNode(from);
            }
            else
            {
                continue;
            }

            if (link.linkType == LinkType.NodeDisabler)
            {
                fromNode.walkable = false;
            }
            else if (link.linkType == LinkType.NodeEnabler)
            {
                fromNode.walkable = true;
            }

        }
    }

    public void CreateGrid(SimpleNode[] nodes)
    {
        SimpleNode[][] nds = new SimpleNode[1][];
        nds[0] = nodes;
        CreateGrid(nds);
    }


    public void CreateGrid(SimpleNode[][] nodes)
    {
        if (nodes.Length < 1)
        {
            Debug.LogError("Make sure you use at least one grid");
        }



        Grid[] allGrids = new Grid[nodes.Length];
        for (int i = 0; i < allGrids.Length; i++)
        {
            allGrids[i] = new Grid();
            allGrids[i].width = nodes[i].Length;
            allGrids[i].depth = 1;

            if (allGrids[i].width < 1)
            {
                Debug.LogError("Make sure you use at least one node for each grid");
                return;
            }
        }

        staticNodes = new Node[allGrids.Length][,];
        totalNodeAmount = 0;

        for (int i = 0; i < allGrids.Length; i++)
        {
            Grid grid = allGrids[i];
            staticNodes[i] = new Node[grid.width, grid.depth];
            totalNodeAmount += grid.width * grid.depth;
        }

        for (int y = 0; y < allGrids.Length; y++)
        {

            SimpleNode[] gridNodes = nodes[y];

            for (int x = 0; x < gridNodes.Length; x++)
            {
                Node node = new Node();
                node.pos = new Int3(x, y, 0);
                gridNodes[x].pos = node.pos;
                node.vectorPos = gridNodes[x].vectorPos;
                staticNodes[y][x, 0] = node;
            }
        }

        for (int y = 0; y < allGrids.Length; y++)
        {
            Grid grid = allGrids[y];

            SimpleNode[] gridNodes = nodes[y];

            for (int x = 0; x < gridNodes.Length; x++)
            {
                Node node = staticNodes[y][x, 0];

                node.neighbours = new Node[gridNodes[x].neighbours.Length];
                node.costs = new int[gridNodes[x].neighbours.Length];
                node.angles = new float[gridNodes[x].neighbours.Length];
                node.neighboursKeys = new int[gridNodes[x].neighbours.Length];

                for (int i = 0; i < node.neighbours.Length; i++)
                {
                    node.neighbours[i] = GetNode(gridNodes[x].neighbours[i].pos);
                    node.costs[i] = gridNodes[x].costs == null ? (int)(Vector3.Distance(node.neighbours[i].vectorPos, node.vectorPos) * 100) : gridNodes[x].costs[i];
                    node.angles[i] = gridNodes[x].angles == null ? 0 : gridNodes[x].angles[i];
                    node.neighboursKeys[i] = i;
                }
            }


            Bounds b = new Bounds(staticNodes[y][0, 0].vectorPos, Vector3.zero);
            for (int x = 0; x < gridNodes.Length; x++)
            {
                b.Encapsulate(staticNodes[y][x, 0].vectorPos);
            }

            b.extents += Vector3.one * boundsMargin;

            grid.globalWidth = b.size.x;
            grid.globalDepth = b.size.z;
            grid.globalHeight = b.size.y;

            grid.globalOffset = (b.center - b.extents);
        }
        grids = allGrids;
        FloodFillAll();

        binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
    }

    public void CreateGridOLD(SimpleNode[] nodes)
    {
        Grid grid = new Grid();
        grids = new Grid[1] { grid };
        grid.width = nodes.Length;
        grid.depth = 1;

        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];
        totalNodeAmount = nodes.Length;

        for (int x = 0; x < nodes.Length; x++)
        {
            Node node = new Node();
            node.pos = new Int3(x, 0, 0);
            nodes[x].pos = node.pos;
            node.vectorPos = nodes[x].vectorPos;

            staticNodes[0][x, 0] = node;
        }

        for (int x = 0; x < nodes.Length; x++)
        {
            Node node = staticNodes[0][x, 0];

            node.neighbours = new Node[nodes[x].neighbours.Length];
            node.costs = new int[nodes[x].neighbours.Length];
            node.angles = new float[nodes[x].neighbours.Length];
            node.neighboursKeys = new int[nodes[x].neighbours.Length];

            for (int i = 0; i < node.neighbours.Length; i++)
            {
                node.neighbours[i] = GetNode(nodes[x].neighbours[i].pos);
                node.costs[i] = nodes[x].costs == null ? (int)(Vector3.Distance(node.neighbours[i].vectorPos, node.vectorPos) * 100) : nodes[x].costs[i];
                node.angles[i] = nodes[x].angles == null ? 0 : nodes[x].angles[i];
                node.neighboursKeys[i] = i;
            }
        }


        Bounds b = new Bounds(staticNodes[0][0, 0].vectorPos, Vector3.zero);
        for (int x = 0; x < nodes.Length; x++)
        {
            b.Encapsulate(staticNodes[0][x, 0].vectorPos);
        }

        b.extents += Vector3.one * boundsMargin;

        grid.globalWidth = b.size.x;
        grid.globalDepth = b.size.z;
        grid.globalHeight = b.size.y;

        grid.globalOffset = (b.center - b.extents);

        FloodFillAll();

        binaryHeap = new BinaryHeap(Mathf.CeilToInt(totalNodeAmount * heapSize));
    }

    //This function should be called after a grid has been generated, but before the FloodFill, it sets up all links to work properly
    public void ApplyLinks()
    {
        for (int i = 0; i < links.Length; i++)
        {
            NodeLink link = links[i];

            //These types of links are not processed here
            if (link.linkType == LinkType.NodeDisabler || link.linkType == LinkType.NodeEnabler)
            {
                continue;
            }

            Int3 from = ToLocal(link.fromVector);
            Node fromNode = null;

            Int3 to = ToLocal(link.toVector);
            Node toNode = null;
            if (from != new Int3(-1, -1, -1))
            {
                fromNode = GetNode(from);
            }
            else
            {
                continue;
            }

            if (to != new Int3(-1, -1, -1))
            {
                toNode = GetNode(to);
            }
            else
            {
                continue;
            }

            if (!fromNode.walkable || !toNode.walkable)
            {
                continue;
            }

            ArrayList costsArr = fromNode.costs == null ? new ArrayList(costs) : new ArrayList(fromNode.costs);
            ArrayList anglesArr = new ArrayList(fromNode.angles);
            ArrayList neighboursArr = new ArrayList(fromNode.neighbours);
            ArrayList neighboursKeysArr = new ArrayList(fromNode.neighboursKeys);

            if (link.linkType == LinkType.Link)
            {
                //if (gridGenerator == GridGenerator.Grid || gridGenerator == GridGenerator.Texture) {
                //neighboursKeysArr.Add (8);
                //fromNode.angles[8] = 0;
                //} else {
                neighboursKeysArr.Add(neighboursArr.Count);
                anglesArr.Add(0);
                costsArr.Add((int)Vector3.Distance(fromNode.vectorPos, toNode.vectorPos));
                //}
                neighboursArr.Add(toNode);
            }

            fromNode.neighbours = neighboursArr.ToArray(typeof(Node)) as Node[];
            fromNode.neighboursKeys = neighboursKeysArr.ToArray(typeof(int)) as int[];

            //if (gridGenerator != GridGenerator.Grid && gridGenerator != GridGenerator.Texture) {
            fromNode.angles = anglesArr.ToArray(typeof(float)) as float[];
            fromNode.costs = costsArr.ToArray(typeof(int)) as int[];
            //}

            //Make sure the link is bi-directional
            if (!link.oneWay)
            {
                costsArr = toNode.costs == null ? new ArrayList(costs) : new ArrayList(toNode.costs);
                anglesArr = new ArrayList(toNode.angles);
                neighboursArr = new ArrayList(toNode.neighbours);
                neighboursKeysArr = new ArrayList(toNode.neighboursKeys);

                //if (gridGenerator == GridGenerator.Grid || gridGenerator == GridGenerator.Texture) {
                //neighboursKeysArr.Add (8);
                //fromNode.angles[8] = 0;
                //} else {
                //For the non-grid type generators the neighboursKeys array is a linear set of numbers (i.e 1,2,3,4,5 etc)
                neighboursKeysArr.Add(neighboursArr.Count);
                anglesArr.Add(0);
                costsArr.Add((int)Vector3.Distance(fromNode.vectorPos, toNode.vectorPos));
                //}

                neighboursArr.Add(fromNode);

                toNode.neighbours = neighboursArr.ToArray(typeof(Node)) as Node[];
                toNode.neighboursKeys = neighboursKeysArr.ToArray(typeof(int)) as int[];

                //if (gridGenerator != GridGenerator.Grid && gridGenerator != GridGenerator.Texture) {
                toNode.angles = anglesArr.ToArray(typeof(float)) as float[];
                toNode.costs = costsArr.ToArray(typeof(int)) as int[];
                //}
            }
        }
    }

    public void GenerateNavmesh(Vector3[] points)
    {
        Bounds bounds = new Bounds();
        for (int i = 0; i < points.Length; i++)
        {
            bounds.Encapsulate(points[i]);
        }

        bounds.extents += Vector3.one * boundsMargin;

        Grid grid = new Grid(20);
        grids = new Grid[1] { grid };
        grid.width = points.Length;
        grid.depth = 1;
        grid.globalWidth = Mathf.CeilToInt(bounds.size.x);
        grid.globalDepth = Mathf.CeilToInt(bounds.size.z);
        grid.globalHeight = Mathf.CeilToInt(bounds.size.y);
        grid.nodeSize = 1;
        grid.offset = bounds.center - bounds.extents;
        //grid.nodeSize = 1;
        staticNodes = new Node[1][,];
        staticNodes[0] = new Node[grid.width, grid.depth];

        totalNodeAmount = grid.depth * grid.width;
        Debug.Log("Navmesh contains " + totalNodeAmount + " nodes");
        //for (int z=0;z<grid.depth;z++) {//Depth
        for (int x = 0; x < grid.width; x++)
        {//Width
            Node node = new Node();
            node.pos = new Int3(x, 0, 0);
            node.vectorPos = points[x];

            node.angles = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            staticNodes[0][x, 0] = node;
        }

        for (int x = 0; x < grid.width; x++)
        {//Width
            Node node = staticNodes[0][x, 0];
            for (int i = 0; i < grid.width; i++)
            {//Width
                Node otherNode = staticNodes[0][i, 0];
                if (otherNode.vectorPos == node.vectorPos && otherNode != node)
                {
                    //Debug.LogWarning ("Similar "+x+" "+i);
                }
            }
        }

        ApplyEnablerLinks();

        //Loop through all nodes
        for (int x = 0; x < grid.width; x++)
        {//Width
            Node node = staticNodes[0][x, 0];

            if (!node.walkable)
            {
                continue;
            }

            ArrayList keys = new ArrayList();
            ArrayList neighbours = new ArrayList();
            ArrayList costs = new ArrayList();
            ArrayList angles = new ArrayList();

            //Loop through all nodes and see which of them qualifies for being neighbours
            for (int i = 0; i < grid.width; i++)
            {//Width
                Node otherNode = staticNodes[0][i, 0];

                //Limit nodes from being at the same position and from being to far away from each other on the Y axis
                if (otherNode.vectorPos == node.vectorPos || Mathf.Abs(node.vectorPos.y - otherNode.vectorPos.y) > yLimit || !otherNode.walkable)
                {
                    continue;
                }
                if (otherNode == node)
                {
                    continue;
                }

                RaycastHit hit;
                if (Physics.Linecast(node.vectorPos, otherNode.vectorPos, out hit, boundsRayHitMask) || Physics.Linecast(otherNode.vectorPos, node.vectorPos, out hit, boundsRayHitMask))
                {
                    continue;
                }
                else
                {
                    float dist = (node.vectorPos - otherNode.vectorPos).sqrMagnitude;
                    if (dist <= neighbourDistanceLimit * neighbourDistanceLimit)
                    {//hit.distance <= neighbourDistanceLimit/* && hit.collider == all[Mathf.FloorToInt (i/4)]*/) {
                        neighbours.Add(otherNode);
                        costs.Add(Mathf.RoundToInt(Mathf.Sqrt(dist) * 100));
                        keys.Add(keys.Count);
                        angles.Add(0);
                    }
                }
            }
            node.neighbours = neighbours.ToArray(typeof(Node)) as Node[];
            node.neighboursKeys = keys.ToArray(typeof(int)) as int[];
            node.costs = costs.ToArray(typeof(int)) as int[];
            node.angles = angles.ToArray(typeof(float)) as float[];
        }

        ApplyLinks();
        FloodFillAll();

    }

    public static Vector3[] BoundPoints(Bounds b)
    {

        Vector3[] points = new Vector3[4];

        points[0] = new Vector3(b.extents.x, 0, b.extents.z) + b.center;

        points[1] = new Vector3(-b.extents.x, 0, -b.extents.z) + b.center;

        points[2] = new Vector3(b.extents.x, 0, -b.extents.z) + b.center;
        points[2] = new Vector3(-b.extents.x, 0, b.extents.z) + b.center;

        return points;
    }

    //Coroutines can't be called in editor scripts so I have to place it here
    public void SendBugReport(string email, string message)
    {
        StartCoroutine(SendBugReport2(email, message));
    }

    public IEnumerator SendBugReport2(string email, string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("message", message);

        WWW w = new WWW("http://arongranberg.com/wp-content/uploads/astarpathfinding/bugreport.php", form);
        yield return w;
        if (w.error != null)
        {
            Debug.LogError("Error: " + w.error);
        }
        else
        {
            Debug.Log("Bug report sent");
        }
    }



    //Binary Heap

    public class BinaryHeap
    {
        private Node[] binaryHeap;
        public int numberOfItems;

        public BinaryHeap(int numberOfElements)
        {
            binaryHeap = new Node[numberOfElements];
            numberOfItems = 1;
        }

        public void Add(Node node)
        {
            if (this.numberOfItems == this.binaryHeap.Length)
            {
                numberOfItems--;
            }

            this.binaryHeap[this.numberOfItems] = node;

            int bubbleIndex = this.numberOfItems;
            while (bubbleIndex != 1)
            {
                int parentIndex = bubbleIndex / 2;
                if (this.binaryHeap[bubbleIndex].f <= this.binaryHeap[parentIndex].f)
                {
                    Node tmpValue = this.binaryHeap[parentIndex];
                    this.binaryHeap[parentIndex] = this.binaryHeap[bubbleIndex];
                    this.binaryHeap[bubbleIndex] = tmpValue;
                    bubbleIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
            this.numberOfItems++;
        }

        public Node Remove()
        {
            this.numberOfItems--;
            Node returnItem = this.binaryHeap[1];

            this.binaryHeap[1] = this.binaryHeap[this.numberOfItems];

            int swapItem = 1, parent = 1;
            do
            {
                parent = swapItem;
                if ((2 * parent + 1) <= this.numberOfItems)
                {
                    // Both children exist
                    if (this.binaryHeap[parent].f >= this.binaryHeap[2 * parent].f)
                    {
                        swapItem = 2 * parent;
                    }
                    if (this.binaryHeap[swapItem].f >= this.binaryHeap[2 * parent + 1].f)
                    {
                        swapItem = 2 * parent + 1;
                    }
                }
                else if ((2 * parent) <= this.numberOfItems)
                {
                    // Only one child exists
                    if (this.binaryHeap[parent].f >= this.binaryHeap[2 * parent].f)
                    {
                        swapItem = 2 * parent;
                    }
                }
                // One if the parent's children are smaller or equal, swap them
                if (parent != swapItem)
                {
                    Node tmpIndex = this.binaryHeap[parent];
                    this.binaryHeap[parent] = this.binaryHeap[swapItem];
                    this.binaryHeap[swapItem] = tmpIndex;
                }
            } while (parent != swapItem);
            return returnItem;
        }
    }


    /*public class BinaryHeap { 
        public Node[] binaryHeap; 
        public int numberOfItems; 

        public BinaryHeap( int numberOfElements ) { 
            binaryHeap = new Node[numberOfElements]; 
            numberOfItems = 1;
        } 
		
        public void Add(Node node) {
            this.binaryHeap[this.numberOfItems] = node;
			
            int bubbleIndex = this.numberOfItems;
            while (bubbleIndex != 1) {
                int parentIndex = bubbleIndex / 2;
                if (this.binaryHeap[bubbleIndex].h+this.binaryHeap[bubbleIndex].g <= this.binaryHeap[parentIndex].h+this.binaryHeap[parentIndex].g) {
                    Node tmpValue = this.binaryHeap[parentIndex];
                    this.binaryHeap[parentIndex] = this.binaryHeap[bubbleIndex];
                    this.binaryHeap[bubbleIndex] = tmpValue;
                    bubbleIndex = parentIndex;
                } else {
                    break;
                }
            }						 
            this.numberOfItems++;
        }
		
        public Node Remove() {
            this.numberOfItems--;
            Node returnItem = this.binaryHeap[1];
		 	
            this.binaryHeap[1] = this.binaryHeap[this.numberOfItems];
		 
            int swapItem = 1, parent = 1;
            do {
                parent = swapItem;
                if ((2 * parent + 1) <= this.numberOfItems) {
                    // Both children exist
                    if (this.binaryHeap[parent].h+this.binaryHeap[parent].g >= this.binaryHeap[2 * parent].g+this.binaryHeap[2 * parent].h) {
                        swapItem = 2 * parent;
                    }
                    if (this.binaryHeap[swapItem].h+this.binaryHeap[swapItem].g >= this.binaryHeap[2 * parent + 1].h+this.binaryHeap[2 * parent + 1].g) {
                        swapItem = 2 * parent + 1;
                    }
                } else if ((2 * parent) <= this.numberOfItems) {
                    // Only one child exists
                    if (this.binaryHeap[parent].h+this.binaryHeap[parent].g >= this.binaryHeap[2 * parent].h+this.binaryHeap[2 * parent].g) {
                        swapItem = 2 * parent;
                    }
                }
                // One if the parent's children are smaller or equal, swap them
                if (parent != swapItem) {
                    Node tmpIndex = this.binaryHeap[parent];
                    this.binaryHeap[parent] = this.binaryHeap[swapItem];
                    this.binaryHeap[swapItem] = tmpIndex;
                }
            } while (parent != swapItem);
            return returnItem;
        }
    }*/
}

//    For documentation see http://www.arongranberg.com/unity/a-pathfinding/docs/

//    © Copyright 2009 Aron Granberg
//    AstarPath.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com