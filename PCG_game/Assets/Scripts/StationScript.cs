using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationScript : MonoBehaviour
{
    [SerializeField] GameObject goalPrompt;
    [SerializeField] Image goalImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goalPrompt.SetActive(true);
            goalImage.color = Gamemanager.goalColor;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goalPrompt.SetActive(false);
        }
    }

}
