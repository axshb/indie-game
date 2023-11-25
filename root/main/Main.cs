using Godot;
using System;

public partial class Main : Node
{
	
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Callable mobTimerCallout = new Callable(this, MethodName._OnMobTimerTimeout);
		Callable scoreTimerCallout = new Callable(this, MethodName._OnScoreTimerTimeout);
		
		Timer mobTimer = GetNode<Timer>("MobTimer");
		mobTimer.Connect("timeout", mobTimerCallout);
		
		Timer scoreTimer = GetNode<Timer>("ScoreTimer");
		scoreTimer.Connect("timeout", scoreTimerCallout);
		
		GetNode<HUD>("HUD").StartGame += OnStartGame;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	
	// Called when game ends to player death
	private void GameOver() {
		
		// Reset game state
		var gameVariables = GetNode<GameVariables>("/root/GameVariables");
		gameVariables.ResetGameState();
		
		// Reset current scene
		GetTree().ReloadCurrentScene(); // TO DO: show game over then reload scene
		
		//GetNode<Timer>("MobTimer").Stop();
		//GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();
	}
	
	// Called when a new game is started
	public void NewGame() {
		
		GetNode<HUD>("HUD").ResetScore();
		_score = 0;
		
		// Set player start position
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
		
		// When the start timer times out, start OnStartTimerTimeout
		GetNode<Timer>("StartTimer").Start();
		GetNode<Timer>("StartTimer").Timeout += OnStartTimerTimeout;
		
		// Delete mobs from previous round if any are left
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		
	}
	
	// Called at the very start, on launch. Sets HUD and launches NewGame()
	private void OnStartGame() {
		
		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("prepare urself");
		NewGame();
	}
	
	// Increments the score every second
	// TO DO: change score calculation
	private void OnScoreTimerTimeout() {
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
		
		// Adds a second to the internal GameVariables timer 
		// This is what tracks spawn rates for enemies
		var gameVariables = GetNode<GameVariables>("/root/GameVariables");
		gameVariables.AddSecond();
		
	}
	
	// When the initial timer ends, begin the game. 
	private void OnStartTimerTimeout() {
		Callable callable = new Callable(this, MethodName.OnStartTimerTimeout);
		
		Timer mobTimer = GetNode<Timer>("MobTimer");
		mobTimer.Start();
		
		Timer scoreTimer = GetNode<Timer>("ScoreTimer");
		scoreTimer.Start();
		
	}
	
	private void _OnMobTimerTimeout() {
		OnMobTimerTimeout();
	}

	private void _OnScoreTimerTimeout() {
		OnScoreTimerTimeout();
	}
	
	// Occurs every increment set by GameVariables.AddSecond() to spawn mobs
	private void OnMobTimerTimeout() {
		
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		Vector2 playerPosition = GetNode<CharacterBody2D>("Player").Position;
		
		var gameVariables = GetNode<GameVariables>("/root/GameVariables");
		int spawnCount = gameVariables.GetSpawnCount();
		
		for (int i = 0; i < spawnCount; i++){
			// Create a new instance of the Mob scene.
			Mob mob = MobScene.Instantiate<Mob>();
			
			// Initialise the mob position and angle 
			mob.InitMob(mobSpawnLocation, playerPosition);

			// Spawn the mob by adding it to the Main scene.
			AddChild(mob);
			
		}
	}
}
