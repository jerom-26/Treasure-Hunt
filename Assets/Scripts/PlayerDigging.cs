using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDigging : MonoBehaviour
{
    private bool isPlayerInDigZone = false;

    void Update()
    {
        if (isPlayerInDigZone && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Player is digging!");
            Dig();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInDigZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInDigZone = false;
        }
    }

    void Dig()
    {
        Debug.Log("Treasure dug!");
        gameObject.SetActive(false);

        GameObject clue = GameObject.Find("ClueText");
        if (clue != null)
        {
            clue.SetActive(false);
        }
    }
}
