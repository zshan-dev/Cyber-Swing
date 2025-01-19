using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a moving platform that follows a predefined waypoint path.
public class MovingPlatform : MonoBehaviour
{
    // Reference to the WaypointPath script that defines the platform's movement path.
    [SerializeField]
    private WaypointPath _waypointPath;

    // Speed at which the platform moves between waypoints.
    [SerializeField]
    private float _speed;

    // Index of the current target waypoint in the waypoint path.
    private int _targetWaypointIndex;

    // Transforms representing the previous and current target waypoints.
    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    // Time it takes to reach the next waypoint based on the platform's speed.
    private float _timeToWaypoint;

    // Elapsed time since starting movement towards the current waypoint.
    private float _elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize and target the next waypoint when the platform is created.
        TargetNextWayPoint();
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the elapsed time based on the frame time.
        _elapsedTime += Time.deltaTime;

        // Calculate the percentage of time elapsed towards reaching the current waypoint.
        float elapsedPercentage = _elapsedTime / _timeToWaypoint;

        // Move the platform smoothly towards the target waypoint using Lerp for both position and rotation.
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);

        // Check if the platform has reached the current waypoint.
        if (elapsedPercentage >= 1)
        {
            // If reached, target the next waypoint.
            TargetNextWayPoint();
        }
    }

    // Targets the next waypoint in the path and sets up variables for movement.
    private void TargetNextWayPoint()
    {
        // Store the current waypoint as the previous one.
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        // Get the index of the next waypoint in the path.
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);

        // Get the transform of the new target waypoint.
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        // Reset the elapsed time for the new movement.
        _elapsedTime = 0;

        // Calculate the time it takes to reach the new waypoint based on the platform's speed.
        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }
}
