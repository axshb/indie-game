using Godot;
using System;

// Handles basic mob logic. Spawning occurs once, and movement direction is
// updated every frame to move towards the player
public partial class Mob : CharacterBody2D
{
	
	private Vector2 UpdatedPlayerPosition;
	
	[Export]
	public Vector2 MOB_SPEED = new Vector2(275, 0);
	
	public Vector2 velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		this.Rotation = 0;
	}
	
	// Initialise mob at a random location on the spawner
	public void InitMob(PathFollow2D mobSpawnLocation, Vector2 playerPosition) {
			
		// Choose a random location on Path2D.
		mobSpawnLocation.ProgressRatio = GD.Randf();
		
		// Set mob position
		this.Position = mobSpawnLocation.GlobalPosition;
	}
}
