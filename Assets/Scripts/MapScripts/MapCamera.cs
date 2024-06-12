using UnityEngine;
using UnityEngine.EventSystems;

public class MapCamera : MonoBehaviour
{
    public float smoothSpeed = 0.125f; 
    public new Camera camera;
    public Bounds bounds;
    
    
    private Vector2 _deltaV;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private Vector2 _prevPosition;
    private Transform _transform;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        _transform = transform;
        bounds = new Bounds(Vector3.zero, new Vector3(8, 50, 50));
    }

    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        HandleTouchInput();
#else
        HandleMouseInput();
#endif

        if (_isMoving)
        {
            SmoothMove();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _deltaV = Vector2.zero;
                _prevPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 curPosition = touch.position;
                MoveCamera(_prevPosition, curPosition);
                _prevPosition = curPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                SetTargetPosition(_deltaV.x / 30, _deltaV.y / 30);
            }
        }
        else
        {
            _deltaV -= _deltaV * Time.deltaTime * 10;
            SetTargetPosition(_deltaV.x / 30, _deltaV.y / 30);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _deltaV = Vector2.zero;
            _prevPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 curMousePosition = Input.mousePosition;
            MoveCamera(_prevPosition, curMousePosition);
            _prevPosition = curMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetTargetPosition(_deltaV.x / 30, _deltaV.y / 30);
        }
        else
        {
            _deltaV -= _deltaV * (Time.deltaTime * 10);
            SetTargetPosition(_deltaV.x / 30, _deltaV.y / 30);
        }
    }

    private void MoveCamera(Vector2 prevPosition, Vector2 curPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 move = camera.ScreenToWorldPoint(prevPosition) - camera.ScreenToWorldPoint(curPosition);
        SetPosition(_transform.position + move);
    }

    public void SetPosition(Vector2 position)
    {
        Vector2 validatedPosition = ApplyBounds(position);
        _targetPosition = new Vector3(validatedPosition.x, validatedPosition.y, _transform.position.z);
        _isMoving = true;
    }

    private void SetTargetPosition(float x, float y)
    {
        Vector3 newTargetPosition = new Vector3(_transform.position.x + x, _transform.position.y + y, _transform.position.z);
        _targetPosition = ApplyBounds(new Vector2(newTargetPosition.x, newTargetPosition.y));
        _isMoving = true;
    }

    private void SmoothMove()
    {
        _transform.position = Vector3.Lerp(_transform.position, _targetPosition, smoothSpeed * Time.deltaTime * 10f);
        if (Vector3.Distance(_transform.position, _targetPosition) < 0.01f)
        {
            _transform.position = _targetPosition;
            _isMoving = false;
        }
    }

    private Vector2 ApplyBounds(Vector2 position)
    {
        float cameraHeight = camera.orthographicSize * 2f;
        float cameraWidth = camera.aspect * cameraHeight;

        position.x = Mathf.Clamp(position.x, bounds.min.x + cameraWidth / 2f, bounds.max.x - cameraWidth / 2f);
        position.y = Mathf.Clamp(position.y, bounds.min.y + cameraHeight / 2f, bounds.max.y - cameraHeight / 2f);

        return position;
    }
}
