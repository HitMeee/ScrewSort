using UnityEngine;

[System.Serializable]
public class ToolData
{
    [Header("Tool Information")]
    public int id;
    public string nameTools;
    public string titleTools;
    public Sprite imageTools;
    public int price;
    public int currentCount;

    // Constructor
    public ToolData()
    {
        currentCount = 3; // Mặc định 3
    }

    public bool HasCount()
    {
        return currentCount > 0;
    }

    public void UseCount()
    {
        if (currentCount > 0)
            currentCount--;
    }

    public void AddCount(int amount)
    {
        currentCount += amount;
    }
}