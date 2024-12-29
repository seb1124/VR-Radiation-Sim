using UnityEngine;

public class RandomTagAssigner : MonoBehaviour
{
    // The tag of the group of objects to select from
    public string groupTag = "Crate";

    // The new tag to apply
    public string newTag = "Radioactive";

    void Start()
    {
        // Find all GameObjects with the specified group tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(groupTag);

        // Check if there are any objects with the specified tag
        if (objectsWithTag.Length == 0)
        {
            Debug.LogWarning("No objects found with the tag: " + groupTag);
            return;
        }

        // Pick a random object from the group
        int randomIndex = Random.Range(0, objectsWithTag.Length);
        GameObject randomObject = objectsWithTag[randomIndex];

        // Assign the new tag to the selected object
        randomObject.tag = newTag;

        Debug.Log($"Random object '{randomObject.name}' has been assigned the tag '{newTag}'");
    }
}
