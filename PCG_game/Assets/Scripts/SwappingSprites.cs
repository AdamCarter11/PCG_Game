using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SwappingSprites : MonoBehaviour
{
    [SerializeField] GameObject spriteSwapObj;
    SpriteResolver sPr;
    bool switchBool = true;
    private void Start()
    {
        sPr = spriteSwapObj.GetComponent<SpriteResolver>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //swap sprite
            if(switchBool)
            {
                sPr.SetCategoryAndLabel("Body", "Entry");
                switchBool = false;
            }
            else
            {
                sPr.SetCategoryAndLabel("Body", "testPerson Copy");
                switchBool = true;
            }
                
        }
    }
}
