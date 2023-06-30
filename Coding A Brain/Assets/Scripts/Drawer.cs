using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Drawer : MonoBehaviour {

    private InputActions _inputActions;

    private bool _isDrawing;

    private Vector2 _xBounds, _yBounds;

    private RawImage _rawImage;
    private Texture2D _texture;

    private Slider _slider;
    private int _drawRadius;

    private Vector2 _mousePosition;

    public void UpdateDrawRadius() {
        _drawRadius = (int) _slider.value;
    }

    private void Awake() {
        _slider = FindObjectOfType<Slider>();

        _texture = new Texture2D(700, 700);

        _rawImage = GetComponent<RawImage>();
        _rawImage.texture = _texture;

        _inputActions = new InputActions();
        _inputActions.Mouse.Enable();

        RectTransform rectTransform = GetComponent<RectTransform>();
        _xBounds = new Vector2(rectTransform.position.x - rectTransform.sizeDelta.x / 2f, rectTransform.position.x + rectTransform.sizeDelta.x / 2f);
        _yBounds = new Vector2(rectTransform.position.y - rectTransform.sizeDelta.y / 2f, rectTransform.position.y + rectTransform.sizeDelta.y / 2f);

        _inputActions.Mouse.MousePressed.started += StartAttemptingToDraw;
        _inputActions.Mouse.MousePressed.canceled += StopDrawing;

        _drawRadius = 10;
    }

    private void StartAttemptingToDraw(InputAction.CallbackContext obj) {
        if (IsMouseWithinBounds()) {
            _isDrawing = true;
        }
    }

    private bool IsMouseWithinBounds() {
        _mousePosition = Mouse.current.position.ReadValue();

        bool withinX = _xBounds.x < _mousePosition.x && _mousePosition.x < _xBounds.y;
        bool withinY = _yBounds.x < _mousePosition.y && _mousePosition.y < _yBounds.y;

        return withinX && withinY;
    }

    private void StopDrawing(InputAction.CallbackContext obj) {
        _isDrawing = false;
    }

    private void Update() {
        if (_isDrawing) {
            if (!IsMouseWithinBounds()) {
                _isDrawing = false;
            }

            for (int j = -_drawRadius; j < _drawRadius; j++) {
                for (int i = -_drawRadius; i < _drawRadius; i++) {
                    if (IsPointDrawable(i, j)) {
                        _texture.SetPixel((int) (i + _mousePosition.x - _xBounds.x), (int) (j + _mousePosition.y - _yBounds.x), Color.black);
                    }
                }
            }
            
            _texture.Apply();
        }
    }

    private bool IsPointDrawable(int i, int j) {
        bool isInCircle = i * i + j * j < _drawRadius * _drawRadius;

        bool isInXBounds = _xBounds.x < _mousePosition.x + i && _mousePosition.x + i < _xBounds.y;
        bool isInYBounds = _yBounds.x < _mousePosition.y + j && _mousePosition.y + j < _yBounds.y;

        return isInXBounds && isInYBounds && isInCircle;
    }

    private void OnDestroy() {
        _inputActions.Mouse.MousePressed.started -= StartAttemptingToDraw;
        _inputActions.Mouse.MousePressed.canceled -= StopDrawing;
    }
}
