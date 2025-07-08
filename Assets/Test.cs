using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    ModelManager modelManager;
    void Start()
    {
        modelManager = FindObjectOfType<ModelManager>();
        modelManager.LoadModel("GLTFs/cone/traffic_cone.gltf", "cone", () =>
        {
            Debug.Log("Model loaded successfully!");
            modelManager.DisplayModel("cone");
        });
    }

}
