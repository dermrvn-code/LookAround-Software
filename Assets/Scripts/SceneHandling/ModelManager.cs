using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using System;
using UnityEngine.Events;


public class ModelManager : MonoBehaviour
{
    [SerializeField]
    int maxModels = 8;

    [SerializeField]
    Dictionary<string, GameObject> loadedModels = new Dictionary<string, GameObject>();

    [SerializeField]
    GameObject sceneElementsContainer;

    [SerializeField]
    GameObject siding;


    void Start()
    {
        if (sceneElementsContainer == null)
        {
            Debug.LogWarning("sceneElementsContainer or dome is not assigned.");
            return;
        }
    }

    public GameObject DisplayModel(string modelName)
    {
        if (loadedModels.TryGetValue(modelName, out GameObject model))
        {
            model.transform.SetParent(sceneElementsContainer.transform, false);
            model.SetActive(true);
            return model;
        }

        Debug.LogWarning("Model not found in loaded models: " + modelName);
        return null;
    }

    public void HideModel(string modelName)
    {
        if (loadedModels.TryGetValue(modelName, out GameObject model))
        {
            model.transform.SetParent(siding.transform, false);
            model.SetActive(false);
            return;
        }

        Debug.LogWarning("Model not found in loaded models: " + modelName);
    }

    public void HideAllModels()
    {
        foreach (var model in loadedModels.Values)
        {
            if (model != null)
            {
                model.transform.SetParent(siding.transform, false);
                model.SetActive(false);
            }
        }
    }

    public void LoadModel(string filepath, string modelName, UnityAction onLoaded = null)
    {
        Importer.ImportGLTFAsync(filepath, new ImportSettings(), (GameObject result, AnimationClip[] clips) =>
        {
            IntegrateModel(modelName, result);
            onLoaded?.Invoke();
            Debug.Log($"Model {modelName} loaded from {filepath}");
        });
    }

    public void UnloadModel(string modelName)
    {
        if (loadedModels.ContainsKey(modelName))
        {
            loadedModels.TryGetValue(modelName, out GameObject model);
            loadedModels.Remove(modelName);
            if (model != null)
            {
                Destroy(model);
            }
            Debug.Log($"Model {modelName} unloaded.");
            return;
        }
        Debug.LogWarning("Model not found in loaded models: " + modelName);
    }

    void IntegrateModel(string modelName, GameObject result)
    {
        if (loadedModels.Count >= maxModels)
        {
            Debug.LogWarning("Maximum number of models loaded. Cannot load more.");
            return;
        }

        if (loadedModels.ContainsKey(modelName))
        {
            Debug.LogWarning("Model already loaded: " + result.name);
            return;
        }
        loadedModels.Add(modelName, result);
        result.transform.SetParent(siding.transform, false);
        result.AddComponent<DomePosition3DObject>();
        result.SetActive(false);
    }

    void UnloadAllModels()
    {
        foreach (var model in loadedModels.Values)
        {
            if (model != null)
            {
                Destroy(model);
            }
        }
        loadedModels.Clear();
        Debug.Log("All models unloaded.");
    }

    void OnDestroy()
    {
        UnloadAllModels();
    }

}
