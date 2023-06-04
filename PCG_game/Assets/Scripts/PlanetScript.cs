using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlanetScript : MonoBehaviour
{
    public WorldStruct planetData;
    private GameObject displayPanel;
    private TMP_Text displayText;
    private bool isHovering = false;
    private void Start()
    {
        displayPanel = GameObject.FindGameObjectWithTag("DisplayPanel");
        displayText = GameObject.FindGameObjectWithTag("DisplayText").GetComponent<TMP_Text>();
    }
    private void Update()
    {
        if (isHovering)
        {
            // Update the display content based on the struct data of the hovered planet
            displayText.text = "Seed: " + planetData.seed.ToString("F5") + "\n" +
                               "Size: " + planetData.size.ToString("F5") + "\n" +
                               "Creature Amount: " + planetData.creatureAmount.ToString() + "\n" +
                               "Creatures: " + string.Join(", ", planetData.creatures);

            // Position the display panel next to the planet
            //displayPanel.transform.position = transform.position + new Vector3(1.5f, 0f, 0f);
            displayPanel.transform.position = Input.mousePosition;
        }
    }

    public void OnMouseEnter()
    {
        isHovering = true;
        displayPanel.SetActive(true);
    }

    public void OnMouseExit()
    {
        isHovering = false;
        displayPanel.SetActive(false);
    }
}
