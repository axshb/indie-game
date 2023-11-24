using Godot;
using System;

public partial class HUD : CanvasLayer
{
	
	[Signal]
	public delegate void StartGameEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<Button>("StartButton").Pressed += OnStartButtonPressed; 
		GetNode<Timer>("MessageTimer").Timeout += OnMessageTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	
	public void ShowMessage(string text) {
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}
	
	async public void ShowGameOver() {
		ShowMessage("ur dogshit lmao");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		//message.Text = "Dodge the enemies!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}
	
	public void UpdateScore(int score) {
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	
	private void OnStartButtonPressed() {
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}
	
	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
	
	public void ResetScore() {
		GetNode<Label>("ScoreLabel").Text = "0"; // TO DO
	}
}
