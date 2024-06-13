using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class CoinManager : MonoBehaviour
{
    public float interval;
    public float lifetime;
    public int quantity;
    public int xSize;
    public int zSize;
    public GameObject coin;

    private void Start()
    {
        //TODO change the start of this function to be called on Maze manager after first maze is created
        StartGeneratingCoins();
    }

    void StartGeneratingCoins()
    {
        StartCoroutine(GenerateRandomCoin());
    }

    IEnumerator GenerateRandomCoin()
    {
        int count = 0;
        while (count < quantity)
        {
            Vector2 gridPosition = new Vector2(Random.Range(0, xSize - 1), Random.Range(0,zSize - 1));
            Vector3 position = new Vector3(gridPosition.x * 5 + 2.5f, 0.5f, gridPosition.y * 5 + 2.5f);
            GameObject newObject = Instantiate(coin, position, Quaternion.identity);
            count++;
            yield return new WaitForSeconds(interval);
        }
    }
}
