using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {
	public GameObject wall; //basic unit of wall in the maze
	public GameObject key;
	private GameObject wall_holder; //the congregation of walls as a single gameobject; easy to manipulate
	private GameObject key_holder;
	public float wallLength = 1.0f; //the length of each wall
	public int xSize = 15; //the number of cells on x-axis
	public int ySize = 15; //the number of cells on y-axis
	private Vector3 initial_pos; //the position of first wall in the left bottom corner
	public Cell[] cells; //data sctructure to hold all cells in the maze

	//private Cell[] cells; //data sctructure to hold all cells in the maze
	private int num_key_room; //total number of key rooms

	//Boulder
	public GameObject boulder;
	public Rigidbody boulder_rigid;


	// Use this for initialization
	void Start () {
		StartGame ();

	}

	// Update is called once per frame
	void Update () {
		//press Q to generate a new maze
		if(Input.GetKeyDown(KeyCode.Q)){
			Destroy (wall_holder);
			Destroy (key_holder);
			num_key_room = 0;
			StartGame ();
		}
	}



	void StartGame(){
		CreateWall ();
		CreateCells ();
		CreateMaze ();

		if(num_key_room<3){
			CreateMaze ();
		}
		wall_holder.transform.localScale += new Vector3 (0.25f,0.25f,0.25f);
		wall_holder.transform.localPosition += new Vector3 (0f, 21f, 0f);
		setkeys ();
		//setBoulder ();

	}

	void RestartGame(){
		
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
		public Vector3 cellPosition;
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
			cells[current_cell].cellPosition.x = cells[current_cell].down.transform.position.x ;
			cells [current_cell].cellPosition.y = cells [current_cell].down.transform.position.y - 1f;
			cells[current_cell].cellPosition.z = cells[current_cell].down.transform.position.z + wallLength/2 ;

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
		//	Debug.Log ("");
			if (neighbour [2] != xSize * ySize + 1 && random ==0 && !cells[neighbour[2]].visited ) {

				//save the next cell's position
				cells[neighbour[2]].cellPosition.x = cells[neighbour[2]].up.transform.position.x ;
				cells[neighbour[2]].cellPosition.y =cells [current_cell].down.transform.position.y - 1f;
				cells[neighbour[2]].cellPosition.z = cells[neighbour[2]].up.transform.position.z - wallLength/2 ;

				//break up wall
				breakwall (2,current_cell);
				current_cell = neighbour [2];

				//num_general_room++;
			
			} else if (neighbour [3] != xSize * ySize + 1 && random == 1 && !cells[neighbour[3]].visited ) {

				//save the next cell's position
				cells[neighbour[3]].cellPosition.x = cells[neighbour[3]].up.transform.position.x ;
				cells[neighbour[3]].cellPosition.y =cells [current_cell].down.transform.position.y - 1f ;
				cells[neighbour[3]].cellPosition.z = cells[neighbour[3]].up.transform.position.z - wallLength/2 ;
				//break down wall
				breakwall (3,current_cell);
				current_cell = neighbour [3];
			}
				 else if (neighbour [1] != xSize * ySize + 1 && random == 2 && !cells[neighbour[1]].visited ) {
				//save the next cell's position
				cells[neighbour[1]].cellPosition.x = cells[neighbour[1]].up.transform.position.x ;
				cells[neighbour[1]].cellPosition.y = cells [current_cell].down.transform.position.y - 1f ;
				cells[neighbour[1]].cellPosition.z = cells[neighbour[1]].up.transform.position.z - wallLength/2 ;

				//break right wall
				breakwall (1,current_cell);
				current_cell = neighbour [1];
			//	num_general_room++;


			}
		}


		//get to the exit
		if(current_cell/xSize!=ySize-1){
			for (int i = 0; i < ySize; i++) {
				int[] neighbour = GiveMeNeighbour (current_cell);
				if(neighbour [2] != xSize * ySize + 1) {
					//save the next cell's position
					cells[neighbour[2]].cellPosition.x = cells[neighbour[2]].up.transform.position.x ;
					cells[neighbour[2]].cellPosition.y = cells [current_cell].down.transform.position.y - 1f ;
					cells[neighbour[2]].cellPosition.z = cells[neighbour[2]].up.transform.position.z - wallLength/2 ;

					//break up wall
					breakwall (2,current_cell);
					current_cell = neighbour [2];
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
		for(int i =0 ;i<keyrooms.Length;i++){
			Vector3 keyposition = new Vector3 (keyrooms[i].left.transform.position.x+0.5f , 20.5f , keyrooms[i].left.transform.position.z);
			//sphere.transform.position = keyposition;
			GameObject sphere1 = Instantiate (key, keyposition, Quaternion.identity, key_holder.transform) as GameObject;
			sphere1.tag = "key";
			//sphere.transform.position = keyposition;
		
		}



	}



	void setBoulder(){
		//transfer visted cells to a new array 
		Cell[] visitCells=new Cell[cells.Length];
		int visit_cells = 0;
		for(int i =0;i<cells.Length;i++){
			if (cells [i].visited && !cells [i].key_room) {
				visitCells[visit_cells] = cells[i];
			//	Debug.Log (visitCells [visit_cells].cellPosition);
				visit_cells++;
			}
		}

		Instantiate (boulder, new Vector3(-8f,20.5f,-8f), Quaternion.identity, wall_holder.transform);
		boulder_rigid = boulder.GetComponent<Rigidbody> ();


		for (int i = 0; i < visitCells.Length; i++) {
			//if(visitCells[i].cellPosition!=null){
			Vector3 boulder_move =visitCells[i].cellPosition;

			boulder_rigid.MovePosition (boulder_move);
			//

		}	
	}
	void fixedUpadate(){
		
	}

}
