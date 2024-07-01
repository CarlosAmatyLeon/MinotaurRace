using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Linq;
using System.Drawing;

public class SpawnManager : MonoBehaviour
{
    public static List<int[]> TakenCells = new List<int[]>();

    [SerializeField] float Lifetime;
    [SerializeField] float Interval;    
    [SerializeField] GameObject Coin;

    
    public void StartGeneratingCoins()
    {
        StartCoroutine(GenerateRandomCoin());
    }

    IEnumerator GenerateRandomCoin()
    {   
        while (SceneManagerMaze.Instance.GameStarted)
        {
            int[] coord = GetRandomOpenPosition();
            Vector3 position = new Vector3(coord[0] * 5 + 2.5f, 0.5f, coord[1] * 5 + 2.5f);
            GameObject newCoin = Instantiate(Coin, position, Quaternion.identity);
            newCoin.GetComponent<CoinController>().Coord = coord;
            newCoin.GetComponent<CoinController>().Lifetime = Lifetime;
            
            yield return new WaitForSeconds(Interval);
        }
    }

    int[] GetRandomOpenPosition()
    {
        int[] coord;
        
        do
        {
            coord = new int[2] { Random.Range(0, SceneManagerMaze.Instance.mazeSize - 1), Random.Range(0, SceneManagerMaze.Instance.mazeSize - 1) };
        }
        while (TakenCells.Any(array => array.SequenceEqual(coord)));

        TakenCells.Add(coord);

        return coord;
    }

    public void SetLifeTime(float lifetime)
    {
        Lifetime = lifetime;
    }

    public void SetInterval(float interval)
    {
        Interval = interval;
    }
}
