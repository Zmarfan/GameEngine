using GameEngine.engine.core;
using GameEngine.engine.data;
using GameEngine.engine.window;
using Color = GameEngine.engine.data.Color;

namespace GameEngine.engine.camera; 

public class Camera {
    public static Camera Main { get; private set; } = null!;

    public Vector2 Position {
        get => _position;
        set {
            SetDirty();
            _position = value;
        }
    }
    public float Size {
        get => _size;
        set {
            SetDirty();
            _size = Math.Max(value, 0.01f);
        }
    }
    public Color defaultDrawColor = Color.BLACK;

    public TransformationMatrix WorldToScreenMatrix {
        get {
            if (_oldResolution != _settings.resolution) {
                SetDirty();
            }
            if (!_isWorldDirty) {
                return _worldToScreenMatrix;
            }

            _isWorldDirty = false;
            _worldToScreenMatrix = TransformationMatrix.CreateWorldToScreenMatrix(
                new Vector2(_oldResolution.x / 2f - _position.x, _oldResolution.y / 2f + _position.y),
                new Vector2(1 / _size, 1 / _size)
            );
            return _worldToScreenMatrix;
        }
    }

    public TransformationMatrix ScreenToWorldMatrix {
        get {
            if (_oldResolution != _settings.resolution) {
                SetDirty();
            }
            if (!_isWorldInverseDirty) {
                return _screenToWorldMatrix;
            }

            _isWorldInverseDirty = false;
            _screenToWorldMatrix = WorldToScreenMatrix.Inverse();
            return _screenToWorldMatrix;
        }
    }
    
    public TransformationMatrix UiToScreenMatrix {
        get {
            if (_oldResolution != _settings.resolution) {
                SetDirty();
            }
            if (!_isUiDirty) {
                return _uiToWorldMatrix;
            }

            _isUiDirty = false;
            _uiToWorldMatrix = TransformationMatrix.CreateWorldToScreenMatrix(
                new Vector2(_oldResolution.x / 2f, _oldResolution.y / 2f),
                Vector2.One()
            );
            return _uiToWorldMatrix;
        }
    }
    
    public TransformationMatrix ScreenToUiMatrix {
        get {
            if (_oldResolution != _settings.resolution) {
                SetDirty();
            }
            if (!_isUiInverseDirty) {
                return _screenToUiMatrix;
            }

            _isUiInverseDirty = false;
            _screenToUiMatrix = UiToScreenMatrix.Inverse();
            return _screenToUiMatrix;
        }
    }

    private Vector2 _position = Vector2.Zero();
    private float _size = 1;
    private TransformationMatrix _worldToScreenMatrix = TransformationMatrix.Identity();
    private TransformationMatrix _screenToWorldMatrix = TransformationMatrix.Identity();
    private TransformationMatrix _uiToWorldMatrix = TransformationMatrix.Identity();
    private TransformationMatrix _screenToUiMatrix = TransformationMatrix.Identity();
    private bool _isWorldDirty = true;
    private bool _isWorldInverseDirty = true;
    private bool _isUiDirty = true;
    private bool _isUiInverseDirty = true;
    private readonly WindowSettings _settings;
    private Vector2Int _oldResolution;

    private Camera(WindowSettings settings) {
        _settings = settings;
        SetDirty();
    }

    internal static void CreateMainCamera(WindowSettings settings) {
        Main = new Camera(settings);
    }

    private void SetDirty() {
        _isWorldDirty = true;
        _isWorldInverseDirty = true;
        _isUiDirty = true;
        _isUiInverseDirty = true;
        _oldResolution = _settings.resolution;
    }
}