using Godot;
using System;

public class Currency : Area2D
{
	[Export]
	public int Speed { get; set; } = 50;
	private HUD HUD;
	private Player Player;
	
	public override void _Ready() {
		HUD = GetNode<HUD>("/root/Stage/HUD");
		Player = GetParent().GetNode<Player>("Player");
	}
	private void _Process() {
		//TODO:Drifts towards player
		//Vector2 Direction = (Player.GlobalTransform.origin - GlobalTransform.origin).Normalized();
		//Position += Direction.Normalized() * Speed;
		//(float)delta;
	}
	private void _on_Currency_area_entered(object area)
	{
		if (area is Player) {
			HUD.AddGold(1);
			QueueFree();
		}
	}
	private void _on_Timer_timeout()
	{
		QueueFree();
	}
}
