using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour
{
    public int columns = 6; // Number of columns in the grid
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform inventoryRectTransform;

    void Start()
    {
        AdjustCellSize();
    }

    void AdjustCellSize()
    {
        // Get the width and height of the screen
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the cell size based on the number of columns and spacing
        float cellWidth = (screenWidth - (gridLayoutGroup.spacing.x * (columns - 1))) / columns;
        float cellHeight = cellWidth; // Assuming square cells, you can adjust this if needed

        // Set the cell size of the Grid Layout Group
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        // Adjust the RectTransform to fit the screen size
        inventoryRectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
    }

    void Update()
    {
        // Optional: Adjust cell size on screen resize
        AdjustCellSize();
    }
}