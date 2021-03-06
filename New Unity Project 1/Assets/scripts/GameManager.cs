﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{

	public static GameManager instance;

	public GameObject TilePrefab;
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;

	public int mapSize = 11;

	public List<List<Tile>> map = new List<List<Tile>>();
	public List<Player> players = new List<Player>();

	public int currentPlayerIndex = 0;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		generateMap ();
		generatePlayers ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnUpdate();
		else nextTurn();
	}
	// video 2 inicio
	void OnGUI ()
	{
		if(players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnOnGUI();
	}
	//video 2 final

	public void nextTurn(){
		if(currentPlayerIndex +1 < players.Count){
			currentPlayerIndex++;
		} else{
			currentPlayerIndex = 0;
		}
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance)
	{
		Debug.Log ("d: "+distance);
		List<Tile> highlightedTiles = TileHighlight.FindHighlights(map[(int)originLocation.x][(int)originLocation.y], distance);

		foreach(Tile t in highlightedTiles)
		{
			t.changeColor(highlightColor);
		}
	}

	public void removeTilesHightlights()
	{
		for(int i = 0; i < mapSize; i++)
		{
			for(int j = 0; j < mapSize; j++)
			{
				if(!map[i][j].impassible) map[i][j].transform.renderer.material.color = Color.white;
			}
		}
	}

	public void moveCurrentPlayer(Tile destinationTile)
	{
		if(destinationTile.transform.renderer.material.color != Color.white && !destinationTile.impassible)
		{
			removeTilesHightlights();
			players[currentPlayerIndex].moving = false;

			foreach(Tile t in TilePathFinder.FindPath(map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y], destinationTile))
			{
				players[currentPlayerIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 1.5f * Vector3.up);
				Debug.Log (players[currentPlayerIndex].positionQueue[players[currentPlayerIndex].positionQueue.Count - 1].x);
			}
			players[currentPlayerIndex].gridPosition = destinationTile.gridPosition;
		}
		else
		{
			Debug.Log ("Destination invalid!");
		}
	}
	// video 2 inicio
	public void attackCurrentPlayer(Tile destinationTile)
	{
		if(destinationTile.transform.renderer.material.color != Color.white && !destinationTile.impassible)
		{
			Player target = null;

			foreach(Player p in players)
			{
				if(p.gridPosition == destinationTile.gridPosition)
				{
					target = p;
				}
			}

			if(target != null)
			{
				if(players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - 1 && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + 1
				   && players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - 1 && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + 1)
				{
					players[currentPlayerIndex].actionPoints--;

					removeTilesHightlights();
					players[currentPlayerIndex].moving = false;
					bool hit = Random.Range(0f,1f) <= players[currentPlayerIndex].attackChance;

					if(hit)
					{
						int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));

						target.HP -= amountOfDamage;

						Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!!");
					}
					else
					{
						Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
					}
				}
				else
				{
					Debug.Log("Target is not adjjacent!");
				}
			}
		}
		else
		{
			Debug.Log ("Destination invalid!");
		}
	}
	//video 2 final

	void generateMap(){
		map = new List<List<Tile>> ();
		for (int i = 0; i < mapSize; i++) {
			List<Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				Tile tile =  ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize/2),0,-j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.gridPosition = new Vector2(i,j);
				row.Add(tile);
			}
			map.Add(row);
		}
	}

	void generatePlayers(){
		UserPlayer player;

		player =  ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(0 - Mathf.Floor(mapSize/2),1.5f,-0 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(0,0);
		player.playerName = "Xico";
		//player.movementPerActionPoint = 2;
		players.Add (player);

		player =  ((GameObject)Instantiate(UserPlayerPrefab, new Vector3((mapSize - 1) - Mathf.Floor(mapSize/2),1.5f, -(mapSize - 1) + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(mapSize - 1,mapSize - 1);
		player.playerName = "Bento";
		//player.movementPerActionPoint = 5;
		players.Add (player);

		player =  ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(4 - Mathf.Floor(mapSize/2),1.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(4,4);
		player.playerName = "Cebolinha";
		//player.movementPerActionPoint = 3;
		players.Add (player);

		//AIPlayer aiplayer =  ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(6 - Mathf.Floor(mapSize/2),1.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		
		//players.Add (aiplayer);
	}
}