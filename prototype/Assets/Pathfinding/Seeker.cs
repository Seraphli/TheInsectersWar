using UnityEngine;
using System.Collections;
using AstarClasses;

[AddComponentMenu("Pathfinding/Seeker")]
public class Seeker : MonoBehaviour
{
    //The path the script is currently proccessing
    private AstarPath.Path path;

    //Should the path get debugged in the scene view during runtime
    public bool debugPath = false;

    //This is the max slope the unit can travel on
    public float maxAngle = 20;

    //The cost of moving in a slope, use a higher cost to make it avoid slopes
    public float angleCost = 2F;

    //If true, removes the first node on the path, reduces jittering
    public bool removeFirst = true;

    //Makes the path get calculated when searching at most one node per frame, good for debugging.
    public bool stepByStep = false;

    //The AstarScript will clamp the end point to the nearest node, should the seeker script replace the last node with the end point specified when calling the StartPath function
    public RealStart startPoint = RealStart.Exact;
    public RealEnd endPoint = RealEnd.Exact;

    //What should the script do when the script couldn't find a path to the target
    public OnError onError = OnError.ErrorMessage;

    //Should we only search on one of the grids or should it use all grids?
    public GridSelection gridSelection = GridSelection.Auto;

    //If Grid Selection is set to Fixed, this will indicate which grid will be used
    public int grid = 0;

    public enum GridSelection
    {
        Auto,
        Fixed
    }

    public enum OnError
    {
        None,
        ErrorMessage,
        EmptyArray
    }

    public enum RealStart
    {
        Snapped,
        Exact
    }

    public enum RealEnd
    {
        Snapped,
        Exact,
        AddExact
    }

    //The end/start point which were used when the StartPath function was called
    private Vector3 endpos;
    private Vector3 startpos;

    //The last path
    private Vector3[] pathPoints;

    //This function will be called when the pathfinding is complete, it will be called when the pathfinding returned an error too.
    public void OnComplete(AstarPath.Path p)
    {

        //If the script was already calculating a path when a new path started calculating, then it would have canceled the first path, that will generate an OnComplete message from the first path, this makes sure we only get OnComplete messages from the path we started calculating latest.
        if (path != p)
        {
            return;
        }

        //What should we do if the path returned an error (there is no available path to the target).
        if (path.error)
        {
            switch (onError)
            {
                case OnError.None:
                    return;
                case OnError.ErrorMessage:
                    SendMessage("PathError", new Vector3[0], SendMessageOptions.DontRequireReceiver);
                    break;
                case OnError.EmptyArray:
                    SendMessage("PathComplete", new Vector3[0], SendMessageOptions.DontRequireReceiver);
                    break;
            }
            return;
        }

        if (path.path == null)
        {
            Debug.LogError("The 'Path' array is not assigned - System Error - Please send a bug report - Include the following info:\nError = " + p.error + "\nStart = " + startpos + "\nEnd = " + endpos + "\nFound End = " + p.foundEnd + "\nError = " + p.error);
        }

        if (path.path.Length > 1)
        {
            //Convert the Node array to a Vector3 array, subract one from the array if Remove First is true and add one to the array if Use Real End is set to Add
            Vector3[] a = new Vector3[path.path.Length - (removeFirst ? 1 : 0) + (endPoint == RealEnd.AddExact ? 1 : 0)];

            for (int i = 0; i < path.path.Length; i++)
            {
                //Ignore the first node if Remove First is set to True
                if (removeFirst && i == 0)
                {
                    continue;
                }

                a[i - (removeFirst ? 1 : 0)] = path.path[i].vectorPos;
            }

            if (startPoint == RealStart.Exact)
            {
                a[0] = startpos;
            }

            //Assign the endpoint
            if (endPoint == RealEnd.AddExact || endPoint == RealEnd.Exact)
            {
                a[a.Length - 1] = endpos;
            }

            //Store the path in a variable so it can be drawn in the scene view for debugging
            pathPoints = a;

            //Send the Vector3 array to a movement script attached to this gameObject
            SendMessage("PathComplete", a, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Vector3[] a2 = new Vector3[1] { (endPoint == RealEnd.AddExact || endPoint == RealEnd.Exact ? endpos : startpos) };
            pathPoints = a2;
            SendMessage("PathComplete", a2, SendMessageOptions.DontRequireReceiver);
        }

    }

    public void OnDrawGizmos()
    {
        if (debugPath && pathPoints != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
        }

        /*if (path != null && debugPath && path.path != null) {
			
            Gizmos.color = new Color (1,0,0,0.5F);
			
            for (int i=0;i<path.path.Length-1;i++) {
                Node node = path.path[i] as Node;
                Node node2 = path.path[i+1] as Node;
                Gizmos.DrawLine (node.vectorPos,node2.vectorPos);
            }
        }*/
    }

    //Call this function to start calculating a path
    public AstarPath.Path StartPath(Vector3 start, Vector3 end)
    {

        //Cancel the previous path
        if (path != null)
        {
            path.error = true;
            //StopCoroutine ("PathUpdate");
        }
        endpos = end;
        startpos = start;
        //SendMessage("Stop",SendMessageOptions.DontRequireReceiver);//This will make the player stop

        //Shall we search all grids or only the one specified in the "grid" variable
        if (gridSelection == GridSelection.Auto)
        {
            path = new AstarPath.Path(start, end, maxAngle, angleCost, stepByStep);//Create a new Path instance
        }
        else
        {
            path = new AstarPath.Path(start, end, maxAngle, angleCost, stepByStep, grid);//Create a new Path instance
        }

        StartCoroutine(AstarPath.StartPathYield(path, this));//Start a coroutine (function including yields) to calculate the path
        return path;
    }
}
