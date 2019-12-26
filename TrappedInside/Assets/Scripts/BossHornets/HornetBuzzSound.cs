using UnityEngine;

/// <summary>
/// Handles the details of a hornet's buzzing sound.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class HornetBuzzSound : MonoBehaviour, IDying
{
    [Tooltip("How many seconds until the buzz sound has completely faded in at start.")]
    public float fadeInSeconds = 1.0f;

    [Tooltip("How many seconds until the buzz sound has completely faded out at death.")]
    public float fadeOutSeconds = 0.5f;

    [Tooltip("Base pitch multiplier.")]
    public float basePitch = 1.0f;

    [Tooltip("Maximum deviation for the random base pitch multiplier.")]
    public float basePitchRandom = 0.2f;

    [Tooltip("Maximum deviation for the continuous pitch multiplier.")]
    public float pitchFluctuationMaxMultiplier = 0.1f;

    [Tooltip("Maximum deviation for the buzz fade out pitch multiplier. 0 = no pitch bend, 1 = bend all the way.")]
    public float pitchFadeOutMaxMultiplier = 0.2f;

    // Set once, either at start or later.
    private AudioSource audioSource;
    private float startTime;
    private float? deathTime;

    private float GetPitchFadeOut()
    {
        if (!deathTime.HasValue) return 1;

        var timeParam = Mathf.InverseLerp(deathTime.Value, deathTime.Value + fadeOutSeconds, Time.time);
        return Mathf.Lerp(1, 1 - pitchFadeOutMaxMultiplier, timeParam);
    }

    private float GetVolumeFadeOut()
    {
        if (!deathTime.HasValue) return 1;

        var timeParam = Mathf.InverseLerp(deathTime.Value, deathTime.Value + fadeOutSeconds, Time.time);
        return Mathf.Lerp(1, 0, timeParam);
    }

    #region MonoBehaviour overrides

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        var randomSeed = Mathf.InverseLerp(int.MinValue, int.MaxValue, GetHashCode());

        var volumeFadeIn = Mathf.InverseLerp(startTime, startTime + fadeInSeconds, Time.time);
        var finalVolume = volumeFadeIn * GetVolumeFadeOut();

        var randomizedPitch = basePitch + basePitchRandom * randomSeed;
        var pitchFluctuationParam = Mathf.PerlinNoise(Time.time, GetHashCode());
        var pitchFluctuation = randomizedPitch + pitchFluctuationMaxMultiplier * pitchFluctuationParam;
        var finalPitch = pitchFluctuation * GetPitchFadeOut();

        audioSource.volume = finalVolume;
        audioSource.pitch = finalPitch;
    }

    #endregion

    // IDying interface
    public void OnDying()
    {
        deathTime = Time.time;
    }
}
