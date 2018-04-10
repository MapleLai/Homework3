using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myGame{
public class Model : MonoBehaviour {

	SSDirector ssDirector;
	public Vector3 leftBank = new Vector3(-8,-2,0);
	public Vector3 rightBank = new Vector3(8,-2,0);
	public Vector3 rightBoat = new Vector3(3,-2,-1);
	public Vector3 leftBoat = new Vector3(-3,-2,-1);
	public Vector3[] rightPriestPosition = { new Vector3 (7, -1, -2), new Vector3 (7, -1, 0), new Vector3 (7, -1, 2) };
	public Vector3[] leftPriestPosition = { new Vector3 (-7, -1, -2), new Vector3 (-7, -1, 0), new Vector3 (-7, -1, 2) };
	public Vector3[] rightDevilPosition = { new Vector3 (9, -1, -2), new Vector3 (9, -1, 0), new Vector3 (9, -1, 2) };
	public Vector3[] leftDevilPosition = { new Vector3 (-9, -1, -2), new Vector3 (-9, -1, 0), new Vector3 (-9, -1, 2) };
	public GameObject [] rightPriest = new GameObject[3];
	public GameObject [] leftPriest = new GameObject[3];
	public GameObject [] rightDevil = new GameObject[3];
	public GameObject [] leftDevil = new GameObject[3];
	public GameObject[] passenger = new GameObject[2];
	public GameObject boat;
	public int isOver = 0;

	// Use this for initialization
	void Start () {
		ssDirector = SSDirector.getInstance ();
		ssDirector.setModel (this);

		Instantiate(Resources.Load("Prefabs/Bank"), leftBank, Quaternion.identity);  
		Instantiate(Resources.Load("Prefabs/Bank"), rightBank, Quaternion.identity);  
		boat = Instantiate(Resources.Load("Prefabs/Boat"), rightBoat, Quaternion.identity) as GameObject; 
		for (int i=0;i<3;i++)
			rightPriest[i] = Instantiate(Resources.Load("Prefabs/Priest"), rightPriestPosition[i], Quaternion.identity) as GameObject; 
		for (int i=0;i<3;i++)
			rightDevil[i] = Instantiate(Resources.Load("Prefabs/Devil"), rightDevilPosition[i], Quaternion.identity) as GameObject; 
	}
		

	public void checkWin(){
		bool isWin = true;
		for (int i = 0; i < 3; i++)
			if (leftPriest [i] == null || leftDevil[i] == null)
				isWin = false;
		if (isWin == true)
			isOver = 1;
	}

	public void checkLose(){
		int leftPriestNum = 0,rightPriestNum = 0;
		int leftDevilNum = 0,rightDevilNum = 0;
		for (int i = 0; i < 3; i++) {
			if (leftPriest [i] != null)
				leftPriestNum++;
			if (rightPriest [i] != null)
				rightPriestNum++;
			if (leftDevil [i] != null)
				leftDevilNum++;
			if (rightDevil [i] != null)
				rightDevilNum++;
		}

		if ((leftDevilNum > leftPriestNum && leftPriestNum != 0) || (rightDevilNum > rightPriestNum && rightPriestNum != 0))
			isOver = -1;
	}


	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		if (isOver == 1) GUI.Label(new Rect(400,100,100,50),"You win！");
		if (isOver == -1) GUI.Label(new Rect(400,100,100,50),"You lose！");
	}
} 


public class SSActionManager : MonoBehaviour {  
	private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();  
	private List<SSAction> waitingAdd = new List<SSAction>();  
	private List<int> waitingDelete = new List<int>();  

	// Use this for initialization  
	void Start()  {  }  

	// Update is called once per frame  
	protected void Update()  {  
		foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;  
		waitingAdd.Clear();  

		foreach (KeyValuePair<int, SSAction> kv in actions)  {  
			SSAction ac = kv.Value;  
			if (ac.destroy)  {  
				waitingDelete.Add(ac.GetInstanceID());  
			}  
			else if (ac.enable)  {  
				ac.Update();  
			}  
		}  

		foreach (int key in waitingDelete)  {  
			SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);  
		}  
		waitingDelete.Clear();  
	}  

	public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)  {  
		action.gameobject = gameobject;  
		action.transform = gameobject.transform;  
		action.callback = manager;  
		waitingAdd.Add(action);  
		action.Start();  
	}  
} 







public enum SSActionEventType : int { Started, Competeted }  

public interface ISSActionCallback  {  
	void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
		int intParam = 0, string strParam = null, Object objectParam = null);  
}  

public class SSAction : ScriptableObject  {  
	public bool enable = true;  
	public bool destroy = false;  

	public GameObject gameobject { get; set; }  
	public Transform transform { get; set; }  
	public ISSActionCallback callback { get; set; }  

	protected SSAction() { }  

	public virtual void Start()  {  
		throw new System.NotImplementedException();  
	}  

	public virtual void Update()  {  
		throw new System.NotImplementedException();  
	}  
}









public class CCOnBoat  : SSAction {
	public Model sceneController;
	public static CCOnBoat GetSSAction(){
		CCOnBoat action = ScriptableObject.CreateInstance<CCOnBoat> ();
		return action;
	}

	public override void Start(){
		sceneController = (Model)SSDirector.getInstance ().currentScenceController;
	}

	public override void Update(){
		Debug.Log("onBoat");
		int num = 0;
		bool onTheSameBank = false;
		for (int i = 0; i < 2; i++)
			if (sceneController.passenger [i] != null)
				num++;

		if (num != 2 && gameobject.tag == "Priest") {
			if (sceneController.boat.transform.position == sceneController.leftBoat) {
				for (int i = 0; i < 3; i++)
					if (sceneController.leftPriest [i] == gameobject) {
						sceneController.leftPriest [i] = null;
						onTheSameBank = true;
						break;
					}
			} 
			else { //boat.transform.position == rightboat
				for (int i = 0; i < 3; i++)
					if (sceneController.rightPriest [i] == gameobject) {
						sceneController.rightPriest [i] = null;
						onTheSameBank = true;
						break;
					}
			}
		} 
		else if(num != 2) { //gameObject.tag == "Devil"
			if (sceneController.boat.transform.position == sceneController.leftBoat) {
				for (int i = 0; i < 3; i++)
					if (sceneController.leftDevil [i] == gameobject) {
						sceneController.leftDevil [i] = null;
						onTheSameBank = true;
						break;
					}
			} 
			else { //boat.transform.position == rightboat
				for (int i = 0; i < 3; i++)
					if (sceneController.rightDevil [i] == gameobject) {
						sceneController.rightDevil [i] = null;
						onTheSameBank = true;
						break;
					}
			}
		}

		if (num != 2 && onTheSameBank ==true) {
			gameobject.transform.parent = sceneController.boat.transform;
			if (sceneController.passenger [0] == null) {
				gameobject.transform.localPosition = new Vector3 (0, 1, 0);
				sceneController.passenger [0] = gameobject;
			} 
			else {
				gameobject.transform.localPosition = new Vector3 (0, 2, 0);
				sceneController.passenger [1] = gameobject;
			}
		}


		this.destroy = true;
		this.callback.SSActionEvent (this);
	}
}




public class CCOffBoat  : SSAction {
	public Model sceneController;
	public static CCOffBoat GetSSAction(){
		CCOffBoat action = ScriptableObject.CreateInstance<CCOffBoat> ();
		return action;
	}

	public override void Start(){
		sceneController = (Model)SSDirector.getInstance ().currentScenceController;
	}

	public override void Update(){
		Debug.Log ("offBoat");
		for (int j = 0; j < 2; j++) {
			if (gameobject == sceneController.passenger [j]) {
				gameobject.transform.parent = null;
				sceneController.passenger [j] = null; 
				if (gameobject.tag == "Priest") {
					if (sceneController.boat.transform.position == sceneController.leftBoat) {
						for (int i = 0; i < 3; i++)
							if (sceneController.leftPriest [i] == null) {
								sceneController.leftPriest [i] = gameobject;
								gameobject.transform.localPosition = sceneController.leftPriestPosition [i];
								break;
							}
					} 
					else {
						for (int i = 0; i < 3; i++)
							if (sceneController.rightPriest [i] == null) {
								sceneController.rightPriest [i] = gameobject;
								gameobject.transform.localPosition = sceneController.rightPriestPosition [i];
								break;
							}
					}
				} 
				else {
					if (sceneController.boat.transform.position == sceneController.leftBoat) {
						for (int i = 0; i < 3; i++)
							if (sceneController.leftDevil [i] == null) {
								sceneController.leftDevil [i] = gameobject;
								gameobject.transform.position = sceneController.leftDevilPosition [i];
								break;
							}
					} 
					else {
						for (int i = 0; i < 3; i++)
							if (sceneController.rightDevil [i] == null) {
								sceneController.rightDevil [i] = gameobject;
								gameobject.transform.position = sceneController.rightDevilPosition [i];
								break;
							}
					}
				}
			}
		}
		sceneController.checkWin ();

		this.destroy = true;
		this.callback.SSActionEvent (this);
	}
}




public class CCMoveBoat  : SSAction {
	public Model sceneController;
	public static CCMoveBoat GetSSAction(){
		CCMoveBoat action = ScriptableObject.CreateInstance<CCMoveBoat> ();
		return action;
	}

	public override void Start(){
		sceneController = (Model)SSDirector.getInstance ().currentScenceController;
	}

	public override void Update(){
		Debug.Log("moveBoat");
		int num = 0;
		for (int i = 0; i < 2; i++)
			if (sceneController.passenger [i] != null)
				num++;
		if (num != 0) {
			if (sceneController.boat.transform.position == sceneController.rightBoat)
				sceneController.boat.transform.position = sceneController.leftBoat;
			else sceneController.boat.transform.position = sceneController.rightBoat;
		}		
		sceneController.checkLose ();

		this.destroy = true;
		this.callback.SSActionEvent (this);
	}
}

}