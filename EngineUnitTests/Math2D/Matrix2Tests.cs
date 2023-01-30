using GameEngine.Math2D;
using System.Security.Cryptography.X509Certificates;

namespace EngineUnitTests.Math2D;

public class Matrix2Tests
{
    [Fact]
    public void TestEquals()
    {
        float[] data = new float[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        Matrix2 matrix1 = new Matrix2(data);
        Matrix2 matrix2 = new Matrix2(data);

        Assert.True(matrix1 == matrix2);
        Assert.False(matrix1 != matrix2);

        for (int i = 0; i < 9; ++i)
        {
            Assert.Equal(data[i], matrix1[i]);
            Assert.Equal(data[i], matrix2[i]);
        }

        float[] data2 = new float[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        for (int i = 0; i < 9; ++i)
        {
            data2[i] = -1;

            Matrix2 matrix3 = new Matrix2(data2);

            Assert.True(matrix1 != matrix3);
            Assert.False(matrix1 == matrix3);

            data2[i] = data[i];
        }
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(2, 0, 2)]
    [InlineData(0, 1, 3)]
    [InlineData(1, 1, 4)]
    [InlineData(2, 1, 5)]
    [InlineData(0, 2, 6)]
    [InlineData(1, 2, 7)]
    [InlineData(2, 2, 8)]
    public void TestIndexOf(int x, int y, int i)
    {
        Assert.Equal(i, Matrix2.IndexOf(x, y));
    }

    [Fact]
    public void TestSubscript()
    {
        float[] productData = new float[9] { 0, 0, 0, 0, 1, 2, 0, 2, 4 };
        float[] sumData = new float[9] { 0, 1, 2, 1, 2, 3, 2, 3, 4 };
        float[] indexData = new float[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        Matrix2 product = new(productData);
        Matrix2 sum = new Matrix2(sumData);
        Matrix2 index = new Matrix2(indexData);

        Vector2 zero = new(0, 0, 0);
        Vector2 one = new(1, 1, 1);
        Vector2 two = new(2, 2, 2);

        Vector2 rowIndexVector = new(0, 1, 2);

        Matrix2 columnIndex = new(two, zero, one, true);
        Matrix2 rowIndex = new(rowIndexVector, rowIndexVector, rowIndexVector, true);

        for (int x = 0; x < 3; ++x)
        {
            for (int y = 0; y < 3; ++y)
            {
                Assert.Equal(x * y, product[x, y]);
                Assert.Equal(x + y, sum[x, y]);
                Assert.Equal(Matrix2.IndexOf(x, y), index[x, y]);
                Assert.Equal(y, columnIndex[x, y]);
                Assert.Equal(x, rowIndex[x, y]);
            }
        }
    }

    [Fact]
    public void TestIdentity()
    {
        float[] data = new float[9] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };

        for (int i = 0; i < 9; ++i)
        {
            float old = data[i];

            data[i] += 0.1f;

            Matrix2 matrix = new(data);

            Assert.False(matrix.IsIdentity());
            Assert.NotEqual(matrix, new Matrix2());

            data[i] = old;
        }

        Matrix2 identity = new(data);

        Assert.True(identity.IsIdentity());

        identity = new();

        Assert.True(identity.IsIdentity());
    }

    [Fact]
    public void TestConstructors()
    {
        float[] data = new float[9] { 1, 0, 0, 0, 1, 0, 4, 5, 6 };

        Vector2 pos = new(4, 5, 6);
        Vector2 right = new(1, 0);
        Vector2 up = new(0, 1);

        Matrix2 matrix1 = new(data);
        Matrix2 matrix2 = new(pos, right, up, true);
        Matrix2 matrix3 = new(pos);
        Matrix2 matrix4 = new(pos, right, up, false);

        Assert.Equal(matrix1, matrix2);
        Assert.Equal(matrix3, matrix4);

        Vector2 scale = new(2, -5);
        Vector2 offset = new(3, 4, 1);
        float[] scaleData = new float[9] { scale.X, 0, 0, 0, scale.Y, 0, offset.X, offset.Y, 1 };

        Matrix2 scaleMatrix1 = new(new Vector2(offset.X, offset.Y, 1), scale);
        Matrix2 scaleMatrix2 = new(scaleData);

        Assert.Equal(scaleMatrix1, scaleMatrix2);

        float angle = MathF.E;
        Vector2 rightRotated = new Vector2(1, 0).Rotate(angle);
        Vector2 upRotated = new Vector2(0, 1).Rotate(angle);

        Matrix2 rotateMatrix1 = new(new(), rightRotated, upRotated);
        Matrix2 rotateMatrix2 = new(new Vector2(), angle);

        Assert.Equal(rotateMatrix1, rotateMatrix2);

        Matrix2 scaleRotateMatrix1 = new(new(), scale, angle);
        Matrix2 scaleRotateMatrix2 = new(new(), scale.X * rightRotated, scale.Y * upRotated);

        Assert.Equal(scaleRotateMatrix1, scaleRotateMatrix2);

        Assert.Equal(new Matrix2(new(), angle), Matrix2.NewRotation(angle));
        Assert.Equal(new Matrix2(offset), Matrix2.NewTranslation(offset));
        Assert.Equal(new Matrix2(new(), scale), Matrix2.NewScale(scale));
    }

    [Fact]
    public void TestProperties()
    {
        Vector2 position = new(2, 3, 1);
        Vector2 scale = new(4, 5);
        float angle = 6;

        Vector2 right = new(1, 0);
        Vector2 up = new(0, 1);
        Vector2 translation = new(0, 0, 1);
        Vector2 regularSize = new(1, 1);

        Vector2 rightRotated = new Vector2(1, 0).Rotate(angle);
        Vector2 upRotated = new Vector2(0, 1).Rotate(angle);

        Matrix2 translateMatrix = Matrix2.NewTranslation(position);
        Matrix2 scaleMatrix = Matrix2.NewScale(scale);
        Matrix2 rotationMatrix = Matrix2.NewRotation(angle);

        Assert.Equal(right, translateMatrix.Right);
        Assert.Equal(up, translateMatrix.Up);
        Assert.Equal(position, translateMatrix.Translation);
        Assert.True(Utils.IsFloatClose(translateMatrix.Rotation, 0));
        Assert.Equal(regularSize, translateMatrix.Scale2);

        Assert.Equal(scale.X * right, scaleMatrix.Right);
        Assert.Equal(scale.Y * up, scaleMatrix.Up);
        Assert.Equal(translation, scaleMatrix.Translation);
        Assert.True(Utils.IsFloatClose(scaleMatrix.Rotation, 0));
        Assert.Equal(scale, scaleMatrix.Scale2);

        Assert.Equal(rightRotated, rotationMatrix.Right);
        Assert.Equal(upRotated, rotationMatrix.Up);
        Assert.Equal(translation, rotationMatrix.Translation);
        Assert.True(Utils.IsFloatClose(rotationMatrix.Rotation, Utils.WrapAngle(angle)));
        Assert.Equal(regularSize, rotationMatrix.Scale2);
    }

    [Fact]
    void TestMultiplication()
    {
        Vector2 position = new(2, 3, 1);
        Vector2 scale = new(4, 5);
        float angle = 6;
        float wrappedAngle = Utils.WrapAngle(angle);

        Matrix2 translateMatrix = Matrix2.NewTranslation(position);
        Matrix2 scaleMatrix = Matrix2.NewScale(scale);
        Matrix2 rotationMatrix = Matrix2.NewRotation(angle);

        Matrix2 ts = translateMatrix * scaleMatrix;
        Matrix2 st = scaleMatrix * translateMatrix;

        Assert.Equal(position.Scale(scale), st.Translation);
        Assert.Equal(scale, st.Scale2);
        Assert.Equal(position, ts.Translation);
        Assert.Equal(scale, ts.Scale2);

        Matrix2 tr = translateMatrix * rotationMatrix;
        Matrix2 rt = rotationMatrix * translateMatrix;

        Assert.Equal(position.Rotate(angle), rt.Translation);
        Assert.True(Utils.IsFloatClose(rt.Rotation, wrappedAngle));
        Assert.Equal(position, tr.Translation);
        Assert.True(Utils.IsFloatClose(tr.Rotation, wrappedAngle));

        Matrix2 sr = scaleMatrix * rotationMatrix;
        Matrix2 rs = rotationMatrix * scaleMatrix;

        Assert.Equal(rs.Translation, sr.Translation);
        Assert.True(Utils.IsFloatClose(rs.Rotation, wrappedAngle));
        Assert.Equal(scale, rs.Scale2);

        Matrix2 trs = translateMatrix * rotationMatrix * scaleMatrix;

        Assert.Equal((rotationMatrix * scaleMatrix) * translateMatrix, rotationMatrix * (scaleMatrix * translateMatrix));
        Assert.Equal(position, trs.Translation);
        Assert.True(Utils.IsFloatClose(trs.Rotation, wrappedAngle, Utils.LooseEpsilonF));
        Assert.Equal(scale, trs.Scale2);

        Assert.Equal(st.Translation, scaleMatrix * position);
        Assert.Equal(rt.Translation, rotationMatrix * position);
        Assert.Equal(position + position - new Vector2(0, 0, 1), translateMatrix * position);
    }

    [Fact]
    void TestInverse()
    {
        Vector2 position = new(2, 3, 1);
        Vector2 scale = new(4, 5);
        float angle = 6;

        Matrix2 translateMatrix = Matrix2.NewTranslation(position);
        Matrix2 scaleMatrix = Matrix2.NewScale(scale);
        Matrix2 rotationMatrix = Matrix2.NewRotation(angle);

        Matrix2 ts = translateMatrix * scaleMatrix;
        Matrix2 st = scaleMatrix * translateMatrix;

        Matrix2 tr = translateMatrix * rotationMatrix;
        Matrix2 rt = rotationMatrix * translateMatrix;

        Matrix2 sr = scaleMatrix * rotationMatrix;
        Matrix2 rs = rotationMatrix * scaleMatrix;

        Matrix2 trs = translateMatrix * rotationMatrix * scaleMatrix;

        Assert.True(Utils.IsFloatClose(translateMatrix.Determinant, 1));
        Assert.True(Utils.IsFloatClose(rotationMatrix.Determinant, 1));
        Assert.True(Utils.IsFloatClose(scaleMatrix.Determinant, 20));

        Assert.True(Utils.IsFloatClose(ts.Determinant, 20));
        Assert.True(Utils.IsFloatClose(st.Determinant, 20));

        Assert.True(Utils.IsFloatClose(tr.Determinant, 1));
        Assert.True(Utils.IsFloatClose(rt.Determinant, 1));

        Assert.True(Utils.IsFloatClose(sr.Determinant, 20));
        Assert.True(Utils.IsFloatClose(rs.Determinant, 20));

        Assert.True(Utils.IsFloatClose(trs.Determinant, 20));

        Assert.True(Utils.IsFloatClose(translateMatrix.Inverse.Determinant, 1));
        Assert.True(Utils.IsFloatClose(rotationMatrix.Inverse.Determinant, 1));
        Assert.True(Utils.IsFloatClose(scaleMatrix.Inverse.Determinant, 1 / 20.0f));

        Assert.True(Utils.IsFloatClose(ts.Inverse.Determinant, 1 / 20.0f));
        Assert.True(Utils.IsFloatClose(st.Inverse.Determinant, 1 / 20.0f));

        Assert.True(Utils.IsFloatClose(tr.Inverse.Determinant, 1));
        Assert.True(Utils.IsFloatClose(rt.Inverse.Determinant, 1));

        Assert.True(Utils.IsFloatClose(sr.Inverse.Determinant, 1 / 20.0f));
        Assert.True(Utils.IsFloatClose(rs.Inverse.Determinant, 1 / 20.0f));

        Assert.True(Utils.IsFloatClose(trs.Inverse.Determinant, 1 / 20.0f));

        Action<Matrix2> testMatrix = (matrix) =>
        {
            Assert.True((matrix * matrix.Inverse).IsIdentity());
            Assert.True((matrix.Inverse * matrix).IsIdentity());
        };

        testMatrix(translateMatrix);
        testMatrix(rotationMatrix);
        testMatrix(scaleMatrix);

        testMatrix(ts);
        testMatrix(st);

        testMatrix(tr);
        testMatrix(rt);

        testMatrix(sr);
        testMatrix(rs);

        testMatrix(trs);
    }

    [Fact]
    public void TestRowAndColumn()
    {
        float[] data = new float[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        Matrix2 matrix = new(data);

        Assert.Equal(new Vector2(0, 1, 2), matrix.Column(0));
        Assert.Equal(new Vector2(3, 4, 5), matrix.Column(1));
        Assert.Equal(new Vector2(6, 7, 8), matrix.Column(2));

        Assert.Equal(new Vector2(0, 3, 6), matrix.Row(0));
        Assert.Equal(new Vector2(1, 4, 7), matrix.Row(1));
        Assert.Equal(new Vector2(2, 5, 8), matrix.Row(2));
    }
}