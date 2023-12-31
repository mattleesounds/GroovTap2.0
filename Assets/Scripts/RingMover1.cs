using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMover : MonoBehaviour
{
    public GameObject ringPrefab;
    public GameObject listenPrefab;
    public GameObject tapPrefab;
    public AudioSource musicSource; // Reference to the music source

    private float spawnDistance = 1000f; // Distance in front of the camera to spawn
    private float speed = 500f; // Speed at which the sprites move towards the camera

    private int spriteCounter = 0;
    private float beatDuration = 1.0f; // Duration of a beat in seconds, should be the same as in SongControl

    public double musicStartTime;

    void Start()
    {
        // Calculate the time it takes for a ring to travel from spawnDistance to the camera
        double travelTime = spawnDistance / speed;



        // Start the sequence to spawn rings immediately
        StartCoroutine(SpawnSpriteSequence());
    }

    IEnumerator StartMusicWithDelay(float delay)
    {
        // Convert the delay from float to double
        double delayInSeconds = delay;

        // Wait for the specified delay before starting the music
        double startTime = AudioSettings.dspTime + delayInSeconds;
        while (AudioSettings.dspTime < startTime)
        {
            yield return null;
        }
    }
    IEnumerator SpawnSpriteSequence()
    {
        //yield return new WaitForSeconds(0.3f);
        bool firstSpriteSpawned = false;

        while (true) // Loop indefinitely
        {
            GameObject prefabToSpawn;
            if (spriteCounter % 8 == 0) // First in sequence
            {
                prefabToSpawn = listenPrefab;
            }
            else if (spriteCounter % 8 == 4) // Fifth in sequence
            {
                prefabToSpawn = tapPrefab;
            }
            else // All others
            {
                prefabToSpawn = ringPrefab;
            }



            // Instantiate the sprite and start its movement
            GameObject spriteInstance = InstantiateSprite(prefabToSpawn);
            StartCoroutine(MoveSprite(spriteInstance));



            // Increment the counter and reset if it reaches 8
            spriteCounter = (spriteCounter + 1) % 8;

            if (!firstSpriteSpawned)
            {
                firstSpriteSpawned = true;
                musicStartTime = AudioSettings.dspTime + (beatDuration * 2) + 0.3; // Set the music start time when the first sprite is spawned
            }

            // Wait for the next sprite instantiation (adjust the time as needed)
            yield return new WaitForSeconds(beatDuration);

            /* if (firstSpriteSpawned)
            {
                musicStartTime = AudioSettings.dspTime;
                // Do not reset firstSpriteSpawned to false here
            } */
        }
    }
    GameObject InstantiateSprite(GameObject prefab)
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;
        GameObject spriteInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        return spriteInstance;
    }

    IEnumerator MoveSprite(GameObject sprite)
    {
        Vector3 destroyPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane)); // Point just behind the camera

        while (sprite != null)
        {
            // Move the sprite towards the camera
            sprite.transform.position = Vector3.MoveTowards(sprite.transform.position, Camera.main.transform.position, speed * Time.deltaTime);

            // Check if the sprite is past the destroy point, then destroy it
            if (Vector3.Distance(sprite.transform.position, destroyPoint) < 0.1f)
            {
                Destroy(sprite);
            }

            yield return null;
        }
    }
}
