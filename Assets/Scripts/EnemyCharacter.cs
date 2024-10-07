using character;
using character.states;
using statemachine;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyCharacterSensor))]
public class EnemyCharacter : MonoBehaviour {
    // Character states
    [SerializeField] SleepState _sleepState;
    [SerializeField] HuntPlayerState _huntState;
    [SerializeField] DeadState _deadState;

    // IVelocity2
    public float VelocityX { get => _agent.velocity.x; set => _agent.velocity = new Vector2(value, _agent.velocity.y); }
    public float VelocityY { get => _agent.velocity.y; set => _agent.velocity = new Vector2(_agent.velocity.x, value); }

    public Animator Animator { get; private set; }

    // Character components
    // Unity components
    SpriteRenderer _spriteRenderer;
    EnemyCharacterSensor _sensor;
    NavMeshAgent _agent;

    StateMachine _stateMachine;
    bool _isDead = false;

    void Awake() {
        _sensor = GetComponent<EnemyCharacterSensor>();
        _agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Health health = GetComponent<Health>();
        health.Died += () => {
            _isDead = true;
            _agent.enabled = false;
        };
    }

    void Start() {
        _sleepState.Init(Animator);
        _huntState.Init(Animator);
        _deadState.Init(Animator);

        _stateMachine = new StateMachine();
        _stateMachine.SetState(_sleepState);

        // Sleep
        // -> Hunt: Player is in range
        _stateMachine.AddTransition(_sleepState, _huntState, () => _sensor.IsPlayerInRange);

        // Hunt
        // -> Sleep: Player is out of range
        _stateMachine.AddTransition(_huntState, _sleepState, () => !_sensor.IsPlayerInRange);

        _stateMachine.AddAnyTransition(_deadState, () => _isDead);
    }

    public bool IsHuntingPlayer() {
        return _stateMachine.GetState() == _huntState;
    }

    void FixedUpdate() {
        _stateMachine.FixedUpdate();
    }

    void Update() {
        _stateMachine.Update();
        // Animator.SetBool("IsMoving", VelocityX != 0 || VelocityY != 0);
        // Animator.SetFloat("Speed", Mathf.Abs(VelocityX));

        if (VelocityX != 0)
            _spriteRenderer.flipX = VelocityX < 0;
    }
}
