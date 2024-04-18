using System.Collections.Generic;

public class Scene
{
    public enum MediaType { Video, Photo };
    public MediaType Type
    {
        get => _type;
    }
    public string Name
    {
        get => _name;
    }
    public string Source
    {
        get => _source;
    }
    public List<SceneElement> SceneElements
    {
        get => _sceneElements;
    }
    public bool IsStartScene
    {
        get => _isStartScene;
    }

    MediaType _type;
    string _name;
    string _source;
    List<SceneElement> _sceneElements;
    bool _isStartScene;

    public Scene(MediaType type, string name, string source, List<SceneElement> sceneElements, bool isStartScene)
    {
        _type = type;
        _source = source;
        _sceneElements = sceneElements;
        _isStartScene = isStartScene;
    }

    public override string ToString()
    {
        return "Scene '" + Name + "' of type '" + Type.ToString() + "' and source '" + Source + "'";
    }
}
