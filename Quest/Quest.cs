using UnityEngine;

public enum QuestType
{
    Collect,
    Reach,
    Talk
}

[System.Serializable]
public class Quest
{
    public string questName;
    public QuestType questType;

    public bool isActive;
    public bool isCompleted;

    public int requiredAmount;
    public int currentAmount;

    public string targetID;

    public void CheckCompletion()
    {
        if (currentAmount >= requiredAmount)
        {
            isCompleted = true;
        }
    }
}