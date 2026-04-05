using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> quests = new List<Quest>();

    void Awake()
    {
        instance = this;
    }

    // --- ACTIVATE QUEST (NPC will use this later)
    public void ActivateQuest(string questName)
    {
        foreach (Quest quest in quests)
        {
            if (quest.questName == questName)
            {
                quest.isActive = true;
                Debug.Log("Quest Started: " + questName);
            }
        }
    }

    // --- ITEM COLLECTION (we connect inventory later)
public void ItemCollected(string itemID)
{
    foreach (Quest quest in quests)
    {
        if (!quest.isActive || quest.isCompleted) continue;

        if (quest.questType == QuestType.Collect && quest.targetID == itemID)
        {
            quest.currentAmount++;

            // 🔥 Store previous state
            bool wasCompleted = quest.isCompleted;

            quest.CheckCompletion();

            Debug.Log(quest.questName + " Progress: " + quest.currentAmount);

            // 🔥 If just completed NOW
            if (!wasCompleted && quest.isCompleted)
            {
                Debug.Log("Task/Quest Completed!!");
            }
        }
    }
}

    // --- REACH LOCATION
    public void ReachLocation(string locationID)
    {
        foreach (Quest quest in quests)
        {
            if (!quest.isActive || quest.isCompleted) continue;

            if (quest.questType == QuestType.Reach && quest.targetID == locationID)
            {
                quest.currentAmount = 1;
                quest.CheckCompletion();

                Debug.Log("Reached location for: " + quest.questName);
            }
        }
    }

    // --- TALK TO NPC
    public void TalkToNPC(string npcID)
    {
        foreach (Quest quest in quests)
        {
            if (!quest.isActive || quest.isCompleted) continue;

            if (quest.questType == QuestType.Talk && quest.targetID == npcID)
            {
                quest.currentAmount = 1;
                quest.CheckCompletion();

                Debug.Log("Talked to NPC for: " + quest.questName);
            }
        }
    }
    public List<Quest> GetActiveQuests()
{
    List<Quest> activeQuests = new List<Quest>();

    foreach (Quest quest in quests)
    {
        if (quest.isActive)
        {
            activeQuests.Add(quest);
        }
    }

    return activeQuests;
}

public bool IsQuestCompleted(string questName)
{
    foreach (Quest quest in quests)
    {
        if (quest.questName == questName)
        {
            return quest.isCompleted;
        }
    }

    return false;
}
}