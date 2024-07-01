using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{   
    public int[] Coord = new int[2];

    [SerializeField] AudioClip coinSound;
    [SerializeField] AudioClip coinSoundMinotaur;
    public float Lifetime;

    private void Start()
    {
        StartCoroutine(SelfDestroyAfterLifetime());
    }

    IEnumerator SelfDestroyAfterLifetime()
    {
        yield return new WaitForSeconds(Lifetime);
        RemoveCoinFromList();
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {   
        if (other.name == "Player")
        {
            SceneManagerMaze.Instance.PlayerScoreUp();
            AudioManager.Instance.PlaySFX(coinSound);
        }
        else
        {
            SceneManagerMaze.Instance.MinotaurScoreUp();
            AudioManager.Instance.PlaySFX(coinSoundMinotaur);
        }

        RemoveCoinFromList();
        Destroy(gameObject);
    }

    private void RemoveCoinFromList()
    {
        for (int i = 0; i < SpawnManager.TakenCells.Count; i++)
        {
            if (SpawnManager.TakenCells[i][0] == Coord[0] && SpawnManager.TakenCells[i][1] == Coord[1])
            {
                SpawnManager.TakenCells.RemoveAt(i);
            }
        }

    }
}
