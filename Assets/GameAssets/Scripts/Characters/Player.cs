using UnityEngine;

public class Player : BaseCharacter
{
    private static Player instance;

    private static InputSystem_Actions inputActions;

    public static Player Instance => instance;
    public static InputSystem_Actions InputActions => inputActions;

    private Vector3 cameraOffset;

    public PlayerXP playerXP;

    public SphereCollider xpMagnet;

    private Vector3 targetPos;
    
    new protected void Awake()
    {
        base.Awake();
        instance = this;
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        cameraOffset = Camera.main.transform.position - transform.position;
    }

    protected override void OnDamaged()
    {
        var healthBar = HealthBar.Instance;
        healthBar.Health = Mathf.CeilToInt(health / maxHealth * healthBar.MaxHealth);
    }

    protected override void OnDeath()
    {
        Debug.Log("Game Over");

        // Debug
        health = maxHealth;
        OnDamaged();
    }

    private void Update()
    {
        Move(inputActions.Player.Move.ReadValue<Vector2>());
        Camera.main.transform.position = transform.position + cameraOffset;

        Vector2 mousePos = inputActions.Player.Look.ReadValue<Vector2>();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hit))
        {
            targetPos = hit.point;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }

        if(inputActions.Player.Attack.IsPressed())
        {
            if (weapon == null) return;
            weapon.Use(targetPos);
        }
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
