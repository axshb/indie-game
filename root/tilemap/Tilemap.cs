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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateBaseMap();
		
		int numIslands = (WIDTH * HEIGHT) / (ISLAND_SIZE * ISLAND_SIZE);
		for (int i = 0; i < numIslands; i++){
			bool[,] islandArray = GenerateIslandArray();
			islandArray = FillIslandGaps(islandArray);
			PlaceIslandTiles(islandArray);
		}
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
							SetCell(0, coords, 2, new Vector2I(1, 0));
						} else if (x%2 == 0 && x != 0){
							SetCell(0, coords, 2, new Vector2I(2, 0));
						}
					
					// If loop is at the bottom boundary
					} else if (y == HEIGHT - 1){
						// Alternating tiles since there are 2 variations
						if (x%2 != 0){
							SetCell(0, coords, 2, new Vector2I(1, 3));
						} else if (x%2 == 0 && x != 0){
							SetCell(0, coords, 2, new Vector2I(2, 3));
						}
						
					// If loop is at the left boundary
					} else if (x == 0){
						
						// Alternating tiles since there are 2 variations
						if (y%2 != 0){
							SetCell(0, coords, 2, new Vector2I(0, 1));
						} else if (y%2 == 0 && y != 0){
							SetCell(0, coords, 2, new Vector2I(0, 2));
						}
					
					// If loop is at the right boundary
					} else if (x == WIDTH - 1){
						
						// Alternating tiles since there are 2 variations
						if (y%2 != 0){
							SetCell(0, coords, 2, new Vector2I(3, 1));
						} else if (y%2 == 0 && y != 0){
							SetCell(0, coords, 2, new Vector2I(3, 2));
						}
					}
					
				// If the tile is not a map boundary 
				} else{
					SetCell(0, coords, 3, new Vector2I(4,0));
					PlaceLitter(x, y);
				}
				
				// Filling in corners of the map boundary
				if (x == 0 && y == 0){
					SetCell(0, coords, 2, new Vector2I(0,0));
				}
				
				if (x == WIDTH - 1 && y == 0){
					SetCell(0, coords, 2, new Vector2I(3,0));
				}
				
				if (x == 0 && y == HEIGHT - 1){
					SetCell(0, coords, 2, new Vector2I(0, 3));
				}
				
				if (x == WIDTH - 1 && y == HEIGHT - 1){
					SetCell(0, coords, 2, new Vector2I(3, 3));
				}
			}
		}
	}
	
	private void PlaceLitter(int x, int y){
		Random rand = new Random();
		int index = rand.Next(5, 9);
		if (rand.Next(0, 100) < 10){
			Vector2I coords = new Vector2I(x, y);
			SetCell(0, coords, 3, new Vector2I(index, 0));
			//SetPattern(0, coords, TileSet.GetPattern(patternIndex));
			
		}
	}
	
	private bool[,] GenerateIslandArray(){
		bool[,] islandArray = InitArray();
		for (int i = 0; i < ITERATIONS; i++){
			islandArray = SimulateIsland(islandArray);
		}
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
		bool[,] filledArray = new bool[width, height];
		
		for (int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				
				filledArray[i, j] = array[i, j];
				
				// Fill gaps by considering diagonal neighbors
				if (!array[i, j]){
					int diagonalNeighbors = CountDiagonalNeighbors(array, i, j);
					filledArray[i, j] = diagonalNeighbors >= 4;
				}
			}
		}
		return filledArray;	
	}
	
	private int CountDiagonalNeighbors(bool[,] array, int x, int y){
		
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
	
	private void PlaceIslandTiles(bool[,] islandArray){
		
		Random rand = new Random();
		int locX = rand.Next(WIDTH - ISLAND_SIZE);
		int locY = rand.Next(HEIGHT - ISLAND_SIZE);
		
		for (int y = 0; y < ISLAND_SIZE; y++){
			for (int x = 0; x < ISLAND_SIZE; x++){
				if (islandArray[x, y]){
					SetCell(0, new Vector2I(x + locX, y + locY), 5, new Vector2I(1, 1));
				}
			}
		}
		
		// placing shadows
		// known issue: shadow placements when adjacent tiles are shadows do not get placed
		// move direct index reference to a class variable for the tiles
		for (int y = 1; y < ISLAND_SIZE; y++){
			for (int x = 1; x < ISLAND_SIZE; x++){
				
				// if no tile below
				if (islandArray[x, y] && !islandArray[x, y + 1]){
					SetCell(0, new Vector2I(x + locX, y + 1 + locY), 5, new Vector2I(1, 2));
				
				// if no tile above
				} else if (islandArray[x, y] && !islandArray[x, y - 1]){
					SetCell(0, new Vector2I(x + locX, y - 1 + locY), 5, new Vector2I(1, 0));
				
				// if no tile left
				} else if (islandArray[x, y] && !islandArray[x - 1, y]){
					SetCell(0, new Vector2I(x - 1 + locX, y + locY), 5, new Vector2I(0, 1));
					
				// if no tile right
				} else if (islandArray[x, y] && !islandArray[x + 1, y]){
					SetCell(0, new Vector2I(x + 1 + locX, y + locY), 5, new Vector2I(2, 1));
				}
				
			}
		}
	}
}
