using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Maze : MonoBehaviour {
	public GameObject wall; //basic unit of wall in the maze
	public GameObject key;
	private GameObject wall_holder; //the congregation of walls as a single gameobject; easy to manipulate
	private GameObject key_holder;
	public float wallLength = 1.0f; //the length of each wall
	public int xSize = 15; //the number of cells on x-axis
	public int ySize = 15; //the number of cells on y-axis
	public Cell[] cells; //data sctructure to hold all cells in the maze
	public Vector3[] Boulder_position;


	//private Cell[] cells; //data sctructure to hold all cells in the maze
	public int num_key_room; //total number of key rooms

	//Boulder
	public GameObject boulder;
	public int Boulder_Move_count;
	public Rigidbody boulder_rigid;

	//bug Reporter
	public Text bugtext;

	// Use this for initialization
	void Start () {
		StartGame ();
		if(num_key_room<3){
			bugtext.text = "Not enough key rooms! Press Q to restart the game!";
		}

	}


	void FixedUpdate(){
		Time.fixedDeltaTime = 0.5f;
		//boulder 

		if (Boulder_Move_count < Boulder_position.Length) {
			//Vector3 force = Boulder_position [Boulder_Move_count] - boulder.transform.position;
			//boulder_rigid.AddForce (force);
			boulder.transform.position = Boulder_position [Boulder_Move_count];
			//boulder_rigid.velocity = force;

			Boulder_Move_count++;	
		} else {
			Boulder_Move_count = 0;
			boulder.transform.position = Boulder_position [Boulder_Move_count];
		}



	}

	// Update is called once per frame
	void Update () {
		
		//BUG report
		if(num_key_room<3){
			
			bugtext.text = "Not enough key rooms! Press Q to restart the game!";
		}

		//press Q to generate a new maze
		if(Input.GetKeyDown(KeyCode.Q)){
			RestartGame ();

		}
	}



	void StartGame(){
		CreateWall ();
		CreateCells ();
		CreateMaze ();

		//wall_holder.transform.localScale += new Vector3 (0.25f,0.25f,0.25f);
		wall_holder.transform.localPosition += new Vector3 (0f, 21f, 0f);
		setkeys ();

		//boulder
		setBoulder ();
		Boulder_Move_count=0;
		boulder.transform.position = Boulder_position [0];
		boulder_rigid = boulder.GetComponent<Rigidbody>();

	}

	void RestartGame(){
		Destroy (wall_holder);
		Destroy (key_holder);

		bugtext.text = "";
		num_key_room = 0;
		//Boulder_Move_count=0;
		StartGame ();
	}




	[System.Serializable]

	//Cell class
	public class Cell{
		public bool visited;
		public GameObject up;
		public GameObject left;
		public GameObject right;
		public GameObject down;
		public bool key_room;
		public int next_cell;
	}

	// create walls for the maze
	void CreateWall(){
		wall_holder = new GameObject ();
		wall_holder.name = "Maze";

		//maze center at (0,0,0)
		//the first wall will be the wall at bot right corner horizonta
		Vector3 initial_pos = new Vector3 ((0-xSize / 2),0f,(0-ySize / 2) + wallLength/2);
		Vector3 my_pos = initial_pos;

		//for x Axis
		for (int i = 0; i < ySize; i++) {
			for (int j = 0; j <= xSize; j++) {
				my_pos = new Vector3 (initial_pos.x + (j * wallLength) , 0f, initial_pos.z + (i * wallLength));
				Instantiate (wall, my_pos, Quaternion.identity,wall_holder.transform);

			}
		}

		//for y Axis
		for (int i = 0; i <= ySize; i++) {
			for (int j = 0; j < xSize; j++) {
				my_pos = new Vector3 (initial_pos.x + (j * wallLength)+wallLength/2, 0f, initial_pos.z + (i * wallLength) - wallLength/2 );
				Instantiate (wall, my_pos, Quaternion.Euler(0.0f,90f,0.0f),wall_holder.transform);
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

		int time_to_break = 0;
		int safe_lock = 0;

		while (current_cell % xSize < xSize - 1 && safe_lock < 1000) {
			safe_lock++;
			cells [current_cell].visited = true;
			int[] neighbour = GiveMeNeighbour (current_cell);


			//breakwall to set key room
			if ( (time_to_break % (xSize) == xSize / 2 && current_cell % xSize < xSize - 1 && current_cell % xSize > 1 && current_cell / xSize < ySize - 1 && current_cell / xSize > 1 && num_key_room < 3 )|| 
				(time_to_break % (xSize) == xSize / 3 && current_cell % xSize < xSize - 1 && current_cell % xSize > 1 && current_cell / xSize < ySize - 1 && current_cell / xSize > 1 && num_key_room < 3)) {
				int break_room = 0;
				time_to_break++;
				//Debug.Log (time_to_break);
				// break the up room and set it as key room
				if (neighbour [2] != xSize * ySize + 1 && !cells [neighbour [2]].visited ) {
					breakwall (2, current_cell);
					break_room = neighbour [2];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;

				}
				//break the down room and set it as key room
				else if (neighbour [3] != xSize * ySize + 1 && !cells [neighbour [3]].visited) {
					breakwall (3, current_cell);
					break_room = neighbour [3];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				} 
				//break the left room and set it as key room
				else if (neighbour [0] != xSize * ySize + 1 && !cells [neighbour [0]].visited) {
					breakwall (0, current_cell);
					break_room = neighbour [0];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				} 
				//break the right room and set it as key room
				else if (neighbour [1] != xSize * ySize + 1 && !cells [neighbour [1]].visited) {
					breakwall (1, current_cell);
					break_room = neighbour [1];
					cells [break_room].visited = true;
					cells [break_room].key_room = true;
					num_key_room++;
				}
				if (break_room > 0) {

					Debug.Log ("break_room: " + break_room);
				}
			}

			time_to_break++;
			int random = Random.Range (0, 4);
			//Debug.Log ("random: "+random);

			if (neighbour [1] != xSize * ySize + 1 && random == 3 && !cells [neighbour [1]].visited) {
				
				cells [current_cell].next_cell = neighbour [1];
				//break right wall
				breakwall (1, current_cell);
				current_cell = neighbour [1];

				//	num_general_room++;

			} else if (neighbour [3] != xSize * ySize + 1 && random == 2 && !cells [neighbour [3]].visited) {

				//break down wall
				cells [current_cell].next_cell = neighbour [3];
				breakwall (3, current_cell);
				current_cell = neighbour [3];
			


			} else if (neighbour [2] != xSize * ySize + 1 && random <= 1 && !cells [neighbour [2]].visited) {

				//break up wall
				cells [current_cell].next_cell = neighbour [2];
				breakwall (2, current_cell);
				current_cell = neighbour [2];

				//num_general_room++;
		
			}
		}


		//get to the exit
		if(current_cell/xSize!=ySize-1){
			for (int i = 0; i < ySize; i++) {
				cells [current_cell].visited = true;
				int[] neighbour2 = GiveMeNeighbour (current_cell);
				if(neighbour2 [2] != xSize * ySize + 1) {
					//break up wall
					cells [current_cell].next_cell = neighbour2 [2];
					breakwall (2,current_cell);
					current_cell = neighbour2 [2];
			//		num_general_room++;
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
		key_holder = new GameObject ();
		key_holder.name = "keys";
		int temp_key = 0;
		Cell[] keyrooms = new Cell[3];
		for (int i = 0; i < cells.Length; i++) {
			if(cells[i].key_room){
				keyrooms [temp_key] = cells [i];
				temp_key++;
			}
		}

		//get keyroom coordinate
		for (int i = 0; i < keyrooms.Length; i++) {
			Vector3 keyposition = new Vector3 (keyrooms [i].left.transform.position.x + 0.5f, 20.5f, keyrooms [i].left.transform.position.z);
			//sphere.transform.position = keyposition;
			GameObject sphere1 = Instantiate (key, keyposition, Quaternion.identity, key_holder.transform) as GameObject;
			sphere1.tag = "key";
		}



	}


	//set boulder and get boulder future move position.
	void setBoulder(){
		//transfer visted cells to a new cell array 
		int num_visit = 0;
		for(int i =0;i<cells.Length;i++){
			if (cells [i].visited && !cells [i].key_room) {
				num_visit++;
			}
		}
		int[] visit_cell = new int[num_visit];
		num_visit = 0;
		int current_cell = 0;
		while(current_cell<xSize*ySize-1){
			visit_cell [num_visit] = cells [current_cell].next_cell;
			current_cell = cells [current_cell].next_cell;
			num_visit++;
		}





//		num_visit = 0;
//		for(int i =0;i<cells.Length;i++){
//			if (cells [i].visited && !cells [i].key_room) {
//				visit_cell[num_visit] = i;
//				num_visit++;
//			}
//		}
		Vector3 Initial_cell_pos = getCellPosition(0);


		Boulder_position = new Vector3[visit_cell.Length];

		for(int i = 0;i<visit_cell.Length;i++){
			//the next position boulder will move to

			Boulder_position[i] = getCellPosition (visit_cell[i]);
		}


	}


	//move boulder
	void BoudlerMove(){
		
	}


	//given cell number and return the position of the cell centre
	Vector3 getCellPosition (int cellnum){
		Vector3 Initial_cell_pos = new Vector3 (-xSize / 2 + wallLength/2, 21f, -ySize / 2 + wallLength/2);
		int xAxis = cellnum % xSize;
		int yAxis = cellnum / xSize;
		return new Vector3 (Initial_cell_pos.x+wallLength*xAxis,Initial_cell_pos.y,Initial_cell_pos.z+wallLength*yAxis);

	}



}
