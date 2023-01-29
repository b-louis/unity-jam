using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// 3rd-person movement that picks direction relative to target (usually the camera)
// commented lines demonstrate snap to direction and without ground raycast
//
// To setup animated character create an animation controller with states for idle, running, jumping
// transition between idle and running based on added Speed float, set those not atomic so that they can be overridden by...
// transition both idle and running to jump based on added Jumping boolean, transition back to idle

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : NetworkBehaviour {
	[SerializeField] Transform target;
	
	public float moveSpeed = 6.0f;
	public float rotSpeed = 15.0f;
	public float jumpSpeed = 15.0f;
	public float gravity = -9.8f;
	public float terminalVelocity = -20.0f;
	public float minFall = -1.5f;

	private bool _canDash = true;
	private float _dashMultiplier = 1.0f;
	public float dashForce = 1.0f;
	public float dashDuration = 0.5f;
	public float dashCoolDown = 2.0f;
	public bool onwnerMode;
	private float vertSpeed;
	private ControllerColliderHit contact;

	private CharacterController charController;
	private Animator animator;
    private PlayerInGame pig;
    private Camera cam;
	public Vector3 mousePoint;
	private bool spawning = true;
	private Vector3 spawnpoint;
	// Use this for initialization
	void Start() {
		vertSpeed = minFall;
		cam = Camera.main;
		charController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		pig = GetComponent<PlayerInGame>();
	}
	public void Spawn(Vector3 pos)
    {
		spawning = true;
		spawnpoint = pos;
		StartCoroutine(SpawnWait());
	}

	IEnumerator SpawnWait()
	{
		yield return new WaitForSeconds(0.5f);
		spawning = false;


	}
	// Update is called once per frame
	void Update() {

		if (onwnerMode)
        {

			if (!IsOwner) return;
        }
        if (!pig.alive.Value && false)
        {
			return;
        }
        if (spawning && false)
		{
			charController.transform.position = spawnpoint;
			return;
		}

		// Mouse rotation
		Vector3 mousePos = Input.mousePosition;
		//Vector3 mousePoint = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 20));
		//mousePoint.y = transform.position.y;
		LayerMask mask = LayerMask.GetMask("Ground");
		Ray ray = cam.ScreenPointToRay(mousePos);
		RaycastHit mouseHit;
		/*		RaycastHit[] hits;
				hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);*/
		if (Physics.Raycast(ray, out mouseHit, Mathf.Infinity, mask))
		{
			mousePoint = mouseHit.point;
		}
        mousePoint.y = transform.position.y;
		var lookPos = mousePoint - transform.position;
		var rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = rotation;
		// start with zero and add movement components progressively
		Vector3 movement = Vector3.zero;

		float horInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");
		if (horInput != 0 || vertInput != 0) {

			// x z movement transformed relative to target
			movement = new Vector3(horInput * moveSpeed, 0, vertInput * moveSpeed);
			movement = Vector3.ClampMagnitude(movement, moveSpeed);

			}
		animator.SetFloat("Speed", movement.sqrMagnitude);

		// raycast down to address steep slopes and dropoff edge
		bool hitGround = false;
		RaycastHit hit;
		if (vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit)) {
			float check = (charController.height + charController.radius) / 1.9f;
			hitGround = hit.distance <= check;	// to be sure check slightly beyond bottom of capsule
		}

		// y movement: possibly jump impulse up, always accel down
		// could _charController.isGrounded instead, but then cannot workaround dropoff edge
		if (hitGround) {
			if (Input.GetButtonDown("Jump") && false) {
				vertSpeed = jumpSpeed;
			} else {
				vertSpeed = minFall;
				animator.SetBool("Jumping", false);
			}
		} else {
			vertSpeed += gravity * 5 * Time.deltaTime;
			if (vertSpeed < terminalVelocity) {
				vertSpeed = terminalVelocity;
			}
			if (contact != null ) {	// not right at level start
				animator.SetBool("Jumping", true);
			}

			// workaround for standing on dropoff edge
			if (charController.isGrounded) {
				if (Vector3.Dot(movement, contact.normal) < 0) {
					movement = contact.normal * moveSpeed;
				} else {
					movement += contact.normal * moveSpeed;
				}
			}
		}
		movement.y = vertSpeed;
		movement.x *= _dashMultiplier;
		movement.z *= _dashMultiplier;

		movement *= Time.deltaTime;
		charController.Move(movement);

		// DASH 
		// Dash Mecanics
		if (Input.GetButtonDown("Jump") && _canDash)
		{
			Debug.Log("I'm dashin");
			StartCoroutine(Dashing());
		}

	}

	// store collision to use in Update
	void OnControllerColliderHit(ControllerColliderHit hit) {
		contact = hit;
	}


	IEnumerator Dashing()
    {
		_canDash = false;
		_dashMultiplier = dashForce;
		yield return new WaitForSeconds(dashDuration);
		_dashMultiplier = 1f;
		yield return new WaitForSeconds(dashCoolDown);
		_canDash = true;
	}
}
