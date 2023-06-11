using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public enum GameState { RUNNING, PAUSED, LEVEL_COMPLETED, GAME_OVER, OPTIONS }
public class GameOptions {
    public static readonly string HIGH_SCORE_LEVEL1 = "HighScoreLevel1";
    public static readonly string HIGH_SCORE_LEVEL2 = "HighScoreLevel2";
    public static readonly string CHEATMODE = "Cheatmode";
}

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text killedEnemiesCountText;
    public TMP_Text gameTime;
    public TMP_Text graphicQualityText;
    public Image firstHeart;
    public Image[] keysTab;

    private float timer = 0;
    private int keysFound = 0;
    private int livesCount = 0;
    private List<Image> hearts = new();
    private int score = 0;
    private int killedEnemiesCount = 0;
    private bool cheatmode = false;

    public Canvas levelCompletedCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas optionsCanvas;
    public Canvas inGameCanvas;
    public GameState currentGameState;

    void Start() { }
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError( "Duplicated Game Manager", gameObject );
        }

        Resume();

        if (!PlayerPrefs.HasKey( GameOptions.HIGH_SCORE_LEVEL1 )) PlayerPrefs.SetInt( GameOptions.HIGH_SCORE_LEVEL1, 0 );

        scoreText.text = "0";
        killedEnemiesCountText.text = "0";
        graphicQualityText.text = QualitySettings.names[ QualitySettings.GetQualityLevel() ];

        firstHeart.gameObject.SetActive( false );
        for (int i = 0; i < keysTab.Length; i++) keysTab[ i ].color = Color.gray;
    }
    void Update() {
        timer += Time.deltaTime;

        var time = Mathf.FloorToInt( timer );
        gameTime.text = string.Format( "{0:00}:{1:00}", Mathf.FloorToInt( time / 60 ) % 60, time % 60 );

        if (Input.GetKeyDown( KeyCode.Escape )) {
            if (currentGameState == GameState.PAUSED) Resume();
            else Pause();
        }

        int heartsDiff = livesCount - hearts.Count();

        if (heartsDiff > 0) {
            int additionalShift = hearts.Count;

            for (int i = 0; i < heartsDiff; i++) {
                Vector3 position = new( firstHeart.transform.position.x + (additionalShift + i) * 100, 60, 0 );

                var heart = Instantiate( firstHeart, position, Quaternion.identity );
                heart.transform.SetParent( inGameCanvas.transform );
                heart.gameObject.SetActive( true );

                hearts.Add( heart );
            }
        } else if (heartsDiff < 0) {
            for (int i = 0; i < -heartsDiff; i++) {
                if (hearts.Count == 0) {
                    livesCount = 0;
                    break;
                };

                Destroy( hearts[ hearts.Count - 1 ] );
                hearts.RemoveAt( hearts.Count - 1 );
            }
        }
    }

    private void SetCanvasActivity( Canvas canvas, bool active ) {
        canvas.gameObject.SetActive( active );
    }

    private void SetGameState( GameState newGameState ) {
        string activeSceneName = SceneManager.GetActiveScene().name;
        currentGameState = newGameState;

        if (newGameState == GameState.RUNNING) Time.timeScale = 1;
        else Time.timeScale = 0;

        if (newGameState == GameState.LEVEL_COMPLETED) {
            string highScoreOptionsKey;

            if (activeSceneName == "Level1") highScoreOptionsKey = GameOptions.HIGH_SCORE_LEVEL1;
            else if (activeSceneName == "Level2") highScoreOptionsKey = GameOptions.HIGH_SCORE_LEVEL2;
            else throw new Exception( "Unknown level" );

            int highScore = PlayerPrefs.GetInt( highScoreOptionsKey );

            Debug.Log( $"Level completed highScore={highScore}; score={score}" );
            if (highScore < score) {
                highScore = score;
                Debug.Log( $"pre-save highScore={highScore}" );
                PlayerPrefs.SetInt( highScoreOptionsKey, highScore );
                Debug.Log( $"post-save highScore={PlayerPrefs.GetInt( highScoreOptionsKey )}" );

                finalScoreText.text = $"New high score! {highScore}";
            } else {
                finalScoreText.text = $"Final score is {score}, high score is {highScore}";
            }
        } else if (newGameState == GameState.GAME_OVER) {
            ExitToMainMenu();
        }

        SetCanvasActivity( pauseMenuCanvas, newGameState == GameState.PAUSED );
        SetCanvasActivity( optionsCanvas, newGameState == GameState.OPTIONS );
        SetCanvasActivity( levelCompletedCanvas, newGameState == GameState.LEVEL_COMPLETED );
    }

    public static void Pause() {
        instance.SetGameState( GameState.PAUSED );
    }
    public static void Resume() {
        instance.SetGameState( GameState.RUNNING );
    }
    public static void Restart() {
        SceneManager.LoadSceneAsync( SceneManager.GetActiveScene().name );
    }
    public static void ExitToMainMenu() {
        SceneManager.LoadSceneAsync( "MainMenu" );
    }
    public static void OpenOptions() {
        instance.SetGameState( GameState.OPTIONS );
    }
    public static void CompleteLevel() {
        instance.SetGameState( GameState.LEVEL_COMPLETED );
    }
    public static void FinishGame() {
        instance.SetGameState( GameState.GAME_OVER );
    }
    public static bool CheckNotRunning() {
        return instance.currentGameState != GameState.RUNNING;
    }

    public static bool CheckCheatmode() {
        return instance.cheatmode;
    }
    public static void SetCheatmode( bool active ) {
        if (instance) instance.cheatmode = active;
    }

    public static void SetPoints( int amount ) {
        instance.score = amount;
    }
    public static void AddPoints( int points ) {
        instance.score += points;
        instance.scoreText.text = instance.score.ToString();
    }
    public static int GetScore() {
        return instance.score;
    }

    public static void AddKilledEnemies( int count ) {
        instance.killedEnemiesCount += count;
        instance.killedEnemiesCountText.text = instance.killedEnemiesCount.ToString();
    }
    public static int GetKilledEnemiesCount() {
        return instance.score;
    }

    public static void SetKeys( int amount ) {
        instance.keysFound = amount;
    }
    public static void AddKey() => AddKey( Array.FindIndex( instance.keysTab, k => k.color == Color.gray ) );
    public static void AddKey( int id ) {
        instance.keysFound++;
        instance.keysTab[ id ].color = Color.white;
    }
    public static bool FoundAllKeys() {
        return instance.keysFound == instance.keysTab.Length;
    }
    public static int GetFoundKeysCount() {
        return instance.keysFound;
    }

    public static void SetHelth( int count ) {
        instance.livesCount = count;
    }
    public static void AddHealth( int count ) {
        instance.livesCount += count;
    }

    public static void IncreaseGraphicQuality() {
        QualitySettings.IncreaseLevel();
        instance.graphicQualityText.text = QualitySettings.names[ QualitySettings.GetQualityLevel() ];
    }
    public static void DecreaseGraphicQuality() {
        QualitySettings.DecreaseLevel();
        instance.graphicQualityText.text = QualitySettings.names[ QualitySettings.GetQualityLevel() ];
    }

    public static void SetVolume( float volume ) {
        AudioListener.volume = volume;
    }
}
