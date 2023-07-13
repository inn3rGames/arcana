using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = transform.GetChild(0).gameObject;

        Instantiate(prefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
