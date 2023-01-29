using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/SoundLibraryObject", order = 1)]
public class SoundLibrary : ScriptableObject
{
    public AudioClip buttonSound; 
    public AudioClip winSound; 
    public AudioClip loseSound;
    public AudioClip joinSound;
    public AudioClip spearSound;
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip deathSound;
    public AudioClip walkSound;
    public AudioClip reload;
    public AudioClip empty;
    //public AudioClip walkSound;


}
