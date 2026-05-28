using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjust : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 12, -8);
    [SerializeField] private float followSpeed = 5f;

    

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        LevelSpawn.OnPlayerSpawned += SetPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.Log("No Player");
            return;
        }

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

    }

    void SetPlayer(PlayerHealth health)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        target = playerObj.transform;

    }

    private void Zoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= zoom * zoomSpeed;

        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize,
            minZoom,
            maxZoom
        );
    }

    private void CameraFollow()
    {

    }
}
