using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    SpriteRenderer[] _spriteRenderer; 
    Rigidbody2D rb;
    Animator Animator;
    [System.NonSerialized]public float MoveSpeed = 3f;
    float knockbackSpeed = 1f;
    [System.NonSerialized]public float MaxJumpSpeed = 5f;
    [System.NonSerialized]public int JumpCount;
    public int MaxJumps = 2;
    public int PlayerNumber; 
    float Gravity = 0.1f;
    int count = 0;
    float JumpSpeed;
    private int goingDown = 0;
    private int aux;
    float downBuffer, dmgSpawnTiming = 0.6f;

    private int downTime; 
    /*public float buttonTime = 0.5f;
    public float jumpHeight = 60;
    public float cancelRate = 100;*/
    public GameObject Arrow, HeavyArrow;

    Vector3 nextPosition;
    [System.NonSerialized]public float DamageCount = 0;
    float yStart;
    float jumpTime;
    bool jumping;
    bool jumpCancelled;
    bool combo;
    bool shoot;
    private bool wallJump;
    private float dashCD, attackCD, wallJumpTime, holdThrowItemTime;

    public GameObject ObjectToThrow, DamageObject, BigDamageObject;
    private int hurtFacing;
    //Inputs
    bool rightKey, leftKey, upKey, downKey,jumpKey,lightAttackKey,heavyAttackKey,DashKey,throwKey, throwKeyReleased,canPickUpWep;
    public enum State
    {
        Main,
        Attack,
        Walljump,
        Hurt,
        Dash,
        Throw,
        Die
    }
    public enum Characters
    {
        Fridsa,
        Zaroth
    }
    public enum isHolding
    {
        Nothing,
        Weapon,
        Item,
    }
    private enum Anim
    {
        Idle,Run,JumpStart,JumpLoop,JumpLand,Walljump,Dash,Throw,Hurt,Knockback,AttackLight,AttackLightUp, AttackLightAir,
        AttackLightAirUp,AttackLightAirDown,AttackHeavy,AttackHeavyUp,AttackHeavyAir,AttackHeavyAirUp,AttackHeavyAirDown,AttackAirSlam  
    }
    public State state;

    private int CurrentCharacter, currentWeapon;
    //Character 0-1/ Weapon 0-2/ Anim(string) 0-12
    //Anim List: Idle,Run,JumpStart,JumpLoop,JumpLand,Dash,Hurt,Throw,Knockback,
    //AttackLight,AttackLightUp, AttackLightAir,AttackLightAirUp,AttackLightAirDown,AttackHeavy,AttackHeavyUp,AttackHeavyAir,AttackHeavyAirUp,AttackHeavyAirDown  
    private string[,,] animation_index = new string[2, 3, 20]; /*{ { { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown"}, { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown"} },
                                                                { { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown" }, { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown" } }, 
                                                                { { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown" }, { "FridsaWeaponIdle", "FridsaWeaponRun", "FridsaWeaponJumpStart", "FridsaWeaponJumpLoop", "FridsaWeaponJumpLand", "FridsaWeaponDash", "FridsaWeaponHurt", "FridsaWeaponThrow", "FridsaWeaponKnockback", "FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown" } }};*/
    //"FridsaWeaponAttackLight", "FridsaWeaponAttackLightUp", "FridsaWeaponAttackLightAir", "FridsaWeaponAttackLightAirUp", "FridsaWeaponAttackLightAirDown", "FridsaWeaponAttackHeavy", "FridsaWeaponAttackHeavyUp", "FridsaWeaponAttackHeavyAir", "FridsaWeaponAttackHeavyAirUp", "FridsaWeaponAttackHeavyAirDown"
   
    // Start is called before the first frame update
    void Awake()
    {
        CurrentCharacter = (int) Characters.Fridsa;
        currentWeapon = (int)isHolding.Nothing;
        
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Idle] = "FridsaWeaponIdle";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Run] = "FridsaWeaponRun";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.JumpStart] = "FridsaWeaponJumpStart";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.JumpLoop] = "FridsaWeaponJumpLoop";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.JumpLand] = "FridsaWeaponJumpLand";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Walljump] = "FridsaWeaponWalljump";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Dash] = "FridsaWeaponDash";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Throw] = "FridsaWeaponThrow";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Hurt] = "FridsaWeaponHurt";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.Knockback] = "FridsaWeaponKnockback";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackLight] = "FridsaWeaponAttackLight";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackLightUp] = "FridsaWeaponAttackLightUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackLightAir] = "FridsaWeaponAttackLightAir";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackLightAirUp] = "FridsaWeaponAttackLightAirUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackLightAirDown] = "FridsaWeaponAttackLightAirDown";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackHeavy] = "FridsaWeaponAttackHeavy";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackHeavyUp] = "FridsaWeaponAttackHeavyUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackHeavyAir] = "FridsaWeaponAttackHeavyAir";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackHeavyAirUp] = "FridsaWeaponAttackHeavyAirUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Weapon, (int)Anim.AttackHeavyAirDown] = "FridsaWeaponAttackHeavyAirDown";

        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Idle] = "FridsaUnarmIdle";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Run] = "FridsaUnarmRun";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.JumpStart] = "FridsaUnarmJumpStart";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.JumpLoop] = "FridsaUnarmJumpLoop";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.JumpLand] = "FridsaUnarmJumpLand";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Walljump] = "FridsaUnarmWalljump";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Dash] = "FridsaUnarmDash";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Throw] = "FridsaWeaponThrow";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Hurt] = "FridsaUnarmHurt";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.Knockback] = "FridsaWeaponKnockback";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackLight] = "FridsaUnarmAttackLight";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackLightUp] = "FridsaUnarmAttackLightUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackLightAir] = "FridsaUnarmAttackLightAir";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackLightAirUp] = "FridsaUnarmAttackLightAirUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackAirSlam] = "FridsaUnarmAttackAirSlam";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackHeavy] = "FridsaUnarmAttackHeavy";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackHeavyUp] = "FridsaUnarmAttackHeavyUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackHeavyAir] = "FridsaUnarmAttackHeavyAir";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackHeavyAirUp] = "FridsaUnarmAttackHeavyAirUp";
        animation_index[(int) Characters.Fridsa, (int)isHolding.Nothing, (int)Anim.AttackHeavyAirDown] = "FridsaUnarmAttackHeavyAirDown";
    }
    
    void Start()
    {
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        nextPosition = Vector3.zero;
        Animator = GetComponent<Animator>();
        Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Idle]);
    }
    // Update is called once per frame
    void OnDestroy()
    {
        if (!GameManager.Reload)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.Reload = true;
        }
    }
    
    void Update()
    {
        if (transform.position.y < -36)
        {
            Destroy(gameObject);
        }
        
        //right-left / jump / Light Key / Heavy Attack / Dodge / Throw  
        switch (PlayerNumber)
        {
            default:
                rightKey = Input.GetKey("d");
                leftKey  = Input.GetKey("a");
                downKey  = Input.GetKey("s");
                upKey = Input.GetKey("w");
                jumpKey = Input.GetKeyDown(KeyCode.Space);
                lightAttackKey = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);
                heavyAttackKey = Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1);
                DashKey = Input.GetKeyDown(KeyCode.L);
                throwKey = Input.GetKeyDown(KeyCode.I) || Input.GetMouseButtonDown(2);
                throwKeyReleased = Input.GetKeyUp(KeyCode.I) || Input.GetMouseButtonUp(2);
                //var buttonA = Convert.ToBoolean (Mathf.Max(Convert.ToSingle (Input.GetKeyDown("p")), Convert.ToSingle(Input.GetMouseButtonDown(0)) ));
                break;
            
            case 1:
                rightKey = Input.GetAxis("Horizontal") > 0.7;
                leftKey  = Input.GetAxis("Horizontal") < -0.7;
                upKey  = Input.GetAxis("Vertical") > 0.7;
                downKey  = Input.GetAxis("Vertical") < -0.7;
                jumpKey = Input.GetKeyDown(KeyCode.Joystick1Button0);
                lightAttackKey = Input.GetKeyDown(KeyCode.Joystick1Button2);
                heavyAttackKey = Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button1);
                DashKey = Input.GetAxis("Trigger LT") > .5f || Input.GetAxis("Trigger RT") > .5f;
                throwKey = Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5);
                throwKeyReleased = Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.Joystick1Button5);
                break;
            
        }

        switch (state)
        {
            case State.Main:
                if (rightKey || leftKey)
                {
                    wallJump = false;
                    wallJumpTime = 0;
                }
                
                //Debug//
                if (rightKey)
                {
                    transform.localScale = new Vector2(1, 1);
                    nextPosition = Vector2.right * MoveSpeed;
                }

                if (leftKey)
                {
                    transform.localScale = new Vector2(-1, 1);
                    nextPosition = Vector2.left * MoveSpeed;
                }

                if (!leftKey && !rightKey)
                {
                    nextPosition = Vector3.zero;
                }

                if (jumpKey && JumpCount < MaxJumps)
                {
                    if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+38),
                        Vector2.up, 10f, LayerMask.GetMask("Collision")) == false ) {
                        jumping = true;
                        JumpSpeed = MaxJumpSpeed;
                        Gravity = 0.1f;
                        JumpCount++;
                        downBuffer = 0f;
                        Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpStart],0,0);
                    }
                }
                
                if (DashKey && !jumping && dashCD <= Time.time)
                {
                    state = State.Dash;
                    Animator.Play(animation_index[(int) Characters.Fridsa, currentWeapon, (int)Anim.Dash]);
                }

                if (throwKey && (currentWeapon == (int)isHolding.Item || currentWeapon == (int)isHolding.Weapon) ) {
                    state = State.Throw;
                    Animator.Play(animation_index[(int) Characters.Fridsa, currentWeapon, (int)Anim.Throw]);
                    Animator.speed = 0;
                    nextPosition = Vector3.zero;
                    holdThrowItemTime = Time.time+2;
                     //= 0;
                    
                }

                if (currentWeapon == (int)isHolding.Nothing && throwKey && canPickUpWep) {
                    currentWeapon = (int)isHolding.Weapon;
                    canPickUpWep = false;
                    //
                }

                if (lightAttackKey && attackCD <= Time.time)
                {
                    state = State.Attack;
                    nextPosition = Vector2.zero;
                    if (jumping)
                    {
                        if (upKey)//Up
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackLightAirUp]);
                        } 
                        else if (downKey)//Down
                        {
                            if (currentWeapon == (int)isHolding.Nothing ) {
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackAirSlam]);
                            } else {
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackLightAirDown]);
                            }
                            
                        }
                        else
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackLightAir]);
                        }
                    }
                    else
                    {
                        if (upKey)
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackLightUp]);
                        }
                        else
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackLight]);
                        }
                        
                    }
                } else if (heavyAttackKey && attackCD <= Time.time)
                {
                    dmgSpawnTiming = 0.6f;
                    state = State.Attack;
                    nextPosition = Vector2.zero;
                    if (jumping)
                    {
                        if (currentWeapon == (int)isHolding.Weapon ) {
                             Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavyAir]);
                             dmgSpawnTiming = 0.4f;
                        } else {
                            if (upKey)//Up
                            {
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavyAirUp]);
                            } 
                            else if (downKey)//Down
                            {
                                if (currentWeapon == (int)isHolding.Nothing ) {
                                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackAirSlam]);
                                } else {
                                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavyAirDown]);
                                }
                                
                            }
                            else
                            {
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavyAir]);
                            }
                        }    
                    }
                    else
                    {
                        if (upKey)
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavyUp]);
                        }
                        else
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.AttackHeavy]);
                        }
                    }
                }
                //Anims
                if (!jumping)
                {
                    if (leftKey || rightKey)
                    {
                        Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Run]);
                    }
                    else 
                    {
                        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLand]) && aux == 0 )
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Idle]);
                        }
                    }
                }
                break;

            case State.Throw:
            if (throwKeyReleased || Time.time >= holdThrowItemTime) {
                Animator.speed = 1;
            }
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                state = State.Main;
                currentWeapon = (int)isHolding.Nothing;
                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Idle]);
                var _wep = Instantiate(ObjectToThrow,new Vector3(transform.position.x+5*transform.localScale.x, transform.position.y+16,transform.position.z) , Quaternion.identity );
                if (transform.localScale.x == 1) {
                    _wep.gameObject.GetComponent<ThrownWeapon>().Right = true;
                } else {
                    _wep.gameObject.GetComponent<ThrownWeapon>().Right = false;
                }
                _wep.gameObject.GetComponent<ThrownWeapon>().Speed = (2 - (holdThrowItemTime-Time.time) ) * 10000;
                _wep.gameObject.GetComponent<ThrownWeapon>().PlayerNumber = PlayerNumber;
                //8  10 - 2
                //10 10 - 0
                //9 10 - 1

            }
            break; 
            
            case State.Attack:
                switch (currentWeapon) {

                    case (int)isHolding.Nothing:
                    if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackAirSlam]))
                    {
                        
                        /*Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Idle]);
                        attackCD = Time.time + 0.1f;
                        shoot = false;
                        state = State.Main;*/
                    } 
                    else if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        
                        if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLight]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+16,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightUp]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+48,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightAir]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+10,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                            
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightAirUp]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+48,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavyAirUp]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+48,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                            _dmg.GetComponent<DamageObject>().Dmg = 2;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavy]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+16,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                            _dmg.GetComponent<DamageObject>().Dmg = 2;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavyUp]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+48,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                            _dmg.GetComponent<DamageObject>().Dmg = 2;
                        } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavyAir]))
                        {
                            var _dmg =Instantiate(DamageObject,new Vector3(transform.position.x+15*transform.localScale.x, transform.position.y+10,transform.position.z),Quaternion.identity);
                            _dmg.GetComponent<DamageObject>().PlayerNumber = PlayerNumber;
                            _dmg.GetComponent<DamageObject>().Dmg = 2;
                            
                        }
                        if (jumping)
                        {
                        Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.JumpLoop]);
                        }  else {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Idle]);
                        }
                        attackCD = Time.time + 0.1f;
                        shoot = false;
                        state = State.Main;
                    }
                    break;

                    case (int)isHolding.Item:
                    break;

                    case (int)isHolding.Weapon:
                    if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= dmgSpawnTiming && !shoot)
                    {
                        if (jumping)
                        {
                            if (Animator.GetCurrentAnimatorStateInfo(0)
                                .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightAirUp]))
                            {
                                var arrow = Instantiate(Arrow, new Vector2(transform.position.x + (2 * transform.localScale.x), transform.position.y + 30), transform.rotation);
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Up;
                                arrow.transform.eulerAngles = new Vector3(
                                    arrow.transform.eulerAngles.x,
                                    arrow.transform.eulerAngles.y,
                                    arrow.transform.eulerAngles.z+90);
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                            } 
                            else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightAirDown]))
                            {
                                var arrow = Instantiate(Arrow, new Vector2(transform.position.x + (2 * transform.localScale.x), transform.position.y + 15), transform.rotation);
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Down;
                                arrow.transform.eulerAngles = new Vector3(
                                    arrow.transform.eulerAngles.x,
                                    arrow.transform.eulerAngles.y,
                                    arrow.transform.eulerAngles.z+270); 
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                            } 
                            else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightAir]))
                            {
                                var arrow = Instantiate(Arrow, new Vector2(transform.position.x + (10 * transform.localScale.x), transform.position.y + 25), transform.rotation);
                                arrow.GetComponent<Arrow>().Facing = transform.localScale.x;
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Side;
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                            } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavyAir])) {
                                
                                var _dmg = Instantiate(BigDamageObject,new Vector3(transform.position.x- (5 * transform.localScale.x), transform.position.y+30,transform.position.z),Quaternion.identity);
                                _dmg.GetComponent<BigDamageObject>().PlayerNumber = PlayerNumber;
                                _dmg.GetComponent<BigDamageObject>().Dmg = 2;          
                                shoot = true;
                                /*var arrow = Instantiate(HeavyArrow, new Vector2(transform.position.x + (2 * transform.localScale.x), transform.position.y + 35), transform.rotation);
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Up;
                                arrow.transform.eulerAngles = new Vector3(
                                    arrow.transform.eulerAngles.x,
                                    arrow.transform.eulerAngles.y,
                                    arrow.transform.eulerAngles.z+90);
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                                arrow.GetComponent<Arrow>().Dmg = 2; */
                            }
                        } else
                        {
                            if (Animator.GetCurrentAnimatorStateInfo(0)
                                .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLightUp]))
                            {
                                var arrow = Instantiate(Arrow, new Vector2(transform.position.x + (2 * transform.localScale.x), transform.position.y + 35), transform.rotation);
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Up;
                                arrow.transform.eulerAngles = new Vector3(
                                    arrow.transform.eulerAngles.x,
                                    arrow.transform.eulerAngles.y,
                                    arrow.transform.eulerAngles.z+90);
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                            } 
                            else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackLight]))
                            {
                                var arrow = Instantiate(Arrow, new Vector2(transform.position.x + (10 * transform.localScale.x), transform.position.y + 19), transform.rotation);
                                arrow.GetComponent<Arrow>().Facing = transform.localScale.x;
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Side;
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                            } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavy])) {
                                var arrow = Instantiate(HeavyArrow, new Vector2(transform.position.x + (10 * transform.localScale.x), transform.position.y + 25), transform.rotation);
                                arrow.GetComponent<Arrow>().Facing = transform.localScale.x;
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Side;
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                                arrow.GetComponent<Arrow>().Dmg = 2; 
                            } else if (Animator.GetCurrentAnimatorStateInfo(0)
                                    .IsName(animation_index[CurrentCharacter, currentWeapon, (int) Anim.AttackHeavyUp])) {
                                var arrow = Instantiate(HeavyArrow, new Vector2(transform.position.x + (2 * transform.localScale.x), transform.position.y + 35), transform.rotation);
                                arrow.GetComponent<Arrow>().MovementDirection = global::Arrow.Direction.Up;
                                arrow.transform.eulerAngles = new Vector3(
                                    arrow.transform.eulerAngles.x,
                                    arrow.transform.eulerAngles.y,
                                    arrow.transform.eulerAngles.z+90);
                                arrow.GetComponent<Arrow>().PlayerNumber = PlayerNumber;
                                arrow.GetComponent<Arrow>().Dmg = 2; 
                            }
                        }
                        shoot = true;
                    } else if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        state = State.Main;
                        if (!jumping)
                        {
                            Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Idle]);
                        }
                        attackCD = Time.time + 0.1f;
                        shoot = false;
                    }
                    break;

                }
                break;
            case State.Dash:
                nextPosition = Vector2.right*transform.localScale.x * MoveSpeed*2;
                if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Animator.GetCurrentAnimatorStateInfo(0).IsName(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Dash]))
                {
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Idle]);
                    state = State.Main;
                    dashCD = Time.time+1f;
                }
                /*
                if (jumping)
                {
                    state = State.Main;
                }*/
                break;
            
            case State.Walljump:
                
                if (jumpKey)
                {
                    jumping = true;
                    JumpSpeed = MaxJumpSpeed;
                    Gravity = 0.1f;
                    JumpCount++;
                    downBuffer = 0f;
                    wallJump = true;
                    wallJumpTime = Time.time+1;
                    state = State.Main;
                    
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLoop]);

                    switch (transform.localScale.x)
                    {
                        case 1:
                            transform.localScale = new Vector2(-1, 1);
                            break;
                        
                        case -1:
                            transform.localScale = new Vector2(1, 1);
                            break;
                        
                    }
                    //SideJump
                }
                break;
            
            case State.Hurt:
                nextPosition = Vector2.right*hurtFacing * (knockbackSpeed*(DamageCount/3));
                if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Animator.GetCurrentAnimatorStateInfo(0).IsName(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Hurt]))
                {
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Idle]);
                    state = State.Main;
                }
                break;
        }
    }
    
    private void FixedUpdate()
    {
        if (wallJumpTime<Time.time)
        {
            wallJump = false;
            wallJumpTime = 0;
        }
        else if (wallJump)
        {
            nextPosition += (Vector3.right*transform.localScale.x * 2);
        }
        
        switch (state)
        {
            case State.Hurt:
                
                break;
            
            case State.Walljump:
                float _speed = .5f;
                transform.Translate(Vector2.down * _speed);
                RaycastHit2D hitTop;
                switch (transform.localScale.x)
                    {
                        case 1:
                            hitTop = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+38), (Vector2.right ), 10,
                            LayerMask.GetMask("Collision"));
                            if (hitTop.collider == null)
                            {
                                state = State.Main;
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLoop]);
                            }
                            break;
                        
                        case -1:
                            hitTop = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+38), (Vector2.left ), 10,
                            LayerMask.GetMask("Collision"));
                            if (hitTop.collider == null)
                            {
                                state = State.Main;
                                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLoop]);
                            }
                            break;
                        
                    }
                return;
        }
        if (aux > 0)
        {
            aux--;
        } 
        //Horizontal
        var offset = 0f;
        if (transform.localScale.x == 1)
        {
            offset = 10;
        }
        else
        {
            offset = -10;
        }

        if (downKey)
        {
            if (jumping)
            {
                downTime++;
                if (downTime > 5) { downBuffer = -2.5f; }
            }
            else
            {
                downTime = 0;
            }
        } else
        {
            downTime = 0;
        }
        
        if (downKey && !jumping)
        {
            RaycastHit2D hitRightDown = Physics2D.Raycast(new Vector2(transform.position.x + 7, transform.position.y),
                Vector2.down, 1f,
                LayerMask.GetMask("CollisionAlt"));
            RaycastHit2D hitLeftDown = Physics2D.Raycast(new Vector2(transform.position.x - 7, transform.position.y),
                Vector2.down, 1f,
                LayerMask.GetMask("CollisionAlt"));
            if (hitLeftDown.collider != null || hitRightDown.collider != null)
            {
                jumping = true;
                Gravity = 0.2f;
                JumpSpeed = 0;
                if (state != State.Hurt && state != State.Dash && state != State.Attack)
                {
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLoop]);
                }
                downBuffer = 0f;
                goingDown = 5;
            }
        }

        if (nextPosition != Vector3.zero)
        {
            RaycastHit2D hitTop = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+38), nextPosition.normalized, 10,
                LayerMask.GetMask("Collision"));
            RaycastHit2D hitMiddle = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+18), nextPosition.normalized, 10,
                LayerMask.GetMask("Collision"));
            RaycastHit2D hitBottom = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+1), nextPosition.normalized, 10,
                LayerMask.GetMask("Collision"));        
            if (hitTop.collider == null && hitBottom.collider == null && hitMiddle.collider == null) 
            { 
                transform.Translate(nextPosition);
                hitTop = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+38), nextPosition.normalized, 10,
                    LayerMask.GetMask("Collision"));
                if (hitTop.collider != null)
                {
                    //var _offset = 10 - hitTop.distance;
                    var _offset = 0.0f;
                    if (hitTop.distance < 10) {
                        _offset = 10-hitTop.distance; 
                    }
                    //state = State.Walljump;
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Walljump]);
                     transform.position = new Vector3(transform.position.x-_offset,transform.position.y, transform.position.z);
                    switch (transform.localScale.x)
                    {
                        case 1:
                            transform.position = new Vector3(transform.position.x-_offset,transform.position.y, transform.position.z);
                            break;
                        
                        case -1:
                            transform.position = new Vector3(transform.position.x+_offset,transform.position.y, transform.position.z);
                            break;
                        
                    }
                }

            } else if (hitTop.collider != null)
            {
                    var _offset = 0.0f;
                    if (hitTop.distance < 10) {
                        _offset = 10-hitTop.distance; 
                    }
                    //var _offset = 10 - hitTop.distance;
                    state = State.Walljump;
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int) Anim.Walljump]);
                    switch (transform.localScale.x)
                    {
                        case 1:
                            transform.position = new Vector3(transform.position.x-_offset,transform.position.y, transform.position.z);
                            break;
                        
                        case -1:
                            transform.position = new Vector3(transform.position.x+_offset,transform.position.y, transform.position.z);
                            break;
                        
                    }
                    /*jumping = false;
                    Gravity = 0.1f;
                    JumpCount = 0;
                    downBuffer = 0;*/
                    return;
                
            }
            
            if (!jumping)
            {
                RaycastHit2D hitVRight = Physics2D.Raycast(new Vector2(transform.position.x + 7, transform.position.y),
                    Vector2.down, 1f,
                    LayerMask.GetMask("Collision"));
                RaycastHit2D hitVLeft = Physics2D.Raycast(new Vector2(transform.position.x - 7, transform.position.y),
                    Vector2.down, 1f,
                    LayerMask.GetMask("Collision"));
                RaycastHit2D hitVAlt = Physics2D.Raycast(new Vector2(transform.position.x + offset, transform.position.y),
                    Vector2.down, 1f,
                    LayerMask.GetMask("CollisionAlt"));

                if (hitVRight.collider == null && hitVLeft.collider == null && hitVAlt.collider == null)
                {
                    jumping = true;
                    Gravity = 0.2f;
                    JumpSpeed = 0;
                    downBuffer = 0f;
                    if (state != State.Hurt && state != State.Dash && state != State.Attack)
                    {
                        Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLoop]);
                    }
                }
            }
        }

        if (goingDown > 0)
        {
            goingDown--;

        }

        if (jumping)
        {
            JumpSpeed -= Gravity;
            if (JumpSpeed - Gravity > 0)
            {
                Gravity += 0.05f;
                Gravity = Mathf.Clamp(Gravity, 0.1f, 0.2f);
            }
            var vspd = Vector2.up * (JumpSpeed+downBuffer);

            float _offset = 7;

            RaycastHit2D hitUpLeft = Physics2D.Raycast(new Vector2(transform.position.x-10, transform.position.y+38),
                        Vector2.up, 5f, LayerMask.GetMask("Collision"));
            RaycastHit2D hitUpRight = Physics2D.Raycast(new Vector2(transform.position.x+10, transform.position.y+38),
                        Vector2.up, 5f, LayerMask.GetMask("Collision"));

            if (hitUpLeft || hitUpRight) {
                if (JumpSpeed - Gravity > 0)
                {
                    JumpSpeed = 0;
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), vspd.normalized, vspd.sqrMagnitude,
                LayerMask.GetMask("Collision"));
            
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x+_offset, transform.position.y), vspd.normalized, vspd.sqrMagnitude,
                LayerMask.GetMask("Collision"));
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x-_offset, transform.position.y), vspd.normalized, vspd.sqrMagnitude,
                LayerMask.GetMask("Collision"));
            
            
            RaycastHit2D hitAltRight = Physics2D.Raycast(new Vector2(transform.position.x+_offset, transform.position.y), vspd.normalized, vspd.sqrMagnitude,
                LayerMask.GetMask("CollisionAlt")); 
            RaycastHit2D hitAltLeft = Physics2D.Raycast(new Vector2(transform.position.x-_offset, transform.position.y), vspd.normalized, vspd.sqrMagnitude,
                LayerMask.GetMask("CollisionAlt"));    
            if ( (hit.distance <= vspd.sqrMagnitude || hitRight.distance <= vspd.sqrMagnitude || hitLeft.distance <= vspd.sqrMagnitude || hitAltLeft.distance <= vspd.sqrMagnitude || hitAltRight.distance <= vspd.sqrMagnitude) && vspd.normalized == Vector2.down && goingDown == 0 && ( hitRight.collider != null || hitLeft.collider != null || hitAltLeft.collider != null || hitAltRight.collider != null && goingDown == 0 && !downKey))
            {
                if (hitLeft.collider != null)
                {
                    vspd.y = -hitLeft.distance;
                } else if (hitRight.collider != null)
                {
                    vspd.y = -hitRight.distance;
                }
                else if (hitAltRight.collider != null)
                {
                    vspd.y = -hitAltRight.distance;
                }
                else if (hitAltLeft.collider != null)
                {
                    vspd.y = -hitAltLeft.distance;
                }
                transform.Translate(vspd);
                jumping = false;
                Gravity = 0.1f;
                JumpCount = 0;
                downBuffer = 0;
                if (state != State.Hurt && state != State.Dash && state != State.Attack)
                {
                    Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.JumpLand]);
                }

                aux = 1;
                wallJump = false;
                wallJumpTime = 0;
            }
            else
            {
                transform.Translate(vspd);
            }
        }

        if (!jumping)
        {
            if (count != 0)
            {
                //print(count);
            }
            count = 0;
        }
        if (JumpSpeed - Gravity > 0 && jumping)
        {
            count++;
        }
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Arrow") && state != State.Hurt )
        {
            if (col.GetComponent<Arrow>().PlayerNumber != PlayerNumber) {
                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Hurt]);
                state = State.Hurt;
                DamageCount+=col.GetComponent<Arrow>().Dmg;
                //CheckHp();
                Destroy(col.gameObject);
                if (col.transform.position.x < transform.position.x)
                {
                    hurtFacing = 1;
                }
                else
                {
                    hurtFacing = -1;
                }
            }
        } 
        if (col.gameObject.CompareTag("DMG") && state != State.Hurt )
        {
            if (col.GetComponent<DamageObject>().PlayerNumber != PlayerNumber) {
                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Hurt]);
                state = State.Hurt;
                DamageCount+=col.GetComponent<DamageObject>().Dmg;
                //CheckHp();
                Destroy(col.gameObject);
                if (col.transform.position.x < transform.position.x)
                {
                    hurtFacing = 1;
                }
                else
                {
                    hurtFacing = -1;
                }
            }
        }

        if (col.gameObject.CompareTag("PickUpWeapon")  )
        {
            if (col.gameObject.GetComponent<ThrownWeapon>().PlayerNumber != PlayerNumber && state != State.Hurt) {
                Animator.Play(animation_index[CurrentCharacter, currentWeapon, (int)Anim.Hurt]);
                state = State.Hurt;
                DamageCount+=col.GetComponent<ThrownWeapon>().Dmg;
                Destroy(col.gameObject);
                if (col.transform.position.x < transform.position.x)
                {
                    hurtFacing = 1;
                }
                else
                {
                    hurtFacing = -1;
                }
            } else {
                canPickUpWep = true;
            }
        } 
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("PickUpWeapon"))
        {
            if (col.gameObject.GetComponent<ThrownWeapon>().PlayerNumber == PlayerNumber) {
                if (!canPickUpWep) {
                    Destroy(col.gameObject);
                }
            }
        } 
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("PickUpWeapon"))
        {
            if (col.gameObject.GetComponent<ThrownWeapon>().PlayerNumber == PlayerNumber)
            {
                canPickUpWep = false;
            }
        } 
    }

    /*private void CheckHp()
    {
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }*/
}