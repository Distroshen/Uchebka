using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    public Player speed;
    void Start()
    {
    }
    void FixedUpdate() 
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed.Speed, Space.World);
    }
}
