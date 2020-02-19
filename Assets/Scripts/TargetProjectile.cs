using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetProjectile : MonoBehaviour
{
    public GameObject target;
    public float birthTime;
    public float v0;
    public float lifetime;
    private float _arrivalTime;
    private float _killDelay = 1;

    public void Update()
    {
        if ((Time.time - birthTime) > lifetime)
        {
            Destroy(this.gameObject);
        }
        /*
        if (Vector3.Distance(target.transform.localPosition, transform.localPosition) < 10)
        {
            Destroy(this.gameObject);
        }
        */
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(target.transform.localPosition, transform.localPosition) < 1)
        {
            transform.localPosition = target.transform.localPosition;
        }
        float v = v0 + v0/10*(Time.time - birthTime);
        Vector3 pos = transform.localPosition;
        transform.localPosition = pos + Vector3.Normalize(target.transform.localPosition - pos) * v * Time.deltaTime;
    }
}
