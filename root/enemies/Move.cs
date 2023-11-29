using Godot;
using System;

public partial class Move : State
{
	public override void Update(double delta){
		
		// Ensure mob always follows player
		
		Mob mob = GetParent().GetParent() as Mob;
		Vector2 mobPosition = mob.Position;
		var playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		Vector2 playerPosition = playerVariables.getPosition();
		
		// Find the directionVector between the player and mob 
		Vector2 directionVector = (playerPosition - mobPosition).Normalized();
		
		// Get the direction as an angle in radians
		float directionAngle = directionVector.Angle();
		
		// Choose the velocity and angle
		var movement = mob.MOB_SPEED.Rotated(directionAngle);
		mob.Velocity = movement;
		
		// Move without affecting speed
		var collision = mob.MoveAndCollide(movement * (float)delta, false, 0.00f);
		
		// Adjust position based on collision -- prevents collisions from affecting speed
		if (collision != null){
			mob.Position += collision.GetRemainder();
		}
		
		var animatedSprite2D = mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "idle";
		animatedSprite2D.Play("idle");
		animatedSprite2D.FlipV = false;
		animatedSprite2D.FlipH = (mob.MOB_SPEED.Rotated(directionAngle).X < 0);
	}
}

