using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string questToGive = "Collect Wood";

    private bool playerInRange = false;
    private bool hasGivenQuest = false;

void Update()
{
    if (playerInRange && Input.GetKeyDown(KeyCode.E))
    {

        if (!hasGivenQuest)
        {
            // Dialogue
            Debug.Log("Hello player! nice to meet you! I have a small task for you!");

            // Give quest
            QuestManager.instance.ActivateQuest(questToGive);
            hasGivenQuest = true;

            Debug.Log("Quest Given: " + questToGive);
        }
        else
        {
            Debug.Log("You already have my task!");
            Debug.Log("finish the task that is given to you , you'll get an new task afterward!");
        }
    }
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}