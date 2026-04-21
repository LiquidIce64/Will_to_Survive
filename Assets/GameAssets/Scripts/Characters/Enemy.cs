using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : BaseCharacter
{
    protected enum NPCState
    {
        Follow,
        Attack,
        Retreat
    }

    protected NavMeshAgent agent;
    [SerializeField] protected float maxAttackDistance = 7.5f;
    [SerializeField] protected float attackDistance = 5f;
    [SerializeField] protected float minAttackDistance = 2.5f;
    [SerializeField] protected float aimingTime = 1f;
    protected float _remainingAimTime;
    protected NPCState _state = NPCState.Follow;

    private void ResetAimTime() => _remainingAimTime = aimingTime * Random.Range(0.95f, 1.05f);

    new protected void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        ResetAimTime();
    }
    
    protected void FollowPlayer()
    {
        ResetAimTime();
        agent.SetDestination(Player.Instance.transform.position);
    }

    protected void Retreat()
    {
        ResetAimTime();

        var playerDirection = Player.Instance.transform.position;
        playerDirection -= transform.position;
        playerDirection.y = 0f;
        playerDirection.Normalize();

        agent.SetDestination(transform.position - playerDirection * maxAttackDistance);
    }

    protected void Attack()
    {
        agent.ResetPath();
        if (_remainingAimTime > 0f)
            _remainingAimTime -= Time.fixedDeltaTime;
            return;
        // Fire
    }

    virtual protected void EnemyLogic()
    {
        var distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        switch (_state)
        {
            case NPCState.Follow:
                FollowPlayer();
                if (distance <= attackDistance)
                    _state = NPCState.Attack;
                break;
            case NPCState.Attack:
                Attack();
                if (distance > maxAttackDistance)
                    _state = NPCState.Follow;
                else if (distance < minAttackDistance)
                    _state = NPCState.Retreat;
                break;
            case NPCState.Retreat:
                Retreat();
                if (distance >= attackDistance)
                    _state = NPCState.Attack;
                break;
        }
    }

    protected void FixedUpdate()
    {
        agent.nextPosition = transform.position;

        Vector3 targetPos = Player.Instance.transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);

        EnemyLogic();

        Vector3 desiredVel = agent.desiredVelocity.normalized;
        Move(new Vector2(desiredVel.x, desiredVel.z));
    }

    override protected void OnDeath()
    {
        Destroy(gameObject);
    }
}
