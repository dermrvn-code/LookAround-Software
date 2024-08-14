public class SceneElement
{
    public enum ElementType { Text, DirectionArrow, Textbox };
    public ElementType type;
    public string text;
    public int x;
    public int y;
    public int distance;
    public int xRotationOffset;
    public string action;
    public string icon;
    public int rotation;

    public SceneElement(ElementType type, string text, int x, int y, int distance, int xRotationOffset, string action = null, string icon = null, int rotation = 0)
    {
        this.type = type;
        this.text = text;
        this.x = x;
        this.y = y;
        this.distance = distance;
        this.xRotationOffset = xRotationOffset;
        this.action = action;
        this.icon = icon;
        this.rotation = rotation;
    }

    public override string ToString()
    {
        if (type == ElementType.Textbox)
        {
            return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + ", a distance of " + distance + ", and icon '" + icon + "'";
        }else if (type == ElementType.DirectionArrow)
        {
            return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + ", a distance of " + distance + ", and rotation of '" + rotation + "'";
        }
        return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + ", a distance of " + distance + ", action '" + action + "'";

    }
}
