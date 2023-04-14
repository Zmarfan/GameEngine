namespace GameEngine.engine.data; 

public struct TransformationMatrix {
    private float _m00 = 1;
    private float _m10 = 0;
    private float _m20 = 0;
    private float _m01 = 0;
    private float _m11 = 1;
    private float _m21 = 0;
    private float _m02 = 0;
    private float _m12 = 0;
    private float _m22 = 1;

    public TransformationMatrix(float m00, float m10, float m20, float m01, float m11, float m21, float m02, float m12, float m22) {
        _m00 = m00;
        _m10 = m10;
        _m20 = m20;
        _m01 = m01;
        _m11 = m11;
        _m21 = m21;
        _m02 = m02;
        _m12 = m12;
        _m22 = m22;
    }
    
    public static TransformationMatrix Identity() {
        return new TransformationMatrix(1, 0, 0, 0, 1, 0, 0, 0, 1);
    }
    
    private static TransformationMatrix Zero() {
        return new TransformationMatrix(0, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    public static TransformationMatrix CreateWorldToScreenMatrix(Vector2 position, Vector2 scale) {
        return Translate(position) * Scale(scale) * RotateCameraYAxis();
    }
    
    public static TransformationMatrix CreateLocalToParentMatrix(Vector2 position, Rotation rotation, Vector2 scale) {
        return Translate(position) * Rotate(rotation) * Scale(scale);
    }

    public Vector2 GetScale() {
        return new Vector2(
            (float)Math.Sqrt(this[0, 0] * this[0, 0] + this[0, 1] * this[0, 1]),
            (float)Math.Sqrt(this[1, 0] * this[1, 0] + this[1, 1] * this[1, 1])
        );
    }

    public Rotation GetRotation() {
        Rotation rotation = Rotation.FromRadians((float)Math.Acos(this[0, 0] / GetScale().x));
        rotation = Math.Sign(this[0, 1]) == 1 ? Rotation.FromDegrees(360 - rotation.Degree) : rotation;
        return rotation;
    }

    public Vector2 ConvertPoint(Vector2 p) {
        float x = MathF.CloseToIntToInt(this[0, 0] * p.x + this[1, 0] * p.y + this[2, 0]);
        float y = MathF.CloseToIntToInt(this[0, 1] * p.x + this[1, 1] * p.y + this[2, 1]);
        return new Vector2(x, y);
    }
    
    public Vector2 ConvertVector(Vector2 v) {
        float x = MathF.CloseToIntToInt(this[0, 0] * v.x + this[1, 0] * v.y);
        float y = MathF.CloseToIntToInt(this[0, 1] * v.x + this[1, 1] * v.y);
        return new Vector2(x, y);
    }

    public TransformationMatrix Inverse() {
        float determinant = this[0, 0] * (this[1, 1] * this[2, 2] - this[2, 1] * this[1, 2])
                            - this[1, 0] * (this[0, 1] * this[2, 2] - this[2, 1] * this[0, 2])
                            - this[2, 0] * (this[0, 1] * this[1, 2] - this[1, 1] * this[0, 2]);
        
        if (determinant == 0) {
            return Zero();
        }

        return 1 / determinant * new TransformationMatrix(
            this[1, 1] * this[2, 2] - this[2, 1] * this[1, 2],
            this[2, 0] * this[1, 2] - this[1, 0] * this[2, 2],
            this[1, 0] * this[2, 1] - this[2, 0] * this[1, 1],
            this[2, 1] * this[0, 2] - this[0, 1] * this[2, 2],
            this[0, 0] * this[2, 2] - this[2, 0] * this[0, 2],
            this[2, 0] * this[0, 1] - this[0, 0] * this[2, 1],
            this[0, 1] * this[1, 2] - this[1, 1] * this[0, 2],
            this[1, 0] * this[0, 2] - this[0, 0] * this[1, 2],
            this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1]
        );
    }

    private static TransformationMatrix Translate(Vector2 position) {
        return new TransformationMatrix(1, 0, position.x, 0, 1, position.y, 0, 0, 1);
    }
    
    private static TransformationMatrix Rotate(Rotation rotation) {
        float cos = MathF.CloseToIntToInt((float)Math.Cos(rotation.Radians));
        float sin = MathF.CloseToIntToInt((float)Math.Sin(rotation.Radians));
        return new TransformationMatrix(cos, sin, 0, -sin, cos, 0, 0, 0, 1);
    }
    
    private static TransformationMatrix RotateCameraYAxis() {
        float cos = MathF.CloseToIntToInt((float)Math.Cos(Rotation.FromDegrees(180).Radians));
        float sin = MathF.CloseToIntToInt((float)Math.Sin(Rotation.FromDegrees(180).Radians));
        return new TransformationMatrix(1, 0, 0, -sin, cos, 0, 0, 0, 1);
    }

    private static TransformationMatrix Scale(Vector2 scale) {
        return new TransformationMatrix(scale.x, 0, 0, 0, scale.y, 0, 0, 0, 1);
    }
    
    public static TransformationMatrix operator *(TransformationMatrix a, TransformationMatrix b) {
        TransformationMatrix matrix = new();
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                matrix[x, y] = MathF.CloseToIntToInt(a[0, y] * b[x, 0] + a[1, y] * b[x, 1] + a[2, y] * b[x, 2]);
            }
        }

        return matrix;
    }
    
    public static TransformationMatrix operator *(float value, TransformationMatrix matrix) {
        TransformationMatrix newMatrix = new();
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                newMatrix[x, y] = MathF.CloseToIntToInt(matrix[x, y] * value);
            }
        }

        return newMatrix;
    }

    private float this[int x, int y] {
        get {
            return x switch {
                0 when y == 0 => _m00,
                1 when y == 0 => _m10,
                2 when y == 0 => _m20,
                0 when y == 1 => _m01,
                1 when y == 1 => _m11,
                2 when y == 1 => _m21,
                0 when y == 2 => _m02,
                1 when y == 2 => _m12,
                2 when y == 2 => _m22,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set {
            switch (x) {
                case 0 when y == 0: _m00 = value; break;
                case 1 when y == 0: _m10 = value; break;
                case 2 when y == 0: _m20 = value; break;
                case 0 when y == 1: _m01 = value; break;
                case 1 when y == 1: _m11 = value; break;
                case 2 when y == 1: _m21 = value; break;
                case 0 when y == 2: _m02 = value; break;
                case 1 when y == 2: _m12 = value; break;
                case 2 when y == 2: _m22 = value; break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    
    public override string ToString() {
        return $"{this[0, 0]}, {this[1, 0]}, {this[2, 0]}\n{this[0, 1]}, {this[1, 1]}, {this[2, 1]}\n{this[0, 2]}, {this[1, 2]}, {this[2, 2]}";
    }
}