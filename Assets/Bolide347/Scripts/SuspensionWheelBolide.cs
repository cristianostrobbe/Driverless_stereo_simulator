using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SuspensionWheelBolide : MonoBehaviour {

	public Transform wheelFL;
	public Transform wheelFR;
	public Transform wheelBL;
	public Transform wheelBR;
	public Transform wheelLook_FL;
	public Transform wheelLook_FR;
	public Transform susLook_FL;
	public Transform susLook_FR;
	public Transform suspensionFL;
	public Transform suspensionFR;
    public Transform suspensionBL;
	public Transform suspensionBR;

	private Vector3 wheelTran;
	private Quaternion lastPos;
	private float trunSpeed = 20f;


	void LateUpdate (){
		suspensionFL.position = wheelFL.position;
		suspensionFR.position = wheelFR.position;
		suspensionBL.position = wheelBL.position;
		suspensionBR.position = wheelBR.position;

		wheelTran = wheelLook_FL.position - susLook_FL.position;
		wheelTran.y = 0f;
		lastPos = Quaternion.LookRotation (-wheelTran);
		susLook_FL.rotation = Quaternion.Slerp (susLook_FL.rotation, lastPos, Time.deltaTime * trunSpeed);

		wheelTran = wheelLook_FR.position - susLook_FR.position;
		wheelTran.y = 0f;
		lastPos = Quaternion.LookRotation (-wheelTran);
		susLook_FR.rotation = Quaternion.Slerp (susLook_FR.rotation, lastPos, Time.deltaTime * trunSpeed);
	
	}
}
