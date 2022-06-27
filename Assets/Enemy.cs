using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float strength;
    public float rotationSpd;
    public List<GameObject> skins = new List<GameObject>();
    void Start()
    {
        if (strength < 1)
		{
            skins[0].SetActive(true);
		}
		else if (strength < 2)
		{
            skins[1].SetActive(true);
        }
        else if (strength < 3)
        {
            skins[2].SetActive(true);
        }
        else
        {
            skins[3].SetActive(true);
        };

        transform.localScale = Vector3.one * strength;
    }

  

    // Update is called once per frame
    void Update()
    {

    }
}
