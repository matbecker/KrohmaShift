using UnityEngine;
using System.Collections;
using Rewired;

public class InputManager : MonoBehaviour {


	private static InputManager instance;
	public static InputManager Instance
	{
		get
		{
			if (!instance) 
				instance = GameObject.FindObjectOfType (typeof(InputManager)) as InputManager;

			return instance;
		}

	}

	//delegate for keyDown event
	public delegate void KeyDownEvent(int playerID);
	//delegate for KeyUp event
	public delegate void KeyUpEvent(int playerID);
	//delegate for Mouse event
	public delegate void MouseEvent(int playerID, float x, float y);
	//delegate for Axis event
	public delegate void AxisEvent(int playerID, float axisValue);

	//Input events
	public event KeyDownEvent Jump;
	public event KeyDownEvent DoubleJump;
	public event KeyDownEvent Hover;
	public event KeyDownEvent Attack;
	public event KeyDownEvent Block;
	public event KeyDownEvent SwitchColour;
	public event KeyDownEvent SwitchShade;
	public event KeyDownEvent Pause;
	public event KeyUpEvent UnBlock;
	public event KeyUpEvent UnAttack;
	public event AxisEvent Run;
	public event AxisEvent HorizontalMovement;
	public event AxisEvent VerticalMovement;
	public event MouseEvent TrackMouseEvent;
	public event KeyDownEvent CameraZoomIn;
	public event KeyDownEvent CameraZoomOut;
	public event AxisEvent RotateBow;
	public event AxisEvent SwitchButton;
	public event AxisEvent Slide;
	public event KeyDownEvent Submit;
	private Player[] players;
	private int numPlayers;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
		numPlayers = 2;
		players = new Player[numPlayers];
		for(int i = 0; i < numPlayers; i++)
		{
			players[i]  = ReInput.players.GetPlayer(i);
		}
	}

	void Update()
	{
		for(int playerID = 0; playerID < numPlayers; playerID++)
		{
			var player = players[playerID];
			//if the Jump button is pushed and there are any subscribers
			if (player.GetButtonDown("Jump"))
			{
				//subscribe all different jump event;
				if (Jump != null)
					Jump(playerID);

				if (DoubleJump != null)
					DoubleJump(playerID);

				if (Hover != null)
					Hover(playerID);
			}
			//if the Attack Button is pushed and there are subscribers
			if (player.GetButtonDown("Attack") && Attack != null)
			{
				//Call the Attack event
				Attack(playerID);
			}
			//if the attack button is pushed and there are subscribers
			if (player.GetButtonUp("Attack") && UnAttack != null)
			{
				//Call the UnAttack event
				UnAttack(playerID);
			}
			//if there are any Run Subscribers
			if (Run != null)
			{
				//Call the Run Event and link it to the horiztonal axis
				Run(playerID, player.GetAxis("Run"));
			}
			//if the Block Button is pushed and there are subscribers
			if (player.GetButtonDown("Block") && Block != null)
			{
				//Call the block event
				Block(playerID);
			}
			//if the Block Button is released and there are subscribers
			if (player.GetButtonUp("Block") && UnBlock != null)
			{
				//call the UnBlock event
				UnBlock(playerID);
			}
			//if the Pause Button is pushed and there are subscriberss
			if (player.GetButtonDown("Pause") && Pause != null)
			{
				//Call the pause event
				Pause(playerID);
			}
			//if the SwitchColour button is pushed and there are subscribers
			if (player.GetButtonDown("SwitchColour") && SwitchColour != null)
			{
				//call the SwitchColour event
				SwitchColour(playerID);
			}
			//if the SwitchShade button is pushed and there are subscribers
			if (player.GetButtonDown("SwitchShade") && SwitchShade != null)
			{
				//call the SwitchShade event
				SwitchShade(playerID);
			}
			//if there are subscribers
			if (TrackMouseEvent != null)
			{
				//track the cursor of the mouse
				TrackMouseEvent(playerID, Input.mousePosition.x, Input.mousePosition.y);
			}
			//Editor options
			if (player.GetButton("CameraZoomIn") && CameraZoomIn != null)
			{
				CameraZoomIn(playerID);
			}
			if (player.GetButton("CameraZoomOut") && CameraZoomOut != null)
			{
				CameraZoomOut(playerID);
			}
			if (HorizontalMovement != null)
			{
				HorizontalMovement(playerID, player.GetAxis("Horizontal"));
			}
			if (VerticalMovement != null)
			{
				VerticalMovement(playerID, player.GetAxis("Vertical"));
			}
			if (RotateBow != null)
			{
				RotateBow(playerID, player.GetAxis("RotateBow"));
			}
			if (SwitchButton != null)
			{
				SwitchButton(playerID, player.GetAxis("SwitchButton"));
			}
			if (Slide != null)
			{
				Slide(playerID, player.GetAxis("Slide"));
			}
			if (Submit != null && player.GetButtonDown("Submit"))
			{
				Submit(playerID);
			}
		}
	}
}
