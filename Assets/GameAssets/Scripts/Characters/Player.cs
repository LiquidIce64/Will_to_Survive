using UnityEngine;

public class Player : BaseCharacter
{
    private static Player instance;

    private static InputSystem_Actions inputActions;

    public static Player Instance => instance;
    public static InputSystem_Actions InputActions => inputActions;

    private Vector3 cameraOffset;

    public PlayerXP playerXP;
    
    new protected void Awake()
    {
        base.Awake();
        instance = this;
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        cameraOffset = Camera.main.transform.position - transform.position;
    }

    protected override void OnDeath()
    {
        Debug.Log("Game Over");
        health = maxHealth;
    }

    private void Update()
    {
        Move(inputActions.Player.Move.ReadValue<Vector2>());
        Camera.main.transform.position = transform.position + cameraOffset;

        Vector2 mousePos = inputActions.Player.Look.ReadValue<Vector2>();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hit))
        {
            Vector3 targetPos = hit.point;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ITriggerable trigger))
            trigger.OnEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ITriggerable trigger))
            trigger.OnExit();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
