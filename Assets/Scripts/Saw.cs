using UnityEngine;

public class Saw : MonoBehaviour
{
    public float speed;

    void Update()
    {
        float angle = speed * Time.deltaTime;
        transform.Rotate(Vector3.forward, angle);
    }
}
