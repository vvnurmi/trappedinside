using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public struct CreditPhase
{
    [TextArea]
    public string text;
    public float visibleSeconds;
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class CreditFlipper : MonoBehaviour
{
    [Tooltip("How many seconds to fade each phase in and out.")]
    public float fadeSeconds = 1;

    [Tooltip("What to show and for how long.")]
    public CreditPhase[] phases;

    private TextMeshProUGUI textField;

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.text = "";
        textField.alpha = 0;

        StartCoroutine(FlipCredits());
    }

    private IEnumerator FlipCredits()
    {
        foreach (var phase in phases)
        {
            {
                // Fade in a new phase.
                textField.text = phase.text;
                var fadeStartTime = Time.time;
                var fadeEndTime = fadeStartTime + fadeSeconds;
                while (true)
                {
                    var nowTime = Time.time;
                    textField.alpha = Mathf.InverseLerp(fadeStartTime, fadeEndTime, nowTime);
                    if (nowTime >= fadeEndTime) break;
                    yield return null;
                }
                // Ensure the text is visible (in case fade time is 0).
                textField.alpha = 1;
            }

            // Keep the phase visible for some time.
            yield return new WaitForSeconds(phase.visibleSeconds);

            {
                // Fade out the old phase.
                var fadeStartTime = Time.time;
                var fadeEndTime = fadeStartTime + fadeSeconds;
                while (true)
                {
                    var nowTime = Time.time;
                    textField.alpha = 1 - Mathf.InverseLerp(fadeStartTime, fadeEndTime, nowTime);
                    if (nowTime >= fadeEndTime) break;
                    yield return null;
                }
            }
        }

        // Clear the text at the end for safety.
        textField.text = "";
    }
}
