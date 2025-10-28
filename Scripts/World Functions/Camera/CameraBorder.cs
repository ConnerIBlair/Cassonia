using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBorder : MonoBehaviour
{
    // Check if the positions in the list are matching correctly and if they are, then the logic in checking if they match in CameraMovement.FindPosition

    public BoxCollider2D boxCollider;
    public CameraMovement newcamera;

    private Vector2 borderMin;
    private Vector2 borderMax;

    [SerializeField]
    private bool NewBorders;

    [SerializeField]
    private int BorderToCheck;
    [SerializeField]
    private bool CheckBorder;
    private void NewBorder()
    {
        Debug.Log(borderMin);
        Debug.Log(borderMax);

        newcamera.maxPosition = borderMax;
        newcamera.minPosition = borderMin;
        PositionTuple tuple = new()
        {
            Item1 = borderMin,
            Item2 = borderMax
        };
        Debug.Log(tuple.Item1 + " " + tuple.Item2);
        newcamera.Add(tuple);
    }



    void OnValidate()
    {
        if (NewBorders)
            GenerateBorders();
            NewBorders = false;
        if (CheckBorder)
        {
            boxCollider.size = new Vector2(newcamera.MinMaxBorders[BorderToCheck].Item2.x - newcamera.MinMaxBorders[BorderToCheck].Item1.x,
                newcamera.MinMaxBorders[BorderToCheck].Item2.y - newcamera.MinMaxBorders[BorderToCheck].Item1.y);
            transform.position = newcamera.MinMaxBorders[BorderToCheck].Item1 + new Vector2(newcamera.MinMaxBorders[BorderToCheck].Item2.x - newcamera.MinMaxBorders[BorderToCheck].Item1.x,
                newcamera.MinMaxBorders[BorderToCheck].Item2.y - newcamera.MinMaxBorders[BorderToCheck].Item1.y) / 2;

            CheckBorder = false;
        }
    }

    void GenerateBorders()
    {
        borderMax = new Vector2(transform.localToWorldMatrix.GetPosition().x + boxCollider.size.x / 2, transform.localToWorldMatrix.GetPosition().y + boxCollider.size.y / 2);
        borderMin = new Vector2(borderMax.x - boxCollider.size.x, borderMax.y - boxCollider.size.y);
        NewBorder();
    }
}
