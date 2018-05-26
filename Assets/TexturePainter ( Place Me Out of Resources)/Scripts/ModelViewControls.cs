#define DEBUG_GUI
using UnityEngine;
using System.Collections;

public class ModelViewControls : MonoBehaviour {
	private int yMinLimit = 0, yMaxLimit = 80;
	private Quaternion currentRotation, desiredRotation, rotation;
	private float yDeg=15, xDeg=0.0f;
	private float currentDistance,desiredDistance=3.0f,maxDistance = 6.0f,minDistance = 9.0f;
	private Vector3 position;
	public GameObject targetObject,camObject;
	float sensitivity=1.25f;
	void Start () {
		currentDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
	}
	
	// Update is called once per frame
	void Update () {
		CameraControlUpdate ();
	}

    private void OnGUI()
    {
#if DEBUG_GUI

        // Si hay touch en la pantalla entonces usar esto como input para rotar la camara.
        if (Input.touchCount > 0)
        {
            GUILayout.Label(new GUIContent("Delta touch position: " + Input.touches[0].deltaPosition.ToString()));
        }
#endif
    }

    // Replace GetAxis for touch input https://answers.unity.com/questions/504707/replace-input-get-axis-horizontal-with-touch-input.html
    void CameraControlUpdate(){
        // Debug.Log(Input.GetAxis("Horizontal"));
		yDeg+=Input.GetAxis("Vertical")*sensitivity;
		xDeg-=Input.GetAxis("Horizontal")*sensitivity;

        // Si hay touch en la pantalla entonces usar esto como input para rotar la camara.
        if (Input.touchCount > 0)
        {
            Vector2 normPos = Input.touches[0].deltaPosition;
            normPos.Normalize();
            yDeg -= normPos.y * (sensitivity+2f);
            xDeg += normPos.x * (sensitivity+2f);
        }


        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);		
		desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);	
        // Va acercando gradualmente la rotación actual a la rotación deseada
		rotation = Quaternion.Lerp(targetObject.transform.rotation, desiredRotation, 0.05f  );
        // Se guarda el valor de la nueva rotación
		targetObject.transform.rotation = desiredRotation;

		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // Va acercando grdualmente la distancia actual a la distancia deseada, con esto se logra el efecto inicial de la camara cuando la escena se instancia
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.05f  );
        // Matematica usando quaternions, no entiendo el concepto aun.
		position = targetObject.transform.position - (rotation * Vector3.forward * currentDistance );
		Vector3 lerpedPos=Vector3.Lerp(camObject.transform.position, position,0.05f);
		camObject.transform.position = lerpedPos;

	}
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}
