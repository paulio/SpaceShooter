using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField]
    private Transform[] _waypoints;

    public Transform[] GetWaypoints => _waypoints;
}
