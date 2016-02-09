//========= Copyright 2014, Valve Corporation, All rights reserved. ===========
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedObject : MonoBehaviour
{
	public enum EIndex
	{
		None = -1,
		Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
		Device1,
		Device2,
		Device3,
		Device4,
		Device5,
		Device6,
		Device7,
		Device8,
		Device9,
		Device10,
		Device11,
		Device12,
		Device13,
		Device14,
		Device15
	}

	public EIndex index;
	public Transform origin; // if not set, relative to parent
    public bool isValid = false;
	public GameObject bulletPrefab;

	float attackSpeed = 0.5f;
	float cooldown;

	private Vector3 offset;
	private Quaternion angles;



	private void OnNewPoses(params object[] args)
	{
		if (index == EIndex.None)
			return;

		var i = (int)index;

        isValid = false;
		var poses = (Valve.VR.TrackedDevicePose_t[])args[0];
		if (poses.Length <= i)
			return;

		if (!poses[i].bDeviceIsConnected)
			return;

		if (!poses[i].bPoseIsValid)
			return;

        isValid = true;

		var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

		if (origin != null)
		{
			pose = new SteamVR_Utils.RigidTransform(origin) * pose;
			pose.pos.x *= origin.localScale.x;
			pose.pos.y *= origin.localScale.y;
			pose.pos.z *= origin.localScale.z;
			transform.position = pose.pos;
			transform.rotation = pose.rot;
		}
		else
		{
			transform.localPosition = pose.pos + pose.rot*offset;
			transform.localRotation = pose.rot*angles;

		}

		var vr = SteamVR.instance;
		var isController = (vr.hmd.GetTrackedDeviceClass((uint)i) == ETrackedDeviceClass.Controller);
		var state = new VRControllerState_t();
		var success = vr.hmd.GetControllerState((uint)i, ref state);
		var triggerPressed = (state.ulButtonPressed & SteamVR_Controller.ButtonMask.Trigger) != 0;

		if (Time.time >= cooldown) {
			if (triggerPressed) {
				Debug.Log ("trigger pressed");
				GameObject bPrefab = Instantiate(bulletPrefab, pose.pos, pose.rot) as GameObject;
				Rigidbody rb = bPrefab.AddComponent<Rigidbody>(); // Add the rigidbody.
				rb.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
				rb.AddForce (pose.rot*Vector3.down * 3000f);
				cooldown = Time.time + attackSpeed;
			}
		}

	}

	void OnEnable()
	{
		SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
		SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
	}

	void OnDisable()
	{
		SteamVR_Utils.Event.Remove("new_poses", OnNewPoses);
		SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);

	}

	public void SetDeviceIndex(int index)
	{
		if (System.Enum.IsDefined(typeof(EIndex), index))
			this.index = (EIndex)index;
	}

	// Use this for initialization
	void Start () {
		offset = transform.position;
		angles = transform.rotation;
	}
	private void OnDeviceConnected(params object[] args)
	{
		var i = (int)args[0];
		var connected = (bool)args[1];
	}

	// Fire a bullet
	void Fire() {
		GameObject bPrefab = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		Rigidbody rb = bPrefab.AddComponent<Rigidbody>(); // Add the rigidbody.
		rb.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
		rb.AddForce (Vector3.down * 100f);
		cooldown = Time.time + attackSpeed;
	}
}

