using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    #region Movement Variables
	[Header("Player")]
	[SerializeField]
	private string _playerName = "";

    [Space(10, order = 1)]
    [Header("Movement", order = 2)]

    //Raw Inputs
	private float _inputRawZ;
	private float _inputRawX;

    private Rigidbody _rigidB;
    private Transform _transf;

	private Vector3 _activeSpeed = new Vector3(0f, 0f, 0f);
	private Vector3 _orientation = new Vector3(0f, 0f, 0f);

	//Inputs
	private string _horizontalAxisName;
	private string _verticalAxisName;
	private string _attackInputName;

	private string _horizontal2AxisName;
	private string _vertical2AxisName;

    //Max speeds
	[Header("Movement")]
    public float _maxSpeed = 10;

    //Vectors affecting the player
    [SerializeField]
    private float _acceleration = 0.1f;
    [SerializeField]
    private float _friction = 0.05f;

    #endregion
    #region Attacks Variables
	private float _invulTime = 0f; //Secondes of invulnerability
	private float _stunTime = 0f; //Secondes of stun on Hit

	//Projectile
	[SerializeField]
	private Weapon _weapon;
	
	private bool _projectileLaunched = false;

    #endregion

	[SerializeField]
	private AudioSource mySounds;

	public SpriteRenderer myShield;

	[SerializeField]
	private AudioClip throwWeaponSound;
	private Animator myAnim;
	private bool throwWeapon = false;

    //States
	private bool _allowInput = true;
	private bool _isStunned = false;
	private RaycastHit _hit;

	private GameObject baby;
	[SerializeField]
	private Transform _aimingLine;

    #region Unity virtuals
    void Start()
    {
		mySounds = GetComponent<AudioSource>();
		myAnim = GetComponent<Animator>();

		this._horizontalAxisName = this._playerName + "_Horizontal";
		this._verticalAxisName = this._playerName + "_Vertical";
		this._attackInputName = this._playerName + "_Attack";

		this._horizontal2AxisName = this._playerName + "_Horizontal 2";
		this._vertical2AxisName = this._playerName + "_Vertical 2";

		Application.targetFrameRate = 60; //Caps the game at 60 fps (optionnal but recommended for precise jump)

		this._rigidB = this.GetComponent<Rigidbody>();
        this._transf = this.GetComponent<Transform>();
        
        //Plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:    
        foreach (Transform child in _transf)
            child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

    }


	/*
	 ########################################################
	 ###########           UPDATE            ################
	 ########################################################
	 */
	void Update()
    {
		this._inputRawX = this._allowInput ? Input.GetAxisRaw(this._horizontalAxisName) : 0;
		this._inputRawZ = this._allowInput ? Input.GetAxisRaw(this._verticalAxisName) : 0;

        if (this._allowInput)
            this.ProcessInputs();
		
        this.ProcessCoolDowns();

		this.ProcessActiveSpeed();
		this.ProcessOrientation();

		this.ApplyCharacterFinalVelocity();

		if (this._invulTime > 0)
		{
			myShield.enabled = true;
		}
		else
			myShield.enabled = false;

		//if (baby != null && Input.GetKeyDown(KeyCode.P))
		//{
		//	baby.GetComponent<Baby>().Ejection(Vector3.forward);
		//	baby = null;
		//}
    }

	private void ProcessOrientation()
	{
		if (Mathf.Sign(-this._orientation.x) != 0)
			this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(this._orientation.x), this.transform.localScale.y, this.transform.localScale.z);

		if (Input.GetAxis(this._horizontal2AxisName) != 0 || Input.GetAxis(this._vertical2AxisName) != 0)
			this.ShowAimingLine(new Vector3(Input.GetAxis(this._horizontal2AxisName) * Mathf.Sign(this.transform.localScale.x), 0, Input.GetAxis(this._vertical2AxisName)));
		else if (this._activeSpeed.magnitude > 0.2f)
			this.ShowAimingLine(new Vector3(this._activeSpeed.x * Mathf.Sign(this.transform.localScale.x), 0, -this._activeSpeed.z));
	}

	private void ShowAimingLine(Vector3 direction)
	{
		this._aimingLine.forward = this.transform.forward;
		this._aimingLine.rotation = Quaternion.Euler(new Vector3(0, Vector3.Angle(Vector3.right, direction) * Mathf.Sign(direction.z) - this.transform.rotation.y + 90, 0));
	}
    #endregion
    
    
    private void ProcessCoolDowns()
    {
        //Melee Attacks
		this._invulTime -= this._invulTime > 0 ? Time.deltaTime : 0;

		if (this._stunTime > 0)
		{
			this._allowInput = false;
			this._stunTime -= Time.deltaTime;
		}
		else if(this._isStunned)
		{
			this._isStunned = false;
			this._allowInput = true;
			this._stunTime = 0;
			myAnim.SetBool("Stun", _isStunned);
		}
    }

    void ProcessInputs()
    {
        //Get the input raw
		if (this._inputRawX != Mathf.Sign(Input.GetAxisRaw(this._horizontalAxisName)) && Input.GetAxisRaw(this._horizontalAxisName) != 0f)
			this._activeSpeed.x = 0;
		if (this._inputRawZ != Mathf.Sign(Input.GetAxisRaw(this._verticalAxisName)) && Input.GetAxisRaw(this._verticalAxisName) != 0f)
			this._activeSpeed.z = 0;

		//this._inputX = this._allowInput ? Input.GetAxis(this._horizontalAxisName) : 0;
		//this._inputZ = this._allowInput ? Input.GetAxis(this._verticalAxisName) : 0;

		this._orientation.x = Input.GetAxis(this._horizontalAxisName) != 0 ? Input.GetAxis(this._horizontalAxisName) : this._orientation.x;
		this._orientation.z = Input.GetAxis(this._verticalAxisName) != 0 ? Input.GetAxis(this._verticalAxisName) : this._orientation.z;


		if (Input.GetButtonDown(this._attackInputName))
		{
			this.ProcessAction();
		}
    }


    private void ProcessActiveSpeed()
    {
        //Grounded control
		if (this._inputRawX != 0)
		{
			this._activeSpeed.x += this._acceleration * this._inputRawX;
			this._activeSpeed.x = Mathf.Clamp(this._activeSpeed.x, -this._maxSpeed * 0.7f, this._maxSpeed * 0.7f);
		}
        //Grounded friction
		else 
        {
			if (this._activeSpeed.x != 0f) 
			{
				if (Mathf.Abs(this._activeSpeed.x) < this._friction)
					this._activeSpeed.x = 0f;
				else
					this._activeSpeed.x = Mathf.Sign(this._activeSpeed.x) * (Mathf.Abs(this._activeSpeed.x) - this._friction);
			}
        }

		if (this._inputRawZ != 0)
		{
			this._activeSpeed.z += this._acceleration * this._inputRawZ;
			this._activeSpeed.z = Mathf.Clamp(this._activeSpeed.z, -this._maxSpeed * 0.7f, this._maxSpeed * 0.7f);
		}
		else
		{
			if (this._activeSpeed.z != 0f)
			{
				if (Mathf.Abs(this._activeSpeed.z) < this._friction)
					this._activeSpeed.z = 0f;
				else
					this._activeSpeed.z = Mathf.Sign(this._activeSpeed.z) * (Mathf.Abs(this._activeSpeed.z) - this._friction);
			}
		}
		myAnim.SetFloat("Speed", Mathf.Abs(_activeSpeed.x + _activeSpeed.z));

    }
    
    void ApplyCharacterFinalVelocity()
    {
		this._rigidB.velocity = this._activeSpeed;
    }
    
    public void setInvulTime(float amount)
    {
		this._invulTime = this._invulTime < 0 ? 0 : this._invulTime;
    	this._invulTime += amount;
    }


	private void ProcessAction()
	{
		if (this._projectileLaunched)
		{
			this._projectileLaunched = !this._weapon.Retrieve(this.transform);
		}
		else
		{
			myAnim.SetTrigger("Throw");
			mySounds.PlayOneShot(throwWeaponSound);
			this._projectileLaunched = this._weapon.Launch(this._transf.position, new Vector3(this._aimingLine.forward.x * Mathf.Sign(this.transform.localScale.x), this._aimingLine.forward.y, this._aimingLine.forward.z), this.tag);
		}
	}
	
    
    #region Attacks

	public void Damage(float stunTime, Vector3 ejection)
	{
		if (this._invulTime > 0)
			return;
		this.setInvulTime(stunTime + 1);
		this.addStun(stunTime);
		this._activeSpeed = ejection;
		this._inputRawX = 0;
		this._inputRawZ = 0;

		this.EjectBaby(new Vector3(UnityEngine.Random.Range(-1, 1), 0.9f, UnityEngine.Random.Range(-1, 1)).normalized * 10);
	}

	private void EjectBaby(Vector3 direction)
	{
		if (this.baby != null)
		{
			this.baby.GetComponent<Baby>().Ejection(this.transform.position, direction);
			this.baby = null;
		}
	}


	private void addStun(float amount, bool stunAnim = true)
	{
		this._isStunned = true;
		this._stunTime += amount;
		if(amount > 0)
			this._allowInput = false;
		if (stunAnim)
			myAnim.SetBool("Stun", _isStunned);
	}
    
    #endregion

	#region Collisions

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Babies" && baby == null)
		{
			if (other.gameObject.GetComponent<Baby>().isCatchable) 
			{
				other.gameObject.GetComponent<Baby>().Catch(this.transform);
				baby = other.gameObject;
			}
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Traps")
		{
			if (other.GetComponent<Trap>().canStun)
			{
				Damage(1, -_orientation.normalized * 15);
			}
		}
		else if (other.gameObject.tag == "Altar" && baby != null)
		{
			baby.GetComponent<Baby>().Kill();
			GameManager.Instance.AddScore(this._playerName);
			addStun(1, false);
			myAnim.SetTrigger("Sacrifice");
		}
	}

	#endregion
}
