using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class CharacterInput {
    [field: SerializeField] public Vector2 Direction { get; set; }
    [field: SerializeField] public bool Run { get; set; }
}
public interface IDirection2Provider {
    Vector2 Direction { get; }
}

public class PlayerInput : MonoBehaviour, IDirection2Provider {
    [SerializeField] PlayerInputConfig _config;
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] InputActionReference _moveAction;
    [SerializeField] InputActionReference _runAction;
    [field: SerializeField] public CharacterInput Input { get; private set; } = new CharacterInput();

    public Vector2 Direction => Input.Direction;

    void OnValidate() {
        if (_playerCharacter == null)
            _playerCharacter = GetComponent<PlayerCharacter>();
    }

    void Start() {
        _moveAction.action.Enable();
    }

    void OnEnable() {
        _moveAction.action.performed += ctx => Move(ctx.ReadValue<Vector2>());
        _runAction.action.performed += ctx => Input.Run = true;
        _runAction.action.canceled += ctx => Input.Run = false;
    }

    void OnDisable() {
        _moveAction.action.performed -= ctx => Move(ctx.ReadValue<Vector2>());
        _runAction.action.performed -= ctx => Input.Run = true;
        _runAction.action.canceled -= ctx => Input.Run = false;
    }

    void Move(Vector2 direction) {
        if (_config.SnapInput) {
            direction.x = Mathf.Abs(direction.x) < _config.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(direction.x);
            direction.y = Mathf.Abs(direction.y) < _config.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(direction.y);
        }
        Input.Direction = direction;
    }
}

