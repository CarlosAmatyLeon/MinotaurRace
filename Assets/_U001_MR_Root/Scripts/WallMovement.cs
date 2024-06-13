using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public bool wallUp = true;
    public float speed = 5.0f;
    public float posUp = 0f;
    public float posDown = -3.0f;

    public AudioSource wallAudioSource;
    public AudioClip[] stoneGrindSounds;

    float posX;
    float posZ;
    bool moving = false;

    void Start()
    {
        posX = transform.position.x;
        posZ = transform.position.z;
        
    }

    void Update()
    {
        if (wallUp && transform.position.y < posUp)
        {
            if (!moving)
            {
                
                wallAudioSource.clip = stoneGrindSounds[UnityEngine.Random.Range(0, stoneGrindSounds.Length-1)];
                wallAudioSource.enabled = true;
                moving = true;
            }
            float newYpos = transform.position.y + speed * Time.deltaTime;
            if (newYpos > posUp) newYpos = posUp;
            transform.position = new Vector3(posX, newYpos, posZ);
        }
        else if (!wallUp && transform.position.y > posDown)
        {
            float newYpos = transform.position.y - speed * Time.deltaTime;
            if (newYpos < posDown) newYpos = posDown;
            transform.position = new Vector3(posX, newYpos, posZ);
        }
        else
        {
            wallAudioSource.enabled = false;
            moving = false;
        }
    }

}
