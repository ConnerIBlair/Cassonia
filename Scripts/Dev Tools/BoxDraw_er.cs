using UnityEngine;

public class BoxDraw_er : MonoBehaviour
{
    // BoxCast parameters
    public Vector3 boxSize = new Vector2(1f, 1f);
    public float boxAngle = 0f; // Rotation in degrees
    public Vector3 boxDirection = Vector2.down;
    public float boxDistance = 5f;
    public LayerMask hitLayers;
    public Vector3 origin = Vector2.zero;

    // Gizmo drawing parameters
    public Color castColor = Color.green;
    public Color hitColor = Color.red;

    void OnDrawGizmosSelected()
    {

        // Perform the BoxCast
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, boxAngle, boxDirection, boxDistance, hitLayers);

        // Determine the color based on whether a hit occurred
        Color currentColor = hit ? hitColor : castColor;
        Gizmos.color = currentColor;

        // Calculate the corners of the initial box
        Vector3 halfSize = boxSize / 2f;
        Vector3 p1 = new Vector2(-halfSize.x, halfSize.y);
        Vector3 p2 = new Vector2(halfSize.x, halfSize.y);
        Vector3 p3 = new Vector2(halfSize.x, -halfSize.y);
        Vector3 p4 = new Vector2(-halfSize.x, -halfSize.y);

        // Rotate the corners
        Quaternion rotation = Quaternion.Euler(0, 0, boxAngle);
        p1 = rotation * p1;
        p2 = rotation * p2;
        p3 = rotation * p3;
        p4 = rotation * p4;

        // Offset the corners by the origin
        p1 += origin;
        p2 += origin;
        p3 += origin;
        p4 += origin;

        // Draw the initial box
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // Calculate the end position of the box cast
        Vector3 endPosition = origin + (boxDirection.normalized * boxDistance);

        // Calculate the corners of the final box (at max distance or hit point)
        Vector3 endP1, endP2, endP3, endP4;
        if (hit)
        {
            // If a hit occurred, draw the end box at the hit point
            Vector3 hitPoint = hit.centroid; // Or hit.point depending on your needs
            endP1 = rotation * new Vector2(-halfSize.x, halfSize.y) + hitPoint;
            endP2 = rotation * new Vector2(halfSize.x, halfSize.y) + hitPoint;
            endP3 = rotation * new Vector2(halfSize.x, -halfSize.y) + hitPoint;
            endP4 = rotation * new Vector2(-halfSize.x, -halfSize.y) + hitPoint;
        }
        else
        {
            // If no hit, draw the end box at the maximum distance
            endP1 = rotation * new Vector2(-halfSize.x, halfSize.y) + endPosition;
            endP2 = rotation * new Vector2(halfSize.x, halfSize.y) + endPosition;
            endP3 = rotation * new Vector2(halfSize.x, -halfSize.y) + endPosition;
            endP4 = rotation * new Vector2(-halfSize.x, -halfSize.y) + endPosition;
        }

        // Draw the end box
        Gizmos.DrawLine(endP1, endP2);
        Gizmos.DrawLine(endP2, endP3);
        Gizmos.DrawLine(endP3, endP4);
        Gizmos.DrawLine(endP4, endP1);

        // Draw lines connecting the initial and final boxes
        Gizmos.DrawLine(p1, endP1);
        Gizmos.DrawLine(p2, endP2);
        Gizmos.DrawLine(p3, endP3);
        Gizmos.DrawLine(p4, endP4);
    }
}
