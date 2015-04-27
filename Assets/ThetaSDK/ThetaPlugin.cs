/*
 * ThetaPlugin.cs
 * 
 * Copyright (C) 2015 Noshipu
 * URL:https://twitter.com/noshipu
 * 
 */

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class ThetaPlugin : MonoBehaviour
{
#if UNITY_IOS
	private IntPtr theta;
	
	[DllImport("__Internal")]
	private static extern IntPtr Theta_Connect (string ip_address, string game_object_name, 
	                                          ConnectSuccess success, ConnectError error);
	[DllImport("__Internal")]
	private static extern void Theta_DisConnect (IntPtr instance, DisConnectSuccess success, DisConnectError error);
	[DllImport("__Internal")]
	private static extern void Theta_Capture (IntPtr instance, CaptureSuccess success, CaptureError error);
	[DllImport("__Internal")]
	private static extern void Theta_Capture (IntPtr instance, LoadHandleCallback callback);

	// Call Back Method
	private delegate void ConnectSuccess ();
	private delegate void ConnectError ();
	private delegate void DisConnectSuccess ();
	private delegate void DisConnectError ();
	private delegate void CaptureSuccess ();
	private delegate void CaptureError ();
	private delegate void LoadHandleCallback ();

	private static Action connectSuccess;
	private static Action connectError;
	private static Action disConnectSuccess;
	private static Action disConnectError;
	private static Action captureSuccess;
	private static Action captureError;

	// theta`s call callback
	private Action<string> callError; 
	private Action<Texture2D> callAddObject;

	/// <summary>
	/// Theta Connect Method
	/// </summary>
	/// <param name="ip_address">Ip_address.</param>
	/// <param name="success">Success Action</param>
	/// <param name="error">Error Action</param>
	public void Connect (string ip_address, Action success, Action error)
	{
		if (CheckConnectTheta ())
		{
			if(error != null)
			{
				error();
			}
			return;
		}

		connectSuccess = success;
		connectError = error;

		theta = Theta_Connect (ip_address, gameObject.name, ConnectCallbackSuccess, ConnectCallbackError);
	}

	/// <summary>
	/// Theta DisConnect Method
	/// </summary>
	/// <param name="success">Success Action</param>
	/// <param name="error">Error Action</param>
	public void DisConnect (Action success, Action error)
	{
		if (!CheckConnectTheta ())
		{
			if(error != null)
			{
				error();
			}
			return;
		}

		disConnectSuccess = success;
		disConnectError = error;

		Theta_DisConnect (theta, DisConnectCallbackSuccess, DisConnectCallbackError);
		theta = IntPtr.Zero;
	}

	/// <summary>
	/// Theta Capture Method
	/// </summary>
	/// <param name="success">Success Action</param>
	/// <param name="error">Error Action</param>
	public void Capture (Action success, Action error)
	{
		if (!CheckConnectTheta ())
		{
			if(error != null)
			{
				error();
			}
			return;
		}
		Debug.Log (theta);

		captureSuccess = success;
		captureError = error;

		Theta_Capture (theta, CaptureCallbackSuccess, CaptureCallbackError);
	}
	
	/// <summary>
	/// Set the callback ObjectAdd Function.
	/// </summary>
	/// <param name="callback">callback:pram is texture</param>
	public void SetCallbackObjectAdd (Action<Texture2D> callback)
	{
		if (!CheckConnectTheta ())
		{
			return;
		}
		callAddObject = callback;
	}

	/// <summary>
	/// Sets the callback error.
	/// </summary>
	/// <param name="callback">callback:parma is error message</param>
	public void SetCallbackError (Action<string> callback)
	{
		if (!CheckConnectTheta ())
		{
			return;
		}
		callError = callback;
	}

	// theta callback method
	public void CallbackObjectAdd(string image_path)
	{
		Debug.Log ("FILE PATH:"+image_path);
		if (callAddObject != null) 
		{
			if (image_path.Length != 0)
			{
				Texture2D texture = new Texture2D (2048, 1280);
				byte[] imageBytes = File.ReadAllBytes(image_path);
				texture.LoadImage(imageBytes);
				callAddObject(texture);
			}
		}
	}
	public void CallbackError(string error_text)
	{
		if (callError != null) 
		{
			callError(error_text);
		}
	}

	private bool CheckConnectTheta()
	{
		if (theta == IntPtr.Zero)
		{
			Debug.LogError("Don`t connect theta!");
			return false;
		}

		return true;
	}

	// Connect Call Back Method
	[AOT.MonoPInvokeCallbackAttribute(typeof(ConnectSuccess))]
	private void ConnectCallbackSuccess ()
	{
		if (connectSuccess != null) {
			connectSuccess ();
		}
	}
	[AOT.MonoPInvokeCallbackAttribute(typeof(ConnectError))]
	private void ConnectCallbackError ()
	{
		if (connectError != null) {
			connectError ();
		}
	}

	// DisConnect Call Back Method
	[AOT.MonoPInvokeCallbackAttribute(typeof(DisConnectSuccess))]
	private void DisConnectCallbackSuccess ()
	{
		if (disConnectSuccess != null) {
			disConnectSuccess ();
		}
	}
	[AOT.MonoPInvokeCallbackAttribute(typeof(DisConnectError))]
	private void DisConnectCallbackError ()
	{
		if (disConnectError != null) {
			disConnectError ();
		}
	}

	// Capture Call Back Method
	[AOT.MonoPInvokeCallbackAttribute(typeof(CaptureSuccess))]
	private void CaptureCallbackSuccess ()
	{
		if (captureSuccess != null) {
			captureSuccess ();
		}
	}
	[AOT.MonoPInvokeCallbackAttribute(typeof(CaptureError))]
	private void CaptureCallbackError ()
	{
		if (captureError != null) {
			captureError ();
		}
	}
#elif UNITY_ANDROID
	// Android logic
	// please wait...
#endif
}
