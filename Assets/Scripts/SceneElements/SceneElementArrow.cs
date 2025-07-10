public class SceneElementArrow : SceneElement
{
    public string icon;
    public int rotation;
    public string color;
    public string action;

    public SceneElementArrow(
        int x = 0, int y = 0,
        int distance = 0, int xRotationOffset = 0,
        string icon = null, int rotation = 0,
        string color = null, string action = null)
        : base(x, y, distance, xRotationOffset)
    {
        this.icon = icon;
        this.rotation = rotation;
        this.color = color;
        this.action = action;
    }

    public override string ToString()
    {
        return $"DirectionArrow with color {color} at x:{x} y:{y}, a distance of {distance}, and rotation of '{rotation}' and action '{action}'";
    }
}