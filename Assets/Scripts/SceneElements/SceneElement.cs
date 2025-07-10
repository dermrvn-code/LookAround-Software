public abstract class SceneElement
{
    public int x;
    public int y;
    public int distance;
    public int xRotationOffset;

    protected SceneElement(int x = 0, int y = 0, int distance = 0, int xRotationOffset = 0)
    {
        this.x = x;
        this.y = y;
        this.distance = distance;
        this.xRotationOffset = xRotationOffset;
    }

    public abstract override string ToString();
}
