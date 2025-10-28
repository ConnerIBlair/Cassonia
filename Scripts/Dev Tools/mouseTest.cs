using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mouseTest : MonoBehaviour
{
    public Camera testcamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = testcamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Debug.Log("CLICKED " + hit.collider.name);
                Debug.Break();
            }
        }
    }
}
