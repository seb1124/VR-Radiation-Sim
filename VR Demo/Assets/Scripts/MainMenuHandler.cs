using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuHandler : MonoBehaviour
{

    // Menu Object References
    public GameObject mainMenu;
    public GameObject aboutMenu;
    public GameObject statsMenu;

    // Stats Menu Object References
    public TMP_Text bestTimeText;
    public TMP_Text averageTimeText;
    public TMP_Text gamesPlayedText;


    public void MainToAbout(){
        mainMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    public void AboutToMain(){
        aboutMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void MainToStats(){
        mainMenu.SetActive(false);
        DisplayStatistics();
        statsMenu.SetActive(true);
    }

    public void StatsToMain(){
        statsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void LoadGame(){
        SceneManager.LoadScene("GameScene");
    }

    public void Quit(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void DisplayStatistics(){

        // Retrieve stats
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        float totalTime = PlayerPrefs.GetFloat("TotalTime", 0f);
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);

        // Calculate average
        float averageTime = gamesPlayed > 0 ? totalTime / gamesPlayed : 0f;

        // Format times as minutes:seconds
        string bestTimeFormatted = bestTime == float.MaxValue ? "N/A" : $"{(int)(bestTime / 60):00}:{(bestTime % 60):00.00}";
        string averageTimeFormatted = gamesPlayed > 0 ? $"{(int)(averageTime / 60):00}:{(averageTime % 60):00.00}" : "N/A";

        // Debug
        Debug.Log($"Best Time: {bestTimeFormatted}, Average Time: {averageTimeFormatted}, Games Played: {gamesPlayed}");

        // Display stats
        bestTimeText.text = $"{bestTimeFormatted}";
        averageTimeText.text = $"{averageTimeFormatted}";
        gamesPlayedText.text = $"{gamesPlayed}";

    }

    public void ResetStats(){

        // Delete all & save changes
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Debug
        Debug.Log("All stats have been reset.");

        // Display changes
        DisplayStatistics();

    }
}
