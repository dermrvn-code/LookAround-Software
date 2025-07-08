using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DomePosition3DObject : DomePosition
{
    protected float scaleFactor = 1f;
    public float scale = 1f;

    float realismFactor = 0.3f;

    protected override void Start()
    {
        base.Start();

        GameObject dome = GameObject.Find("Dome");

        Renderer resultRenderer = GetComponentInChildren<Renderer>();
        Renderer domeRenderer = dome.GetComponent<Renderer>();

        if (resultRenderer == null)
        {
            Debug.LogWarning("No Renderer found on result to calculate size.");
            return;
        }

        if (domeRenderer == null)
        {
            Debug.LogWarning("No Renderer found on dome to calculate size.");
            return;
        }

        Vector3 resultSize = resultRenderer.bounds.size;
        Vector3 domeSize = domeRenderer.bounds.size;

        Debug.Log($"Result size (world units): {resultSize}");
        Debug.Log($"Dome size (world units): {domeSize}");

        float resultMax = Mathf.Max(resultSize.x, resultSize.y, resultSize.z);

        if (resultMax == 0)
        {
            Debug.LogWarning("Result size is zero, cannot scale.");
            return;
        }

        if (resultMax == resultSize.x)
        {
            scaleFactor = domeSize.x / resultMax;
        }
        else if (resultMax == resultSize.y)
        {
            scaleFactor = domeSize.y / resultMax;
        }
        else
        {
            scaleFactor = domeSize.z / resultMax;
        }
        scaleFactor = scaleFactor * realismFactor; // realistic scaling
    }


    protected override void Update()
    {
        base.Update();
        if (scale <= 0)
        {
            return;
        }
        elementTransform.localScale = Vector3.one * scaleFactor * scale;
    }

}
