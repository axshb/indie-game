using Godot;
using System;

// Class handles overarching game variables
public partial class GameVariables : Node
{

	private double timeElapsed;
	private int _seconds;
	
	// The foollowing variables are for balancing difficulty
	// The values are initially set to assume the easiest difficulty
	
	private int SPAWN_COUNT = 1; // how enemies can be spawned minimum
	private int SPAWN_INCREMENTOR = 2; // how many enemies the min and max should be incremented by 
	private int INCREMENT_INTERVAL = 30; // how many seconds between each increase in spawn rate
	private double MAX_SPAWN_RATE = 1; // max spawn rate per second
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// use score timer to calculate time % n, so the spawn rate incremented
		// every increment of time
		
	}
	
	public int GetSpawnCount(){
		if (_seconds % INCREMENT_INTERVAL == 0){
			SPAWN_COUNT += SPAWN_INCREMENTOR;
		}
		Random rand = new Random();
		// choose a random number between the minimum spawn count and it +3
		int chosenCount = rand.Next(SPAWN_COUNT, SPAWN_COUNT + 3); 
		GD.Print(chosenCount);
		return chosenCount;
	}
	
	// Handles enemy spawn rate adjustments via an internal timer
	public void AddSecond() {
		_seconds++;
		GetSpawnCount();
		
		// Every INCREMENT_INTERVAL seocnds, increase the spawn rate of the enemies
		if (_seconds % INCREMENT_INTERVAL == 0){
			
			// Getting the current wait time, set in the UI for MobTimer
			double currTime = GetNode<Timer>("/root/Main/MobTimer").WaitTime;
			
			if (currTime > MAX_SPAWN_RATE){
				// Increasing spawn rate by a factor of root(2)
				GetNode<Timer>("/root/Main/MobTimer").WaitTime = currTime / Math.Sqrt(2);
			}
		}
		
		// DEBUG to check callable timer properties
		/**
		Timer timer = new Timer();
		foreach (var method in timer.GetType().GetMethods()){
			GD.Print(method.Name);
		}*/
	}
}
