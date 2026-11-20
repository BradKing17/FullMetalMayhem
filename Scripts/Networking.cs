using Godot;
using GodotPlugins.Game;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;


[GlobalClass]
public partial class Networking : Node
{	


	private ENetMultiplayerPeer enetPeer = new ENetMultiplayerPeer();

	[Export] public Node tubeClient;
	public bool tubeClientEnabled = true;
	private int port = 9999;
	private string ipAddress = "127.0.0.1";

	public Node main;

	public override void _Ready()
	{
		Node currentScene = GetTree().CurrentScene;
		tubeClient = currentScene.GetNode<Node>("TubeClient");
	}

	public void CreateTube()
	{
		Multiplayer.PeerConnected += (id) => AddPlayer((int)id);
		Multiplayer.PeerDisconnected += (id) => RemovePlayer((int)id);
		tubeClient.Call("create_session");
		AddPlayer(1);
	}

	public void JoinTube(string sessionId)
	{
		Multiplayer.PeerConnected += (id) => AddPlayer((int)id);
		Multiplayer.PeerDisconnected += (id) => RemovePlayer((int)id);
		Multiplayer.ConnectedToServer += () => OnConnectedToServer();
		tubeClient.Call("join_session", sessionId);
	}
 	public void StartServer()
	{
		enetPeer.CreateServer(port, 10);
		Multiplayer.MultiplayerPeer = enetPeer;
		GD.Print("Server started on port: " + port);
		Multiplayer.PeerConnected += (id) => AddPlayer((int)id);
		Multiplayer.PeerDisconnected += (id) => RemovePlayer((int)id);	
	}

	public void JoinServer()
	{
		enetPeer.CreateClient(ipAddress, port);
		Multiplayer.PeerConnected += (id) => AddPlayer((int)id);
		Multiplayer.PeerDisconnected += (id) => RemovePlayer((int)id);
		Multiplayer.ConnectedToServer += () => OnConnectedToServer();
		Multiplayer.MultiplayerPeer = enetPeer;
		GD.Print("Connected to server at: " + ipAddress + ":" + port);
	} 

	public void LeaveServer()
	{
		if(tubeClientEnabled)
		{
			tubeClient.Call("leave_session");
		}
		Multiplayer.MultiplayerPeer.Close();
		Multiplayer.MultiplayerPeer = null;
		GD.Print("Disconnected from server.");
		CleanUpSignals();
		GetTree().ReloadCurrentScene();

	}

	private void OnConnectedToServer()
	{
		AddPlayer(Multiplayer.GetUniqueId());
		GD.Print("Successfully connected to the server.");
	}

	private void AddPlayer(int id)
	{
		if((id == 1) && Multiplayer.MultiplayerPeer is ENetMultiplayerPeer)
		{
			return;
		}

		GD.Print("Player connected with ID: " + id);

		var newPlayer = GD.Load<PackedScene>("res://Scenes/Player/Player.tscn").Instantiate();
		newPlayer.Name = "" + id;
		GetTree().CurrentScene.AddChild(newPlayer, true);
		
		
	}

	private void RemovePlayer(int id)
	{
		if(id == 1)
		{
			LeaveServer();
		}

		GD.Print("Player disconnected with ID: " + id);
		var playerNode = GetTree().CurrentScene.GetNodeOrNull<Node>(new NodePath(id.ToString()));
		if (playerNode != null)
		{
			playerNode.QueueFree();
		}
	}

	public void CleanUpSignals()
	{
		Multiplayer.PeerConnected -= (id) => AddPlayer((int)id);
		Multiplayer.PeerDisconnected -= (id) => RemovePlayer((int)id);
		Multiplayer.ConnectedToServer -= () => OnConnectedToServer();
	}

	private  void ExitTree()
	{
		if(tubeClientEnabled)
		{
			tubeClient.Call("leave_session");
		}
	}
}
