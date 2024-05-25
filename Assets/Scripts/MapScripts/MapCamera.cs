using UnityEngine;
using UnityEngine.EventSystems;

public class MapCamera : MonoBehaviour
{
    private Vector2 _prevPosition;
    private Transform _transform;

    public Camera camera;
    public Bounds bounds;
    private Vector2 deltaV;
    private Vector3 targetPosition;
    private bool isMoving;
    public float smoothSpeed = 0.125f; 

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

        if (isMoving)
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
                deltaV = Vector2.zero;
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
                SetTargetPosition(deltaV.x / 30, deltaV.y / 30);
            }
        }
        else
        {
            deltaV -= deltaV * Time.deltaTime * 10;
            SetTargetPosition(deltaV.x / 30, deltaV.y / 30);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            deltaV = Vector2.zero;
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
            SetTargetPosition(deltaV.x / 30, deltaV.y / 30);
        }
        else
        {
            deltaV -= deltaV * (Time.deltaTime * 10);
            SetTargetPosition(deltaV.x / 30, deltaV.y / 30);
        }
    }

    private void MoveCamera(Vector2 prevPosition, Vector2 curPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;

        Vector3 move = camera.ScreenToWorldPoint(prevPosition) - camera.ScreenToWorldPoint(curPosition);
        SetPosition(_transform.position + move);
    }

    public void SetPosition(Vector2 position)
    {
        Vector2 validatedPosition = ApplyBounds(position);
        targetPosition = new Vector3(validatedPosition.x, validatedPosition.y, _transform.position.z);
        isMoving = true;
    }

    private void SetTargetPosition(float x, float y)
    {
        Vector3 newTargetPosition = new Vector3(_transform.position.x + x, _transform.position.y + y, _transform.position.z);
        targetPosition = ApplyBounds(new Vector2(newTargetPosition.x, newTargetPosition.y));
        isMoving = true;
    }

    private void SmoothMove()
    {
        _transform.position = Vector3.Lerp(_transform.position, targetPosition, smoothSpeed * Time.deltaTime * 10f);
        if (Vector3.Distance(_transform.position, targetPosition) < 0.01f)
        {
            _transform.position = targetPosition;
            isMoving = false;
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
