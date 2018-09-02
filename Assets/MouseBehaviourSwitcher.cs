using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehaviourSwitcher : MonoBehaviour {
    public enum MouseControlState { Admin, Picker, Director };
    public MouseControlState startingState;
    private MouseControlState state = MouseControlState.Admin;
    // Use this for initialization

    private void Start()
    {
        state = startingState;
        UpdateControllers();
    }

    // Update is called once per frame
    void Update () {
		
	}
    private void UpdateControllers()
    {
        var picker = transform.GetComponent<PickerController>();
        var admin = transform.GetComponent<MouseController>();
        var director = transform.GetComponent < DirectorController> ();
        switch (state)
        {
            case MouseControlState.Admin:
                director.enabled = false;
                picker.enabled = false;
                admin.enabled = true;
                break;
            case MouseControlState.Picker:
                director.enabled = false;
                admin.enabled = false;
                picker.enabled = true;
                break;
            case MouseControlState.Director:
                admin.enabled = false;
                picker.enabled = false;
                director.enabled = true;
                break;
        }
       
    }
    public void SwitchToAdmin()
    {
        state = MouseControlState.Admin;
        UpdateControllers();
    }
    public void SwitchToDirector()
    {
        state = MouseControlState.Director;
        UpdateControllers();
    }
    public void SwitchToPicker()
    {
        state = MouseControlState.Picker;
        UpdateControllers();
    }
}
