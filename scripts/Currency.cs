using Godot;
using System;

public class Currency : Area2D
{
	private HUD HUD;
	
	public override void _Ready() {
		HUD = GetNode<HUD>("/root/Stage/HUD");
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
