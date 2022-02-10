using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTest : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        dir(Random.insideUnitCircle*20, Random.insideUnitCircle*50, Random.insideUnitCircle*20, Random.Range(10, 20), out Missile m);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 dir(Vector2 missilePos, Vector2 missileVelo, Vector2 targetPos, float a, out Missile m)
    {
        target.transform.position = targetPos;
        float threshold = .2f;
        float di = (targetPos - missilePos).magnitude;
        Ray2D r = new Ray2D(targetPos, -missileVelo);
        float t = Mathf.Sqrt(2 * di / a);
        Vector2 missilePoint, targetPoint;
        targetPoint = r.GetPoint(t * missileVelo.magnitude);
        missilePoint = missilePos + (targetPoint - missilePos).normalized * .5f * a * t * t;
        int count = 0;
        while ((missilePoint - targetPoint).magnitude > threshold)
        {
            if (count++ > 1000)
            {
                break;
            }
            float dt = (missilePoint - targetPoint).magnitude / (a * t);
            if ((missilePoint + (missilePoint - missilePos).normalized * .1f - targetPoint).magnitude < (missilePoint - targetPoint).magnitude)
            {
                t += dt;
            }
            else
            {
                t -= dt;
            }
            targetPoint = r.GetPoint(t * missileVelo.magnitude);
            missilePoint = missilePos + (targetPoint - missilePos).normalized * .5f * a * t * t;
        }
        print(targetPoint);
        print(missilePoint);
        print(count);
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Quad);
        g.AddComponent<Missile>();
        g.transform.position = missilePos;
        m = g.GetComponent<Missile>();
        m.velocity = missileVelo;
        m.acceleration = a * (targetPoint - missilePos).normalized;
        return (targetPoint - missilePos).normalized;
    }
    public class Missile : MonoBehaviour
    {
        public Vector2 velocity;
        public Vector2 acceleration;
        private void Update()
        {
            velocity += acceleration * Time.deltaTime;
            transform.position += Time.deltaTime * (Vector3)velocity;
        }
    }
}
