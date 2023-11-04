using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMover : MonoBehaviour
{
    public GameObject ringPrefab;
    public GameObject listenPrefab;
    public GameObject tapPrefab;

    private float spawnDistance = 1000f; // Distance in front of the camera to spawn
    private float speed = 500f; // Speed at which the sprites move towards the camera


    private int spriteCounter = 0;

    void Start()
    {
        // Start the sequence
        StartCoroutine(SpawnSpriteSequence());
    }

    IEnumerator SpawnSpriteSequence()
    {
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

            // Wait for the next sprite instantiation (adjust the time as needed)
            yield return new WaitForSeconds(1.0f);
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
        while (sprite != null)
        {
            // Move the sprite towards the camera
            sprite.transform.position = Vector3.MoveTowards(sprite.transform.position, Camera.main.transform.position, speed * Time.deltaTime);

            // Check if the sprite is behind the camera, then destroy it
            if (Vector3.Dot(Camera.main.transform.forward, sprite.transform.position - Camera.main.transform.position) < 0)
            {
                Destroy(sprite);
            }

            yield return null;
        }
    }

}
