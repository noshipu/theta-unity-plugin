using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ThetaManager : MonoBehaviour {

	// iOS側のコードを呼び出すための処理
	[DllImport("__Internal")]
	private static extern void Theta_Connect ();
	[DllImport("__Internal")]
	private static extern void Theta_DisConnect ();
	[DllImport("__Internal")]
	private static extern void Theta_Capture ();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Connect()
	{
		Theta_Connect ();
	}

	public void DisConnect()
	{
		Theta_DisConnect ();
	}

	public void Capture()
	{
		Theta_Capture ();
	}
}
