using Godot;
using System;

public class HitAudio : AudioStreamPlayer2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public AudioStream[] Sounds { get; set; }

    private Random _rnd;

    public HitAudio()
    {
        _rnd = new Random();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void Hit()
    {
        Stream = Sounds[_rnd.Next(0, Sounds.Length)];
        Play();
    }
}
