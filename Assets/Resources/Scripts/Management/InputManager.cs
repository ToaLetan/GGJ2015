﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct GamepadControls
{
    public string buttonA;
    public string buttonB;
    public string buttonX;
    public string buttonY;

    public string leftBumper;
    public string rightBumper;
    public string leftRightTriggers;

    public string startButton;
    public string backButton;

    public string leftThumbstickClick;
    public string rightThumbstickClick;

    public string leftThumbstickHorizontal;
    public string leftThumbstickVertical;
    public string rightThumbstickHorizontal;
    public string rightThumbstickVertical;
    public string dPadHorizontal;
    public string dPadVertical;

    public string controllerIdentifier;

    public int controllerNumber;
}

public struct Keybinds
{
    //Tentacle selection, ABXY equivalents
    public KeyCode buttonAKey;
    public KeyCode buttonBKey;
    public KeyCode buttonXKey;
    public KeyCode buttonYKey;

    //Movement keys, equivalent to the thumbstick
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    //RightTrigger equivalent
    public KeyCode rightTriggerKey;

    //Start equivalent
    public KeyCode startKey;
}

public class InputManager
{
    //Input Constants
    private const string BUTTON_A = "Button A";
    private const string BUTTON_B = "Button B";
    private const string BUTTON_X = "Button X";
    private const string BUTTON_Y = "Button Y";

    private const string LEFT_BUMPER = "Left Bumper";
    private const string RIGHT_BUMPER = "Left Bumper";
    private const string LEFT_RIGHT_TRIGGERS = "Left/Right Triggers";

    private const string BUTTON_START = "Button Start";
    private const string BUTTON_BACK = "Button Back";

    private const string LEFT_THUMBSTICK_CLICK = "Left Stick Click";
    private const string RIGHT_THUMBSTICK_CLICK = "Right Stick Click";

    private const string LEFT_THUMBSTICK_HORIZONTAL = "Left Stick Horizontal";
    private const string LEFT_THUMBSTICK_VERTICAL = "Left Stick Vertical";
    private const string RIGHT_THUMBSTICK_HORIZONTAL = "Right Stick Horizontal";
    private const string RIGHT_THUMBSTICK_VERTICAL = "Right Stick Vertical";
    private const string DPAD_HORIZONTAL = "D-Pad Horizontal";
    private const string DPAD_VERTICAL = "D-Pad Vertical";

    private const string CONTROLLER_NAME = "Controller";

    //Controller events
    public delegate void ButtonPressEvent(List<string> buttonsPressed);
    public delegate void ButtonHoldEvent(List<string> buttonsHeld);
    public delegate void ButtonReleaseEvent(List<string> buttonsReleased);

    public delegate void AxisEvent(int controllerNum, Vector2 axisValues);
    public delegate void TriggerEvent(int controllerNum, float triggerValue);

    public event ButtonPressEvent Button_Pressed;
    public event ButtonHoldEvent Button_Held;
    public event ButtonReleaseEvent Button_Released;

    public event AxisEvent Left_Thumbstick_Axis;
    public event AxisEvent Right_Thumbstick_Axis;
    public event AxisEvent DPad_Axis;
    public event TriggerEvent Left_Trigger_Axis;
    public event TriggerEvent Right_Trigger_Axis;

    //Keyboard events
    public delegate void KeyPressEvent(List<string> keysPressed);
    public delegate void KeyHoldEvent(List<string> keysHeld);
    public delegate void KeyReleaseEvent(List<string> keysReleased);

    public event KeyPressEvent Key_Pressed;
    public event KeyHoldEvent Key_Held;
    public event KeyReleaseEvent Key_Released;

    public GamepadControls[] ControllerArray = new GamepadControls[2]; //Supporting two controllers.
    public Keybinds[] KeybindArray = new Keybinds[2]; //Supporting two keyboard users.

    private static InputManager instance = null;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
                instance = new InputManager();
            return instance;
        }
    }

    // Use this for initialization
    public InputManager() 
    {
        //Establish controller buttons.
        for (int i = 0; i < ControllerArray.Length; i++)
        {
            ControllerArray[i].controllerNumber = i + 1;
            ControllerArray[i].controllerIdentifier = CONTROLLER_NAME + ControllerArray[i].controllerNumber + " ";
            ControllerArray[i].buttonA = ControllerArray[i].controllerIdentifier + BUTTON_A;
            ControllerArray[i].buttonB = ControllerArray[i].controllerIdentifier + BUTTON_B;
            ControllerArray[i].buttonX = ControllerArray[i].controllerIdentifier + BUTTON_X;
            ControllerArray[i].buttonY = ControllerArray[i].controllerIdentifier + BUTTON_Y;

            ControllerArray[i].leftBumper = ControllerArray[i].controllerIdentifier + LEFT_BUMPER;
            ControllerArray[i].rightBumper = ControllerArray[i].controllerIdentifier + RIGHT_BUMPER;
            ControllerArray[i].leftRightTriggers = ControllerArray[i].controllerIdentifier + LEFT_RIGHT_TRIGGERS;

            ControllerArray[i].startButton = ControllerArray[i].controllerIdentifier + BUTTON_START;
            ControllerArray[i].backButton = ControllerArray[i].controllerIdentifier + BUTTON_BACK;

            ControllerArray[i].leftThumbstickClick = ControllerArray[i].controllerIdentifier + LEFT_THUMBSTICK_CLICK;
            ControllerArray[i].rightThumbstickClick = ControllerArray[i].controllerIdentifier + RIGHT_THUMBSTICK_CLICK;

            ControllerArray[i].leftThumbstickHorizontal = ControllerArray[i].controllerIdentifier + LEFT_THUMBSTICK_HORIZONTAL;
            ControllerArray[i].leftThumbstickVertical = ControllerArray[i].controllerIdentifier + LEFT_THUMBSTICK_VERTICAL;
            ControllerArray[i].rightThumbstickHorizontal = ControllerArray[i].controllerIdentifier + RIGHT_THUMBSTICK_HORIZONTAL;
            ControllerArray[i].rightThumbstickVertical = ControllerArray[i].controllerIdentifier + RIGHT_THUMBSTICK_VERTICAL;
            ControllerArray[i].dPadHorizontal = ControllerArray[i].controllerIdentifier + DPAD_HORIZONTAL;
            ControllerArray[i].dPadVertical = ControllerArray[i].controllerIdentifier + DPAD_VERTICAL;
        }

        //Establish keybinds. 0 Is left side, 1 is right side of keyboard.
        //Keyboard Left side
        KeybindArray[0].buttonAKey = KeyCode.Alpha1;
        KeybindArray[0].buttonBKey = KeyCode.Alpha2;
        KeybindArray[0].buttonXKey = KeyCode.Alpha3;
        KeybindArray[0].buttonYKey = KeyCode.Alpha4;

        KeybindArray[0].upKey = KeyCode.W;
        KeybindArray[0].downKey = KeyCode.S;
        KeybindArray[0].leftKey = KeyCode.A;
        KeybindArray[0].rightKey = KeyCode.D;

        KeybindArray[0].rightTriggerKey = KeyCode.F;

        KeybindArray[0].startKey = KeyCode.Return;

        //Keyboard Right side
        KeybindArray[1].buttonAKey = KeyCode.Alpha7;
        KeybindArray[1].buttonBKey = KeyCode.Alpha8;
        KeybindArray[1].buttonXKey = KeyCode.Alpha9;
        KeybindArray[1].buttonYKey = KeyCode.Alpha0;

        KeybindArray[1].upKey = KeyCode.I;
        KeybindArray[1].downKey = KeyCode.K;
        KeybindArray[1].leftKey = KeyCode.J;
        KeybindArray[1].rightKey = KeyCode.L;

        KeybindArray[1].rightTriggerKey = KeyCode.Semicolon;

        KeybindArray[1].startKey = KeyCode.Escape;
    }
    
    // Update is called once per frame
    public void Update () 
    {
        UpdateButtonInput();
        UpdateKeyboardInput();
    }

    private void UpdateButtonInput()
    {
        for (int i = 0; i < ControllerArray.Length; i++)
        {
            //========================== Pressed Buttons ==========================
            List<string> allButtonsPressed = new List<string>();

            if (Input.GetButtonDown(ControllerArray[i].buttonA) )
                allButtonsPressed.Add(ControllerArray[i].buttonA);
            if (Input.GetButtonDown(ControllerArray[i].buttonB) )
                allButtonsPressed.Add(ControllerArray[i].buttonB);
            if (Input.GetButtonDown(ControllerArray[i].buttonX) )
                allButtonsPressed.Add(ControllerArray[i].buttonX);
            if (Input.GetButtonDown(ControllerArray[i].buttonY) )
                allButtonsPressed.Add(ControllerArray[i].buttonY);

            if (Input.GetButtonDown(ControllerArray[i].leftBumper) )
                allButtonsPressed.Add(ControllerArray[i].leftBumper);
            if (Input.GetButtonDown(ControllerArray[i].rightBumper) )
                allButtonsPressed.Add(ControllerArray[i].rightBumper);

            if (Input.GetButtonDown(ControllerArray[i].startButton) )
                allButtonsPressed.Add(ControllerArray[i].startButton);
            if (Input.GetButtonDown(ControllerArray[i].backButton) )
                allButtonsPressed.Add(ControllerArray[i].backButton);

            if (Input.GetButtonDown(ControllerArray[i].leftThumbstickClick) )
                allButtonsPressed.Add(ControllerArray[i].leftThumbstickClick);
            if (Input.GetButtonDown(ControllerArray[i].rightThumbstickClick) )
                allButtonsPressed.Add(ControllerArray[i].rightThumbstickClick);


            if (allButtonsPressed.Count > 0)
            {
                if (Button_Pressed != null)
                    Button_Pressed(allButtonsPressed);
            }
            //====================================================================

            //========================== Held Buttons ==========================
            List<string> allButtonsHeld = new List<string>();

            if (Input.GetButton(ControllerArray[i].buttonA))
                allButtonsHeld.Add(ControllerArray[i].buttonA);
            if (Input.GetButton(ControllerArray[i].buttonB))
                allButtonsHeld.Add(ControllerArray[i].buttonB);
            if (Input.GetButton(ControllerArray[i].buttonX))
                allButtonsHeld.Add(ControllerArray[i].buttonX);
            if (Input.GetButton(ControllerArray[i].buttonY))
                allButtonsHeld.Add(ControllerArray[i].buttonY);

            if (Input.GetButton(ControllerArray[i].leftBumper))
                allButtonsHeld.Add(ControllerArray[i].leftBumper);
            if (Input.GetButton(ControllerArray[i].rightBumper))
                allButtonsHeld.Add(ControllerArray[i].rightBumper);

            if (Input.GetButton(ControllerArray[i].startButton))
                allButtonsHeld.Add(ControllerArray[i].startButton);
            if (Input.GetButton(ControllerArray[i].backButton))
                allButtonsHeld.Add(ControllerArray[i].backButton);

            if (Input.GetButton(ControllerArray[i].leftThumbstickClick))
                allButtonsHeld.Add(ControllerArray[i].leftThumbstickClick);
            if (Input.GetButton(ControllerArray[i].rightThumbstickClick))
                allButtonsHeld.Add(ControllerArray[i].rightThumbstickClick);

            if (allButtonsHeld.Count > 0)
            {
                if (Button_Held != null)
                    Button_Held(allButtonsHeld);
            }
            //====================================================================

            //========================== Released Buttons ==========================
            List<string> allButtonsReleased = new List<string>();

            if (!Input.GetButton(ControllerArray[i].buttonA))
                allButtonsReleased.Add(ControllerArray[i].buttonA);
            if (!Input.GetButton(ControllerArray[i].buttonB))
                allButtonsReleased.Add(ControllerArray[i].buttonB);
            if (!Input.GetButton(ControllerArray[i].buttonX))
                allButtonsReleased.Add(ControllerArray[i].buttonX);
            if (!Input.GetButton(ControllerArray[i].buttonY))
                allButtonsReleased.Add(ControllerArray[i].buttonY);

            if (!Input.GetButton(ControllerArray[i].leftBumper))
                allButtonsReleased.Add(ControllerArray[i].leftBumper);
            if (!Input.GetButton(ControllerArray[i].rightBumper))
                allButtonsReleased.Add(ControllerArray[i].rightBumper);

            if (!Input.GetButton(ControllerArray[i].startButton))
                allButtonsReleased.Add(ControllerArray[i].startButton);
            if (!Input.GetButton(ControllerArray[i].backButton))
                allButtonsReleased.Add(ControllerArray[i].backButton);

            if (!Input.GetButton(ControllerArray[i].leftThumbstickClick))
                allButtonsReleased.Add(ControllerArray[i].leftThumbstickClick);
            if (!Input.GetButton(ControllerArray[i].rightThumbstickClick))
                allButtonsReleased.Add(ControllerArray[i].rightThumbstickClick);

            if (allButtonsReleased.Count > 0)
            {
                if (Button_Released != null)
                    Button_Released(allButtonsReleased);
            }
            //====================================================================

            //========================== Thumbstick and Trigger Axes ==========================
            if (Left_Thumbstick_Axis != null)
                Left_Thumbstick_Axis(ControllerArray[i].controllerNumber, new Vector2(Input.GetAxis(ControllerArray[i].leftThumbstickHorizontal), Input.GetAxis(ControllerArray[i].leftThumbstickVertical) ) );

            if (Right_Thumbstick_Axis != null)
                Right_Thumbstick_Axis(ControllerArray[i].controllerNumber, new Vector2(Input.GetAxis(ControllerArray[i].rightThumbstickHorizontal), Input.GetAxis(ControllerArray[i].rightThumbstickVertical)));

            if (DPad_Axis != null)
                DPad_Axis(ControllerArray[i].controllerNumber, new Vector2(Input.GetAxis(ControllerArray[i].dPadHorizontal), Input.GetAxis(ControllerArray[i].dPadVertical)));

            if (Left_Trigger_Axis != null)
            {
                if (Input.GetAxis(ControllerArray[i].leftRightTriggers) >= 0)
                    Left_Trigger_Axis(ControllerArray[i].controllerNumber, Input.GetAxis(ControllerArray[i].leftRightTriggers));
            }

            if (Right_Trigger_Axis != null)
            {
                if (Input.GetAxis(ControllerArray[i].leftRightTriggers) <= 0)
                    Right_Trigger_Axis(ControllerArray[i].controllerNumber, Input.GetAxis(ControllerArray[i].leftRightTriggers));
            }
            //=================================================================================
        }
    }
    private void UpdateKeyboardInput()
    {
        for (int i = 0; i < KeybindArray.Length; i++)
        {

            //========================== Pressed Keys ==========================
            List<string> allKeysPressed = new List<string>();

            if (Input.GetKeyDown(KeybindArray[i].buttonAKey))
                allKeysPressed.Add(KeybindArray[i].buttonAKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].buttonBKey))
                allKeysPressed.Add(KeybindArray[i].buttonBKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].buttonXKey))
                allKeysPressed.Add(KeybindArray[i].buttonXKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].buttonYKey))
                allKeysPressed.Add(KeybindArray[i].buttonYKey.ToString());

            if (Input.GetKeyDown(KeybindArray[i].upKey))
                allKeysPressed.Add(KeybindArray[i].upKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].downKey))
                allKeysPressed.Add(KeybindArray[i].downKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].leftKey))
                allKeysPressed.Add(KeybindArray[i].leftKey.ToString());
            if (Input.GetKeyDown(KeybindArray[i].rightKey))
                allKeysPressed.Add(KeybindArray[i].rightKey.ToString());

            if (Input.GetKeyDown(KeybindArray[i].rightTriggerKey))
                allKeysPressed.Add(KeybindArray[i].rightTriggerKey.ToString());

            if (Input.GetKeyDown(KeybindArray[i].startKey))
                allKeysPressed.Add(KeybindArray[i].startKey.ToString());

            if (allKeysPressed.Count > 0)
            {
                if (Key_Pressed != null)
                    Key_Pressed(allKeysPressed);
            }
            //=================================================================================

            //========================== Held Keys ==========================
            List<string> allKeysHeld = new List<string>();

            if (Input.GetKey(KeybindArray[i].buttonAKey))
                allKeysHeld.Add(KeybindArray[i].buttonAKey.ToString());
            if (Input.GetKey(KeybindArray[i].buttonBKey))
                allKeysHeld.Add(KeybindArray[i].buttonBKey.ToString());
            if (Input.GetKey(KeybindArray[i].buttonXKey))
                allKeysHeld.Add(KeybindArray[i].buttonXKey.ToString());
            if (Input.GetKey(KeybindArray[i].buttonYKey))
                allKeysHeld.Add(KeybindArray[i].buttonYKey.ToString());

            if (Input.GetKey(KeybindArray[i].upKey))
                allKeysHeld.Add(KeybindArray[i].upKey.ToString());
            if (Input.GetKey(KeybindArray[i].downKey))
                allKeysHeld.Add(KeybindArray[i].downKey.ToString());
            if (Input.GetKey(KeybindArray[i].leftKey))
                allKeysHeld.Add(KeybindArray[i].leftKey.ToString());
            if (Input.GetKey(KeybindArray[i].rightKey))
                allKeysHeld.Add(KeybindArray[i].rightKey.ToString());

            if (Input.GetKey(KeybindArray[i].rightTriggerKey))
                allKeysHeld.Add(KeybindArray[i].rightTriggerKey.ToString());

            if (Input.GetKey(KeybindArray[i].startKey))
                allKeysHeld.Add(KeybindArray[i].startKey.ToString());

            if (allKeysHeld.Count > 0)
            {
                if (Key_Held != null)
                    Key_Held(allKeysHeld);
            }
            //=================================================================================

            //========================== Released Keys ==========================
            List<string> allKeysReleased = new List<string>();

            if (!Input.GetKey(KeybindArray[i].buttonAKey))
                allKeysReleased.Add(KeybindArray[i].buttonAKey.ToString());
            if (!Input.GetKey(KeybindArray[i].buttonBKey))
                allKeysReleased.Add(KeybindArray[i].buttonBKey.ToString());
            if (!Input.GetKey(KeybindArray[i].buttonXKey))
                allKeysReleased.Add(KeybindArray[i].buttonXKey.ToString());
            if (!Input.GetKey(KeybindArray[i].buttonYKey))
                allKeysReleased.Add(KeybindArray[i].buttonYKey.ToString());

            if (!Input.GetKey(KeybindArray[i].upKey))
                allKeysReleased.Add(KeybindArray[i].upKey.ToString());
            if (!Input.GetKey(KeybindArray[i].downKey))
                allKeysReleased.Add(KeybindArray[i].downKey.ToString());
            if (!Input.GetKey(KeybindArray[i].leftKey))
                allKeysReleased.Add(KeybindArray[i].leftKey.ToString());
            if (!Input.GetKey(KeybindArray[i].rightKey))
                allKeysReleased.Add(KeybindArray[i].rightKey.ToString());

            if (!Input.GetKey(KeybindArray[i].rightTriggerKey))
                allKeysReleased.Add(KeybindArray[i].rightTriggerKey.ToString());

            if (!Input.GetKey(KeybindArray[i].startKey))
                allKeysReleased.Add(KeybindArray[i].startKey.ToString());

            if (allKeysReleased.Count > 0)
            {
                if (Key_Released != null)
                    Key_Released(allKeysReleased);
            }
            //=================================================================================
        }
    }

}
