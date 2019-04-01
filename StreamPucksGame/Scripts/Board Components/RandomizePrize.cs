using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePrize : MonoBehaviour
{
    public int prizeTier; // don't use 0, it's reserved to mean ignore
    public int[] possiblePointValues;
    public int[] possiblePuckValues;

    void Start()
    {
        EventList.RandomizePrize += RandomizePrizeHandler;
    }

    private void OnDestroy()
    {
        EventList.RandomizePrize -= RandomizePrizeHandler;
    }

    private void RandomizePrizeHandler(GameObject objWithPrize)
    {
        PrizeComponent prize = objWithPrize.GetComponent<PrizeComponent>();
        if (prize != null && prize.Randomize && prize.prizeTier == prizeTier)
        {
            if ((possiblePointValues != null && possiblePointValues.Length > 0) ||
                (possiblePuckValues != null && possiblePuckValues.Length > 0))
            {
                // set random value
                int points = -1;
                int pucks = -1;
                SetRandomValue(possiblePointValues, ref points);
                SetRandomValue(possiblePuckValues, ref pucks);

                // make sure we don't get a result of 0 points and 0 pucks
                while (points == 0 && pucks == 0)
                {
                    if (UnityEngine.Random.Range(0, 1) == 0)
                    {
                        SetRandomValue(possiblePointValues, ref points);
                    }
                    else
                    {
                        SetRandomValue(possiblePuckValues, ref pucks);
                    }
                }

                if (points != -1)
                {
                    prize.Points = points;
                }

                if (pucks != -1)
                {
                    prize.Pucks = pucks;
                }
            }
        }
    }

    private void SetRandomValue(int[] possibleValues, ref int target)
    {
        if (possibleValues == null || possibleValues.Length <= 0)
        {
            return;
        }

        target = possibleValues[UnityEngine.Random.Range(0, possibleValues.Length)];
    }

}
