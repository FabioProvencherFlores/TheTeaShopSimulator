using UnityEngine;
using UnityEngine.Events;

public enum ObjectID
{
    INVALID,
    Cube
}

public class ObjectInteractionBase : MonoBehaviour
{
    Outline _outlineComponent;

    [SerializeField] private float outlineWidth = 5f;
    [SerializeField] float timeBeforeStopHover = 0.1f;
    private float _lastHoveredTimestamp = 0f;
    private bool _isHovered;

	protected ComplexInteraction myComplexInteraction;

    [HideInInspector] public int myDeliverySpotIdx = -1;
    [HideInInspector] public UnityEvent<ObjectInteractionBase> OnInteracted = new UnityEvent<ObjectInteractionBase>();

	public virtual InteractionUID GetInteractionUID()
    {
        return InteractionUID.INVALID;
    }
    
    public virtual InteractableObjectUID GetInteractableObjectUID()
    {
        return InteractableObjectUID.GenericObject;
    }

    public virtual bool CanInteract()
    {
        return true;
    }

    public virtual bool Interact(PlayerController player)
    {
        Debug.LogError("Base Interact should never be called. Did you forget to downcast?", this);
        return false;
    }

    private protected virtual Color GetOutlineColor()
    {
        Debug.LogError("This outline color should not be called, did you forget to override my methods?", this);
        return Color.red;
    }

    public virtual void Awake()
    {
        _outlineComponent = gameObject.AddComponent<Outline>();

        _outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
        _outlineComponent.OutlineColor = GetOutlineColor();
        _outlineComponent.OutlineWidth = outlineWidth;
        _outlineComponent.enabled = false;
        _lastHoveredTimestamp = 0f;
        _isHovered = false;

        myComplexInteraction = GetComponent<ComplexInteraction>();
    }

    void LateUpdate()
    {
        if (_isHovered && Time.time - _lastHoveredTimestamp > timeBeforeStopHover) TurnHoverOff();
    }

    public void OnHoveredChange(bool isHovered)
    {
        if (isHovered) TurnHoverOn();
        else TurnHoverOff();
    }

    protected virtual void TurnHoverOn()
    {
        _lastHoveredTimestamp = Time.time;
        _outlineComponent.enabled = true;
        _isHovered = true;
    }

    protected virtual void TurnHoverOff()
    {
        _outlineComponent.enabled = false;
        _isHovered = false;
    }
    
}
