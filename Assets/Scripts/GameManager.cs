using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum GameState { RUNNING, PAUSED, LEVEL_COMPLETED, GAME_OVER }

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public TMP_Text scoreText;
    public TMP_Text gameTime;
    public Image[] keysTab;

    private float timer = 0;
    private int keysFound = 0;
    private int score = 0;

    public Canvas inGameCanvas;
    public GameState currentGameState = GameState.PAUSED;

    void Start() { }
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError( "Duplicated Game Manager", gameObject );
        }

        scoreText.text = "0";

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
    }

    private void SetGameState( GameState newGameState ) {
        currentGameState = newGameState;

        if (newGameState == GameState.PAUSED) inGameCanvas.enabled = true;
        else inGameCanvas.enabled = false;
    }
    public static void Pause() {
        instance.SetGameState( GameState.PAUSED );
    }
    public static void Resume() {
        instance.SetGameState( GameState.RUNNING );
    }
    public static void CompleteLevel() {
        instance.SetGameState( GameState.LEVEL_COMPLETED );
    }
    public static void FinishGame() {
        instance.SetGameState( GameState.GAME_OVER );
    }
    public static bool CheckPause() {
        return instance.currentGameState != GameState.RUNNING;
    }

    public static void AddPoints( int points ) {
        instance.score += points;
        instance.scoreText.text = instance.score.ToString();
    }
    public static int GetScore() {
        return instance.score;
    }

    public static void AddKey() => AddKey( Array.FindIndex( instance.keysTab, k => k.color == Color.gray ) );
    public static void AddKey( int id ) {
        Debug.Log( $"Add key {id}" );

        instance.keysFound++;
        instance.keysTab[ id ].color = Color.white;
    }
    public static bool FoundAllKeys() {
        return instance.keysFound == instance.keysTab.Length;
    }
}
