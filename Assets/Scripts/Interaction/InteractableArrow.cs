
using TMPro;
using UnityEngine;

public class InteractableArrow : Interactable
{
    [SerializeField]
    GameObject arrow;

    // Start is called before the first frame update
    private Animation anim;
    private Material mat;

    [SerializeField]
    Color color;
    [SerializeField]
    Color hoverColor;

    public override void Setup()
    {
        anim = GetComponentInChildren<Animation>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        color = mat.color;
    }

    public void SetRotation(int rotation)
    {
        arrow.transform.Rotate(0, rotation, 0);
    }

    public override void Highlight()
    {
        anim.enabled = true;
        mat.color = hoverColor;
    }
    public override void Unhighlight()
    {
        anim.enabled = false;
        mat.color = color;
    }
}
