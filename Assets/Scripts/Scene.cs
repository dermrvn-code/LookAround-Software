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
    public bool IsMainScene
    {
        get => _isMainScene;
    }

    MediaType _type;
    string _name;
    string _source;
    List<SceneElement> _sceneElements;
    bool _isMainScene;

    public Scene(MediaType type, string name, string source, List<SceneElement> sceneElements)
    {
        this._type = type;
        this._name = name;
        this._source = source;
        this._sceneElements = sceneElements;
    }

    public override string ToString()
    {
        return "Scene '" + Name + "' of type '" + Type.ToString() + "' and source '" + Source + "'";
    }
}
