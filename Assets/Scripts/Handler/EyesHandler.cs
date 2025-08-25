using UnityEngine;

public class EyesHandler : ViewHandler
{
    [Header("Eye Cameras")]
    [SerializeField] Camera leftEye;
    [SerializeField] Camera rightEye;

    [Header("Eye Settings")]
    public float eyeSpacing = 0f;

    private bool splitScreen = true;

    Vector3 posZero;

    void Awake()
    {
        LoadValues();
        currentZoom = leftEye.fieldOfView;
        posZero = transform.localPosition;
    }

    public override void Update()
    {
        base.Update();
        UpdateSpacing();
        UpdateHeight();
        UpdateSplitScreen();
    }

    void OnApplicationQuit()
    {
        SaveValues();
    }

    public void ToggleSplitScreen()
    {
        splitScreen = !splitScreen;
    }

    void UpdateSplitScreen()
    {
        if (splitScreen)
        {
            leftEye.rect = new Rect(0, 0, 0.5f, 1);
            rightEye.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else
        {
            leftEye.rect = new Rect(0, 0, 1, 1);
            rightEye.rect = new Rect(0.5f, 0, 0, 0);
        }
    }

    void UpdateSpacing()
    {
        float halfSpacing = eyeSpacing / 2f;
        leftEye.transform.localPosition = new Vector3(halfSpacing, 0, 0);
        rightEye.transform.localPosition = new Vector3(-halfSpacing, 0, 0);
    }

    public override void UpdateRotation()
    {
        currentRotation = Mathf.SmoothDampAngle(currentRotation, rotation, ref rotationVelocity, rotationSpeed * Time.deltaTime) % 360;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotation, transform.localEulerAngles.z);
    }

    public override void SetRotation(float rot)
    {
        rotation = (360 + rot) % 360;
    }

    public override void SetZoom(float newZoom)
    {
        leftEye.fieldOfView = newZoom;
        rightEye.fieldOfView = newZoom;
    }

    void UpdateHeight()
    {
        Vector3 pos = transform.localPosition;
        pos.y = posZero.y + heightOffset;
        transform.localPosition = pos;
    }

    public override void UpdateZoom()
    {
        zoom = Mathf.Clamp(zoom, 0, 100);
        int targetZoom = (int)Map(zoom, 0, 100, minZoom, maxZoom);
        currentZoom = (int)Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSpeed * Time.deltaTime);
        SetZoom(currentZoom);
    }

    void LoadValues()
    {
        heightOffset = PlayerPrefs.GetFloat("heightOffset", 0);
        eyeSpacing = PlayerPrefs.GetFloat("eyeSpacing", 0);
    }

    void SaveValues()
    {
        PlayerPrefs.SetFloat("heightOffset", heightOffset);
        PlayerPrefs.SetFloat("eyeSpacing", eyeSpacing);
        PlayerPrefs.Save();
    }
}
