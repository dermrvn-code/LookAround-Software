public class SceneElement
{
    public enum ElementType { Text, Image, Textbox };
    public ElementType type;
    public string text;
    public int x;
    public int y;
    public string action;
    public string icon;

    public SceneElement(ElementType type, string text, int x, int y, string action = null, string icon = null)
    {
        this.type = type;
        this.text = text;
        this.x = x;
        this.y = y;
        this.action = action;
        this.icon = icon;
    }

    public override string ToString()
    {
        if (type == ElementType.Textbox)
        {
            return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + " and icon '" + icon + "'";
        }
        return type.ToString() + " with value " + text + " at x:" + x + " y:" + y + " and action '" + action + "'";

    }
}
