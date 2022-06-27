using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float strength;
    public List<GameObject> skins = new List<GameObject>();
    void Start()
    {
        if (strength < 3)
		{
            skins[0].SetActive(true);
		}
		else
		{
            skins[1].SetActive(true);
        } ;

        transform.localScale = Vector3.one * strength;
    }

  

    // Update is called once per frame
    void Update()
    {
        
    }
}
