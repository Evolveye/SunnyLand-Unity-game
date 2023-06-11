using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour {
    [SerializeField] private int keysToShow = -1;
    [SerializeField] private int keysToHide = -1;

    void Start() { }

    void Update() {
        int foundKeys = GameManager.GetFoundKeysCount();

        if (keysToHide != -1 && foundKeys >= keysToHide) gameObject.SetActive( false );
        if (keysToShow != -1 && foundKeys >= keysToShow) gameObject.SetActive( true );
    }
}
