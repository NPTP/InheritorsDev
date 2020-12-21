using UnityEngine;

[CreateAssetMenu]
public class FootstepData : ScriptableObject
{
    [Header("Common length of all sound arrays")]
    public int clipArrayLength = 10;
    [Space]

    [Header("Footstep material sound arrays")]
    public AudioClip[] grass;
    public AudioClip[] gravel;
    public AudioClip[] leaves;
    public AudioClip[] mud;
    public AudioClip[] wood;
    public AudioClip[] sand;
    public AudioClip[] water;
    [Space]

    [Header("Test sound")]
    public AudioClip tester;
}
