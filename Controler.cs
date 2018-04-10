using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

namespace myGame{
	public class SSDirector: System.Object{
		
		private static SSDirector _instance;
		public Model currentScenceController;

		public static SSDirector getInstance(){
			if (_instance == null)
				_instance = new SSDirector ();
			return _instance;
		}

		public Model getModel(){
			return currentScenceController;
		}

		internal void setModel(Model model){
			currentScenceController = model;
		}
	}
}

public class Controler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
}
