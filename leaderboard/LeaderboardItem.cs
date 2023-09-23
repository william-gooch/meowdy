using Godot;
using System;

public class LeaderboardItem : Node
{
    [Export]
    public int Rank { get; set; }
    [Export]
    public string PlayerName { get; set; }
    [Export]
    public int Score { get; set; }

    public override void _EnterTree()
    {
        GetNode<Label>("Rank").Text = Rank.ToString();
        GetNode<Label>("PlayerName").Text = PlayerName;
        GetNode<Label>("Score").Text = Score.ToString();
    }
}
