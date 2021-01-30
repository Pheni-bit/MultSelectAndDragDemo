using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTick : MonoBehaviour
{
    // Start is called before the first frame update
    public static Action InGameTick;
    public float tickTime = 5;

    private void Awake()
    {
        StartCoroutine(StartResourceTickRoutine());
    }
    IEnumerator StartResourceTickRoutine()
    {
        yield return new WaitForSeconds(tickTime);
        InGameTick();
        StartCoroutine(StartResourceTickRoutine());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public interface ITickable
{
    void Tick();
}
