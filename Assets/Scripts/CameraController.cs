using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControlActions cameraControlActions;
    private InputAction movement;
    private Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField]
    private float maxSpeed = 5f;
    private float speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;

    [Header("Zoom Settings")]
    [SerializeField]
    private float stepSize = 2f;
    [SerializeField]
    private float zoomDampening = 7.5f;
    [SerializeField]
    private float minZoom = 5f;
    [SerializeField]
    private float maxZoom = 15f;
    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float maxRotationSpeed = 1f;

    [SerializeField]
    [Range(0f, 0.1f)]
    private float edgeTolerance = 0.05f;
    [SerializeField]
    private bool useScreenEdge = true;

    private Vector3 targetPosition;
    private float zoomHeight;

    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    private bool usingMovement;

    Vector3 startDrag;

    private void Awake()
    {
        cameraControlActions = new CameraControlActions();
        cameraTransform = this.GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
        GameObject baseObject = GameObject.FindGameObjectWithTag("Base");
        if (baseObject != null)
        {
            transform.position = baseObject.transform.position;
        }
    }

    private void OnEnable()
    {
        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(this.transform);

        lastPosition = this.transform.position;
        movement = cameraControlActions.Camera.Movement;
        cameraControlActions.Camera.RotateCamera.performed += RotateCamera;
        cameraControlActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraControlActions.Camera.Enable();
    }

    private void OnDisable()
    {
        cameraControlActions.Camera.RotateCamera.performed -= RotateCamera;
        cameraControlActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraControlActions.Disable();
    }

    private void Update()
    {
        GetKeyboardMovement();
        if(useScreenEdge)
        {
            CheckMouseAtScreenEdge();
        }
        UpdateVelocity();
        UpdateBasePosition();
        UpdateCameraPosition();
    }

    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
            + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if(inputValue.sqrMagnitude >0.1f)
        {
            usingMovement = true;
            targetPosition += inputValue;
        }
        else
        {
            usingMovement = false;
        }
    }

    Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0;
        return right;
    }

    Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        return forward;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }

    private void RotateCamera(InputAction.CallbackContext inputValue)
    {
        useScreenEdge = true;
        if (!Mouse.current.middleButton.isPressed)           
            return;

        useScreenEdge = false;
        float value = inputValue.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(0, value * maxRotationSpeed + transform.rotation.eulerAngles.y , 0f);
    }

    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y / 100f;

        if(Mathf.Abs(value) > 0.1f)
        {
            zoomHeight = cameraTransform.localPosition.y + value * stepSize;
            if(zoomHeight < minZoom)
            {
                zoomHeight = minZoom;
            }
            else if(zoomHeight > maxZoom)
            {
                zoomHeight = maxZoom;
            }
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
        zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
        cameraTransform.LookAt(this.transform);
    }

    void CheckMouseAtScreenEdge()
    {
        if (usingMovement) return;
        

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if(mousePosition.x < edgeTolerance * Screen.width)
        {
            moveDirection += -GetCameraRight();
        }
        else if(mousePosition.x > (1f - edgeTolerance) * Screen.width)
        {
            moveDirection += GetCameraRight();
        }

        if(mousePosition.y < edgeTolerance * Screen.height)
        {
            moveDirection += -GetCameraForward();
        }
        else if(mousePosition.y > (1f - edgeTolerance) * Screen.height)
        {
            moveDirection += GetCameraForward();
        }

        targetPosition += moveDirection;
    }

}
