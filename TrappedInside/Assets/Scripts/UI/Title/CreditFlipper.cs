using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public struct NullableFloat
{
    public float value;
    public bool hasValue;

    public float GetValueOrDefault(float defaultValue) =>
        hasValue ? value : defaultValue;
}

[Serializable]
public struct CreditPhase
{
    [TextArea]
    public string text;

    [Tooltip("Possible override for the default visible seconds.")]
    [UnityEngine.Serialization.FormerlySerializedAs("visibleSeconds")]
    public NullableFloat visibleSecondsOverride;

    [Tooltip("Possible override for the default fade in seconds.")]
    public NullableFloat fadeInSecondsOverride;

    [Tooltip("Possible override for the default fade out seconds.")]
    public NullableFloat fadeOutSecondsOverride;
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class CreditFlipper : MonoBehaviour
{
    [Tooltip("How many seconds to fade each phase in and out, unless overridden in a phase.")]
    [UnityEngine.Serialization.FormerlySerializedAs("fadeSeconds")]
    public float fadeSecondsDefault = 1;

    [Tooltip("How many seconds to keep each phase visible, unless overridden in a phase.")]
    public float visibleSecondsDefault = 2;

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
            var fadeInSeconds = phase.fadeInSecondsOverride.GetValueOrDefault(fadeSecondsDefault);
            var fadeOutSeconds = phase.fadeOutSecondsOverride.GetValueOrDefault(fadeSecondsDefault);
            var visibleSeconds = phase.visibleSecondsOverride.GetValueOrDefault(visibleSecondsDefault);

            {
                // Fade in a new phase.
                textField.text = phase.text;
                var fadeStartTime = Time.time;
                var fadeEndTime = fadeStartTime + fadeInSeconds;
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
            yield return new WaitForSeconds(visibleSeconds);

            {
                // Fade out the old phase.
                var fadeStartTime = Time.time;
                var fadeEndTime = fadeStartTime + fadeOutSeconds;
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

        // Deactivate parent at the end.
        gameObject.SetActive(false);
    }
}
