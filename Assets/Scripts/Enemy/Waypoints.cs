using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField]
    private Transform[] _waypoints;

    public Transform[] GetWaypoints => _waypoints;
}
