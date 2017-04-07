using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "ChromaShift";
	private const string gameName = "Matts Sweet Game";

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;

	private static NetworkManager instance;
	public static NetworkManager Instance
	{
		get
		{
			if (!instance) 
				instance = GameObject.FindObjectOfType (typeof(NetworkManager)) as NetworkManager;

			return instance;
		}

	}

	public void StartServer(string roomName) 
	{
		
		PhotonNetwork.CreateRoom(roomName);
	}

	public void JoinServer(string roomName)
	{
		
		PhotonNetwork.JoinRoom(roomName);
	}
	public void Connect()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");

	}
	public void Disconnect()
	{
		PhotonNetwork.Disconnect();
	}
	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}
	void OnJoinedRoom()
	{
		Debug.Log("Connected to Room");
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType<NetworkManager>().Length > 1)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}

	
	// Update is called once per frame
	void Update () {
	
	}

//	void OnGUI()
//	{
//		if (!Network.isClient && !Network.isServer)
//		{
//			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
//				StartServer2();
//
//			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
//				RefreshHostList();
//
//			if (hostList != null)
//			{
//				for (int i = 0; i < hostList.Length; i++)
//				{
//					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
//						JoinServer2(hostList[i]);
//				}
//			}
//		}
//	}

}
