using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{

    RaycastHit hit;
    Interactable target;
    EyesHandler eyesHandler;

    Dictionary<DomePosition, Interactable> elements = new Dictionary<DomePosition, Interactable>();

    [SerializeField]
    LayerMask layer;

    public bool updateElementsNextFrame = true;

    void Start()
    {
        eyesHandler = GetComponent<EyesHandler>();
    }

    void UpdateElements()
    {
        elements = new Dictionary<DomePosition, Interactable>();
        target = null;
        var domeElements = FindObjectsOfType<DomePosition>();
        foreach (var domeElement in domeElements)
        {
            Interactable elementToAdd;
            if (domeElement.GetComponent<Interactable>() != null)
            {
                elementToAdd = domeElement.GetComponent<Interactable>();
            }
            else if (domeElement.GetComponentInChildren<Interactable>() != null)
            {
                elementToAdd = domeElement.GetComponentInChildren<Interactable>();
            }
            else
            {
                return;
            }
            elements.Add(domeElement, elementToAdd);
        }
        updateElementsNextFrame = false;
    }

    public void Interact()
    {
        target?.Interact();
    }


    // Update is called once per frame
    float oldRotation = -20;
    bool checkedElements = false;
    int offset = 10;
    void Update()
    {
        if (updateElementsNextFrame) UpdateElements();

        if (eyesHandler.rotation != oldRotation)
        {
            oldRotation = eyesHandler.rotation;
            target?.Unhighlight();
            target = null;
            checkedElements = false;
        }
        else if (!checkedElements)
        {
            checkedElements = true;
            if (elements.Count > 0)
            {
                foreach (var element in elements)
                {
                    var leftOffset = (oldRotation - offset) % 360;
                    var rightOffset = (oldRotation + offset) % 360;
                    if (leftOffset > rightOffset)
                    {
                        leftOffset = leftOffset - 360;
                    }
                    if (leftOffset < element.Key.position.x && element.Key.position.x < (oldRotation + offset) % 360)
                    {
                        target = element.Value;
                    }
                }
                target?.Highlight();
            }
        }
    }
}
