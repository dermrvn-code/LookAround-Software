using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SceneChangerExtend : SceneChanger
{
    public void LoadWorld()
    {
        SceneManagerExtend.worldsList.TryGetValue(SceneManagerExtend.currentWorld, out string path);
        if (path == "")
        {
            Debug.LogWarning("The current world is not in the worlds list");
            return;
        }
        sceneManager.LoadScenesOverview(path, () =>
        {
            Debug.Log("Loading World: " + path);
            ToStartScene();
        });
    }
}
