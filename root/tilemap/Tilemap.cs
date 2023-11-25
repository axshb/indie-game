using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class Tilemap : TileMap
{
	private int WIDTH = 300;
	private int HEIGHT = 150;
	private string[,] mapCells; // edit to track node type. 0 = wall, 1 = ground, 2 = obj
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateBoundaries();
		PlaceTeleporter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void GenerateBoundaries(){
		mapCells = new string[WIDTH, HEIGHT];
		Random rand = new Random();
		// Nested loop to generate a WIDTH x HEIGHT room of maze walls
		for (int x = 0; x < WIDTH; x++){
			for (int y = 0; y < HEIGHT; y++){
				Vector2I coords = new Vector2I (x,y);
				// If the tile is a map boundary
				if (x == 0 || y == 0 || x == WIDTH - 1 || y == HEIGHT - 1){
					Vector2I wallCoords = new Vector2I(0, 0); // Selecting tile with collision
					SetCell(0, coords, 2, wallCoords);
					mapCells[x, y] = "wall";
					if (x == 0 || x == WIDTH - 1){
						SetCell(0, coords, 2, wallCoords, 1);
					}
					
				// If the tile is not a map boundary 
				} else{
					
					var atlasX = rand.Next(0,2);
					//GD.Print(atlasX);
					Vector2I altCoords = new Vector2I(1, 0);
					Vector2I groundCoords = new Vector2I(atlasX, 0); // TO DO Selecting tile
					
					if (rand.Next(0,100) > 10){
						SetCell(0, coords, 1, altCoords, 1);
					} else{
						SetCell(0, coords, 1, groundCoords);
					}
					mapCells[x, y] = "ground";
					
				}
			}
		}
	}
	
	private void PlaceTeleporter(){
		Random rand = new Random();
		int x = rand.Next(1, WIDTH);
		int y = rand.Next(1, HEIGHT);
		// GD.Print("teleporter at " + x + ", " + y);
		Vector2I coords = new Vector2I(x, y);
		Vector2I teleporterCoords = new Vector2I(8, 12);
		if (mapCells[x, y].Equals("ground")){
			SetCell(0, coords, 1, teleporterCoords);
		}
	}
}
