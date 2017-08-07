using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour{

	//	private Enemy enemy;
	public GameObject player;
	public AudioClip[] footSounds;
	public Transform eyes;
	public AudioSource growl;
	public GameObject deathCam;
	public Transform camPos;

	private NavMeshAgent nav;
	private AudioSource sound;
	private Animator anim;
	private string state = "idle";
	private bool alive = true;
	private float wait = 0f;
	private bool highAlert = false;
	private float alertness = 20f;

	// Use this for initialization
	void Start () {
		this.nav = GetComponent<NavMeshAgent>();
		this.sound = GetComponent<AudioSource>();
		this.anim = GetComponent<Animator>();
		this.nav.speed = 1.2f;
		this.anim.speed = 1.2f;

//		this.enemy = new Enemy (player, nav, sound, anim, true, wait, highAlert, alertness);
	}
	
	// Update is called once per frame
	void Update () {
//		this.enemy.hunt ();

//		Debug.DrawLine (this.eyes.position, this.player.transform.position, Color.green);

		if (this.alive) {
			this.anim.SetFloat ("velocity", this.nav.velocity.magnitude); 

			if (this.state.Equals ("idle")) {
			 	// pick a Random place To walk to
				Vector3 randomPos = Random.insideUnitSphere * this.alertness;
				NavMeshHit navHit;
				NavMesh.SamplePosition (this.transform.position + randomPos, out navHit, 20f, NavMesh.AllAreas);

				// go near the player
				if (this.highAlert) {
					NavMesh.SamplePosition (this.player.transform.position + randomPos, out navHit, 20f, NavMesh.AllAreas);
					this.alertness += 5f;
					if(this.alertness > 20f){
						this.highAlert = false;
						this.nav.speed = 1.2f;
						this.anim.speed = 1.2f;
					}
				}

				this.nav.SetDestination (navHit.position);
				this.state = "walk";
			}

			if (this.state.Equals ("walk")) {
				if(this.nav.remainingDistance <= this.nav.stoppingDistance && !this.nav.pathPending){
					// define outra rota
					this.state = "search";
					this.wait = 5f;
				}
			}

			if (this.state.Equals ("chase")) {
				this.nav.destination = this.player.transform.position;
				float distance = Vector3.Distance (transform.position, this.player.transform.position);
				if(distance > 10f){
					this.state = "hunt";
				} else if(this.nav.remainingDistance <= this.nav.stoppingDistance + 1f && !this.nav.pathPending){
					if(this.player.GetComponent<Player>().alive){
						this.state = "kill";
						this.player.GetComponent<Player> ().alive = false;
						this.player.GetComponent<FirstPersonController> ().enabled = false;
						this.deathCam.SetActive (true);

						this.deathCam.transform.position = Camera.main.transform.position;
						this.deathCam.transform.rotation = Camera.main.transform.rotation;
						Camera.main.gameObject.SetActive (false);
						this.growl.pitch = 0.7f;
						this.growl.Play ();
						Invoke ("reset", 1f);

					}
				}
			}

			if (this.state.Equals ("hunt")) {
				if(this.nav.remainingDistance <= this.nav.stoppingDistance && !this.nav.pathPending){
					this.state = "search";
					this.wait = 5f;
					this.highAlert = true;
					this.alertness = 5f;
					this.checkSight ();
				}
			}

			if (this.state.Equals ("search")) {
				if (this.wait > 0f) {
					this.wait -= Time.deltaTime;
					this.transform.Rotate (0f, 120f * Time.deltaTime, 0f);
				} else {
					this.state = "idle";
				}
			}

			if (this.state.Equals ("kill")) {
				this.deathCam.transform.position = Vector3.Slerp (this.deathCam.transform.position, this.camPos.position, 10f * Time.deltaTime);
				this.deathCam.transform.rotation = Quaternion.Slerp (this.deathCam.transform.rotation, this.camPos.rotation, 10f * Time.deltaTime);
				this.anim.speed = 1f;
				this.nav.SetDestination (this.deathCam.transform.position);
			}
		}

//		this.nav.SetDestination (this.player.transform.position);
	}

	/**
	 * Check if can see the player 
	 */
	public void checkSight(){
		if (this.alive){
			RaycastHit rayHit;
			if(Physics.Linecast(this.eyes.position, this.player.transform.position, out rayHit)){
//				Debug.Log ("hit " + rayHit.collider.gameObject.name);
				if(rayHit.collider.gameObject.name.Equals("Player")){
					if(!this.state.Equals("kill")){
						this.state = "chase";
						this.nav.speed = 3.5f;
						this.anim.speed = 3.5f;
						this.growl.pitch = 1.2f;
						this.growl.Play ();
					}
				}
			}
		}
	}

	/**
	 * Play a sound 
	 */
	public void footStep(int _num){
		this.sound.clip = this.footSounds [_num];
		this.sound.Play ();
	}

	void reset(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void death(){
		this.anim.SetTrigger ("dead");
		this.anim.speed = 1f;
		this.alive = false;
		this.nav.isStopped = true; 
	}
}
