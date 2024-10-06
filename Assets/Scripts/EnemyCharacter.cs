using character;
using character.states;
using statemachine;
using UnityEngine;

[RequireComponent(typeof(EnemyCharacterSensor), typeof(Rigidbody2D))]
public class EnemyCharacter : MonoBehaviour {
    // Character states
    [SerializeField] SleepState _sleepState;
    [SerializeField] HuntPlayerState _huntState;

    // IVelocity2
    public float VelocityX { get => _rigidbody.linearVelocityX; set => _rigidbody.linearVelocityX = value; }
    public float VelocityY { get => _rigidbody.linearVelocityY; set => _rigidbody.linearVelocityY = value; }

    public Animator Animator { get; private set; }

    // Character components
    // Unity components
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;
    EnemyCharacterSensor _sensor;

    StateMachine _stateMachine;

    void Awake() {
        _sensor = GetComponent<EnemyCharacterSensor>();
        _rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start() {
        _sleepState.Init(Animator);
        _huntState.Init(Animator);

        _stateMachine = new StateMachine();
        _stateMachine.SetState(_sleepState);

        // Sleep
        // -> Hunt: Player is in range
        _stateMachine.AddTransition(_sleepState, _huntState, () => _sensor.IsPlayerInRange);

        // Hunt
        // -> Sleep: Player is out of range
        _stateMachine.AddTransition(_huntState, _sleepState, () => !_sensor.IsPlayerInRange);
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
