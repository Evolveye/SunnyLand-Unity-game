using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

class PlatformData {
    public Vector3 initialPosition;

    public PlatformData( float initialX, float initialY, float initialZ ) {
        initialPosition = new( initialX, initialY, initialZ );
    }
}

public class GeneratedPlatforms : MonoBehaviour {
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float distance = 0.0f;
    [SerializeField] private int platformsNum = 1;
    [SerializeField] public GameObject platformPrefab;

    private float spawnDistance = 5f;
    private float spawningTime = 0f;
    private int spawnedPlatforms = 0;
    private GameObject[] platforms;
    private PlatformData[] positions;

    private void Awake() {
        platforms = new GameObject[ platformsNum ];
        positions = new PlatformData[ platformsNum ];

        //for (int i = 0; i < platforms.Length; i++) {
        //    var x = (i - PLATFORMS_NUM / 2) * spawnDistance;

        //    positions[ i ] = new PlatformData(
        //        transform.position.x + x,
        //        transform.position.y + Mathf.Sin( x ),
        //        transform.position.z
        //    );

        //    platforms[ i ] = Instantiate( platformPrefab, positions[ i ].initialPosition, Quaternion.identity );
        //}

        positions[ 0 ] = new PlatformData(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );

        platforms[ 0 ] = Instantiate( platformPrefab, positions[ 0 ].initialPosition, Quaternion.identity );
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (spawnedPlatforms != platformsNum - 1) {
            float distanceBetween = Vector2.Distance( transform.position, platforms[ spawnedPlatforms ].transform.position );

            if (distanceBetween >= distance) {
                spawnedPlatforms++;

                positions[ spawnedPlatforms ] = new PlatformData(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                );

                platforms[ spawnedPlatforms ] = Instantiate( platformPrefab, positions[ spawnedPlatforms ].initialPosition, Quaternion.identity );
            }
        }

        for (int i = 0; i <= spawnedPlatforms; i++) {
            var pos = platforms[ i ].transform.position;

            pos.x += speed;
            pos.y = transform.position.y + Mathf.Sin( pos.x );

            float distanceBetween = Vector2.Distance( pos, positions[ i ].initialPosition );

            if (distanceBetween >= distance * platformsNum) {
                pos = positions[ i ].initialPosition;
            }

            platforms[ i ].transform.position = pos;
        }
    }
}
