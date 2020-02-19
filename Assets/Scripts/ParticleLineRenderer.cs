using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLineRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _target;

    private float _lastLaunchTime = 0;
    public float emitRate = 1;
    public float lifetime = 5;
    public float startSpeed = 10;
    public bool followCursor = true;
    private void Awake()
    {
    }

    private void Update()
    {
        if (followCursor) { _target.position = Input.mousePosition; }

        if ((Time.time - _lastLaunchTime) > emitRate)
        {
            GameObject projectile = Instantiate(_projectile, transform.position, Quaternion.identity, this.transform) as GameObject;
            TargetProjectile tp = projectile.GetComponent<TargetProjectile>();
            tp.birthTime = Time.time;
            tp.lifetime = lifetime;
            tp.target = _target.gameObject;
            tp.v0 = startSpeed;
            _lastLaunchTime = Time.time;
        }
    }

    public void SetTarget(Vector3 position)
    {
        followCursor = false;
        _target.transform.position = position;
    }
}
