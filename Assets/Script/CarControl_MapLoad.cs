using UnityEngine;
using System.Collections;

public class CarControl_MapLoad : MonoBehaviour {
	//public Transform target;//要跟踪的车
	public GameObject followcar ;
	private float Speed;
	//private float RotateSpeed;
	private Vector3 Direction;
	private Vector3 RotateDirection;
	//private Vector3 CurrentRoad;//当前所在第几块道路
	private bool ReadyToLoad;
	private GameObject CurrentRoad;
	//private int CurrentRoadType;
	private int CurrentRoadNum;
	private float TurnAngle = 30;
	private int IsTypeone;//
	private Quaternion rotation = Quaternion.identity;//欧拉角转换工具变量
	//当前道路的重心位置
	private float Curx;
	private float Cury;
	private float CurZ;
	private float NewP;
	private float RoadModuleSize;
	//地图存储数组
	public int maprows=10+2;
	public int mapcols=10+2;
	public int[,] map = new int[12,12];
	public int Start_i,Start_j;
	private int End_i, End_j;
	private int LastRoadNum;//记录上一个模块
	private int NextRoadNum;
	//private float MaxSpeed = 10;
	// Use this for initialization
	void Start () {
		map = new int[12,12]
		   {{0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0},
			{0,3,1,1,6,0,0,0,0,0,0,0},
			{0,2,0,0,2,0,0,0,0,0,0,0},
			{0,2,0,0,2,0,0,0,0,0,0,0},
			{0,2,0,0,4,1,1,1,1,6,0,0},
			{0,2,0,0,0,0,0,0,0,2,0,0},
			{0,4,1,1,1,1,1,1,1,5,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0}};
		Start_i = 10;
		Start_j = 8;
		CurrentRoadNum = mapcols * Start_i + Start_j + 1;//初始化起点为[9,7]
		End_i = 10;
		End_j = 9;
		LastRoadNum = maprows * End_i + End_j + 1;//初始化LastRoadNum为终点
		RoadModuleSize = 4.9f;
		ReadyToLoad = false;
		//CurrentRoadType = 1;
		CurrentRoad = LoadRoad(1,CurrentRoadNum);
		Speed = 2f;
		//RotateSpeed = 10f;
		Direction = new Vector3 (0,0,1);
		RotateDirection = new Vector3 (0,TurnAngle,0);//转弯幅度，-1左转，1右转
		Debug.Log ("IsCorrecttMap:"+IsCorrecttMap());
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Direction * Speed * Time.deltaTime, Space.World);
		//模拟按键操作方向
		if (Input.GetKeyDown (KeyCode.W)) {
			//Debug.Log ("You press W-go straight");
			//Direction = new Vector3 (0,0,1);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			//Debug.Log ("You press A-turn left");
			transform.Rotate (-RotateDirection);
			//前进方向与车头保持一致
			Direction = new Vector3(Direction.x*Mathf.Cos((float)3.14*TurnAngle/180)-Direction.z*Mathf.Sin((float)3.14*TurnAngle/180),0,
			                        Direction.x*Mathf.Sin ((float)3.14*TurnAngle/180)+Direction.z*Mathf.Cos ((float)3.14*TurnAngle/180));
			Direction = Direction.normalized;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			//Debug.Log ("You press D-turn right");
			transform.Rotate (RotateDirection);
			Direction = new Vector3(Direction.x*Mathf.Cos(-(float)3.14*TurnAngle/180)-Direction.z*Mathf.Sin(-(float)3.14*TurnAngle/180),0,
			                        Direction.x*Mathf.Sin (-(float)3.14*TurnAngle/180)+Direction.z*Mathf.Cos (-(float)3.14*TurnAngle/180));
			Direction = Direction.normalized;
		}
		/*if (Input.GetKeyDown (KeyCode.S)) {
			Debug.Log ("You press S");
			transform.Translate (new Vector3(0,0,-1) * Speed * Time.deltaTime, Space.World);
		}*/
		//获得当前道路模块的中心坐标
		Curx = CurrentRoad.transform.position.x;
		Cury = CurrentRoad.transform.position.y;
		CurZ = CurrentRoad.transform.position.z;
		//车靠近当前模块中心时，Load下一模块
		if (Vector3.Distance(transform.position,CurrentRoad.transform.position)<2 && !ReadyToLoad) {
			ReadyToLoad = true;
			Debug.Log ("---Load Next Road---");
			NextRoadNum = LoadNeighborRoad(CurrentRoadNum,LastRoadNum);
			//CreateNewEnvioronment ();
			//更改当前模块
		} else if (Vector3.Distance(transform.position,CurrentRoad.transform.position)>2.5 ) {
			Debug.Log ("---Change Current Road---");
			LastRoadNum = CurrentRoadNum;
			CurrentRoadNum = NextRoadNum;
			int ii,jj;
			ii = CurrentRoadNum / mapcols;
			if (CurrentRoadNum % mapcols == 0) {
				ii = ii-1;
				jj = mapcols-1;
			} else {
				jj = CurrentRoadNum - ii * mapcols - 1;
			}
			CurrentRoad = LoadRoad(map[ii,jj],CurrentRoadNum);
			Debug.Log("last:"+LastRoadNum+" current:"+CurrentRoadNum+" next:"+NextRoadNum);
			ReadyToLoad = false;
			//ReadyToChangeCurrent = true;
			//CurrentRoadNum = CurrentRoadNum-1;
		}
	
	}

	private int GetMapNum(int ii,int jj){
		return ii * mapcols + jj + 1;
	}
	/*
	private int[] Getij(int num){
		int[] pos =new int[2];
		pos[0] = num / 12;
		if (num % 12 == 0) {
			pos[0] = pos[0]-1;
			pos[1] = 11;
		} else {
			pos[1] = num - pos[0] * 12 - 1;
		}
		return pos;
	}*/

	//根据当前道路类型Load当前道路GameObject,后期生成接口给用户，可自定义地图
	private int LoadNeighborRoad(int currentroadnum,int lastroadnum){
		int i, j;
		int nextroadnum = currentroadnum;
		int nextroad = 0;
		i = currentroadnum / mapcols;
		if (currentroadnum % mapcols == 0) {
			i = i-1;
			j = mapcols-1;
		} else {
			j = currentroadnum - i * mapcols - 1;
		}
		Vector3 nextposition = new Vector3 (Curx,Cury,CurZ);
		//获得下一模块的位置
		if (map [i,j - 1] > 0 && GetMapNum(i,j-1) != lastroadnum) {
			nextroad = map [i,j - 1];
			nextroadnum = GetMapNum(i,j-1);
			nextposition.z = CurZ + RoadModuleSize;
		}
		if (map [i-1,j] > 0 && GetMapNum(i-1,j) != lastroadnum) {
			nextroad = map [i-1,j];
			nextroadnum = GetMapNum(i-1,j);
			nextposition.x = Curx + RoadModuleSize;
		}
		if (map [i,j+1] > 0 && GetMapNum(i,j+1) != lastroadnum) {
			nextroad = map [i,j+1];
			nextroadnum = GetMapNum(i,j+1);
			nextposition.z = CurZ - RoadModuleSize;
		}
		if (map [i+1,j] > 0 && GetMapNum(i+1,j) != lastroadnum) {
			nextroad = map [i+1,j];
			nextroadnum = GetMapNum(i+1,j);
			nextposition.x = Curx - RoadModuleSize;
		}
		//获得对应模块的GameObject
		if (nextroadnum > 0 && nextroadnum != currentroadnum && nextroadnum != lastroadnum) {
			GameObject NextRoad = LoadRoad(nextroad,nextroadnum);
			NextRoad.transform.position = nextposition;
			Debug.Log("nextnum:"+nextroadnum+" nextpos:"+nextposition);
		}
		return nextroadnum;
	}


	private GameObject LoadRoad(int roadtype,int roadnum){
		int rows;
		rows = roadnum / mapcols;
		if (roadnum % mapcols != 0) {
			rows =rows+1;
		}
		GameObject road;
		road = GameObject.Find("straight10");
		if (roadtype == 1) {
			if(roadnum % 2 == 0){road = GameObject.Find("straight10");}
			else{road = GameObject.Find("straight11");}
		} else if (roadtype == 2) {
			if(rows % 2 == 0){road = GameObject.Find("straight20");}
			else{road = GameObject.Find("straight21");}
		} else if (roadtype == 3) {
			road = GameObject.Find("turn3");
		} else if (roadtype == 4) {
			road = GameObject.Find("turn4");
		} else if (roadtype == 5) {
			road = GameObject.Find("turn5");
		} else if (roadtype == 6) {
			road = GameObject.Find("turn6");
		}
		Debug.Log ("LoadRoadPos:"+road.transform.position);
		return road;
	}

	private bool IsCorrecttMap(){
		for (int m=0; m<maprows; m++) {
			for(int n=0; n<mapcols; n++){
				if(map[m,n] == 1){
					if(!((map[m,n-1] == 1 || map[m,n-1] == 3 || map[m,n-1] == 4) 
					   && (map[m,n+1] == 1 || map[m,n+1] == 5 || map[m,n+1] == 6)
					     && map[m-1,n] == 0 && map[m+1,n] == 0)){return false;}
				}else if(map[m,n] == 2){
					if(!((map[m-1,n] == 2 || map[m-1,n] == 3 || map[m-1,n] == 6) 
					     && (map[m+1,n] == 2 || map[m+1,n] == 4 || map[m+1,n] == 5)
					     && map[m,n-1] == 0 && map[m,n+1] == 0)){return false;}
				}else if(map[m,n] == 3){
					if(!((map[m+1,n] == 2 || map[m+1,n] == 4 || map[m+1,n] == 5)
					   &&(map[m,n+1] == 1 || map[m,n+1] == 5 || map[m,n+1] == 6)
					   && map[m,n-1] == 0 && map[m-1,n] == 0)){return false;}
				}else if(map[m,n] == 4){
					if(!((map[m-1,n] == 2 || map[m-1,n] == 3 || map[m-1,n] == 6)
					   && (map[m,n+1] == 1 || map[m+1,n] == 5 || map[m+1,n] == 6)
					     && map[m,n-1] == 0 && map[m+1,n] == 0)){return false;}
				}else if(map[m,n] == 5){
					if(!((map[m,n-1] == 1 || map[m,n-1] == 3 || map[m,n-1] == 4)
					   && (map[m-1,n] == 2 || map[m-1,n] == 3 || map[m-1,n] == 6)
					     && map[m+1,n] == 0 && map[m,n+1] == 0)){return false;}
				}else if(map[m,n] == 6){
					if(!((map[m,n-1] == 1 || map[m,n-1] == 3 || map[m,n-1] == 4)
					     && (map[m+1,n] == 2 || map[m+1,n] == 4 || map[m+1,n] == 5)
					     && map[m-1,n] == 0 && map[m,n+1] == 0)){return false;}
				}
			}
		}
		return true;
	}
	
}
