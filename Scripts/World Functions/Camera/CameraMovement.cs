using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Animations;
// Camera stacking to make canvas follow camera size possibly... Use overlay camera type

[Serializable] public class PositionTuple { public Vector2 Item1, Item2; }
public class CameraMovement : MonoBehaviour
{
    public ParentConstraint parent;
    public Transform target;
    public ColorSwatches colours;
    public float smoothing;

    public Vector2 minPosition, maxPosition;

    public bool transitioning;

    //[HideInInspector]
    public bool stationary = false;
    private PlayerMovement player;

    [SerializeField]
    private List<PositionTuple> _borders = new List<PositionTuple>();

    public List<PositionTuple> MinMaxBorders => _borders;

    private static CameraMovement instance = null;

    public static CameraMovement Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        player = FindFirstObjectByType<PlayerMovement>();
        target = player.transform;
        minPosition = MinMaxBorders[0].Item1;
        maxPosition = MinMaxBorders[0].Item2;
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            if (player = FindFirstObjectByType<PlayerMovement>())
            {
                target = player.transform;
            }
            return;
        }

        if (stationary && smoothing != 1)
        {
            parent.locked = false;
            parent.translationAxis = Axis.Z;
        }
        if (stationary) { transform.position = new Vector3(maxPosition.x, maxPosition.y, -10.0625f); smoothing = 1; return; }
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (target.position.x <= minPosition.x)
        {
            ChangePositions("Left");
        }
        if (target.position.x >= maxPosition.x)
        {
            ChangePositions("Right");
        }
        if (target.position.y >= maxPosition.y)
        {
            ChangePositions("Up");
        }
        if (target.position.y <= minPosition.y) // Player is at or past the border
        {
            ChangePositions("Down");
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x + 12.5f, maxPosition.x - 12.5f);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y + 7, maxPosition.y - 7);
        if (!transitioning)
        {
            if (target.position.x > maxPosition.x - 12.5f || target.position.x < minPosition.x + 12.5f)
            {
                parent.locked = false;
                if (target.position.y > maxPosition.y - 7 || target.position.y < minPosition.y + 7)
                {
                    parent.translationAxis = Axis.Z;
                    return;
                }
                parent.translationAxis = Axis.Y | Axis.Z;
            }
            else
            {
                parent.locked = true;// true
                parent.translationAxis = Axis.X | Axis.Z;
            }

            if (target.position.y > maxPosition.y - 7 || target.position.y < minPosition.y + 7)
            {
                parent.locked = false;
                parent.translationAxis = Axis.X | Axis.Z;
            }
            else
            {
                parent.locked = false; // true
                parent.translationAxis = Axis.Y | Axis.Z;
            }

            if (target.position.x < maxPosition.x - 12.5f && target.position.x > minPosition.x + 12.5f && target.position.y < maxPosition.y - 7 && target.position.y > minPosition.y + 7)
            {
                parent.translationAxis = Axis.X | Axis.Y | Axis.Z;
            }
        }
        else
        {

            if (Mathf.Abs(transform.position.x - targetPosition.x) < .05f && Mathf.Abs(transform.position.y - targetPosition.y) < .05f)
            {
                transitioning = false;
                smoothing = 1;
            }
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }

    public void Add(PositionTuple item)
    {
        _borders.Add(item);
    }
    public void ChangePositions(string direction)
    {
        //if (transitioning) return;
        //player.paused = true;
        transitioning = true;
        smoothing = .05f;

        //StartCoroutine(PausePlayerCo());
        switch (direction)
        {
            case "Left":
                for (int i = 0; i < MinMaxBorders.Count; i++)
                {
                    if (minPosition.x == MinMaxBorders[i].Item2.x)
                    {
                        if (MinMaxBorders[i].Item1.y == minPosition.y || MinMaxBorders[i].Item2.y == maxPosition.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }else if (minPosition.y > MinMaxBorders[i].Item1.y && minPosition.y < MinMaxBorders[i].Item2.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (maxPosition.y > MinMaxBorders[i].Item1.y && maxPosition.y < MinMaxBorders[i].Item2.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                    }
                }
                break;
            case "Right":
                for (int i = 0; i < MinMaxBorders.Count; i++)
                {
                    if (maxPosition.x == MinMaxBorders[i].Item1.x)
                    {
                        if (MinMaxBorders[i].Item1.y == minPosition.y || MinMaxBorders[i].Item2.y == maxPosition.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (minPosition.y > MinMaxBorders[i].Item1.y && minPosition.y < MinMaxBorders[i].Item2.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (maxPosition.y > MinMaxBorders[i].Item1.y && maxPosition.y < MinMaxBorders[i].Item2.y)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                    }
                }
                break;
            case "Up":
                for (int i = 0; i < MinMaxBorders.Count; i++)
                {
                    if (maxPosition.y == MinMaxBorders[i].Item1.y)
                    {
                        if (MinMaxBorders[i].Item1.x == minPosition.x || MinMaxBorders[i].Item2.x == maxPosition.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (minPosition.x > MinMaxBorders[i].Item1.x && minPosition.x < MinMaxBorders[i].Item2.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (maxPosition.x > MinMaxBorders[i].Item1.x && maxPosition.x < MinMaxBorders[i].Item2.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                    }
                }
                break;
            case "Down":
                for (int i = 0; i < MinMaxBorders.Count; i++)
                {
                    if (minPosition.y == MinMaxBorders[i].Item2.y)
                    {
                        if (MinMaxBorders[i].Item1.x == minPosition.x || MinMaxBorders[i].Item2.x == maxPosition.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (minPosition.x > MinMaxBorders[i].Item1.x && minPosition.x < MinMaxBorders[i].Item2.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                        else if (maxPosition.x > MinMaxBorders[i].Item1.x && maxPosition.x < MinMaxBorders[i].Item2.x)
                        {
                            minPosition = MinMaxBorders[i].Item1;
                            maxPosition = MinMaxBorders[i].Item2;
                            break;
                        }
                    }
                }
                break;
        }
    }

    private IEnumerator PausePlayerCo()
    {
        target.GetComponent<PlayerMovement>().paused = true;
        yield return new WaitUntil(() => transitioning == false);
        target.GetComponent<PlayerMovement>().paused = false;
    }
}
