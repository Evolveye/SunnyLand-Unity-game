using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void Awake() {
        Button level2Btn = GameObject.Find( "Level2Btn" ).GetComponent<Button>();

        if (level2Btn) {
            int level2HighScore = PlayerPrefs.GetInt( GameOptions.HIGH_SCORE_LEVEL1 );

            level2Btn.interactable = level2HighScore != 0;
        }
    }

    public void OnLevel1ButtonClick() {
        SceneManager.LoadSceneAsync( "Level1" );
    }

    public void OnLevel2ButtonClick() {
        SceneManager.LoadSceneAsync( "Level2" );
    }

    public void OnClearDataButtonClick() {
        PlayerPrefs.SetInt( GameOptions.HIGH_SCORE_LEVEL1, 0 );
        PlayerPrefs.SetInt( GameOptions.HIGH_SCORE_LEVEL2, 0 );

        SceneManager.LoadSceneAsync( "MainMenu" );
    }

    public void OnExitButtonClick() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
