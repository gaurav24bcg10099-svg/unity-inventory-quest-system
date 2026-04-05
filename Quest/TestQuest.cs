using UnityEngine;

public class TestQuest : MonoBehaviour
{
    void Update()
    {
        // Press T → Start Quest
        if (Input.GetKeyDown(KeyCode.T))
        {
            QuestManager.instance.ActivateQuest("Collect Wood");
        }

        // Press Y → Simulate collecting Wood
        if (Input.GetKeyDown(KeyCode.Y))
        {
            QuestManager.instance.ItemCollected("Wood");
        }
    }
}