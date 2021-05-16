using UnityEngine;

public class TalkAnimator : MonoBehaviour {

    public Animator characterAnimator;

    public void StartTalkingAnimation() => characterAnimator.Play("Talking");

    public void StopTalkingAnimation() => characterAnimator.Play("Idle");

}
