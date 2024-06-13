using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{   
    public AudioClip coinSound;
    public float lifetime = 5.0f;

    private void Start()
    {
        StartCoroutine(SelfDestroyAfterLifetime());
    }

    IEnumerator SelfDestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        //TODO Handle Score
        AudioManager.Instance.PlaySFX(coinSound);
        Destroy(gameObject);
    }
}
