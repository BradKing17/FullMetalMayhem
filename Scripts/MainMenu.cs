using Godot;
using System;
using Godot.Collections;
using System.ComponentModel;

public partial class MainMenu : CanvasLayer
{

	[Export] VBoxContainer tubeMenu;
	[Export] VBoxContainer enetMenu;

	private Button createSessionButton;
	private LineEdit sessionIdInput;
	private LineEdit userNameInput;
	private Button joinButtonTube;
	private Button quitButtonTube;

	[Export] private Button joinButton;
	[Export] private Button quitButton;

	[Export] private PackedScene mainScene;
	[Export] private PackedScene playerScene;

	Networking networking;


	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		networking = GetNode<Networking>("/root/Networking");

		if(networking.tubeClientEnabled)
		{
			enetMenu.Hide();
			tubeMenu.Show();
		}
		else
		{
			tubeMenu.Hide();
			enetMenu.Show();
		}
		createSessionButton = GetNode<Button>("%CreateSessionButton");
		sessionIdInput = GetNode<LineEdit>("%SessionIdInput");
		userNameInput = GetNode<LineEdit>("%UserNameInput");
		joinButtonTube = GetNode<Button>("%JoinButtonTube");
		quitButtonTube = GetNode<Button>("%QuitButtonTube");

		sessionIdInput.TextChanged += (newText) => UpdateSession(newText);
		userNameInput.TextChanged += (newText) => UpdateUserName(newText);

		joinButtonTube.Disabled = true;
		joinButtonTube.Pressed += OnJoinTube;
		quitButtonTube.Pressed += () => GetTree().Quit();
		createSessionButton.Pressed += () => OnCreateTube();

		joinButton.Pressed += OnJoinButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;

		if(OS.HasFeature("server"))
		{
			networking.StartServer();
			await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
			CreateWorld();
		}
	}

	private void UpdateSession(string newText)
	{
		if(newText != "")
		{
			joinButtonTube.Disabled = false;
		}

		GD.Print("Session ID changed to: " + newText);
	}

	private void UpdateUserName(string newText)
	{
		Global.userName = newText;
		GD.Print("User Name changed to: " + newText);
	}

	private void OnCreateTube()
	{
		networking.CreateTube();
		CreateWorld();
	}

	private void OnJoinTube()
	{
		networking.JoinTube(sessionIdInput.Text);
		Multiplayer.ConnectedToServer += () => CreateWorld();
	}

    private void OnJoinButtonPressed()
    {
		networking.JoinServer();
		CreateWorld();
    }

	    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

	private void CreateWorld()
	{
		var scene = mainScene.Instantiate();
		GetTree().CurrentScene.AddChild(scene, true);
		Hide();
	}

	private void OnErrorOccurred(int error, string message)
	{
		sessionIdInput.Text = "";
		joinButtonTube.AddThemeColorOverride("font_color", Color.Color8(255, 0, 0));
		joinButtonTube.Disabled = true;
		networking.CleanUpSignals();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
