using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour {
	//public Transform target;//要跟踪的车
	public GameObject followcar ;
	private float Speed;
	//private float RotateSpeed;
	private Vector3 Direction;
	private Vector3 RotateDirection;
	//private Vector3 CurrentRoad;//当前所在第几块道路
	private bool ReadyToLoad;
	private GameObject CurrentRoad;
	private int CurrentRoadType;
	private float TurnAngle = 30;
	private int IsTypeone;//
	private Quaternion rotation = Quaternion.identity;//欧拉角转换工具变量
	//当前道路的重心位置
	private float Curx;
	private float Cury;
	private float CurZ;
	private float NewP;
	private float RoadModuleSize;
	//private float MaxSpeed = 10;

	// Use this for initialization
	void Start () { 
		RoadModuleSize = 4.9f;
		ReadyToLoad = false;
		CurrentRoadType = 1;
		CurrentRoad = LoadCurrentRoad (CurrentRoadType);
		//CurrentRoad = 1;//初始化直行道路
		//target = gameObject.GetComponent<Transform>();
		//followcar = gameObject.GetComponent<GameObject>();
		Speed = 2f;
		//RotateSpeed = 10f;
		Direction = new Vector3 (0,0,1);
		RotateDirection = new Vector3 (0,TurnAngle,0);//转弯幅度，-1左转，1右转
		//transform.position = new Vector3 (0,0,-2);
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Direction * Speed * Time.deltaTime, Space.World);
		//模拟按键操作方向
		if (Input.GetKeyDown (KeyCode.W)) {
			Debug.Log ("You press W-go straight");
			//Direction = new Vector3 (0,0,1);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("You press A-turn left");
			transform.Rotate (-RotateDirection);
			//前进方向与车头保持一致
			Direction = new Vector3(Direction.x*Mathf.Cos((float)3.14*TurnAngle/180)-Direction.z*Mathf.Sin((float)3.14*TurnAngle/180),0,
			                        Direction.x*Mathf.Sin ((float)3.14*TurnAngle/180)+Direction.z*Mathf.Cos ((float)3.14*TurnAngle/180));
			Direction = Direction.normalized;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			Debug.Log ("You press D-turn right");
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
			Debug.Log ("Load Next Road!");
			CreateNewEnvioronment ();
		//更改当前模块
		} else if (Vector3.Distance(transform.position,CurrentRoad.transform.position)>2.5) {
			CurrentRoadType = CurrentRoadType+1;
			if(CurrentRoadType > 26) CurrentRoadType = CurrentRoadType%26;
			Debug.Log ("Change Current Road!");
			CurrentRoad = LoadCurrentRoad(CurrentRoadType);
			ReadyToLoad = false;
		}
	}

	//根据当前道路类型Load当前道路GameObject,后期生成接口给用户，可自定义地图
	private GameObject LoadCurrentRoad(int currentroadtype){
		GameObject currentroad;
		Debug.Log ("传入RoadType:"+currentroadtype);
		currentroad = GameObject.Find ("straight1");
		if (currentroadtype == 8 || currentroadtype == 13 || currentroadtype == 16 || currentroadtype == 19 || currentroadtype == 24 || currentroadtype == 26) {
			currentroad = GameObject.Find ("turnleft");
			if(currentroadtype == 8 || currentroadtype == 19){
				rotation.eulerAngles = new Vector3(0,90,0);
			}else if(currentroadtype == 13){
				rotation.eulerAngles = new Vector3(0,180,0);
			}else if(currentroadtype == 16 || currentroadtype == 24){
				rotation.eulerAngles = new Vector3(0,270,0);
			}else if(currentroadtype == 26){
				rotation.eulerAngles = new Vector3(0,0,0);
			}
			currentroad.transform.rotation = rotation;
		} else if (currentroadtype % 2 == 1) {
			currentroad = GameObject.Find ("straight1");
			if((currentroadtype>8 && currentroadtype<13) || (currentroadtype>16 && currentroadtype<19) || (currentroadtype>24 && currentroadtype<26)){
				rotation.eulerAngles = new Vector3(0,90,0);
			}else{
				rotation.eulerAngles = new Vector3(0,0,0);
			}
			currentroad.transform.rotation = rotation;
			//rotation.eulerAngles = new Vector3(0,30,0);
			//currentroad.transform.rotation = rotation;
		} else if (currentroadtype % 2 == 0) {
			currentroad = GameObject.Find ("straight2");
			if((currentroadtype>8 && currentroadtype<13) || (currentroadtype>16 && currentroadtype<19) || (currentroadtype>24 && currentroadtype<26)){
				rotation.eulerAngles = new Vector3(0,90,0);
			}else{
				rotation.eulerAngles = new Vector3(0,0,0);
			}
			currentroad.transform.rotation = rotation;
		}
		return currentroad;

	}

	private void CreateNewEnvioronment(){
		int NextRoadType = CurrentRoadType + 1;
		if(NextRoadType > 26) NextRoadType = NextRoadType%26;
		//分6种情况移动Road
		GameObject NextRoad = LoadCurrentRoad (NextRoadType);
		if (NextRoadType > 0 && NextRoadType < 9) {
			NewP = CurZ + RoadModuleSize;
			NextRoad.transform.position = new Vector3(Curx,Cury,NewP);
		} else if (NextRoadType >= 9 && NextRoadType < 14) {
			NewP = Curx + RoadModuleSize;
			NextRoad.transform.position = new Vector3(NewP,Cury,CurZ);
		} else if ((NextRoadType >= 14 && NextRoadType < 17) || (NextRoadType >= 20 && NextRoadType < 25)) {
			NewP = CurZ - RoadModuleSize;
			NextRoad.transform.position = new Vector3(Curx,Cury,NewP);
		} else if ((NextRoadType >= 17 && NextRoadType < 20) || NextRoadType == 25 || NextRoadType == 26) {
			NewP = Curx - RoadModuleSize;
			NextRoad.transform.position = new Vector3(NewP,Cury,CurZ);
		}
		//Debug.Log ("NextRoadType:"+NextRoadType);



		//ReadyToLoad = false;
	}
}
