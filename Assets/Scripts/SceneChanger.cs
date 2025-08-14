using UnityEngine;

public class SceneChanger : SceneChangerBase
{

    public override void Awake()
    {
        base.Awake();
    }

    public void LoadWorld()
    {
        SceneManager.worldsList.TryGetValue(SceneManager.currentWorld, out string path);
        if (path == "")
        {
            Debug.LogWarning("The current world is not in the worlds list");
            return;
        }
        Debug.Log(sceneManager);
        sceneManager.LoadScenesOverview(path, () =>
        {
            Debug.Log("Loading World: " + path);
            ToStartScene();
        });
    }
}
