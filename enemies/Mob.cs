using Godot;
using System;

// Handles basic mob logic. Spawning occurs once, and movement direction is
// updated every frame to move towards the player
public partial class Mob : RigidBody2D
{
	
	private Vector2 UpdatedPlayerPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// selecting animations randomly(?? TO DO)
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
		animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		setDirection();
	}
	
	// Set movement direction of mob to always approach to player
	private void setDirection() {
		
		// Get the position of the player and mob
		Vector2 mobPosition = this.Position;
		var playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		Vector2 playerPosition = playerVariables.getPosition();
		
		// Find the directionVector between the player and mob 
		Vector2 directionVector = (playerPosition - mobPosition).Normalized();
		
		// Get the direction as an angle in radians
		float directionAngle = directionVector.Angle();
		
		// Set the mob's rotation
		this.Rotation = directionAngle;
		
		// Choose the velocity and angle
		var VELOCITY = new Vector2(50, 0);
		this.LinearVelocity = VELOCITY.Rotated(directionAngle);
	}
	
	// Initialise mob at a random location on the spawner
	public void InitMob(PathFollow2D mobSpawnLocation, Vector2 playerPosition) {
			
		// Choose a random location on Path2D.
		mobSpawnLocation.ProgressRatio = GD.Randf();
		
		// Set mob position
		this.Position = mobSpawnLocation.GlobalPosition;
	}
}
