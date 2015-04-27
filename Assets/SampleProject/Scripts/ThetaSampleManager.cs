using UnityEngine;
using System.IO;
using System.Collections;

public class ThetaSampleManager : MonoBehaviour
{
	private ThetaPlugin thetaPlugin;
	private string ipAddress = "192.168.1.1";

	[SerializeField]
	public GameObject viewObject;

	void Start()
	{
		thetaPlugin = GetComponent<ThetaPlugin> ();
	}
	
	public void Connect ()
	{
		thetaPlugin.Connect (
			ipAddress,
 	       	// Success
			() => {
				Debug.Log ("CONNECT:SUCCESS");
			},
			// Error
			() => {
				Debug.Log ("CONNECT:ERROR");
			}
		);


		SetCaptureCallback ();
		SetErrorCallback ();
	}

	public void DisConnect ()
	{
		thetaPlugin.DisConnect (
			// Success
			() => {
				Debug.Log ("DISCONNECT:SUCCESS");
			},
			// Error
			() => {
				Debug.Log ("DISCONNECT:ERROR");
			}
		);
	}
	
	public void Capture ()
	{
		// capture
		thetaPlugin.Capture (
			// Success
			() => {
				Debug.Log ("CAPTURE:SUCCESS");
			},
			// Error
			() => {
				Debug.Log ("CAPTURE:ERROR");
			}
		);
	}

	private void SetCaptureCallback()
	{
		// set call back method
		thetaPlugin.SetCallbackObjectAdd (texture => {
			viewObject.GetComponent<Renderer>().material.mainTexture = texture;
		});
	}

	private void SetErrorCallback()
	{
		// set call back method
		thetaPlugin.SetCallbackError (error_text => {
			Debug.Log ("CALLBACK:ERROR"+error_text);
		});
	}
}