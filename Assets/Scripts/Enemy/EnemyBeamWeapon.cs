using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeamWeapon : EnemyWithWaypoints
{
    private int _fireWaypoint = -1;
    private LaserBeam _laserBeam;
    private bool _isFiring;

    // Start is called before the first frame update
    private void StartBeam()
    {
        _fireWaypoint = UnityEngine.Random.Range(0, Waypoints.GetWaypoints.Length-1);
        _laserBeam =  LaserPrefab.GetComponent<LaserBeam>();
        print($"EnemyBeamWeapon started, fireWaypoint {_fireWaypoint}");
    }

    protected override void Move(bool isAlive)
    {
     
        if (isAlive && !_isFiring)
        {
            base.Move(isAlive);
        }
        else
        {
            this.transform.Translate(Vector3.zero);
        }
    }

    protected override void Fire()
    {
        // don't fire, we only fire once we're at a waypoint
    }

    protected override void SetNextWaypoint()
    {
        print("EnemyBeamWeapon SetNextWaypoint");
        if (_fireWaypoint == -1)
        {
            StartBeam();
        }

        print($"EnemyBeamWeapon CurrentWaypoint {CurrentWaypoint} = _fireWaypoint {_fireWaypoint}");
        if (CurrentWaypoint == _fireWaypoint)
        {
            print("at Firing waypoint, pause and beam laser");
            var laserObject = Instantiate(LaserPrefab, transform.position + (Vector3.up * -1f), Quaternion.identity);
            print(laserObject.name);
            StartCoroutine(FireBeamRoutine());
        }
        else
        {
            base.SetNextWaypoint();
        }
    }

    private IEnumerator FireBeamRoutine()
    {
        print("EnemyBeamWeapon FireBeam");
        _isFiring = true;
        yield return new WaitForSeconds(1.0f);
        _laserBeam.enabled = true;
        yield return new WaitForSeconds(3.0f);
        _isFiring = false;
        base.SetNextWaypoint();
    }
}
