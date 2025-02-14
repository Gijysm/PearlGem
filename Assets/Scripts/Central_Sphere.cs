using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Central_Sphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(20 * Time.deltaTime, 30 * Time.deltaTime, 0);
        
    }
}
