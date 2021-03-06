using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.LEGO.Minifig;
using Cinemachine;

public class GhostData
{
    public float speed = 0.1f;
    public float sensitivity = 2.0f;
    public float floor = 1.0f;
    public float deadzone = 0.1f;
    public Vector3 direction = new Vector3(0, 0, 0);
    public Vector3 newpos = new Vector3(0, 0, 0);
    public Vector2 rightStick = new Vector2(0, 0);
    public Vector2 leftStick = new Vector2(0, 0);
    public bool rightTrigger = false;
    public bool leftTrigger = false;
}

public class GhostMode : MonoBehaviour
{
    public List<GameObject> PlayableNPCs = new List <GameObject>();

    private RaycastHit hit = new RaycastHit();
    private Ray ray = new Ray();
    private Transform HitNPC = null;
    private Transform CursedNPC = null;
    private Transform PossessedNPC = null;
    private Color CursedNPC_DefaultColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color CursedNPC_HighlightColour = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    private Material[] TempMaterials = null;
    private Vector3 TempFacing = new Vector3(0.0f, 0.0f, 0.0f);
    bool possessed = false;

    private GameObject CameraActual = null;
    private CinemachineBrain CameraBrain = null;
    private CinemachineFreeLook CameraSettings = null;
    private GhostData Ghost = null;

    void Start()
    {
        foreach (GameObject go in PlayableNPCs)
        {
            //register raycast handler
            Debug.Log(go.GetComponents<MonoBehaviour>()[0]);
            go.GetComponents<MinifigController>()[0].inputEnabled = false;
        }
        CameraActual = GameObject.FindWithTag("CameraActual");
        CameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        CameraSettings = CameraActual.GetComponent<CinemachineFreeLook>();
        CameraBrain.enabled = false;
        Ghost = new GhostData();
    }

    void Update()
    {
        //ray = Camera.main.ViewportPointToRay(new Vector3(Input.mousePosition.x / Screen.width,
        //                                                 Input.mousePosition.y / Screen.height, 0.0f)); //Cursor
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)); //Center Screen Crosshairs
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.collider.transform.gameObject.name);
            HitNPC = hit.collider.transform;
        }

        if ((HitNPC != null) && (HitNPC.gameObject.GetComponents<MinifigController>().Length > 0) && (!possessed))
        {
            if (CursedNPC == null)
            {
                CursedNPC = HitNPC;
                TempMaterials = CursedNPC.gameObject.GetComponent<Renderer>().materials;
                if (TempMaterials.Length > 0)
                {
                    CursedNPC_DefaultColour = TempMaterials[0].GetColor("_Color");
                    CursedNPC.gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", CursedNPC_HighlightColour);
                }
            }
            else { CursedNPC = HitNPC; }
        }
        else if (CursedNPC != null)
        {
            TempMaterials = CursedNPC.gameObject.GetComponent<Renderer>().materials;
            if (TempMaterials.Length > 0)
            {
                CursedNPC.gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", CursedNPC_DefaultColour);
                CursedNPC_DefaultColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            CursedNPC = null;
        }

        //Debug.Log(Input.GetAxis("Left Trigger").ToString() + " " + Input.GetAxis("Right Trigger").ToString()); //Axis 9 and 10, not ScrollWheel Axis 3
        Ghost.rightTrigger = Input.GetAxis("Right Trigger") < -0.5f;
        Ghost.leftTrigger = Input.GetAxis("Left Trigger") < -0.5f;
        //Debug.Log(Input.GetKey("joystick button 6")); //Back on Xbox One
        if ((Input.GetMouseButton(0)
        || Input.GetKeyDown(KeyCode.Return)
        || Ghost.rightTrigger
        || Input.GetKeyDown("joystick button 0")) && (CursedNPC != null)) { possessed = true; }
        else if ((Input.GetKeyDown(KeyCode.Backspace) //Escape is in use by Pointer Lock API
        || Input.GetKeyDown("joystick button 1")
        || Input.GetKeyDown("joystick button 6"))) { possessed = false; }

        if ((possessed) && (PossessedNPC == null))
        {
            CursedNPC.gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", CursedNPC_DefaultColour);
            CursedNPC_DefaultColour = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            PossessedNPC = CursedNPC;
            //CursedNPC = null; //Reference Bug, this also mangles PossessedNPC
            //Debug.Log("Possessed: " + PossessedNPC.gameObject.name);
            PossessedNPC.gameObject.GetComponents<MinifigController>()[0].inputEnabled = true;
            CameraSettings.m_LookAt = PossessedNPC;
            CameraSettings.m_Follow = PossessedNPC;
            CameraBrain.enabled = true;
        }
        else if ((!possessed) && (PossessedNPC != null))
        {
            //Debug.Log("Return to Ghost Mode");
            PossessedNPC.gameObject.GetComponents<MinifigController>()[0].inputEnabled = false;
            PossessedNPC = null;
            CameraBrain.enabled = false;
            CameraSettings.m_LookAt = null;
            CameraSettings.m_Follow = null;
            TempFacing = Camera.main.transform.eulerAngles;
            CameraActual.transform.eulerAngles = new Vector3(TempFacing.x, TempFacing.y, 0.0f);
            Camera.main.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        if (!possessed) //Ghost Mode
        {
            CameraActual.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * Ghost.sensitivity, 0, 0), Space.Self);
            CameraActual.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Ghost.sensitivity, 0), Space.World);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    { Ghost.direction += new Vector3( 0,  0,  1); }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  { Ghost.direction += new Vector3( 0,  0, -1); }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  { Ghost.direction += new Vector3(-1,  0,  0); }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { Ghost.direction += new Vector3( 1,  0,  0); }
/*
            Ghost.rightStick = Input.GetAxis("RightStick");
            Ghost.rightStick = new Vector2((Mathf.Abs(Ghost.rightStick.x) > Ghost.deadzone) ? Ghost.rightStick.x * Ghost.sensitivity : 0.0f,
                                           (Mathf.Abs(Ghost.rightStick.y) > Ghost.deadzone) ? Ghost.rightStick.y * Ghost.sensitivity : 0.0f);
            CameraActual.transform.Rotate(new Vector3(-Ghost.rightStick.y, 0, 0), Space.Self);
            CameraActual.transform.Rotate(new Vector3(0, Ghost.rightStick.x, 0), Space.World);
*/
            Ghost.leftStick = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
            Ghost.leftStick = new Vector2((Mathf.Abs(Ghost.leftStick.x) > Ghost.deadzone) ? Ghost.leftStick.x * Ghost.sensitivity : 0.0f,
                                          (Mathf.Abs(Ghost.leftStick.y) > Ghost.deadzone) ? Ghost.leftStick.y * Ghost.sensitivity : 0.0f);
            Ghost.direction += new Vector3(Ghost.leftStick.x, 0, -Ghost.leftStick.y);

            Ghost.direction = Ghost.direction * Ghost.speed;
            CameraActual.transform.Translate(new Vector3(Ghost.direction.x, 0, 0), Space.Self);
            CameraActual.transform.Translate(new Vector3(0, 0, Ghost.direction.z), Space.Self);
            Ghost.newpos = CameraActual.transform.position;
            if (Ghost.newpos.y < Ghost.floor) { Ghost.newpos.y = Ghost.floor; }
            CameraActual.transform.position = Ghost.newpos;
        }
    }
}
