using Evolution.Commons.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Evolution.Commons
{
    public class CameraControll : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField]
        private InputActionAsset _playerActions;
        private InputAction _scroll;
        private InputAction _click;
        private InputAction _mouseMov;
        private InputAction _space;

        [SerializeField]
        private float _scrollSpeed = 5f;
        [SerializeField]
        private float _moveSpeed = 5f;
        [SerializeField]
        private bool _invertScroll = true;

        private float _scrollMovement;
        private Vector2 _mouseMovement;
        private bool _isDragging;
        private bool _isPause;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _scroll = _playerActions.FindAction("Scroll");
            _click = _playerActions.FindAction("MiddleMouse");
            _mouseMov = _playerActions.FindAction("Look");
            _space = _playerActions.FindAction("Jump");
        }
        private void OnEnable()
        {
            _playerActions.Enable();
            _playerActions.FindActionMap("Player").Enable();
        }
        private void Update()
        {
            _scrollMovement = _scroll.ReadValue<float>();
            _mouseMovement = _mouseMov.ReadValue<Vector2>();
            _isDragging = _click.ReadValue<float>() != 0;
            _isPause = _space.WasPerformedThisFrame();
            if (_invertScroll)
            {
                _scrollMovement*=-1;
            }
            if (_isPause)
            {
                GameManager.Instance.Pause();
            }
            _camera.orthographicSize += _scrollMovement * _scrollSpeed;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 1, float.MaxValue);
            if (_isDragging)
            {
                Vector3 move = new Vector3(-_mouseMovement.x, -_mouseMovement.y, 0);
                transform.position += move * _moveSpeed * Time.deltaTime;
            }
            if(transform.position.z != 10)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -15);
            }
        }
    }
}
