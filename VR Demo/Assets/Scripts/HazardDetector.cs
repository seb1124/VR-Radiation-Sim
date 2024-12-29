using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class HazardDetector : MonoBehaviour
{
    // Assign the two GameObjects in the Inspector
    public GameObject hazardCone;

    // Set the threshold distance in meters
    public float detectionRadius = 1.0f;    // 1 by default
    private GameObject radioactiveObject;   // Dynamically assigned to first object found with "Radioactive tag"

    // Set "Radioactive" tag to random object with "Crate" tag
    private string groupTag = "Crate";
    private string newTag = "Radioactive";

    // UI Components
    public GameObject uiText;
    public float duration;
    public CanvasGroup uiCanvasGroup; 

    public GameObject successMsg;
    public GameObject failureMsg;

    public GameObject reportMsg;

    public GameObject timeMsg;

    public float menuDisplayTime;

    // Audio Components
    public AudioSource warningSound;
    public AudioClip warningClip;

    private bool isDetected = false;

    // Timer
    public TMP_Text timerText; // display time on UI
    private float elapsedTime;
    private bool isTimerRunning;


    void Awake()
    {
        //================================ Randomly assign tag========================================

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

        //Debug.Log($"Random object '{randomObject.name}' has been assigned the tag '{newTag}'");
        Debug.Log($"Random object '{randomObject.name}' has been assigned the tag '{randomObject.tag}'");


        //================================= Get reference to radioactive object =======================

        // Find the radioactive object by tag
        GameObject radioactiveObjectTag = GameObject.FindWithTag("Radioactive");
        if (radioactiveObjectTag == null)
        {
            Debug.LogError("No GameObject with the 'Radioactive' tag found in the scene.");
            return;
        }
        radioactiveObject = radioactiveObjectTag;

        // Get UI Canvas Group
        uiCanvasGroup = uiText.GetComponent<CanvasGroup>();

        //====== Start Timer ======
        StartTimer();
    }

    void Update()
    {
        // Ensure the hazard cone reference is updated dynamically
        if (hazardCone == null)
        {
            // Find the hazard cone in the scene, or handle appropriately
            hazardCone = GameObject.FindWithTag("HazardCone"); // Use tags for consistency
            if (hazardCone == null)
            {
                Debug.LogWarning("Hazard cone reference is missing and cannot be found!");
                return;
            }
        }

        if (radioactiveObject == null)
        {
            Debug.LogWarning("Radioactive object reference is missing!");
            return;
        }

        float distance = Vector3.Distance(hazardCone.transform.position, radioactiveObject.transform.position);

        if(isTimerRunning){
            elapsedTime += Time.deltaTime; //increment timer
        }

        if (distance <= detectionRadius && !isDetected)
        {
            Debug.Log("Hazard Cone successfully placed near radioactive object");
            isDetected = true;
            StopTimer();

            // Update PlayerPrefs with elapsed time
            SaveTime(elapsedTime);

            StartCoroutine(ShowTextForDuration(duration));
        }
    }

    private IEnumerator ShowTextForDuration(float duration){

        if(uiText != null){

            if(!warningSound.isPlaying){
                warningSound.clip = warningClip;
                warningSound.Play();
            }
            
            uiText.SetActive(true); // enable msg
            yield return StartCoroutine(FadeTextToFullAlpha(0f, 1f, 1f)); // Fade in over 1 second

            yield return new WaitForSeconds(duration);

            yield return StartCoroutine(FadeTextToFullAlpha(1f, 0f, 1f)); // Fade out over 1 second
            uiText.SetActive(false);    // disable msg

            //isDetected = false; <--Removed for now
        }
    }

    private IEnumerator FadeTextToFullAlpha(float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0f;

        // Set the initial alpha value
        uiCanvasGroup.alpha = startAlpha;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            yield return null; // Wait until the next frame
        }

        // Ensure the final alpha is exactly the target value
        uiCanvasGroup.alpha = endAlpha;
    }

    private void StartTimer(){
        isTimerRunning = true;
    }

    private void StopTimer(){
        isTimerRunning = false;
    }

    public void UpdateTimerUI(){
        if (timerText != null)
        {
            // Format the time as minutes:seconds
            timerText.text = $"{(int)(elapsedTime / 60):00}:{(elapsedTime % 60):00.00}";
        }
    }

    public void Report(){

        if(isDetected){
            reportMsg.SetActive(false);
            UpdateTimerUI();
            timeMsg.SetActive(true);
            StartCoroutine(ShowTimeElapsedTemporarily());
        }
        else{
            reportMsg.SetActive(false);
            failureMsg.SetActive(true);
            // Start the coroutine to handle the timing
            StartCoroutine(ShowFailureTemporarily());
        }
    }

    private IEnumerator ShowFailureTemporarily()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(menuDisplayTime);
        
        // Switch back to report message
        failureMsg.SetActive(false);
        reportMsg.SetActive(true);
    }

    private IEnumerator ShowTimeElapsedTemporarily()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(menuDisplayTime);
        
        // Switch back to report message
        timeMsg.SetActive(false);
        successMsg.SetActive(true);
    }

    private void SaveTime(float time){

        // Retrieve existing data
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        float totalTime = PlayerPrefs.GetFloat("TotalTime", 0f);
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);

        // Update stats
        if(time < bestTime){
            bestTime = time;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        totalTime += time;
        gamesPlayed++;

        PlayerPrefs.SetFloat("TotalTime", totalTime);
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed);

        // Save changes
        PlayerPrefs.Save();

        Debug.Log($"Time saved! Best: {bestTime}, Average: {totalTime / gamesPlayed}, Games Played: {gamesPlayed}");
    }



    /* IEnumerator WaitForTagAssignment()
    {
        // Wait until the radioactive object is assigned the new tag
        while (true)
        {
            GameObject radioactiveObjectTag = GameObject.FindWithTag("Radioactive");
            if (radioactiveObjectTag != null)
            {
                radioactiveObject = radioactiveObjectTag;
                Debug.Log("Radioactive object found: " + radioactiveObject.name);
                yield break; // Exit the coroutine when found
            }

            yield return null; // Wait for a frame before checking again
        }
    }
    */
}
