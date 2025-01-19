using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a path of waypoints in a 3D environment.
public class WaypointPath : MonoBehaviour
{
    // Returns the transform of the waypoint at the specified index.
    // Parameters:
    //   - waypointIndex: The index of the desired waypoint.
    public Transform GetWaypoint(int waypointIndex)
    {
        // Returns the transform of the child at the specified index.
        return transform.GetChild(waypointIndex);
    }

    // Returns the index of the next waypoint in the path.
    // Parameters:
    //   - currentWaypointIndex: The index of the current waypoint.
    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        // Calculate the index of the next waypoint.
        int nextWaypointIndex = currentWaypointIndex + 1;

        // If the next waypoint index is equal to the total number of waypoints,
        // wrap around to the first waypoint (looping behavior).
        if (nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = 0;
        }

        // Return the index of the next waypoint.
        return nextWaypointIndex;
    }
}
