using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovementManagement : MonoBehaviour
{
    public GameObject activeBlockPrefab;
    public float newBlockWaitTime;

	// Use this for initialization
	void Start ()
    {
        EventList.BlockHit += BlockHitHandler;
	}

    private void BlockHitHandler(GameObject block, GameObject puck)
    {
        StartCoroutine(WaitAndInstantiate());
        //Instantiate(activeBlockPrefab, activeBlockPrefab.transform.position, Quaternion.identity, this.transform);
        //GameObject[] blockObjs = GameObject.FindGameObjectsWithTag(Constants.BLOCK_TAG);
        //if (blockObjs.Length > 1)
        //{
        //    int randIdx = UnityEngine.Random.Range(0, blockObjs.Length);
        //    while (blockObjs[randIdx].gameObject == block)
        //    {
        //        randIdx = UnityEngine.Random.Range(0, blockObjs.Length);
        //    }

        //    GameObject randomBlock = blockObjs[randIdx].gameObject;
        //    GameObject newActiveBlock = Instantiate<GameObject>(activeBlockPrefab, randomBlock.transform.position, Quaternion.identity, this.transform);
        //    Destroy(randomBlock);
        //}
    }

    private IEnumerator WaitAndInstantiate()
    {
        yield return new WaitForSeconds(newBlockWaitTime);
        Instantiate(activeBlockPrefab, activeBlockPrefab.transform.position, Quaternion.identity, this.transform);
    }
}
