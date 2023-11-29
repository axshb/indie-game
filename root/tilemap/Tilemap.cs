using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class Tilemap : TileMap
{
	private int WIDTH = 400;
	private int HEIGHT = 200;
	
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
					
					// If loop is at the top boundary
					if (y == 0){
						
						// Alternating tiles since there are 2 variations
						if (x%2 != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(1, 0));
						} else if (x%2 == 0 && x != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(2, 0));
						}
					
					// If loop is at the bottom boundary
					} else if (y == HEIGHT - 1){
						// Alternating tiles since there are 2 variations
						if (x%2 != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(1, 3));
						} else if (x%2 == 0 && x != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(2, 3));
						}
						
					// If loop is at the left boundary
					} else if (x == 0){
						
						// Alternating tiles since there are 2 variations
						if (y%2 != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(0, 1));
						} else if (y%2 == 0 && y != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(0, 2));
						}
					
					// If loop is at the right boundary
					} else if (x == WIDTH - 1){
						
						// Alternating tiles since there are 2 variations
						if (y%2 != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(3, 1));
						} else if (y%2 == 0 && y != 0){
							SetCell(0, coords, BORDER_ID, new Vector2I(3, 2));
						}
					}
					
				// If the tile is not a map boundary 
				} else{
					SetCell(0, coords, GROUND_ID, new Vector2I(4,0));
					PlaceLitter(x, y);
				}
				
				// Filling in corners of the map boundary
				if (x == 0 && y == 0){
					SetCell(0, coords, BORDER_ID, new Vector2I(0,0));
				}
				
				if (x == WIDTH - 1 && y == 0){
					SetCell(0, coords, BORDER_ID, new Vector2I(3,0));
				}
				
				if (x == 0 && y == HEIGHT - 1){
					SetCell(0, coords, BORDER_ID, new Vector2I(0, 3));
				}
				
				if (x == WIDTH - 1 && y == HEIGHT - 1){
					SetCell(0, coords, BORDER_ID, new Vector2I(3, 3));
				}
			}
		}
	}
	
	private void PlaceLitter(int x, int y){
		Random rand = new Random();
		int index = rand.Next(5, 9);
		if (rand.Next(0, 100) < 10){
			Vector2I coords = new Vector2I(x, y);
			SetCell(0, coords, GROUND_ID, new Vector2I(index, 0));
		}
	}
	
	private bool[,] GenerateIslandArray(){
		bool[,] islandArray = InitArray();
		for (int i = 0; i < ITERATIONS; i++){
			islandArray = SimulateIsland(islandArray);
		}
		islandArray = FillIslandGaps(islandArray);
		return islandArray;
	}
	
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
	
	private void PlaceIslandTiles(){
		
		Random rand = new Random();
		int locX = rand.Next(WIDTH - ISLAND_SIZE);
		locX = 0;
		int locY = rand.Next(HEIGHT - ISLAND_SIZE);
		locY = 0;
		
		for (locX = 0; locX < WIDTH; locX += ISLAND_SIZE){
			for (locY = 0; locY < HEIGHT; locY += ISLAND_SIZE){
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
