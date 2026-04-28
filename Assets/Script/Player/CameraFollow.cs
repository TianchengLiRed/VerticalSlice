using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("CameraFollow")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = 0.08f;

    [Header("MouseZoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float sensitivity = 10f;

    public Camera cam;
    private float targetZoom;

    private Vector3 followVelocity = Vector3.zero;

    void Start()
    {
        if (cam.orthographic)
            targetZoom = cam.orthographicSize;
        else
            targetZoom = cam.fieldOfView;

        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        CameraFollow1();
        CameraZoom();
    }

    private void CameraFollow1()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime
        );
    }

    private void CameraZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollData) > 0.001f)
        {
            targetZoom -= scrollData * sensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }
}