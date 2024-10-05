using character;
using character.abilities;
using character.states;
using statemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(CharacterSensor), typeof(Rigidbody2D))]
public class PlayerCharacter : MonoBehaviour, IVelocity2, ICharacter {
    // Character states
    [SerializeField] GroundState _groundedState;

    // IVelocity2
    public float VelocityX { get => _rigidbody.linearVelocityX; set => _rigidbody.linearVelocityX = value; }
    public float VelocityY { get => _rigidbody.linearVelocityY; set => _rigidbody.linearVelocityY = value; }

    // ICharacter
    public Animator Animator { get; private set; }
    public CharacterSensor Sensor { get; private set; }
    public CharacterInput Input => _playerInput.Input;
    public CharacterMoveAbility MoveAbility => _moveAbility;

    // Character components
    [SerializeField] CharacterMoveAbility _moveAbility;
    PlayerInput _playerInput;
    // Unity components
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;

    StateMachine _stateMachine;

    void Awake() {
        Sensor = GetComponent<CharacterSensor>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _moveAbility.Init(Sensor, _playerInput, this);
    }

    void Start() {
        _groundedState.Init(this);

        _stateMachine = new StateMachine();
        _stateMachine.SetState(_groundedState);

        // Example of transition to another state
        // Ground
        // -> Jump: Pressing jump or walking off a ledge
        // _stateMachine.AddTransition(_groundedState, _jumpState, () => _playerInput.Input.Jump || (!_sensor.IsTouchingGround && !_sensor.IsTouchingWall));
    }

    void FixedUpdate() {
        _stateMachine.FixedUpdate();
    }

    void Update() {
        _stateMachine.Update();
        Animator.SetBool("IsMoving", _playerInput.Input.Direction.magnitude > 0);
        Animator.SetFloat("Speed", Mathf.Abs(VelocityX));

        if (_playerInput.Input.Direction.x != 0)
            _spriteRenderer.flipX = _playerInput.Input.Direction.x < 0;
    }
}
