using UnityEngine;

public class BossCamera : MonoBehaviour
{
    [Header("General")]
    public bool PlayerYAffectsCamera = true;
    public bool BossYAffectsCamera = false;

    [Header("Refs")]
    public Transform PlayerTransform;
    public Transform BossTransform;
    public Camera Cam;

    [Header("Tracking")]
    public Vector3 TrackingOffset = new Vector3(0f, 2f, -20f);
    public float TrackingSpeed = 1.0f;

    [Header("Zoom")]
    public float ZoomSpeed = 4.0f;
    public float YOffsetFromCameraSizeFactor = 0.75f;
    public float MaxYOffsetFromCameraSize = 999f;
    public float MinimumZoom = 4f;
    public float MaximumZoom = 12f;

    [Tooltip("How much player separation increases ortho size.")]
    public float ZoomOutFactor = 0.35f;

    [Header("Bounds (X only)")]
    public Transform leftBound;
    public Transform rightBound;
    public float boundSoftness = 0.90f;









    private float velX, velY, velZ;
    private float zoomVelocity;




    private void Reset()
    {
        Cam = GetComponent<Camera>();
        if (Cam == null) Cam = Camera.main;
    }

    private void Awake()
    {
        if (Cam == null) Cam = GetComponent<Camera>();
        if (Cam == null) Cam = Camera.main;
    }

    private void Start()
    {
        if (PlayerTransform == null || BossTransform == null)
        {
            Debug.LogError("CameraController2: Players not assigned.");
            enabled = false;
            return;
        }

        if (Cam == null)
        {
            Debug.LogError("CameraController2: Camera reference not found.");
            enabled = false;
            return;
        }

        if (!Cam.orthographic)
            Debug.LogWarning("CameraController2: camera must be orthographic.");
    }

    private void LateUpdate()
    {
        if (PlayerTransform == null || BossTransform == null || Cam == null) return;
        
        Vector3 playerPos = PlayerTransform.position;
        Vector3 bossPos = BossTransform.position;

        if (!PlayerYAffectsCamera) playerPos = new Vector3(playerPos.x, 0.0f, playerPos.z);
        if (!BossYAffectsCamera) bossPos = new Vector3(bossPos.x, 0.0f, bossPos.z);

        Vector3 midpoint = (playerPos + bossPos) * 0.5f;

        float separation = Vector3.Distance(PlayerTransform.position, BossTransform.position);
        float desiredSize = Mathf.Clamp(separation * ZoomOutFactor, MinimumZoom, MaximumZoom);

        float zoomSmoothTime = SpeedToSmoothTime(ZoomSpeed);
        Cam.orthographicSize = zoomSmoothTime <= 0f
            ? desiredSize
            : Mathf.SmoothDamp(Cam.orthographicSize, desiredSize, ref zoomVelocity, zoomSmoothTime);

        Vector3 desiredPos = midpoint + TrackingOffset;

        //y offset from szie
        float zoomT = Mathf.Max(0f, Cam.orthographicSize - MinimumZoom);
        float extraY = Mathf.Min(zoomT * YOffsetFromCameraSizeFactor, MaxYOffsetFromCameraSize);

        desiredPos.y += extraY;

        if (leftBound != null && rightBound != null)
        {
            float minX = Mathf.Min(leftBound.position.x, rightBound.position.x);
            float maxX = Mathf.Max(leftBound.position.x, rightBound.position.x);

            if (boundSoftness <= 0f)
            {
                desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            }
            else
            {
                desiredPos.x = SoftClamp(desiredPos.x, minX, maxX, boundSoftness);
            }
        }

        Vector3 current = transform.position;

        float trackingLerpDelta = SpeedToSmoothTime(TrackingSpeed);
        float x = trackingLerpDelta <= 0f ? desiredPos.x : Mathf.SmoothDamp(current.x, desiredPos.x, ref velX, trackingLerpDelta);
        float y = trackingLerpDelta <= 0f ? desiredPos.y : Mathf.SmoothDamp(current.y, desiredPos.y, ref velY, trackingLerpDelta);
        float z = trackingLerpDelta <= 0f ? desiredPos.z : Mathf.SmoothDamp(current.z, desiredPos.z, ref velZ, trackingLerpDelta);

        transform.position = new Vector3(x, y, z);
    }

    private static float SpeedToSmoothTime(float speed)
    {
        if (speed <= 0f) return 0f;
        return 1f / speed;
    }

    private static float SoftClamp(float value, float min, float max, float softness)
    {
        if (value < min)
        {
            float d = min - value;
            return min - d / (1f + d / Mathf.Max(0.0001f, softness));
        }
        if (value > max)
        {
            float d = value - max;
            return max + d / (1f + d / Mathf.Max(0.0001f, softness));
        }
        return value;
    }
}