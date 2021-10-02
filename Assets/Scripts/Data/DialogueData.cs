using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public enum Advisor
    {
        Military,
        Economy,
        Culture,
        Anime
    };

    [TextArea(3, 10)]
    public string[] sentences;
    public Advisor advisor;
}
