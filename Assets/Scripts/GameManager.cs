using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { RUNNING, PAUSED, LEVEL_COMPLETED, GAME_OVER }

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Canvas inGameCanvas;
    public GameState currentGameState = GameState.PAUSED;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError( "Duplicated Game Manager", gameObject );
        }
    }

    void Start() {}
    void Update() {
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
}
