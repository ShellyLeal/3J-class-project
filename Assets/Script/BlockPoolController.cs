﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlockPoolController : MonoBehaviour {
	const int POOL_X = 6;      // width
	const int POOL_Y = 10;     // height
	const int POOL_Z = POOL_X; // depth
	GameObject[,,] blockPool = new GameObject[POOL_X, POOL_Y, POOL_Z];
	GameObject ground, poolCubes;

	// Use this for initialization
	void Start() {
		ground = GameObject.Find("BlockPool/Ground");
		poolCubes = GameObject.Find("BlockPool/Cubes");
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public void ControlBlock(GameObject block) {
		MergeBlock(block);

		SearchCubePos();

		// remove completed row
		RemoveCompletedRow();

		/*
		if (RemoveCompletedRow()) {
			InitPool();
			// wait
			// SearchCubePos();
		}
		*/
		
		print("done");
	}

	// return the 4 walls position
	public Dictionary<string, float> GetWallPosition() {
		Dictionary<string, float> wallPosition = new Dictionary<string, float>();

		GameObject wallXMin = GameObject.Find("BlockPool/Wall(x-min)");
		GameObject wallXMax = GameObject.Find("BlockPool/Wall(x-max)");
		GameObject wallZMin = GameObject.Find("BlockPool/Wall(z-min)");
		GameObject wallZMax = GameObject.Find("BlockPool/Wall(z-max)");
		wallPosition["x-min"] = wallXMin.transform.position.x;
		wallPosition["x-max"] = wallXMax.transform.position.x;
		wallPosition["z-min"] = wallZMin.transform.position.z;
		wallPosition["z-max"] = wallZMax.transform.position.z;
		// print("Wall x min-max : " + wallPos["x-min"] + "..." + wallPos["x-max"]);
		// print("Wall z min-max : " + wallPos["z-min"] + "..." + wallPos["z-max"]);
		return wallPosition;
	}

	// private methods ------------------------------

	// init the pool block
	private void InitPool() {
		for (int z = 0; z < POOL_Z; z++) {
			for (int y = 0; y < POOL_X; y++) {
				for (int x = 0; x < POOL_X; x++) {
					blockPool[x, y, z] = null;
				}
			}
		}
	}

	// merge block cubes in BlockPool/Cubes
	private void MergeBlock(GameObject block) {
		// move block cubes into poolCubes
		block.tag = "BlockPool";
		block.name = "Cube";
		block.transform.parent = poolCubes.transform;
		// remove rigitbody
		Destroy(block.GetComponent<Rigidbody>());

		var blockCubes = new ArrayList();
		foreach (Transform cube in block.transform) {
			blockCubes.Add(cube);
		}
		foreach (Transform cube in blockCubes) {
			cube.tag = "BlockPool";
			cube.transform.parent = poolCubes.transform;
		}
	}

	// collect each position of block's cubes
	private void SearchCubePos() {
		Dictionary<string, float> wallPos = GetWallPosition();

		Vector3 offset = new Vector3(0, 0, 0);
		offset.x = -wallPos["x-min"];
		offset.z = -wallPos["z-min"];
		offset.y = -ground.transform.position.y;

		foreach (Transform cube in poolCubes.transform) {
			SetCubePos(cube, offset);
		}
	}

	// set the cube position to blockPool
	private void SetCubePos(Transform obj, Vector3 offset) {
		float halfOfWidth = 0.5f;
		int x = (int)Mathf.Round(obj.position.x + offset.x - halfOfWidth);
		int y = (int)Mathf.Round(obj.position.y + offset.y - halfOfWidth);
		int z = (int)Mathf.Round(obj.position.z + offset.z - halfOfWidth);
		// print("offset: " + offset);
		// print("target: " + obj.transform.position);
		// print("index : >> " + new Vector3(x, y, z));
		blockPool[x, y, z] = obj.gameObject;
	}

	private bool RemoveCompletedRow() {
		// TODO:
		//   1. Check every completed row
		//   2. Remove it
		//   3. After every cubes is landed, jump to 1.

		bool hasCompletedRow = false;
		bool[,,] willRemoveCube = new bool[POOL_X, POOL_Y, POOL_Z];

		// check completed row
		for (int z = 0; z < POOL_Z; z++) {
			for (int y = 0; y < POOL_Y; y++) {
				bool isCompleted = true;
				for (int x = 0; x < POOL_X; x++) {
					if (blockPool[x, y, z] == null) isCompleted = false;
				}
				if (!isCompleted) continue;

				// check completed row
				hasCompletedRow = true;
				for (int x = 0; x < POOL_X; x++) {
					willRemoveCube[x, y, z] = true;
				}
			}
		}
		for (int x = 0; x < POOL_X; x++) {
			for (int y = 0; y < POOL_Y; y++) {
				bool isCompleted = true;
				for (int z = 0; z < POOL_Z; z++) {
					if (blockPool[x, y, z] == null) isCompleted = false;
				}
				if (!isCompleted) continue;

				// check completed row
				hasCompletedRow = true;
				for (int z = 0; z < POOL_Z; z++) {
					willRemoveCube[x, y, z] = true;
				}
			}
		}
		
		if (!hasCompletedRow) return false;

		// destroy completed row
		for (int z = 0; z < POOL_Z; z++) {
			for (int y = 0; y < POOL_Y; y++) {
				for (int x = 0; x < POOL_X; x++) {
					if (willRemoveCube[x, y, z] == true) 
						Destroy(blockPool[x, y, z]);
				}
			}
		}

		return true;
	}

	private void FullPool() {

	}

	private void NextPhase() {

	}
}










