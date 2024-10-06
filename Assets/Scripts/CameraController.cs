using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour {
    private GameObject _target;
    private Transform _map;
    private Camera _cam;
    private Bounds _bounds;
    [SerializeField] float _speed = 2f;

    void Start() {
        _cam = Camera.main;
    }

    void Update() {
        if (_target == null) {
            _target = GameObject.FindWithTag("Player");
        }

        if (_map == null) {
            _map = GameObject.FindWithTag("Map")?.transform;
            if (_map == null)
                return;

            _bounds = new Bounds();
            foreach (var map in _map.GetComponentsInChildren<Tilemap>()) {
                map.CompressBounds();
                var map_bounds = map.localBounds;
                _bounds.Encapsulate(map_bounds.min);
                _bounds.Encapsulate(map_bounds.max);
            }
        }

        if (_target != null) {
            Vector3 targetPosition = _target.transform.position;
            targetPosition.z = transform.position.z;

            if (_map != null) {
                var viewportBottomLeft = _cam.ViewportToWorldPoint(Vector3.zero);
                var viewportUpperRight = _cam.ViewportToWorldPoint(Vector3.one);
                var viewportMargin = (viewportUpperRight - viewportBottomLeft) / 2f;

                var viewBounds = _bounds;
                viewBounds.min += viewportMargin;
                viewBounds.max -= viewportMargin;

                if (targetPosition.x > viewBounds.max.x) {
                    targetPosition.x = viewBounds.max.x;
                } else if (targetPosition.x < viewBounds.min.x) {
                    targetPosition.x = viewBounds.min.x;
                }
                if (targetPosition.y > viewBounds.max.y) {
                    targetPosition.y = viewBounds.max.y;
                } else if (targetPosition.y < viewBounds.min.y) {
                    targetPosition.y = viewBounds.min.y;
                }
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, _speed * Time.deltaTime);
        }
    }
}
