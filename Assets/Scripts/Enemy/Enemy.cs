using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class Enemy {

	protected GameObject player;
	protected NavMeshAgent nav;
	protected AudioSource sound;
	protected Animator anim;
	protected bool alive = true;
	protected float wait = 0f;
	protected bool highAlert = false;
	protected float alertness = 20f;
	protected Vector3 sourcePosition;

	public Enemy (GameObject player, NavMeshAgent nav, AudioSource sound, Animator anim, bool alive, float wait, bool highAlert, float alertness){
		this.player = player;
		this.nav = nav;
		this.sound = sound;
		this.anim = anim;
		this.alive = alive;
		this.wait = wait;
		this.highAlert = highAlert;
		this.alertness = alertness;
	}
	/**
	 * 
	 */
	public void idle(Vector3 sourcePosition) {
		//pick a random place to walk to//
		Vector3 randomPos = Random.insideUnitSphere*alertness;
		NavMeshHit navHit;
		NavMesh.SamplePosition(sourcePosition + randomPos, out navHit,20f,NavMesh.AllAreas);

		//go near the player//
		if(highAlert)
		{
			NavMesh.SamplePosition(player.transform.position + randomPos, out navHit,20f,NavMesh.AllAreas);
			//each time, lose awareness of player general position//
			alertness += 5f;

			if(alertness > 20f)
			{
				highAlert = false;
				nav.speed = 1.2f;
				anim.speed = 1.2f;
			}
		}

	 	nav.SetDestination(navHit.position);
	}
	/**
	 * 
	 */
	public void walk(){
		Debug.Log ("Walk state");
	}
	/**
	 * 
	 */
	public void search(){
		Debug.Log ("Search state");
	}
	/**
	 * 
	 */
	public void chase(){
		Debug.Log ("Chase state");
	}
	/**
	 * 
	 */
	public void hunt(){
		Debug.Log ("Hunt state");
		nav.SetDestination (player.transform.position);
	}
	/**
	 * 
	 */
	public void kill(){
		Debug.Log ("Kill state");
	}

}
