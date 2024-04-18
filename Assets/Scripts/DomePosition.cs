using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DomePosition : MonoBehaviour
{

    private Transform elementTransform;
    public Vector2 position;
    public float x;
    public float y;
    public int distance = 10;


    void Start()
    {
        elementTransform = GetComponent<Transform>();
    }

    Vector2 currentPosition;
    void Update()
    {
        if (currentPosition == position) return;
        currentPosition = position;
        Vector3 cartesianPosition = PolarToCartesian(currentPosition);
        SetRectPosition(cartesianPosition);
    }

    void SetRectPosition(Vector3 position)
    {
        elementTransform.localPosition = position;
        elementTransform.rotation = Quaternion.Euler(y, x, 1);
    }

    public Vector3 PolarToCartesian(Vector2 polar)
    {

        Vector3 origin = new Vector3(0, 0, distance);
        Quaternion rotation = Quaternion.Euler(polar.y, polar.x, 1);
        Vector3 point = rotation * origin;

        return point;
    }

}
