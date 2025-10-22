using TMPro;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Debug settings")]
    [SerializeField] bool showControllerDebug = false;
    [SerializeField] TMP_Text timeOfDayTxt;

    [Header("Control settings")]
    #region Player Movement
    [SerializeField]
    float movementSpeed = 10f;
    [SerializeField]
    float maxMovementSpeed = 5f;
    Rigidbody rb;
    #endregion

    #region Camera Controls
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField] GameObject cameraHolder;
    float _maxLookAngle = 90f;
    float rotationAngle;
    float verticalAngle;
    #endregion

    [Header("Interaction settings")]
    #region Interact
    [SerializeField] float maxDistanceToInteract = 1f;
    private TimeOfDayHoursMinutes currentTimeOfDay;
    float _maxDistanceToInteractSqrd;
    private ObjectInteractionBase currentPriorizedInteract;
    private ComplexInteraction currentPriorizedComplexInteract;
    [SerializeField] Transform _smallObjHoldPosition;
    public Transform GetSmallHoldPosition() { return _smallObjHoldPosition; }

    [SerializeField] Transform feetPosition;
    [SerializeField] float maxDistanceForComplexInteraction = 1f;
    private Vector3 myLastComplexInteractionPos;
    #endregion

    private InteractionManager myInteractionManagerInstance;

    Vector3 _inputVelocity;
    Vector3 _lastHitPosition;
    public Vector3 GetLastHitLocation() { return _lastHitPosition; }

    LineRenderer lineRenderer;

    bool _movementIsLocked;
    private void UpdateMovementLock(bool aMovementIsLocked) { _movementIsLocked = aMovementIsLocked; }

    void Awake()
    {
        _maxDistanceToInteractSqrd = maxDistanceToInteract * maxDistanceToInteract;
        _movementIsLocked = false;
        lineRenderer = GetComponent<LineRenderer>();
        // Set the color
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.yellow;
        // Set the width
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.003f;

        // Set the number of vertices
        lineRenderer.positionCount = 2;
    }

    void Start()
    {
        if (GameManager.Instance == null) Debug.LogError("Wowowo, you need a Game Manager to use the player, plz", this);

        GameManager.Instance.RegisterToWorldMovementLock(UpdateMovementLock);

        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        myInteractionManagerInstance = InteractionManager.Instance;
        myInteractionManagerInstance.PlayerReference = this;
    }

    void Update()
    {
        currentTimeOfDay.SetTime(GameManager.Instance.TimeOfDayInMinutes);
        timeOfDayTxt.text = currentTimeOfDay.ToString();

        if (_movementIsLocked) return;

        if (myInteractionManagerInstance.ComplexInteractionMemory != null)
        {
            if ((transform.position - myLastComplexInteractionPos).sqrMagnitude > maxDistanceForComplexInteraction)
            {
                myInteractionManagerInstance.ComplexInteractionMemory = null;
            }
        }

        // Check for interaction
        currentPriorizedInteract = null;
		currentPriorizedComplexInteract = null;
        RaycastHit[] hits = Physics.RaycastAll(cameraHolder.transform.position, cameraHolder.transform.forward, 100f);
        float shortestDistanceSQRDSoFar = _maxDistanceToInteractSqrd;

		foreach (RaycastHit hit in hits)
        {
            #region Debug
            if (showControllerDebug)
            {
                Debug.DrawLine(cameraHolder.transform.position, cameraHolder.transform.position+(cameraHolder.transform.forward*maxDistanceToInteract), Color.red, 1f);
                print(hit.collider.gameObject.name);
            }
            #endregion
            Vector3 positionDiff = transform.position - hit.point;
            positionDiff.y = 0; // ignore heigt diff
            float distanceSqrd = positionDiff.sqrMagnitude;
            
            if (distanceSqrd < shortestDistanceSQRDSoFar)
            {
                bool foundSomething = false;
                ObjectInteractionBase interactable = hit.collider.GetComponent<ObjectInteractionBase>();
                if (interactable != null && interactable.CanInteract())
                {
					currentPriorizedInteract = interactable;
                    foundSomething = true;
				}

                ComplexInteraction complexInteraction = hit.collider.GetComponent<ComplexInteraction>();
				if (complexInteraction != null)
                {
                    currentPriorizedComplexInteract = complexInteraction;
                    foundSomething = true;
				}

                if (foundSomething)
                {
                    shortestDistanceSQRDSoFar = distanceSqrd;
                    _lastHitPosition = hit.point;
                }
                else
                {
                    // prevents interacting through walls since collision will be closer
                    currentPriorizedInteract = null;
                    currentPriorizedComplexInteract = null;
                }
            }
        }

        // prio should be highlighted
        if (currentPriorizedInteract != null) currentPriorizedInteract.OnHoveredChange(true);
		
        // Clicked
		if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentPriorizedInteract != null)
            {
                if (currentPriorizedInteract.Interact(this)) currentPriorizedInteract.OnInteracted.Invoke(currentPriorizedInteract);

                currentPriorizedInteract = null;
			}
            else
            {
                myInteractionManagerInstance.RequestDropItemOnGround(feetPosition.position + transform.forward * 0.5f, transform.rotation);
            }
        }

        // Right-Click
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            myInteractionManagerInstance.ComplexInteractionMemory = currentPriorizedComplexInteract;
            if (currentPriorizedComplexInteract != null) myLastComplexInteractionPos = currentPriorizedComplexInteract.transform.position;
        }

        // draw line to make outline clearer
        if (myInteractionManagerInstance.ComplexInteractionMemory == null)
        {
            lineRenderer.positionCount = 0;
        }
        else
        {
            // Set the positions of the vertices
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, myLastComplexInteractionPos);
            lineRenderer.SetPosition(1, cameraHolder.transform.position + cameraHolder.transform.forward);
        }
    }

    private void FixedUpdate()
    {
        if (_movementIsLocked) return;

        //rotation horizontal
        rotationAngle = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.localEulerAngles = new Vector3(0, rotationAngle, 0);

        // rotation vertical
        verticalAngle += -mouseSensitivity * Input.GetAxis("Mouse Y");
        verticalAngle = Mathf.Clamp(verticalAngle, -_maxLookAngle, _maxLookAngle);
        cameraHolder.transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);

        // read input
        _inputVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // turn forward direction
        _inputVelocity = transform.TransformDirection(_inputVelocity) * movementSpeed;
        // difference with current direction
        _inputVelocity -= rb.linearVelocity;
        // prevent too fast movement
        _inputVelocity.x = Mathf.Clamp(_inputVelocity.x, -maxMovementSpeed, maxMovementSpeed);
        _inputVelocity.z = Mathf.Clamp(_inputVelocity.z, -maxMovementSpeed, maxMovementSpeed);

        rb.AddForce(_inputVelocity, ForceMode.VelocityChange);
    }
}
