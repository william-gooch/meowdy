using Godot;
using System;

public class Bullet : Area2D
{
	private int Speed = 1000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
	private void _on_Bullet_body_entered(object body)
	{
		//Hide();
		//GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
	public void LaunchBullet()
	{
		
	}
}
