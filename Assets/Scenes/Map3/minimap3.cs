using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimap3 : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void LateUpdate()
    {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
