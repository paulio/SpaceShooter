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

    protected override void Fire(bool isDownwardsMissile)
    {
        // don't fire, we only fire once we're at a waypoint
    }

    protected override void SetNextWaypoint()
    {
        if (_fireWaypoint == -1)
        {
            StartBeam();
        }

        if (CurrentWaypoint == _fireWaypoint)
        {
            var laserObject = Instantiate(LaserPrefab, transform.position + (Vector3.up * -1f), Quaternion.identity);
            StartCoroutine(FireBeamRoutine());
        }
        else
        {
            base.SetNextWaypoint();
        }
    }

    protected override void SetAsDestroyed()
    {
        base.SetAsDestroyed();
        _laserBeam.SetAsDestroyed();
    }

    private IEnumerator FireBeamRoutine()
    {
        _isFiring = true;
        yield return new WaitForSeconds(1.0f);
        _laserBeam.enabled = true;
        yield return new WaitForSeconds(3.0f);
        _isFiring = false;
        base.SetNextWaypoint();
    }
}
