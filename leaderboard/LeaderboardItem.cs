using Godot;
using System;

public class LeaderboardItem : Node
{
    [Export]
    public string PlayerName { get; set; }
    [Export]
    public int Score { get; set; }

    public override void _EnterTree()
    {
        GD.Print(Score);
        GetNode<Label>("PlayerName").Text = PlayerName;
        GetNode<Label>("Score").Text = Score.ToString();
    }
}
