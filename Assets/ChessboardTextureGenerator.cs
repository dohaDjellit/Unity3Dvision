using UnityEngine;

public class ChessboardTextureGenerator : MonoBehaviour
{
    void Start()
    {
        GenerateChessboardTexture();
    }
    [Header("Chessboard Settings")]
    public int rows = 8; // Number of rows in the chessboard
    public int columns = 8; // Number of columns in the chessboard
    public int squareSize = 32; // Size of each square in pixels
    public Color color1 = Color.black; // Color 1 (default black)
    public Color color2 = Color.white; // Color 2 (default white)

    [Header("Output")]
    public string textureName = "ChessboardTexture";

    public void GenerateChessboardTexture()
    {
        int width = columns * squareSize;
        int height = rows * squareSize;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Generate the chessboard pattern
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isEven = ((x / squareSize) + (y / squareSize)) % 2 == 0;
                texture.SetPixel(x, y, isEven ? color1 : color2);
            }
        }

        texture.Apply();

        // Save the texture to the object’s material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;
            renderer.material = material;
        }
    }

}
