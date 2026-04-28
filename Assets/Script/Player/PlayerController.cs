using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Aim
    }

    public PlayerState CurrentState { get; private set; }
    public event System.Action<PlayerState> OnStateChanged;

    [Header("Move")]
    [SerializeField] private CharacterController cc;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    private float verticalVelocity;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float detectRange = 3f;
    private List<Interactive> interObjs = new List<Interactive>();

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        CurrentState = PlayerState.Idle;
    }

    void Update()
    {
        Move();
        InterRange();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CurrentState == PlayerState.Idle)
            {
                ChangeState(PlayerState.Aim);
            }
            else if (CurrentState == PlayerState.Aim)
            {
                ChangeState(PlayerState.Idle);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentState == PlayerState.Idle)
            {
                ClickInteract();
            }
            else if (CurrentState == PlayerState.Aim)
            {

            }
        }
    }

    private void Move()
    {

        if (cameraTransform == null) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        Vector3 moveVelocity = Vector3.zero;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDir = (camForward * v + camRight * h).normalized;

        if (inputDir.magnitude > 0.01f)
        {
            moveVelocity = inputDir * currentSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (cc.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        moveVelocity.y = verticalVelocity;

        cc.Move(moveVelocity * Time.deltaTime);
    }

    private void InterRange()
    {
        foreach (var obj in interObjs)
        {
            if (obj != null) obj.SetHighlight(false);
        }

        interObjs.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, interactLayer);
        foreach (var hit in hits)
        {
            Interactive interactable = hit.GetComponent<Interactive>();
            if (interactable != null)
            {
                interactable.SetHighlight(true);
                interObjs.Add(interactable);
            }
        }
    }

    private void ClickInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactLayer))
        {
            Interactive clickedObj = hit.collider.GetComponent<Interactive>();

            if (clickedObj != null && interObjs.Contains(clickedObj))
            {
                clickedObj.OnInteract();
            }
            else if (clickedObj != null)
            {
                Debug.Log("看见了，但够不到（超出交互范围）");
            }
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
