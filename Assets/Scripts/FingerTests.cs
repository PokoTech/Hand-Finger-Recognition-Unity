using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enter the number of fingers you would like to test in untity
/// context menu.
/// Right click for context menu and click 'Test Fingers'.
/// </summary>
public class FingerTests : MonoBehaviour
{

    public OutputImage outputImage;

    [Header("Set time in seconds the test will run for.")]
    public float timerValue = 60;

    [Header("Set how many fingers you would like to test, then right click \n", order = 0)]
    [Space(-10, order = 1)]
    [Header("this context menu and select 'Test Fingers'", order = 2)]
    [Space(10, order = 4)]
    [Range(0, 5, order = 5)]
    public int fingersToTest = 5;


    private int frames = 0;
    private int successfulFrames = 0;

    private bool startTimer = false;
    private float currentTimerValue = 0;

    private SegmentationManager manager;

    private void Update()
    {
        if (!startTimer)
            return;

        frames++;
        if (fingersToTest == manager.GetHullPoints().Count)
            successfulFrames++;

    }

    [ContextMenu("Test Fingers")]
    public void FingerRecognitionTest()
    {   
        successfulFrames = 0;
        frames = 0;
        manager = outputImage.GetSegmentationManagerForTesting();
        StartCoroutine(StartTimer(timerValue));
        startTimer = true;
    }

    private IEnumerator StartTimer(float value)
    {
        currentTimerValue = value;
        while(currentTimerValue > 0)
        {
            Debug.Log(currentTimerValue);
            yield return new WaitForSeconds(1.0f);
            currentTimerValue--;
        }

        Debug.Log("Timer ended.");
        startTimer = false;
        LogResults();
        StopAllCoroutines();
    }


    private void LogResults()
    {
        Debug.Log("--- Results ---");
        Debug.Log("Frames Tested: " + frames);
        Debug.Log("Successfull frames: " + successfulFrames);
        Debug.Log("Percentage Successfull:" + ((float)successfulFrames / (float)frames) * 100);
    }

}
