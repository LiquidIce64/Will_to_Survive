using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    private static InputSystem_Actions inputActions;

    public float speed = 10000f;
    public float maxHealth = 100f;
    public float knockbackResistance = 1f;
    [SerializeField] private float xpPickupRange = 4f;
    public PlayerXP playerXP;

    private Vector3 targetPos;
    private float xpReceived = 0f;
    [HideInInspector] public bool heal = false;

    public EntityManager entityManager;
    public EntityQuery playerQuery;
    public EntityQuery magnetQuery;
    public EntityQuery spawnerQuery;

    public static PlayerController Instance => instance;
    public static InputSystem_Actions InputActions => inputActions;

    public float XPPickupRange
    {
        get => xpPickupRange;
        set
        {
            xpPickupRange = value;
            if (!magnetQuery.TryGetSingletonEntity<XPMagnetTag>(out Entity magnetEntity)) return;
            var transform = entityManager.GetComponentData<LocalTransform>(magnetEntity);
            transform.Scale = xpPickupRange;
            entityManager.SetComponentData(magnetEntity, transform);
        }
    }

    public void OnLevelUp()
    {
        if (!spawnerQuery.TryGetSingletonEntity<EnemySpawnConfig>(out Entity spawnerEntity)) return;
        var config = entityManager.GetComponentData<EnemySpawnConfig>(spawnerEntity);
        config.level = playerXP.Level;
        entityManager.SetComponentData(spawnerEntity, config);
    }

    private void OnValidate()
    {
        instance = this;
    }

    private void Awake()
    {
        instance = this;
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag));
        magnetQuery = entityManager.CreateEntityQuery(typeof(XPMagnetTag));
        spawnerQuery = entityManager.CreateEntityQuery(typeof(EnemySpawnConfig));

        inputActions.UI.Pause.started += _ => PauseMenu.Instance.TogglePause();
    }

    private void Update()
    {
        bool isFiring = inputActions.Player.Attack.IsPressed();
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>().normalized;
        Vector2 mousePos = inputActions.Player.Look.ReadValue<Vector2>();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hit))
        {
            targetPos = hit.point;
            targetPos.y = transform.position.y;
        }

        if (!playerQuery.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        if (!entityManager.HasComponent<CharacterData>(playerEntity)) return;
        var characterData = entityManager.GetComponentData<CharacterData>(playerEntity);
        characterData.speed = speed;
        characterData.maxHealth = maxHealth;
        characterData.knockbackResistance = knockbackResistance;
        characterData.moveVector = new float3(moveVector.x, 0f, moveVector.y);
        characterData.targetPos = new float3(targetPos.x, targetPos.y, targetPos.z);
        characterData.isFiring = isFiring;
        if (heal)
        {
            characterData.health = maxHealth;
            heal = false;
        }
        entityManager.SetComponentData(playerEntity, characterData);

        var healthBar = HealthBar.Instance;
        healthBar.Health = Mathf.CeilToInt(characterData.health / maxHealth * healthBar.MaxHealth);
        if (characterData.health == 0f)
        {
            DeadMenuManager.Instance.ShowDeadMenu();
            enabled = false;
        }

        if (!magnetQuery.TryGetSingletonEntity<XPMagnetTag>(out Entity magnetEntity)) return;
        var config = entityManager.GetComponentData<XPOrbConfig>(magnetEntity);
        playerXP.AddXP(config.xpGained - xpReceived);
        xpReceived = config.xpGained;
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
