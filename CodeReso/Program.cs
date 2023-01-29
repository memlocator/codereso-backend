// See https://aka.ms/new-console-template for more information

using GameEngine;
using GameEngine.Math2D;

public class GameInit
{
    static void Main(string[] args)
    {
        Matrix2 a = new(new(), new Vector2(2, 2));
        Matrix2 b = new(new Vector2(1, 1));
        Matrix2 c = new(new(), new Vector2(0, 1), new Vector2(-1, 0));

        Matrix2 ab = a * b;
        Matrix2 ba = b * a;
        Matrix2 cb = c * b;
        Matrix2 bc = b * c;

        Console.WriteLine("Hello, World!");
    }
}