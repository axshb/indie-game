using Godot;
using System;

public partial class PlayerVariables : Node
{
	private Vector2 playerPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void updatePosition(Vector2 newPos) {
		playerPosition = newPos;
	}
	
	public Vector2 getPosition() {
		return playerPosition;
	}
}
