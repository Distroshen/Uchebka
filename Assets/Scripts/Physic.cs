using UnityEngine;

public class PhysicsOptimization : MonoBehaviour
{
    public GameObject[] physicalObjects;

    private void Update()
    {
        foreach (GameObject obj in physicalObjects)
        {
            if (IsVisible(obj))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }

    private bool IsVisible(GameObject obj)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, obj.GetComponent<Collider>().bounds);
    }
}