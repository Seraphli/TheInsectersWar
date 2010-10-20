//    � Copyright 2009 Aron Granberg
//    AstarPathEditor.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com

//For documentation see http://www.arongranberg.com/unity/a-pathfinding/docs/

using UnityEngine;
using System.Collections;
using UnityEditor;
using AstarClasses;
using AstarMath;

[CustomEditor (typeof (AstarPath))]

public class AstarPathEditor : Editor {
	public AstarTab settings = AstarTab.Static;
	public bool showGrids = true;
	public GUISkin gskin;
	public int selectedLink;
	public bool anyGridsChanged = false;
	public bool isReportingBug = false;
	private string bugReportText = "";
	private string bugReportEmail = "";
	public Texture[] icons = new Texture[6];
	// Use this for initialization
	
	public int linkClickStatus = 0;
	public Node firstLinkNode;
	public LinkType linkType;
	public bool isContextViewOpen = false;
	public int contextLinkIndex = 0;
	public bool oneWay = false;
	
	public int dontWarnAboutWorldPositions = 0;
	
	//Gets set to true if there are more than one instance of the AstarPath script in the scene
	public bool singletonWarning = false;
	
	public string version {
		get {
			return "2.52";
		}
	}
	private string updateVersion = "";
	
	public bool checkForUpdates = true;
	
	public GUIStyle separator;
	public enum AstarTab {
		Static,
		Runtime,
		Links,
		Debug
	}
	
	public void OnEnable () {
		AstarPath[] scripts = FindObjectsOfType (typeof(AstarPath)) as AstarPath[];
		if (scripts.Length > 1) {
			singletonWarning = true;
		} else {
			singletonWarning = false;
		}
		
		//Set up the system
		
		if (EditorPrefs.GetBool ("AstarInitialized")) {
			return;
		}
		
		AstarWelcome.Init ();
	}
	
	//Here goes all the code which will show stuff in the Scene View, for example links
	public void OnSceneGUI () {
		AstarPath path = target as AstarPath;
		Event currentEvent = Event.current;
		
		//Scan the grid if it isn't already scanned
		if (path.staticNodes == null && !Application.isPlaying) {
			Scan ();
		}
		
		if (path.anyPhysicsChanged) {
			path.OnDrawGizmos ();
		}
		
		
		//If the active tab is the "Links" tab and if there are any links...
		if (settings == AstarTab.Links && path.links.Length > 0) {
			
			//Draw the handles for each link
			if (selectedLink >= 0 && selectedLink < path.links.Length) {
				NodeLink link = path.links[selectedLink];
				if (link.linkType == LinkType.Link || link.linkType == LinkType.Disabler) {
					link.fromVector = Handles.PositionHandle (link.fromVector,Quaternion.identity);
					link.toVector = Handles.PositionHandle (link.toVector,Quaternion.identity);
				} else {
					link.fromVector = Handles.PositionHandle (link.fromVector,Quaternion.identity);
				}
			}
			
			//Draw lines for the links and check which link is nearest the mouse
			for (int i=0;i<path.links.Length;i++) {
				NodeLink link = path.links[i];
				float dist = 0;
				if (link.linkType == LinkType.Link || link.linkType == LinkType.Disabler) {
					if (link.oneWay) {
						Vector3 relativePos = link.toVector-link.fromVector;
						Handles.DrawLine (link.fromVector,link.toVector); 
						Handles.DrawArrow (1,link.fromVector,Quaternion.LookRotation(relativePos),relativePos.magnitude*0.5F);
					} else {
						Handles.DrawLine (link.fromVector,link.toVector);
					}
					
					dist = HandleUtility.DistanceToLine (link.fromVector,link.toVector);
				} else {
					dist = HandleUtility.DistanceToLine (link.fromVector,link.fromVector+Vector3.up*0.3F);
				}
				Handles.Label (link.fromVector,""+i);
				//Debug.Log (dist);
				HandleUtility.AddControl (-i,dist);
				
			}
			
			//Should we show the links context menu with the button "Remove"
			if (isContextViewOpen && contextLinkIndex >= 0 && contextLinkIndex < path.links.Length) {
				NodeLink contextLink = path.links[contextLinkIndex];
				Vector2 linkScreenPos = HandleUtility.WorldToGUIPoint (contextLink.fromVector);
				Handles.BeginGUI (new Rect (linkScreenPos.x,linkScreenPos.y,100,100));
				if (GUILayout.Button ("Remove")) {
					RemoveLink (contextLinkIndex);
					isContextViewOpen = false;
				}
				Handles.EndGUI ();
				Repaint ();
			}
			
			HandleUtility.AddDefaultControl (1);
			
			if (currentEvent.type == EventType.MouseDown && currentEvent.control && HandleUtility.nearestControl <= 0 && HandleUtility.nearestControl >= -path.links.Length) {
				isContextViewOpen = true;
				contextLinkIndex = -HandleUtility.nearestControl;
				Repaint ();
				HandleUtility.Repaint ();
			}
			
			if (currentEvent.type == EventType.MouseDown && !currentEvent.control) {
				isContextViewOpen = false;
			}
			
			//If no link was in range of the mouse, select the previous selected link
			HandleUtility.AddDefaultControl (-selectedLink);
			
			//REPAINT!!
			Repaint ();
			HandleUtility.Repaint ();
			
			//Select the link nearest the mouse
			if (currentEvent.type == EventType.MouseDown && !currentEvent.control && HandleUtility.nearestControl <= 0 && HandleUtility.nearestControl >= -path.links.Length) {
				selectedLink = -HandleUtility.nearestControl;
				HandleUtility.Repaint ();
				Repaint ();
			}
			
		}
		
		//If active tab is the "Links" tab
		if (settings == AstarTab.Links) {
			
			//Is the grid scanned and is the "Shift" button pressed
			if (path.staticNodes != null && path.grids != null && currentEvent.shift ) {
				
				//Check which node is nearest the mouse cursor
				float minDist = Mathf.Infinity;
				Node minNode = null;
				Vector2 mouse = currentEvent.mousePosition;
				mouse.y = Screen.height - mouse.y;
				for (int y=0;y<path.grids.Length;y++) {
					foreach (Node node in path.staticNodes[y]) {
						Vector2 pos = HandleUtility.WorldToGUIPoint (node.vectorPos);//Camera.current.WorldToScreenPoint (node.vectorPos);
						pos.y = Screen.height - pos.y;
						float dist = Mathf.Abs (pos.x-mouse.x)+Mathf.Abs(pos.y-mouse.y);//(mouse-pos).sqrMagnitude;
						if (dist<minDist) {
							minDist = dist;
							minNode = node;
						}
					}
				}
				
				if (minNode != null) {
					//This control will always be selected since the distance is 0
					HandleUtility.AddControl (1,0);
					
					if (currentEvent.type == EventType.MouseUp) {
						//Select the first node for a link
						if (linkClickStatus == 0) {
							//Make the link type selection buttons pop up
							linkClickStatus = 1;
							firstLinkNode = minNode;
							Repaint ();
							currentEvent.Use();
							
						//Select the second node for a link
						} else if (linkClickStatus == 2 && minNode != firstLinkNode) {
							AddLink (minNode);
						}
					}
					
					//Render the link type selection buttons
					if (linkClickStatus == 1) {
						//Find the screen position of the link's start node
						Vector2 firstLinkScreenPos = HandleUtility.WorldToGUIPoint (firstLinkNode.vectorPos);
						Handles.BeginGUI (new Rect (firstLinkScreenPos.x,firstLinkScreenPos.y,100,100));
						if (GUILayout.Button ("Link")) {
							linkType = LinkType.Link;
							oneWay = false;
							linkClickStatus = 2;
						}
						if (GUILayout.Button ("One Way Link")) {
							linkType = LinkType.Link;
							oneWay = true;
							linkClickStatus = 2;
						}
						if (GUILayout.Button ("Node Disabler")) {
							linkType = LinkType.NodeDisabler;
							//There is no need to select the end node since the NodeDisabler type does only use one node, create the link directly
							AddLink (null);
						}
						if (GUILayout.Button ("Node Enabler")) {
							linkType = LinkType.NodeEnabler;
							AddLink (null);
						}
						
						//The Disabler Type Is Not Fully Functional Yet
						/*if (GUILayout.Button ("Disabler")) {
							linkType = LinkType.Disabler;
							oneWay = false;
							//Now the user can select the end node
							linkClickStatus = 2;
						}*/
						Handles.EndGUI ();
					}
					
					//If the shift key is down and the link type selection buttons aren't showing, render a sphere at the node nearest to the mouse
					if (linkClickStatus != 1) {
						Handles.SphereCap (1,minNode.vectorPos,Quaternion.identity,path.grids[minNode.pos.y].nodeSize);
					}
					
					//If the first node is already selected, render it as a green sphere
					if (firstLinkNode != null) {
						Handles.color = Color.green;
						Handles.SphereCap (0,firstLinkNode.vectorPos,Quaternion.identity,path.grids[firstLinkNode.pos.y].nodeSize);
					}
					
					Repaint ();
					HandleUtility.Repaint ();
				}
				
			} else {
				//Reset
				linkClickStatus = 0;
				firstLinkNode = null;
			}
		} else {
			//Reset
			linkClickStatus = 0;
			firstLinkNode = null;
		}
		
		
		if (path.staticNodes != null && settings == AstarTab.Debug && currentEvent.shift) {
				
			//Check which node is nearest the mouse cursor
			float minDist = Mathf.Infinity;
			Node minNode = null;
			Vector2 mouse = currentEvent.mousePosition;
			mouse.y = Screen.height - mouse.y;
			for (int y=0;y<path.grids.Length;y++) {
				foreach (Node node in path.staticNodes[y]) {
					Vector2 pos = HandleUtility.WorldToGUIPoint (node.vectorPos);//Camera.current.WorldToScreenPoint (node.vectorPos);
					pos.y = Screen.height - pos.y;
					float dist = Mathf.Abs (pos.x-mouse.x)+Mathf.Abs(pos.y-mouse.y);//(mouse-pos).sqrMagnitude;
					if (dist<minDist) {
						minDist = dist;
						minNode = node;
					}
				}
			}
			
			if (minNode == null) {
				return;
			}
			
			for (int i=0;i<minNode.neighbours.Length;i++) {
				Handles.Slider (minNode.vectorPos,minNode.neighbours[i].vectorPos-minNode.vectorPos);
			}
			
			Vector2 debugNodePosition = HandleUtility.WorldToGUIPoint (minNode.vectorPos);
			Handles.BeginGUI (new Rect (debugNodePosition.x,debugNodePosition.y,100,100));
			GUI.color = Color.black;
			GUILayout.Label ("G = "+minNode.g+"\nH = "+minNode.h+"\nF = "+minNode.f+"\nNeighbours "+minNode.neighbours.Length);
			Handles.EndGUI ();
			
			Repaint ();
			HandleUtility.Repaint ();
		}
		if (GUI.changed) {
        	EditorUtility.SetDirty(target); 
		}
	}
	
	public void AddLink (Node endNode) {
		AstarPath path = target as AstarPath;
		Undo.RegisterUndo (path,"Added Link");
		NodeLink link = new NodeLink ();
		link.linkType = linkType;
		link.fromVector = firstLinkNode.vectorPos;
		link.oneWay = oneWay;
		if (endNode != null) {
			link.toVector = endNode.vectorPos;
		}
		ArrayList linkArr = new ArrayList (path.links);
		linkArr.Add (link);
		path.links = linkArr.ToArray (typeof(NodeLink)) as NodeLink[];
		
		//Reset
		linkClickStatus = 0;
		firstLinkNode = null;
	}
	
	public void RemoveLink (int index) {
		AstarPath path = target as AstarPath;
		ArrayList a= new ArrayList (path.links);
		a.RemoveAt (index);
		path.links = a.ToArray (typeof(NodeLink)) as NodeLink[];
		Repaint ();
	}	
	
	
	public override void OnInspectorGUI () {
		
		if (!EditorPrefs.GetBool ("AstarInitialized")) {
			return;
		}
		
		//First the editor will do a series of checks to see if any assets are damaged or if there is a new version of the pathfinding system available
		
		AstarPath path = target as AstarPath;
		EditorGUIUtility.LookLikeInspector ();
		
		//Should we check for updates
		if (EditorPrefs.GetBool ("CheckForUpdates")) {
			GetUpdateNews ();
		} else {
			updateVersion = "noCheck";
		}
		
		//This is the A* info bar at the top of the inspector
		GUIContent co = new GUIContent (version != updateVersion && updateVersion != "error" && updateVersion != "noCheck" ? "You are using an outdated version of this script" : "Your are using version "+version);
		Rect r = GUILayoutUtility.GetRect (co,EditorStyles.toolbarButton);
		GUI.BeginGroup (r);
		
		if (GUI.Button (new Rect (0,0,Screen.width,20),co,EditorStyles.toolbarButton)) {
			if (version != updateVersion && updateVersion != "" && updateVersion != "noCheck") {
				if (updateVersion == "error") {
					if (EditorUtility.DisplayDialog ("A* Pathfinding Project Info","You are using version "+version+" of the A* Pathfinding Project.\n\nHowever the update-check for the system failed, please make sure you have an internet connection or if that isn't the problem, visit the download page to find out the latest info.\n\nThe A* Pathfinding Project was made by\nAron Granberg","Go to download page","Close")) {
						Application.OpenURL ("http://arongranberg.com/unity/a-pathfinding");
					}
				} else {
					if (EditorUtility.DisplayDialog ("A* Pathfinding Project Update","You are using version "+version+" of the A* Pathfinding Project. The latest version is "+updateVersion+".","Go to download page","Close")) {
						Application.OpenURL ("http://arongranberg.com/unity/a-pathfinding");
					}
				}
			} else if (updateVersion != "noCheck") {
				EditorUtility.DisplayDialog ("A* Pathfinding Project Info","You are using version "+version+" of the A* Pathfinding Project, this is the latest version.\n\nThe A* Pathfinding Project was made by\nAron Granberg","Close");
			} else {
				EditorUtility.DisplayDialog ("A* Pathfinding Project Info","You are using version "+version+" of the A* Pathfinding Project\n\nAutomatic update check is turned off, you can turn this on by changing the settings in \nWindow/A*/Welcome\n\nThe A* Pathfinding Project was made by\nAron Granberg","Close");
			}
		}
		GUI.EndGroup ();
		
		if (singletonWarning) {
			co = new GUIContent ("There are more than one instance of the AstarPath script in the scene\n\nYou should never have more than one AstarPath script in the scene");
			GUI.color = Color.red;//(GUILayoutUtility.GetRect (co,EditorStyles.toolbarButton,GUILayout.Height(60))
			GUILayout.Button (co,EditorStyles.notificationBackground);
			GUI.color = Color.white;
			return;
		}
		
		string[] iconNames = new string[11] {
			"grid","texture","mesh","bounds",
			"DontCutCorners_Off","DontCutCorners_On",
			"HeightFlat","HeightTerrain","HeightRaycast",
			"NeighboursFour","NeighboursEight"
		};
		
		if (icons == null || icons[0] == null) {
			icons = new Texture[11];
			
			for (int i=0;i<icons.Length;i++) {
				icons[i] = EditorGUIUtility.Load ("AstarSkin/Icons/"+iconNames[i]+".png") as Texture;
			}
		}
		
		//The script will now check if all icons/skins/styles required for this editor script to run exists in the project, and if not, point the user to a unitypackage which includes the missing files.
		bool allIconsAreFound = true;
		bool skinIsFound = true;
		bool styleIsFound = true;
		string missingIcons = "";
		
		//Does all icons exist
		for (int i=0;i<icons.Length;i++) {
			if (icons[i] == null) {
				allIconsAreFound = false;
				missingIcons += "\n	"+iconNames[i]+".png";
			}
		}
		
		//Try to load the GUISkin
		GUISkin astarSkin = EditorGUIUtility.Load ("AstarSkin/AstarSkin.GUISkin") as GUISkin;
		
		if (astarSkin == null) {
			skinIsFound = false;
			
			//Check if the skin is just named to something else and still exists in the correct folder, but do not show an error message if the icons are also missing, since then the posability is large that all editor assets are missing and another error message should be shown
			Object folder = EditorGUIUtility.Load ("AstarSkin");
			if (folder != null && allIconsAreFound) {
				EditorGUIUtility.PingObject (folder);
				
				//Move the selection to the folder where the skin should exist and search for GUISkins there
				Selection.activeObject = folder;
				Object[] ob = Selection.GetFiltered (typeof(GUISkin),SelectionMode.DeepAssets);
				
				//GUISkins where found
				if (ob.Length > 0) {
					astarSkin = ob[0] as GUISkin;
					string skinName = ob[0].name;
					if (astarSkin != null) {
						if (Event.current.type == EventType.Repaint) {
							//Display an error message
							if (EditorUtility.DisplayDialog ("Damaged Assets","The AstarSkin asset may be named incorrectly.\nFound a GUISkin named "+skinName+" but the correct name should be AstarSkin.","Change Name Automatically","Cancel")) {
								
								string success = AssetDatabase.RenameAsset ("Assets/Editor Default Resources/AstarSkin/"+skinName+".GUISkin","AstarSkin");
								if (success == "") {
									EditorUtility.DisplayDialog ("Damaged Assets","The skin was renamed correctly","Ok");
									EditorGUIUtility.PingObject (astarSkin);
									//Selection.activeObject = path.gameObject;
								} else {
									EditorUtility.DisplayDialog ("Damaged Assets","An error ocurred when renaming the skin:\n"+success,"Ok");
								}
								AssetDatabase.Refresh ();
								return;
							} else {
								return;
							}
						} else {
							return;
						}
					}
				}
			}
		}
		
		//Only look for the style if the skin has been found
		if (skinIsFound) {
			separator = astarSkin.FindStyle ("separator");
			if (separator == null) {
				styleIsFound = false;
			}
			
			GUIStyle minusButton = astarSkin.FindStyle ("MinusButton");
			if (minusButton == null) {
				styleIsFound = false;
			} else if (minusButton.normal.background == null) {
				styleIsFound = false;
			}
		}
		
		//Display an error message if some editor resources are missing
		if (!allIconsAreFound || !styleIsFound || !skinIsFound) {
			if (Event.current.type == EventType.Repaint) {
				switch (EditorUtility.DisplayDialogComplex ("A* Editor Resources Not Found","Some A* editor resources are not found or are damaged, you can download a UnityPackage with the required editor assets or download a package with both editor resources and scripts.\nThe packages can also be found in the example project folder (outside the assets folder)."
				
				+(allIconsAreFound ? "" : "\n\nAll or some icons were not found, the correct location for the icons should be 'Assets/Editor Default Resources/AstarSkin/Icons/'.\n\nThe missing icon files are:"+missingIcons+"\n\nMake sure you haven't renamed the icons since importing a package wont work then.")
				
				+(skinIsFound ? "" : "\n\nThe AstarSkin was not found, the correct location should be 'Assets/Editor Default Resources/AstarSkin/'")
				
				+(styleIsFound ? "" : "\n\nThe AstarSkin asset is damaged, please download and import the asset again")
				
				,"Download Editor Assets","Cancel","Download All Required Assets")) {
					case 0:
						Application.OpenURL ("http://arongranberg.com/wp-content/uploads/astarpathfinding/editorAssets2_5.unitypackage");
						return;
					case 1:
						return;
					case 2:
						Application.OpenURL ("http://arongranberg.com/wp-content/uploads/astarpathfinding/allRequiredAssets2_5.unitypackage");
						return;
				}
			} else {
				return;
			}
		}
		
		GUIStyle labelStyle = EditorStyles.miniLabel;
		labelStyle.wordWrap = true;
		
		settings = (AstarTab)GUILayout.Toolbar ((int)settings,new string [4] {"Static Settings","Runtime Settings","Node Links","Debug"});
		EditorGUILayout.Separator ();
		
		//LINKS
		if (settings == AstarTab.Links) {
			path.showLinks = GUILayout.Toggle (path.showLinks,"Show Links","Button");
			Color preCol = GUI.color;
			GUI.color = new Color (0,0.5F,0);
			if (linkClickStatus == 2) {
				GUILayout.Label ("And now click on a second node which will be the end node",EditorStyles.whiteLabel);
			} else if (Event.current.shift) {	
				GUILayout.Label ("Now click on a node to make it the start node",EditorStyles.whiteLabel);
			} else {
				GUI.color = new Color (0,0.0F,0);
				GUILayout.Label ("Hold shift in the scene view to create links easier\n\nYou can click on links in the scene view to select them\nbut be sure you use the move tool otherwise the it wont work\n\nClick on links with ctrl pressed to remove links",EditorStyles.whiteLabel);
			}
			GUI.color = preCol;
			
			if (GUILayout.Button ("Add Link")) {
				ArrayList a= new ArrayList (path.links);
				a.Add (new NodeLink ());
				path.links = a.ToArray (typeof(NodeLink)) as NodeLink[];
				Repaint ();
				HandleUtility.Repaint ();
			}
			for (int i=0;i<path.links.Length;i++) {
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Toggle (selectedLink == i,"Link "+i,"Button")) {
					selectedLink = i;
				}
				
				if (GUILayout.Button ("Remove",GUILayout.MaxWidth (70))) {
					RemoveLink (i);
					break;
				}
				EditorGUILayout.EndHorizontal ();
				
				if (selectedLink==i) {
					NodeLink link = path.links[i];
					//GUILayout.Label ("Offset");
					//grid.offset.x = EditorGUILayout.FloatField ("X",grid.offset.x);
					//grid.offset.y = EditorGUILayout.FloatField ("Y",grid.offset.y);
					//grid.offset.z = EditorGUILayout.FloatField ("Z",grid.offset.z);
					link.fromVector = Vector3Field ("From",link.fromVector);
					
					if (link.linkType == LinkType.Link || link.linkType == LinkType.Disabler) {
						link.toVector = Vector3Field ("To",link.toVector);
					}
					
					GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					if (link.oneWay && GUILayout.Button ("Reverse")) {
						Vector3 tmpVector = link.fromVector;
						link.fromVector = link.toVector;
						link.toVector = tmpVector;
					}
					link.oneWay = GUILayout.Toggle (link.oneWay,"One Way","Button");
					
					GUILayout.EndHorizontal ();
					
					link.linkType = (LinkType)GUILayout.Toolbar ((int)link.linkType,new string[3] {
						"Link",
						"NodeDisabler",
						"NodeEnabler"
					});
				}
			}
		}
			
			
		//Static Settings
		if (settings == AstarTab.Static) {
			
			//GRIDS START
			path.showGrid = GUILayout.Toggle (path.showGrid,"Show Grid","Button");
			
			path.gridGenerator = (GridGenerator)GUILayout.SelectionGrid ((int)path.gridGenerator,new GUIContent [6] {
				new GUIContent ("Grid",icons[0]),
				new GUIContent ("Texture",icons[1]),
				new GUIContent ("Mesh",icons[2]),
				new GUIContent ("Bounds",icons[3]),
				new GUIContent ("List"),
				new GUIContent ("Procedural")
				}, 3,GUILayout.Height (40));
				
			EditorGUILayout.Separator ();
			 
			if (path.gridGenerator == GridGenerator.Grid || path.gridGenerator == GridGenerator.Texture) {
				path.dontCutCorners = GUILayout.Toggle (path.dontCutCorners,new GUIContent ("Dont Cut Corners",(path.dontCutCorners ? icons[5] : icons[4])) , "Button");
			}
			
			path.cachePaths = GUILayout.Toggle (path.cachePaths, "Cache Paths", "Button");
			
			if (path.cachePaths) {
				path.cacheLimit = EditorGUILayout.IntField ("Cache Limit",path.cacheLimit);
				path.cacheTimeLimit = EditorGUILayout.FloatField ("Cache Time Limit",path.cacheTimeLimit);
			}
			
			EditorGUILayout.Separator ();
			
			if (path.gridGenerator == GridGenerator.Grid) {
				string listTooltip = "This list contains all grids, this also works as a priority list, the higher up a grid is in the list the higher priority it has.\nIf a small grid is inside a big grid you would want the smaller grid to have a higher priority, to do so you can move it higher up";
				
				EditorGUILayout.Separator ();
				showGrids = EditorGUILayout.Foldout (showGrids,new GUIContent ("Grids",listTooltip));
				
				if (showGrids) {
					
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
					EditorGUILayout.BeginVertical ();
					
					if (GUILayout.Button ("Add Grid")) {
						ArrayList a= new ArrayList (path.grids);
						
						a.Add (new Grid (100));
						
						path.grids = a.ToArray (typeof(Grid)) as Grid[];
						Scan ();
					}
					
					//Reset the variable each OnGUI so it will not get turned on and then stay like that forever
					//path.anyPhysicsChanged = false;
					
					for (int i=0;i<path.grids.Length;i++) {
						Grid grid = path.grids[i];
						EditorGUILayout.BeginHorizontal ();
						grid.showInEditor = EditorGUILayout.Foldout (grid.showInEditor,grid.name == "New Grid" ? "Grid "+i : grid.name);
						if (GUILayout.Button ("Remove",astarSkin.GetStyle ("MinusButton"))) {
							ArrayList a= new ArrayList (path.grids);
							a.RemoveAt (i);
							path.grids = a.ToArray (typeof(Grid)) as Grid[];
							Scan ();
							break;
						}
						if (GUILayout.Button (new GUIContent ("Move Up",listTooltip),astarSkin.GetStyle ("ArrowUp")) && i > 0) {
							Grid tmpGrid = path.grids[i-1];
							path.grids[i-1] = path.grids[i];
							path.grids[i] = tmpGrid;
							break;
						}
						if (GUILayout.Button (new GUIContent ("Move Down",listTooltip),astarSkin.GetStyle ("ArrowDown")) && i < path.grids.Length-1) {
							Grid tmpGrid = path.grids[i+1];
							path.grids[i+1] = path.grids[i];
							path.grids[i] = tmpGrid;
							break;
						}
						
						GUILayout.FlexibleSpace ();
						EditorGUILayout.EndHorizontal ();
						
						
						
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.Space ();
						EditorGUILayout.BeginVertical ();
						
						if (grid.showInEditor) {
							bool preChange = GUI.changed;
							GUI.changed = false;
							grid.debug = GUILayout.Toggle (grid.debug,"Show Debug");
							//grid.name = EditorGUILayout.TextField ("Name",grid.name);
							
							grid.width = EditorGUILayout.IntField ("Grid Width",grid.width);
							grid.globalWidth = grid.width-1;
							
							grid.depth = EditorGUILayout.IntField ("Grid Depth",grid.depth);
							grid.globalDepth = grid.depth-1;
							
							grid.nodeSize = EditorGUILayout.FloatField ("Node Size",grid.nodeSize);
							grid.height = EditorGUILayout.FloatField ("Height",grid.height);
							
							//EditorGUILayout.BeginHorizontal ();
							grid.offset = Vector3Field ("Offset",grid.offset);
							//GUILayout.Label ("Offset",GUILayout.Width (100));
							//grid.offset.x = EditorGUILayout.FloatField ("X",grid.offset.x,GUILayout.Width (70));
							//grid.offset.y = EditorGUILayout.FloatField ("Y",grid.offset.y,GUILayout.Width (70));
							//grid.offset.z = EditorGUILayout.FloatField ("Z",grid.offset.z,GUILayout.Width (70));
							//EditorGUILayout.EndHorizontal ();
							
							grid.showPhysics = EditorGUILayout.Foldout (grid.showPhysics,new GUIContent ("Physics","The settings here determines how the script will check for obstacles"));
							if (grid.showPhysics) {
								//Physics Check START
								bool preGUIChanged = GUI.changed;
								GUI.changed = false;
								
								EditorGUILayout.BeginHorizontal ();
								GUILayout.Space (14);
								EditorGUILayout.BeginVertical ();
								
								EditorGUILayout.BeginHorizontal ();
								GUILayout.Label ("Walkable Check");
								
								
								grid.physicsType = (PhysicsType)GUILayout.Toolbar (((int)grid.physicsType),new string [4] {"Overlap","Touch","Capsule","Raycast"});
								
								EditorGUILayout.EndHorizontal ();
								
								if (grid.physicsType==PhysicsType.OverlapSphere) {
									grid.physicsRadius = EditorGUILayout.FloatField ("Radius",grid.physicsRadius);
									grid.ignoreLayer = EditorGUILayout.LayerField ("Ignore Layer",grid.ignoreLayer);
									
								} else if (grid.physicsType==PhysicsType.TouchCapsule) {
									
									grid.capsuleHeight = EditorGUILayout.FloatField ("Height",grid.capsuleHeight);
									grid.physicsRadius = EditorGUILayout.FloatField ("Radius",grid.physicsRadius);
									
									grid.physicsMask = LayerMaskField(grid.physicsMask);
									
								} else if (grid.physicsType==PhysicsType.Raycast) {
									
									grid.physicsMask = LayerMaskField(grid.physicsMask);
									EditorGUILayout.BeginHorizontal ();
									//GUILayout.Label ("Direction");
									GUILayout.Label ("Direction");
									grid.raycastUpDown = (UpDown)GUILayout.Toolbar (((int)grid.raycastUpDown),new string [2] {"Up","Down"});
									EditorGUILayout.EndHorizontal ();
									grid.raycastLength = EditorGUILayout.FloatField ("Length",grid.raycastLength);
									
									
								} else if (grid.physicsType==PhysicsType.TouchSphere) {
									grid.physicsRadius = EditorGUILayout.FloatField ("Radius",grid.physicsRadius);
									grid.physicsMask = LayerMaskField(grid.physicsMask);
								}
								
								EditorGUILayout.EndVertical ();
								EditorGUILayout.EndHorizontal ();
								
								//if (GUI.changed) {
								//	path.physicsChangedGrid = i;
								//	path.anyPhysicsChanged = true;
								//}
								
								GUI.changed = GUI.changed || preGUIChanged;
								//Physics Check END
							}
							
							if (GUI.changed) {
								grid.changed = true;
								anyGridsChanged = true;
							}
							GUI.changed = GUI.changed ? GUI.changed : preChange;
							
							
							
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.EndHorizontal ();
						
					}
					
					
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
					
				}
				
				EditorGUILayout.Separator ();
				GUILayout.Box ("",separator);
				//GRIDS END
			
				//Y POSITION
				EditorGUILayout.BeginHorizontal ();
				
				GUILayout.Label ("Y position");
				
				path.heightMode = (Height)GUILayout.Toolbar (((int)path.heightMode),new GUIContent[3] {
					new GUIContent ("Flat",icons[6]),
					new GUIContent ("Terrain",icons[7]),
					new GUIContent ("Raycast",icons[8])
					});
				
				EditorGUILayout.EndHorizontal ();
				
				if (path.heightMode == Height.Raycast) {
					
					path.heightRaycast = EditorGUILayout.FloatField ("From Height",path.heightRaycast);
					
					path.useNormal = GUILayout.Toggle (path.useNormal,"Use normal for max angle calculation","Button");
					
					path.staticMaxAngle = EditorGUILayout.Slider (new GUIContent ("Static Max Angle", path.useNormal ? "If a normal of a node or an angle between two nodes exceed the static max angle value it will be considered as unwalkable with these settings"
					: "If an angle between two nodes exceed the static max angle value it will be considered as unwalkable with these settings"
					),
					path.staticMaxAngle,0.0F,90.0F);
					
					path.groundLayer = LayerMaskField(path.groundLayer);
					
				} else if (path.heightMode == Height.Terrain) {
					
					path.staticMaxAngle = EditorGUILayout.Slider (new GUIContent ("Static Max Angle",
					"Note: Static Max Angle works best with raycast mode since it can use normals and angles instead of just angles which is the only thing terrain mode can use.\n\nIf an angle between two nodes exceed the static max angle value it will be considered as unwalkable")
					,path.staticMaxAngle,0.0F,90.0F);
				}
				
				EditorGUILayout.Separator ();
			}
			
			
			
			
			if (path.gridGenerator == GridGenerator.Texture) {
				RenderTextureSettings ();
				
			}
			
			if (path.gridGenerator == GridGenerator.Mesh) {
				RenderMeshSettings ();
			}
			
			if (path.gridGenerator == GridGenerator.Procedural) {
				RenderProceduralSettings ();
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (4);
			GUILayout.BeginVertical ();
			if (path.gridGenerator == GridGenerator.Bounds) {
				path.boundsTag = EditorGUILayout.TagField ("Used Tag",path.boundsTag);
				path.neighbourDistanceLimit = EditorGUILayout.FloatField ("Neighbour Distance Limit",path.neighbourDistanceLimit);
				path.boundMargin = EditorGUILayout.FloatField ("Margin",path.boundMargin);
				
				path.boundsRayHitMask = LayerMaskField (path.boundsRayHitMask);
				path.boundsMargin = EditorGUILayout.FloatField ("Grid Bounds Margin",path.boundsMargin);
				path.yLimit = EditorGUILayout.FloatField ("Y Limit",path.yLimit);
			}
			
			if (path.gridGenerator == GridGenerator.List) {
				path.neighbourDistanceLimit = EditorGUILayout.FloatField ("Neighbour Distance Limit",path.neighbourDistanceLimit);
				path.boundMargin = EditorGUILayout.FloatField ("Margin",path.boundMargin);
				
				path.boundsRayHitMask = LayerMaskField (path.boundsRayHitMask);
				path.boundsMargin = EditorGUILayout.FloatField ("Grid Bounds Margin",path.boundsMargin);
				path.yLimit = EditorGUILayout.FloatField ("Y Limit",path.yLimit);
				path.listRootNode = EditorGUILayout.ObjectField ("Root Node",path.listRootNode,typeof (Transform)) as Transform;
			}
				
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		
			GUILayout.Box ("",separator);
			if (path.gridGenerator == GridGenerator.Grid || path.gridGenerator == GridGenerator.Texture) {
				EditorGUILayout.Separator ();
				
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Neighbours");
				path.isNeighbours = (IsNeighbour)GUILayout.Toolbar (((int)path.isNeighbours),new GUIContent [2] {
					new GUIContent ("Eight",icons[10]),
					new GUIContent ("Four",icons[9])
					});
					
				EditorGUILayout.EndHorizontal ();
					
					
				EditorGUILayout.Separator ();
			} 
				
			path.heapSize = EditorGUILayout.Slider ("Heap Size",path.heapSize,0.0F,1.0F);
			if (path.gridGenerator == GridGenerator.Texture) {
				if (path.navTex == null) {
					GUI.color = Color.red;
					if (GUILayout.Button ("Scan Map")) {
						EditorUtility.DisplayDialog ("Can't Generate Navmesh","Please assign a navigation texture","Ok");
					}
					GUI.color = Color.white;
				} else {
					if (GUILayout.Button ("Scan Map")) {
						Scan ();
						path.showGrid = true;
					}
				}
			} else if (path.gridGenerator == GridGenerator.Mesh) {
				if (path.navmesh == null) {
					GUI.color = Color.red;
					if (GUILayout.Button ("Scan Map")) {
						EditorUtility.DisplayDialog ("Can't Generate Navmesh","Please assign a navmesh","Ok");
					}
					
					GUI.color = Color.white;
				} else {
					if (GUILayout.Button ("Scan Map")) {
						Scan ();
						path.showGrid = true;
					}
				}
			} else if (path.gridGenerator == GridGenerator.List) {
				if (path.listRootNode == null) {
					GUI.color = Color.red;
					if (GUILayout.Button ("Scan Map")) {
						EditorUtility.DisplayDialog ("Can't Generate Navmesh","Please assign a root node","Ok");
					}
					GUI.color = Color.white;
				} else {
					if (GUILayout.Button ("Scan Map")) {
						Scan ();
						path.showGrid = true;
					}
				}
			} else {
				if (anyGridsChanged) {
					GUI.color = Color.green;
				}
				if (GUILayout.Button ("Scan Map")) {
					Scan ();
					path.showGrid = true;
				}
				GUI.color = Color.white;
			}
			
			
			
			
			path.calculateOnStartup = GUILayout.Toggle (path.calculateOnStartup,new GUIContent ("Calculate Grid On Startup","Do you want to calculate the grid when the game starts?\nIf not you will have to manually call Scan () to calculate the grid attach a AstarSerializer component and call Load () to load the grid from saved data"),"Button");
			
		}
		
		
		
		//Start for Runtime	settings ----------------------->
		if (settings == AstarTab.Runtime) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Simplification");
			path.simplify = (Simplify)GUILayout.Toolbar (((int)path.simplify),new GUIContent [3] {new GUIContent ("None"),new GUIContent ("Simple","Removes nodes so there are only two nodes for each line, not one node for every step"),new GUIContent ("Full","Tests if there is a better way to travel along the path and removes all nodes which are not neccecery")});
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			
			path.maxFrameTime = EditorGUILayout.FloatField ("Max Frame Time",path.maxFrameTime);
			path.maxPathsPerFrame = EditorGUILayout.IntField ("Max Paths per Frame",path.maxPathsPerFrame);
			
			path.levelCost = EditorGUILayout.IntField (new GUIContent ("Grid to grid cost","The cost for moving from one grid to another\nThe cost from one node to another is 10 or 14 for diagonal"),path.levelCost);
			
			EditorGUILayout.Separator ();
			path.testStraightLine = GUILayout.Toggle (path.testStraightLine,new GUIContent ("Straight Line Testing","Tests if there is a straight line directly to the target first"),"Button");
			
			EditorGUILayout.Separator ();
			path.lineAccuracy = EditorGUILayout.FloatField (new GUIContent ("Line Test Accuracy"
			,"Used for simplification and for straight line testing, lower is better"),
			path.lineAccuracy);
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Formula");
			path.formula = (Formula)GUILayout.Toolbar (((int)path.formula),new GUIContent [3] {
				new GUIContent ("HG","H+G, this is what is commonly used in A*, the cost of moving to this point (G) plus the estimated distance to the target (H)"),
				new GUIContent ("H","H (the heuristic) uses only the estimated distance to the end\nthis is the fastest option but has the lowest accuracy"),
				new GUIContent ("G","G uses only the cost of moving to this point (the current node), this is the slowest option but with the best accuracy.\nThis formula is also often refered to as Dijkstra's algorithm.")
				});
				
			EditorGUILayout.EndHorizontal ();
			
			//'Use World Position' Setting
			if (path.useWorldPositions == false && (path.gridGenerator == GridGenerator.Mesh || path.gridGenerator == GridGenerator.Bounds || path.gridGenerator == GridGenerator.List || path.gridGenerator == GridGenerator.Procedural)) {
				GUI.backgroundColor = Color.red;
				GUILayout.Label ("WARNING: It is recomended to use 'World Positions' when using the 'Mesh', 'List' or the 'Bounds' generator, otherwise you might end up with strange paths",EditorStyles.wordWrappedLabel);
				
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("H calculation");
			path.useWorldPositions = GUILayout.Toolbar ((path.useWorldPositions == true ? 1 : 0),new GUIContent [2] {
			new GUIContent ("Array Positions","Uses the nodes array position to calculate an H value from, recomended for the 'Texture' and 'Grid' generators"),
			new GUIContent ("World Positions","Uses the world position of the node to calculate an H value from, recomended for the 'Mesh', 'List' and 'Bounds' generators")
			}) == 1 ? true : false;
			
			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndHorizontal ();
			//End 'Use World Position' Setting
			
		}
		
		//Start for Debug settings ----------------------->
		if (settings == AstarTab.Debug) {
			path.showGrid = GUILayout.Toggle (path.showGrid,"Show Grid","Button");
			path.showGridBounds = GUILayout.Toggle (path.showGridBounds,"Show Grid Bounds","Button");
			
			GUILayout.Box ("",separator);
			
			GUILayout.Label ("Hold shift in the Scene View to show debug info for the nodes",EditorStyles.miniLabel);
			
			path.showUnwalkable = GUILayout.Toggle (path.showUnwalkable,"Show Unwalkable nodes","Button");
			path.debugMode = (DebugMode)GUILayout.Toolbar (((int)path.debugMode),new GUIContent[5] {new GUIContent ("Areas"),new GUIContent ("Angles"),new GUIContent ("H","H is called the heuristic value\nit is the estimated cost of moving to the target"),new GUIContent ("G","G is the distance from the start to the current node"),new GUIContent ("F","F is most often H+G (see Formula setting for more info) but can be different depending on what settings are used in the Runtime Settings")});
			if (path.debugMode  == DebugMode.H || path.debugMode  == DebugMode.G || path.debugMode  == DebugMode.F) {
				if (!Application.isPlaying) {
					GUILayout.Label ("The H, G & F values are runtime values, you won't see anything in editmode",labelStyle);
				}
				path.debugModeRoof = EditorGUILayout.FloatField ("Max Value",path.debugModeRoof);
			}
			EditorGUILayout.Separator ();
			path.onlyShowLastPath = GUILayout.Toggle (path.onlyShowLastPath,new GUIContent ("Only Show Last Path","In play mode, if this is true, only the nodes which were searched in the last path will be showed"),"Button");
			path.showParent = GUILayout.Toggle (path.showParent,new GUIContent ("Show Search Tree","In play mode, if this is true all nodes will point towards their parents showing the search tree"),"Button");
			GUILayout.Box ("",separator);
			//GUILayout.Space (10);
			if (path.staticNodes != null) {
				GUILayout.Label ("Grid Contains:\n"+AstarPath.totalNodeAmount+" Nodes ("+path.GetWalkableNodeAmount ()+" Walkable and "+path.GetUnwalkableNodeAmount ()+" Unwalkable)\n"
				+path.area+" Areas\n");
			} else {
				GUILayout.Label ("No Info Available");
			}
			
			GUILayout.Box ("",separator);
			if (GUILayout.Button ("Documentation")) {
				Application.OpenURL ("http://arongranberg.com/unity/a-pathfinding/docs/");
			}
			isReportingBug = GUILayout.Toggle (isReportingBug,"Report Bug","Button");
			
			if (isReportingBug) {
				EditorGUIUtility.LookLikeControls ();
				//GUI.color = new Color (0.5F,0.5F,0.5F,1F);
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Email");
				bugReportEmail = EditorGUILayout.TextField ("",bugReportEmail,GUILayout.MinWidth (250));
				GUILayout.EndHorizontal ();
				
				GUILayout.Label ("Message");
				bugReportText = EditorGUILayout.TextArea (bugReportText,GUILayout.MinHeight (200));
				
				if (GUILayout.Button ("Send Bug Report")) {
					path.SendBugReport (bugReportEmail,bugReportText);
					EditorUtility.DisplayDialog ("Thank You!","Your bug report has been sent!","Ok");
					isReportingBug = false;
					bugReportText = "";
				}
				
				/*if (path.reportReturnMessage != "") {
					path.reportReturnMessage = "";
					isReportingBug = false;
					bugReportText = "";
					EditorUtility.DisplayDialog ("Thank You!","Your bug report has been sent!","Ok");
				}*/
			}
		}
		
		
		
		if (GUI.changed) {
			EditorUtility.SetDirty (target);
		}
	}
	
	public void RenderTextureSettings () {
		AstarPath path = target as AstarPath;
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space (4);
		GUILayout.BeginVertical ();
		//GUILayout.Label ("The colors in the navigation texture affects the generated grid, all modification is grayscale so far\nBlack up to [Walkable Threshold] are unwalkable pixels, all other pixels are walkable\nNo color dependent penalty or other special stuff is applied",EditorStyles.miniLabel);
		path.navTex = EditorGUILayout.ObjectField (new GUIContent ("Navigation Texture","The colors in the navigation texture affects the generated grid, all modification is grayscale so far\nBlack up to [Walkable Threshold] are unwalkable pixels, all other pixels are walkable, but...[see penaltyMultiplier tooltip]\n")
		,path.navTex,typeof(Texture2D)) as Texture2D;
		//GUILayout.Box ("",separator);
		
		path.threshold = EditorGUILayout.Slider (new GUIContent ("Walkable Threshold","Black pixels and grayscale values up to this value are unwalkable pixels, the others are walkable"),path.threshold,0.0F,1.0F);
		path.penaltyMultiplier = EditorGUILayout.IntField (new GUIContent ("Penalty Multiplier","All red areas on your navigation texture applies a penalty to the nodes, i.e the nodes becomes harder to walk.\nThe red amount in a pixel can go from 0 to 1 but as a penalty even 1 is quite small, so here's a penalty multiplier so you can scale up the penalty.\n\nAs a comparison the cost of moving from one node to another one is 10 for straight and 14 for diagonal\n\nTip: Don't use white as a background on your texture since white contains red (White = RGB (1,1,1)) and would make all your all your nodes get max penalty, instead you can use green."),path.penaltyMultiplier);
		
		if (path.areaColors.Length > 1) {
			path.areaColors[1] = EditorGUILayout.ColorField ("Area Color",path.areaColors[1]);
		}
		
		Grid grid = path.textureGrid;
		grid.showInEditor = EditorGUILayout.Foldout (grid.showInEditor,grid.name);
		if (grid.showInEditor && path.navTex != null) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (18);
			GUILayout.BeginVertical ();
			
			grid.debug = GUILayout.Toggle (grid.debug,"Show Debug");
			//grid.name = EditorGUILayout.TextField ("Name",grid.name);
			
			GUI.color = new Color (1F,1F,1F,0.5F);
			
			EditorGUILayout.LabelField ("Grid Width",""+path.navTex.width);
			EditorGUILayout.LabelField ("Grid Depth",""+path.navTex.height);
			grid.globalWidth = path.navTex.width-1;//The first node is placed at position 0, so the actual width of the grid will be one less than the number of nodes in one direction
			grid.globalDepth = path.navTex.height-1;
			
			GUI.color = Color.white;
			grid.nodeSize = EditorGUILayout.FloatField ("Node Size",grid.nodeSize);
			grid.height = EditorGUILayout.FloatField ("Height",grid.height);
			//GUILayout.Label ("Offset");
			
			grid.offset = Vector3Field ("Offset",grid.offset);
			/*EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25);
			EditorGUILayout.BeginVertical ();
			grid.offset.x = EditorGUILayout.FloatField ("X",grid.offset.x);
			grid.offset.y = EditorGUILayout.FloatField ("Y",grid.offset.y);
			grid.offset.z = EditorGUILayout.FloatField ("Z",grid.offset.z);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();*/
			
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
		}
		
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.EndHorizontal ();
		
	}
	
	public void RenderProceduralSettings () {
		AstarPath path = target as AstarPath;
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space (4);
		GUILayout.BeginVertical ();
		GUILayout.Label ("This mode is meant to be used along with external scripts to generate grids\nThis mode wont generate any grid unless CreateGrid (...) is called\nIt will behave just as the Mesh/Bounds/List modes during runtime\n",EditorStyles.miniLabel);
		path.boundsMargin = EditorGUILayout.FloatField ("Grid Bounds Margin",path.boundsMargin);
		
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.EndHorizontal ();
	}
	
	public void RenderMeshSettings () {
		AstarPath path = target as AstarPath;
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space (4);
		GUILayout.BeginVertical ();
		
		path.navmesh = EditorGUILayout.ObjectField ("Navmesh",path.navmesh,typeof(Mesh)) as Mesh;
		
		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Node Position");
			path.meshNodePosition = (MeshNodePosition)GUILayout.Toolbar (((int)path.meshNodePosition),new GUIContent [2] {new GUIContent ("Edge"),new GUIContent ("Center")});
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Mesh Coordinate System");
			path.meshCoordinateSystem = (CoordinateSystem)GUILayout.Toolbar (((int)path.meshCoordinateSystem),new GUIContent [2] {new GUIContent ("Left Handed"),new GUIContent ("Right Handed")});
		GUILayout.EndHorizontal ();
		
		EditorGUIUtility.LookLikeControls ();
		path.navmeshRotation = EditorGUILayout.Vector3Field ("Mesh Rotation",path.navmeshRotation);
		EditorGUIUtility.LookLikeInspector ();
		
		path.boundsMargin = EditorGUILayout.FloatField ("Bounds Margin",path.boundsMargin);
		Grid grid = path.meshGrid;
		grid.showInEditor = EditorGUILayout.Foldout (grid.showInEditor,grid.name);
		if (grid.showInEditor && path.navmesh != null) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (18);
			GUILayout.BeginVertical ();
			
			grid.debug = GUILayout.Toggle (grid.debug,"Show Debug");
			//grid.name = EditorGUILayout.TextField ("Name",grid.name);
			
			//GUI.color = new Color (1F,1F,1F,0.5F);
			//EditorGUILayout.LabelField ("Grid Width",""+path.navTex.width);
			//EditorGUILayout.LabelField ("Grid Depth",""+path.navTex.height);
			//GUI.color = Color.white;
			grid.nodeSize = EditorGUILayout.FloatField ("Scale",grid.nodeSize);
			
			//grid.height = EditorGUILayout.FloatField ("Height",grid.height);
			grid.offset = Vector3Field ("Offset",grid.offset);
			
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
		}
		
		if (GUI.changed) {
			path.GenerateRotationMatrix (grid);
			HandleUtility.Repaint ();
			Repaint ();
			OnSceneGUI ();
		}
		
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.EndHorizontal ();
		
	}
	
	
	//----
	
	public static Vector3 Vector3Field (string label, Vector3 vector) {
		EditorGUIUtility.LookLikeControls ();
		vector = EditorGUILayout.Vector3Field (label,vector);
		EditorGUIUtility.LookLikeInspector ();
		return vector;
	}
	
	public static int LayerMaskField (LayerMask selected) {
		
		ArrayList layers = new ArrayList ();
		
		ArrayList layerNumbers = new ArrayList ();
		
		//
		layerNumbers.Add (-1);
		
		//layers.Add ("o "+layerName);
		
		string selectedLayers = "";
		for (int i=0;i<32;i++) {
			
			string layerName = LayerMask.LayerToName (i);
			
			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					
					selectedLayers += (selectedLayers != "" ? " I " : "") +layerName;
					
					if (selectedLayers.Length >= 25) {
						string selectedLayers2 = "";
						for (int y=0;y<selectedLayers.Length && y< 25;y++) {
							selectedLayers2 += selectedLayers[y];
						}
						
						selectedLayers = selectedLayers2;
						selectedLayers+="...";
						i = 32;
						break;
					}
				}
			}
		}
		
		if (selectedLayers == "") {
			layers.Add ("Choose Layers");
		} else {
			layers.Add (selectedLayers);
		}
		
		for (int i=0;i<32;i++) {
			
			string layerName = LayerMask.LayerToName (i);
			
			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					layers.Add ("o "+layerName);
				} else {
					layers.Add ("   "+layerName);
				}
				layerNumbers.Add (i);
			}
		}
		
		bool preChange = GUI.changed;
		
		GUI.changed = false;
		
		int[] LayerNumbers = layerNumbers.ToArray (typeof(int)) as int[];
		
		int newSelected = EditorGUILayout.Popup ("Mask",0,layers.ToArray(typeof(string)) as string[],EditorStyles.layerMaskField);
		
		
		if (GUI.changed && newSelected != 0) {
			if (selected == (selected | (1 << LayerNumbers[newSelected]))) {
				selected &= ~(1 << LayerNumbers[newSelected]);
				//Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
			} else {
				//Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
				selected = selected | (1 << LayerNumbers[newSelected]);
			}
			
		} else {
			GUI.changed = preChange;
		}
		
		return selected;
	}
	
	public void Scan () {
		AstarPath path = target as AstarPath;
		
		//Check if the navigation texture uses the correct settings, the texture need to be readable and need to use either RGB24 or ARGB32
		if (path.gridGenerator == GridGenerator.Texture && path.navTex != null) {
			
			string texturePath = AssetDatabase.GetAssetPath(path.navTex);
			TextureImporter texImport = AssetImporter.GetAtPath (texturePath) as TextureImporter;
			if (!texImport.isReadable) {
				if (EditorUtility.DisplayDialog ("Navigation Texture is not Readable!","The navigation texture you have assigned is not readable","Make readable automatically","Cancel")) {
					texImport.isReadable = true;
				} else {
					return;
				}
				AssetDatabase.ImportAsset (texturePath);
			}
			
			if(texImport.textureFormat != TextureImporterFormat.RGB24 && texImport.textureFormat != TextureImporterFormat.ARGB32) {
				switch (EditorUtility.DisplayDialogComplex ("Wrong Texure Format!","The navigation texture you have assigned does not use an accepted texture format.\nThe texture should use either RGB24 or ARGB32.","Change to RGB24","Cancel","Change to ARGB32")) {
					case 0:
						texImport.textureFormat = TextureImporterFormat.RGB24;
						break;
					case 1:
						return;
					case 2:
						texImport.textureFormat = TextureImporterFormat.ARGB32;
						break;
				}
				
				AssetDatabase.ImportAsset (texturePath);
			}
		}
		
		dontWarnAboutWorldPositions--;
		if (path.useWorldPositions == false && dontWarnAboutWorldPositions <= 0 && (path.gridGenerator == GridGenerator.Mesh || path.gridGenerator == GridGenerator.Bounds || path.gridGenerator == GridGenerator.List)) {
			switch (EditorUtility.DisplayDialogComplex ("Use World Positions","WARNING: It is recomended to use 'World Positions' when using the 'Mesh', 'List' or the 'Bounds' generator, otherwise you might end up with strange paths.\nYou can change this setting in the Runtime Settings.","Change to world positions automatically","Continue anyway","Remind me later")) {
				case 0:
					path.useWorldPositions = true;
					break;
				case 1:
					break;
				case 2:
					dontWarnAboutWorldPositions = 15;
					break;
			}
		}
		
		anyGridsChanged = false;
		((AstarPath)target).Scan (true,0);
	}
	
	private WWW w;
	public void GetUpdateNews () {
		updateVersion = version;
		if (w == null) {
			w = new WWW ("http://arongranberg.com/wp-content/uploads/astarpathfinding/update.php");
		} else if (w.isDone && w.error == null) {
			string data = w.data;
			updateVersion = data;
		} else if (w.error != null) {
			updateVersion = "error";
		}
	}
}

//    � Copyright 2009 Aron Granberg
//    AstarPathEditor.cs script is licenced under a Creative Commons Attribution-Noncommercial 3.0 Unported License.
//    If you want to use the script in commercial projects, please contact me at aron.g@me.com