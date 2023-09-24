using Godot;
using System;

public class Rat : Enemy
{
	[Export]
	public int Speed { get; set; } = 200;
	[Export]
	public float SwarmFactor { get; set; } = 1f; // How much the enemies will swarm the player.

	private Random _rnd;
	private float _movementTimer = 0;
	private Vector2 _velocity = Vector2.Zero;
	
	public Rat() : base() {
		_rnd = new Random();
	}

	public override Vector2 Move(float delta) {
		if (_movementTimer <= 0) {
			// Gaussian random movement. Directions further towards the player are more likely to occur, based on the swarming factor.
			float u1 = (float) _rnd.NextDouble();
			float u2 = (float) _rnd.NextDouble();
			float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(Mathf.Tau * u2);
			float randAngle = randStdNormal / SwarmFactor;
			_velocity = DirectionToPlayer().Rotated(randAngle);
			_movementTimer = 0.2f;
		}

		_movementTimer = Mathf.Max(0, _movementTimer - delta);
		_velocity = _velocity.Normalized() * Speed;
		Position += _velocity * (float)delta;
		Position = new Vector2(Position.x, Position.y);

		return _velocity;
	}
}
