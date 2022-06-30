using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    bool shake = true;
    void Start()
    {
        
    }

    void Update()
    {
        if(shake == true)
        {
            Debug.Log("Prout");
            shake = false;
            StartCoroutine(Shaking(20));
        }
    }
    public IEnumerator Shaking(float duration)
    {
        Debug.Log("Shake");
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPosition + Random.insideUnitSphere;
            yield return null;
        }

        transform.position = startPosition;
    }
}
