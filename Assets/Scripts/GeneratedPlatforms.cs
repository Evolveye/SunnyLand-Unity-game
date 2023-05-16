using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour {
    [SerializeField] private float speed = 0.1f;
    [SerializeField] public GameObject platformPrefab;

    const int PLATFORMS_NUM = 5;

    private float distance = 5f;
    private GameObject[] platforms;
    private Vector3[] positions;

    private void Awake() {
        platforms = new GameObject[ PLATFORMS_NUM ];
        positions = new Vector3[ PLATFORMS_NUM ];

        for (int i = 0; i < platforms.Length; i++) {
            var x = (i - PLATFORMS_NUM / 2) * distance;

            positions[ i ] = new Vector3(
                transform.position.x + x,
                transform.position.y + Mathf.Sin( x ),
                transform.position.z
            );

            platforms[ i ] = Instantiate( platformPrefab, positions[ i ], Quaternion.identity );
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < platforms.Length; i++) {
            var pos = platforms[ i ].transform.position;

            pos.x += speed;
            pos.y = transform.position.y + Mathf.Sin( pos.x );

            platforms[ i ].transform.position = pos;
        }
    }
}
