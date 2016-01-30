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

	private float _inputZ;
	private float _inputX;

    private Rigidbody _rigidB;
    private Transform _transf;

	private Vector3 _activeSpeed = new Vector3(0f, 0f, 0f);
	private Vector3 _orientation = new Vector3(0f, 0f, 0f);

	//Inputs
	private string _horizontalAxisName;
	private string _verticalAxisName;
	private string _attackInputName;

    //Max speeds
	[Header("Movement")]
	[SerializeField]
    private float _maxSpeed = 10;

    //Vectors affecting the player
    [SerializeField]
    private float _acceleration = 0.1f;
    [SerializeField]
    private float _friction = 0.05f;

    #endregion
    #region Attacks Variables
	private float _invulTime = 0f; //Secondes of invulnerability
	private float _stunTime = 0f; //Secondes of stun on Hit

	private bool _projectileLaunched = false;
    #endregion

    //States
	private bool _allowInput = true;
	private bool _isStunned = false;
	private RaycastHit _hit;

    #region Unity virtuals
    void Start()
    {
		this._horizontalAxisName = this._playerName +"_Horizontal";
		this._verticalAxisName = this._playerName + "_Vertical";
		this._attackInputName = this._playerName + "_Attack";
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
        if (this._allowInput)
            this.ProcessInputs();
		
        this.ProcessCoolDowns();

		this.ProcessActiveSpeed();
		this.ProcessOrientation();

		this.ApplyCharacterFinalVelocity();
    }

	private void ProcessOrientation()
	{
		//this._orientation.x = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f ? Input.GetAxis("Horizontal") : this._orientation.x;
		//this._orientation.z = Mathf.Abs(Input.GetAxis("Vertical")) > 0.5f ? Input.GetAxis("Vertical") : this._orientation.z;

		// A REFAIRE AC LES SPRITES
		if(this._activeSpeed.magnitude > 0.5f)
			this.transform.rotation = Quaternion.LookRotation(this._activeSpeed.normalized);
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
		}
    }

    void ProcessInputs()
    {
        //Get the input raw
		if (this._inputRawX != Mathf.Sign(Input.GetAxisRaw(this._horizontalAxisName)) && Input.GetAxisRaw(this._horizontalAxisName) != 0f)
			this._activeSpeed.x = 0;
		if (this._inputRawZ != Mathf.Sign(Input.GetAxisRaw(this._verticalAxisName)) && Input.GetAxisRaw(this._verticalAxisName) != 0f)
			this._activeSpeed.z = 0;


		this._inputRawX = this._allowInput ? Input.GetAxisRaw(this._horizontalAxisName) : 0;
		this._inputRawZ = this._allowInput ? Input.GetAxisRaw(this._verticalAxisName) : 0;

		this._inputX = this._allowInput ? Input.GetAxis(this._horizontalAxisName) : 0;
		this._inputZ = this._allowInput ? Input.GetAxis(this._verticalAxisName) : 0;

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
			Debug.Log("retrieve Weapon");
			this._projectileLaunched = false;
		}
		else
		{
			Debug.Log("launch weapon");
			this._projectileLaunched = true;
		}
	}
	
    
    #region Attacks

	public void Damage(int amount, float stunTime, Vector3 ejection)
	{
		if (this._invulTime > 0)
			return;
		this.setInvulTime(stunTime + 1);
		this.addStun(stunTime);
		this._activeSpeed = ejection;
		this._inputRawX = 0;
		this._inputRawZ = 0;
	}

	private void addStun(float amount)
	{
		this._isStunned = true;
		this._stunTime += amount;
		if(amount > 0)
			this._allowInput = false;
	}
    
    #endregion
}
