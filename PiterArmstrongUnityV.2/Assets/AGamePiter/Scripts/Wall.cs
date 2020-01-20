using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Piter Armstrong

  
    private Animator animatorwall;    //Store a component reference to the attached SpriteRenderer.

    // Start is called before the first frame update
    void Awake()
    {
        //Get a component reference to the SpriteRenderer.
        animatorwall = GetComponent<Animator>();
    }
}
