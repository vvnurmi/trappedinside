using UnityEngine;
using UnityEngine.Playables;

public class TimelineSuspenderSettings : MonoBehaviour
{
    [Tooltip("Which timeline to suspend when child TimelineSuspender instances are activated.")]
    public PlayableDirector timeline;
}
