using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class Tilemap : TileMap
{	
	[Export]
	public int WIDTH = 400;
	[Export]
	public int HEIGHT = 200;
	
	private const int ISLAND_SIZE = 20;
	private const int ITERATIONS = 5;
	
	private const int GROUND_ID = 1;
	private const int RAISED_ID = 2;
	private const int BORDER_ID = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateBaseMap();
		PlaceIslandTiles();
		GenerateInnerWalls();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void GenerateBaseMap(){
		Random rand = new Random();
		
		// Nested loop to generate a HEIGHT x WIDTH base map
		for (int y = 0; y < HEIGHT; y++){
			for (int x = 0; x < WIDTH; x++){
				
				Vector2I coords = new Vector2I (x, y); // placement coords
				
				// If the tile is a map boundary
				if (x == 0 || y == 0 || x == WIDTH - 1 || y == HEIGHT - 1){
					int varX = rand.Next(x - 2, x + 2);
					int varY = rand.Next(y - 2, y + 2);
					SetCell(5, new Vector2I(varX, varY), 3, new Vector2I(0,0));
					
				// If the tile is not a map boundary 
				} else{
					SetCell(0, coords, GROUND_ID, new Vector2I(4,0));
					PlaceLitter(x, y);
				}
			}
		}
	}
	
	// Generating inner walls
	private void GenerateInnerWalls(){
		Random rand = new Random();
		for (int y = 0; y < HEIGHT; y++){
			for (int x = 0; x < WIDTH; x++){
				
				Vector2I coords = new Vector2I (x, y); // placement coords
				int GAP_WIDTH = 10;
				
				if ((x == y || y == WIDTH - x - 1)){
					if (y < HEIGHT / 2 - GAP_WIDTH / 2 || y > HEIGHT / 2 + GAP_WIDTH / 2){
						
						int varX = rand.Next(x - 2, x + 2);
						int varY = rand.Next(y - 2, y + 2);
						SetCell(5, new Vector2I(varX, varY), 3, new Vector2I(0,0));
					}
					
				}
				
			}
		} 
	}
	
	// Placing debris
	private void PlaceLitter(int x, int y){
		Random rand = new Random();
		int index = rand.Next(5, 9);
		if (rand.Next(0, 100) < 7){
			Vector2I coords = new Vector2I(x, y);
			SetCell(3, coords, GROUND_ID, new Vector2I(index, 0));
		}
	}
	
	// Main method to create elevated islands
	private bool[,] GenerateIslandArray(){
		bool[,] islandArray = InitArray();
		for (int i = 0; i < ITERATIONS; i++){
			islandArray = SimulateIsland(islandArray);
		}
		islandArray = FillIslandGaps(islandArray);
		return islandArray;
	}
	
	// Initializing the boolean array to store island
	private bool[,] InitArray(){
		
		bool[,] newIslandArray = new bool[ISLAND_SIZE, ISLAND_SIZE];
		Random rand = new Random();
		
		for (int i = 0; i < ISLAND_SIZE; i++){
			for (int j = 0; j < ISLAND_SIZE; j++){
				newIslandArray[i, j] = rand.Next(2) == 0;
			}
		}
		
		return newIslandArray; 
	}
	
	// Uses cellular automaton to generate island
	private bool[,] SimulateIsland(bool[,] currentArray){
		
		int width = currentArray.GetLength(0);
		int height = currentArray.GetLength(1);
		bool[,] newIslandArray = new bool[width, height];
		
		for (int i = 1; i < width - 1; i++){
			for (int j = 1; j < height - 1; j++){
				int neighbors = CountNeighbors(currentArray, i, j);
			
				if (currentArray[i, j]){
					newIslandArray[i, j] = neighbors >= 4;
				} else{
					newIslandArray[i, j] = neighbors >= 5;
				}
			}
		}
		
		return newIslandArray;
	}
	
	// Helper method
	private int CountNeighbors(bool[,] array, int x, int y){
		int count = 0;
		for (int i = x - 1; i <= x + 1; i++){
			for (int j = y - 1; j <= y + 1; j++){
				if (i != x || j != y){
					if (i >= 0 && i < array.GetLength(0) && j >= 0 && j < array.GetLength(1)){
						if (array[i, j]){
							count++;
						}
					}
				}
			}
		}
		
		return count;
	}
	
	// Postprocessing to ensure that there are no "holes" in the island, called after generation
	private bool[,] FillIslandGaps(bool[,] array){
		
		int width = array.GetLength(0);
		int height = array.GetLength(1);
		//bool[,] filledArray = new bool[width, height];
		
		for (int i = 1; i < width - 1; i++){
			for (int j = 1; j < height - 1; j++){
				
				if (!array[i, j] &&
					array[i - 1, j] && array[i + 1, j] &&
					array[i, j - 1] && array[i, j + 1]){
						array[i, j] = true;
					}
			}
		}
		return array;
	}
	
	// Places generate islands in a grid like pattern across the map
	private void PlaceIslandTiles(){
		
		for (int locX = 0; locX < WIDTH; locX += ISLAND_SIZE){
			for (int locY = 0; locY < HEIGHT; locY += ISLAND_SIZE){
				bool[,] islandArray = GenerateIslandArray();
				for (int y = 0; y < ISLAND_SIZE; y++){
					for (int x = 0; x < ISLAND_SIZE; x++){
						if (islandArray[x, y]){
							SetCell(0, new Vector2I(x + locX, y + locY), RAISED_ID, new Vector2I(1, 1));
						}
					}
				}
				PlaceIslandShadows(islandArray, locX, locY);
			}
		}
	}
	
	// Places shadow tiles around the tiles placed above to create depth
	private void PlaceIslandShadows(bool[,] islandArray, int locX, int locY){
		
		for (int x = 0; x < ISLAND_SIZE; x++){
			for (int y = 0; y < ISLAND_SIZE; y++){
				
				Vector2I noShadowTile = GetCellAtlasCoords(0, new Vector2I(x + locX, y + locY));
				
				if (islandArray[x, y] && noShadowTile == new Vector2I(1, 1)){
					
					// if no tile below
					if (!islandArray[x, y + 1] && 
							GetCellSourceId(0, new Vector2I(x + locX, y + 1 + locY)) == GROUND_ID){
						SetCell(1, new Vector2I(x + locX, y + 1 + locY), RAISED_ID, new Vector2I(1, 2));
					
					// if no tile above
					} if (!islandArray[x, y - 1] && 
							GetCellSourceId(0, new Vector2I(x + locX, y - 1 + locY)) == GROUND_ID){
						SetCell(2, new Vector2I(x + locX, y - 1 + locY), RAISED_ID, new Vector2I(1, 0));

					// if no tile left
					} if (!islandArray[x - 1, y] && 
							GetCellSourceId(0, new Vector2I(x - 1 + locX, y + locY)) == GROUND_ID){
						SetCell(3, new Vector2I(x - 1 + locX, y + locY), RAISED_ID, new Vector2I(0, 1));

					// if no tile right
					} if (!islandArray[x + 1, y] && 
							GetCellSourceId(0, new Vector2I(x + 1 + locX, y + locY)) == GROUND_ID){
						SetCell(4, new Vector2I(x + 1 + locX, y + locY), RAISED_ID, new Vector2I(2, 1));

					}
				}
			}
		}
	}
}
