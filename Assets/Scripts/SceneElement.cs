public class SceneElement
{
    public enum ElementType { Text, Image };
    public ElementType type;
    public string text;
    public int x;
    public int y;
    public string action;

    public SceneElement(ElementType type, string text, int x, int y, string action)
    {
        this.type = type;
        this.text = text;
        this.x = x;
        this.y = y;
        this.action = action;
    }

    public override string ToString()
    {
        return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + " and action '" + action + "'";
    }
}
