using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressBarExtend : ProgressBar
{
    [SerializeField]
    Transform progress;
    [SerializeField]
    MeshRenderer meshRenderer;


    public override void _Update()
    {
        progress.gameObject.SetActive(show);
        progressText.gameObject.SetActive(show);
        meshRenderer.enabled = show;

        if (show)
        {
            if (currentProgressValue != progressValue)
            {
                currentProgressValue = Mathf.Lerp(currentProgressValue, progressValue, progressSpeed * Time.deltaTime);
            }
            ScaleProgress(currentProgressValue);
        }
    }

    void ScaleProgress(float value)
    {
        Vector3 pos = progress.localPosition;
        Vector3 scale = progress.transform.localScale;

        scale.y = value;
        pos.y = scale.y - 1;

        progress.localPosition = pos;
        progress.transform.localScale = scale;
    }
}
