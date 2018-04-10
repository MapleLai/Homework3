using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myGame{
public class CCActionManager : SSActionManager, ISSActionCallback  {  
	public Model sceneController;  
	public CCOnBoat onBoatA;  
	public CCOffBoat offBoatB;  
	public CCMoveBoat moveBoatC;

	// Use this for initialization  
	protected void Start () {  
		sceneController = (Model)SSDirector.getInstance().currentScenceController;  
	}  

	// Update is called once per frame  
	protected new void Update () {
		if (Input.GetMouseButtonDown (0) ) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit) && sceneController.isOver == 0) {
				Debug.Log ("click");
				if (hit.transform.tag == "Priest" || hit.transform.tag == "Devil") {
					if (sceneController.passenger [0] == hit.collider.gameObject ||
						sceneController.passenger [1] == hit.collider.gameObject) {
						offBoatB = CCOffBoat.GetSSAction ();
						this.RunAction (hit.collider.gameObject, offBoatB, this);
					} 
					else {
						onBoatA = CCOnBoat.GetSSAction ();
						this.RunAction (hit.collider.gameObject, onBoatA, this);
					}
				} 
				else if (hit.transform.tag == "Boat") {
					moveBoatC = CCMoveBoat.GetSSAction ();
					this.RunAction (hit.collider.gameObject, moveBoatC, this);
				}
			}
		}

		base.Update();
	}

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,  
		int intParam = 0, string strParam = null, Object objectParam = null)  {    
	}  
}

}