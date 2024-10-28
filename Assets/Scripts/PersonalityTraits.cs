using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChatGPT AI/AI Traits")]
public class PersonalityTraits : ScriptableObject
{
    [Tooltip("Name Of The Boss")]
    public string name;
    [Tooltip("Type Of Job You Are Applying For")]
    public string jobTitle;
    [Tooltip("What The Boss Likes")]
    public string interests;
    [Tooltip("Bosses Speaking Style (eg. Professional, Stern)")]
    public string speakingStyle;
    [Tooltip("What The Boss Dislikes")]
    public string dislikes;
    [Tooltip("What Sort Of Emotions Should The Boss Replicate?")]
    public string emotions;
}
