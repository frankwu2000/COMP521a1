using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {
	public GameObject wall; //basic unit of wall in the maze
	public GameObject key;
	private GameObject wall_holder; //the congregation of walls as a single gameobject; easy to manipulate
	public float wallLength = 1.0f; //the length of each wall
	public int xSize = 15; //the number of cells on x-axis
	public int ySize = 15; //the number of cells on y-axis
	private Vector3 initial_pos; //the position of first wall in the left bottom corner
	private Cell[] cells; //data sctructure to hold all cells in the maze
	private int num_key_room; //total number of key rooms


	[System.Serializable]

	//Cell class
	public class Cell{
		public bool visited;
		public GameObject up;
		public GameObject left;
		public GameObject right;
		public GameObject down;
		public bool key_room;
	}


	// Use this for initialization
	void Start () {
		CreateWall ();
		CreateCells ();
		CreateMaze ();

		if(num_key_room<3){
			CreateMaze ();
		}
		wall_holder.transform.localScale += new Vector3 (0.25f,0.25f,0.25f);
		setkeys ();
	}


	// create walls for the maze
	void CreateWall(){
		wall_holder = new GameObject ();
		wall_holder.name = "Maze";

		//maze center at (0,0,0)
		//the first wall will be the wall at bot right corner horizonta
		initial_pos = new Vector3 ((0-xSize / 2),0f,(0-ySize / 2) + wallLength/2);
		Vector3 my_pos = initial_pos;
		GameObject temp_wall; 

		//for x Axis
		for (int i = 0; i < ySize; i++) {
			for (int j = 0; j <= xSize; j++) {
				my_pos = new Vector3 (initial_pos.x + (j * wallLength) , 0f, initial_pos.z + (i * wallLength));
				temp_wall = Instantiate (wall, my_pos, Quaternion.identity,wall_holder.transform) as GameObject;

			}
		}

		//for y Axis
		for (int i = 0; i <= ySize; i++) {
			for (int j = 0; j < xSize; j++) {
				my_pos = new Vector3 (initial_pos.x + (j * wallLength)+wallLength/2, 0f, initial_pos.z + (i * wallLength) - wallLength/2 );
				temp_wall = Instantiate (wall, my_pos, Quaternion.Euler(0.0f,90f,0.0f),wall_holder.transform) as GameObject;
			}
		}
	}


	void CreateCells(){
		int children = wall_holder.transform.childCount;
		GameObject[] all_walls = new GameObject[children];
		cells = new Cell[xSize * ySize];
		int leftrightProcess = 0;
		int j = 0;
		int termCount = 0;


		//gets all the children
		for(int i=0; i<children;i++){
			all_walls [i] = wall_holder.transform.GetChild (i).gameObject;
		}

		//assigns walls to the cells
		for (int i = 0; i < cells.Length; i++) {
			cells [i] = new Cell ();
			cells [i].key_room = false;
			if (termCount == xSize) {
				leftrightProcess++; // number of walls = number of cells + 1 in each line 
				termCount = 0;
			} 
			cells [i].left = all_walls[leftrightProcess];
			cells [i].down = all_walls[j+(xSize+1)*ySize];
			cells [i].up = all_walls[j+(xSize+1)*ySize+xSize];
			leftrightProcess++; //get right wall and proceed

			termCount++;
			j++; 
			cells[i].right = all_walls[leftrightProcess];
		}
	}


	//helper method to break wall at certain direction of a cell
	//input direction 0 - left , 1 - right, 2 - up, 3 - down
	void breakwall(int direction, int cell){
		if (direction == 0) {
			Destroy (cells [cell].left);
		} else if (direction == 1) {
			Destroy (cells [cell].right);
		} else if (direction == 2) {
			Destroy (cells [cell].up);
		} else if (direction == 3) {
			Destroy (cells [cell].down);
		}

	}


	void CreateMaze(){
		num_key_room = 0;
		int current_cell = 0;
		//break wall of entrance ( entrance of maze)
		if (current_cell == 0) {
			breakwall (0,current_cell);
		}

		int time_to_break = 1;
		int safe_lock = 0;

		while(current_cell%xSize < xSize-1 && safe_lock<1000){
			safe_lock++;
			cells [current_cell].visited = true;
			int[] neighbour = GiveMeNeighbour (current_cell);
			System.Random rnd = new System.Random();


			//Debug.Log ("current cell: "+current_cell);
//			int prob_up = rnd.Next(30,90);
//			int prob_down = rnd.Next(50,90);
//			int prob_right = rnd.Next(60,80);
			//Debug.Log ("prob_up: "+prob_up);
			//Debug.Log ("prob_down: "+prob_down);

			//breakwall
			if ( time_to_break % (xSize/3) == 0 && current_cell % xSize < xSize - 1 && current_cell % xSize > 1 && current_cell / xSize < ySize - 1 && current_cell / xSize > 1 && num_key_room<3) {
				int break_room = 0;
				time_to_break++;

				//by 1/4 chance break the left room and set it as key room
				if (neighbour [0] != xSize * ySize + 1 && rnd.Next (0, 4) == 0) {
					breakwall (0, current_cell);
					break_room = neighbour [0];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;

				}
				//by 1/4 chance break the right room and set it as key room
				else if (neighbour [1] != xSize * ySize + 1 && rnd.Next (0, 4) == 0) {
					breakwall (1, current_cell);
					break_room = neighbour [1];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				} 
				//by 1/4 chance break the up room and set it as key room
				else if (neighbour [3] != xSize * ySize + 1 && rnd.Next (0, 1) == 0) {
					breakwall (3, current_cell);
					break_room = neighbour [3];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				} 
				//break the down room and set it as key room
				else if (neighbour [2] != xSize * ySize + 1) {
					breakwall (2, current_cell);
					break_room = neighbour [2];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				}
				if (break_room > 0) {

					Debug.Log ("break_room: " + break_room);
				}
			}

			time_to_break++;
			int random = rnd.Next(0,3); 
			Debug.Log ("");
//
			if (neighbour [2] != xSize * ySize + 1 && random ==0 && !cells[neighbour[2]].visited ) {
				//break up wall
				breakwall (2,current_cell);
				current_cell = neighbour [2];
			
			} else if (neighbour [3] != xSize * ySize + 1 && random == 1 && !cells[neighbour[3]].visited ) {
				//break down wall
				breakwall (3,current_cell);
				current_cell = neighbour [3];



			} else if (neighbour [1] != xSize * ySize + 1 && random == 2 && !cells[neighbour[1]].visited ) {
				//break right wall
				breakwall (1,current_cell);
				current_cell = neighbour [1];


			}



		}
		//get to the exit
		if(current_cell/xSize!=ySize-1){
			for (int i = 0; i < ySize; i++) {
				int[] neighbour = GiveMeNeighbour (current_cell);
				if(neighbour [2] != xSize * ySize + 1) {
					//break up wall
					breakwall (2,current_cell);
					current_cell = neighbour [2];

				}
			}
		}

		//break wall of exit ( exit of maze)
		if (current_cell == xSize * ySize - 1) {
			breakwall (1,current_cell);
		}



	}


	//helper method to return number of current cell
	//if a cell has no neighbour or the neighbour is illegal then the neighbour value will be set as xsize*ysize+1
	int[] GiveMeNeighbour(int cell){
		int[] neighbour = new int[4];

		//left 
		if (cell % xSize != 0 && !cells[cell-1].visited) {
			neighbour [0] = cell - 1;
		} else {
			neighbour [0] = xSize*ySize + 1; 
		}
			
		//right 
		if ((cell+1) % xSize != 0 && !cells[cell+1].visited) {
			neighbour [1] = cell + 1;
		} else {
			neighbour [1] = xSize*ySize + 1;
		}

		//up 
		if (cell/xSize < ySize-1 && !cells[cell+xSize].visited) {
			neighbour [2] = cell + xSize;
		} else {
			neighbour [2] = xSize*ySize + 1;
		}

		//down
		if (cell/xSize >= 1 && !cells[cell-xSize].visited) {
			neighbour [3] = cell - xSize;
		} else {
			neighbour [3] = xSize*ySize + 1;
		}

//		Debug.Log ("left: "+neighbour [0]);
//		Debug.Log ("right: "+neighbour [1]);
//		Debug.Log ("up: "+neighbour [2]);
//		Debug.Log ("down: "+neighbour [3]);

		return neighbour;



	}

	void setkeys(){
		int temp_key = 0;
		Cell[] keyrooms = new Cell[3];
		for (int i = 0; i < cells.Length; i++) {
			if(cells[i].key_room){
				keyrooms [temp_key] = cells [i];
				temp_key++;
			}
		}

		//get keyroom coordinate
		for(int i =0 ;i<keyrooms.Length;i++){
			Vector3 keyposition = new Vector3 (keyrooms[i].left.transform.position.x+0.5f , 0.5f , keyrooms[i].left.transform.position.z);
			//sphere.transform.position = keyposition;
			GameObject sphere1 = Instantiate (key, keyposition, Quaternion.identity) as GameObject;

		}



	}

	// Update is called once per frame
	void Update () {
		
	}
}
