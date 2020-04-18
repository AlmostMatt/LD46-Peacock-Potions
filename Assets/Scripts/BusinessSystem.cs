using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessSystem : MonoBehaviour
{
    private float mPaymentTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Money: " + BusinessState.Money);
        mPaymentTimer -= Time.deltaTime;
        if(mPaymentTimer <= 0)
        {
            BusinessState.Money += 100;
            mPaymentTimer = Random.Range(0.25f, 2);
        }
        
    }
}
