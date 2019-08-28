using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPun,IPunObservable
{
    public CharacterController2D controller;
    public  Animator animator;
    public float runSpeed = 40f;
    public GameObject CurrenPlayerDenote;

    public PhotonView pv;
    float horizontalMove = 0f;
   public bool jump = false;
  public  bool crouch = false;
  //  public Text t1;
   // public Text t2;
    public Manager manage;
    public GameObject won;
    public GameObject failed;
    public bool run;
    public Sprite PlayerSPrite;
    public string username;
    
 


    public List<Anima2D.SpriteMeshInstance> Order = new List<Anima2D.SpriteMeshInstance>();
  
        private void Start()
    {
        
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        manage = GameObject.Find("Manager").GetComponent<Manager>();
        controlData = GameObject.Find("ControlData").GetComponent<ControlData>();
        Debug.LogError(GetComponent<Rigidbody2D>().gravityScale);
       rb2d.gravityScale = controlData.playerGravityScale;

        //  username = controlData.userName;

        //  manage.totalPlayerCharacterNo.Add(manage.UI.chosenCharacter);
        //   t1 = GameObject.Find("t1").GetComponent<Text>();

        //while(PhotonNetwork.CountOfPlayers!=2)
        //{
        //    yield return null;
        //}
        run = true;
         GetComponent<Animator>().SetBool("Idle", true);
    //   runSpeed = 0;
        //  pv.RPC("WaitForPlayerFunc", RpcTarget.AllBuffered, null);
        WaitForPlayerFunc();
       username= manage.userNameClass.userName;
        manage.ReloadBtn.onClick.AddListener(() => TowardsLobby());
        if (pv.IsMine)
        {
            for (int i=0;i<Order.Count;i++)
            {
                Order[i].sortingOrder += 1;
            }
            manage.t2.text = manage.userNameClass.userName;

            manage.startBtn.onClick.AddListener(() => startcountFunc());
            FrontCheckOffset = this.transform.position - frontCheck.transform.position;
            BackCheckOffset = this.transform.position - BackCheck.transform.position;
           // GetComponent<SpriteRenderer>().sortingOrder = 2;
            pv.RPC("PlayerAdd", RpcTarget.AllBuffered, null);
            //  pv.RPC("NameSet", RpcTarget.AllBuffered, null);
           
            // manage.totalPlayerName.Add(username);

            //  pv.RPC("PlayerCharacterSend", RpcTarget.AllBuffered, null);

            CurrenPlayerDenote.gameObject.SetActive(true);
            MinForceSet();
            manage.LocalPlayer = this.gameObject;
            manage.attackBtn.onClick.RemoveAllListeners();
            manage.attackBtn.onClick.AddListener(() => speedUpFunc());
            manage.ShurikenBtn.onClick.RemoveAllListeners();

            manage.ShurikenBtn.onClick.AddListener(() => ShurikenaAttackFunc());
        }
        else
        {
          //  pv.RPC("PlayerAdd", RpcTarget.AllBuffered, null);

            CurrenPlayerDenote.gameObject.SetActive(false);

        }
        for(int i=0;i<manage.totalPlayer.Count;i++)
        {
            manage.PlayerDistBG[i].sprite = manage.totalPlayer[i].GetComponent<PlayerMovement>().PlayerSPrite;
        }

        GetComponent<CharacterController2D>().OnLandEvent.AddListener(OnCharacterLanded);
    }

    const int MIN_WALL_BOOST = 2, MAX_WALL_BOOST = 5;
    const float BOOST_TIME_PER_WALL_BOOST = 0.2f;

    void OnCharacterLanded()
    {
        if (wallBoostBuildup > MIN_WALL_BOOST) StartCoroutine(SpeedUp(gameObject, Mathf.Min(wallBoostBuildup - MIN_WALL_BOOST, MAX_WALL_BOOST) * BOOST_TIME_PER_WALL_BOOST));
        wallBoostBuildup = 0;
    }

    public GameObject ShurikenObj;

    public void ShurikenaAttackFunc()
    {
      PowerUp temp = PhotonNetwork.Instantiate(Manager.manage.ShurikenPrefab.name, ShurikenObj.transform.position, Quaternion.identity).GetComponent<PowerUp>();
        temp.SentBy = this.gameObject;
        manage.ShurikenBtn.gameObject.SetActive(false);
        manage.PowerUpUsedSave();
        Statistics.stats.Pref("StunHit");

    }
    public void speedUpFunc()
    {
        manage.PowerUpUsedSave();
        StartCoroutine(SpeedUp(this.gameObject));
        Manager.manage.attackBtn.gameObject.SetActive(false);

    }

    public IEnumerator SpeedUp(GameObject temp, float speedUpTime = 1f)
    {
        if (pv.IsMine)
        {
            Manager.manage.attackBtn.interactable = false;

            float temp1 = runSpeed;
            controlData.TargetSpeed += 50;
            controlData.MaxRunForce += 2000;
            MinRunForce += 2000;
            //  runSpeed += Time.deltaTime * controlData.MaxRunForce * 100;
            //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed*1.5f;
            yield return new WaitForSeconds(1f);
            manage.BooseTimeSave();
            Debug.Log("BoosterUSed");
            //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed/1.5f ;
            //   runSpeed += Time.deltaTime * controlData.MaxRunForce / 100;
            controlData.TargetSpeed -= 50;
            controlData.MaxRunForce -= 2000;
            MinRunForce -= 2000;
            runSpeed = temp1;
            Manager.manage.attackBtn.interactable = true;


        }


    }
    public void startcountFunc()
    {
       pv.RPC("startCount", RpcTarget.AllBuffered, null);
        manage.startBtn.gameObject.SetActive(false);

    }
    [PunRPC]
    public void startCount()
    {
        manage.startCount += 1;
    }
   
    [PunRPC]
    public void PlayerAdd()
    {

        if(manage==null)
        {
            manage = GameObject.Find("Manager").GetComponent<Manager>();

        }
        if (!manage.totalPlayer.Contains(this.gameObject))
        {
            
            manage.totalPlayer.Add(this.gameObject);
           
           
            
          
        }

    }

    [PunRPC]
    public void WaitForPlayerFunc()
    {
        StartCoroutine(WaitPlayers());

    }
    public AudioControl Ac;
    public UIHandler UI;

    IEnumerator WaitPlayers()
    {
        Ac = GameObject.Find("AudioCtrl").GetComponent<AudioControl>();
        UI = GameObject.Find("Launcher").GetComponent<UIHandler>();

        Ac.GetComponent<AudioSource>().Stop();
        if (Manager.manage.UI.EnteredFirst)
        {
            manage.startBtn.gameObject.SetActive(true);
        }

        if (!Manager.manage.UI.EnteredFirst)
        {
            manage.PlayerReady.gameObject.SetActive(true);
        }

        //  Debug.LogError(manage.totalPlayer.Count);
        //Debug.LogError(manage.startCount+"startcount");

        yield return new WaitForSeconds(0.5f);
        while (manage.startCount != 1 ) {
            yield return null;
        }




        Ac.GetComponent<AudioSource>().clip = Ac.BG_Game;
        Ac.GetComponent<AudioSource>().Play();
        manage.t1.gameObject.SetActive(false);
        manage.startBtn.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        manage.PlayerReady.gameObject.SetActive(false);

        manage.TrafficLight[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        manage.TrafficLight[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        manage.TrafficLight[2].gameObject.SetActive(true);
       
        yield return new WaitForSeconds(1f);
        //while(manage.FirstTouch!=manage.UI.PlayerCount)
        //{
        //    yield return null;
        //}
        run = false;
          runSpeed = 10;
        GetComponent<Animator>().SetBool("Idle", false);

        for (int i = 0; i < manage.TrafficLight.Count; i++)
        {
            manage.TrafficLight[i].gameObject.SetActive(false);
        }
        secondTaken = 0;
        SecStart = true;
    }
    public float secondTaken;
    public bool SecStart;
    public float MinRunForce;
    public Vector3 FrontCheckOffset;
    public Vector3 BackCheckOffset;
    void MinForceSet()
    {
        // runSpeed = 10;

         MinRunForce = controlData.MaxRunForce / 10.0f;
        MinRunForce = 10;
    }
    [PunRPC]
    public void offlight()
    {
        for (int i = 0; i < manage.TrafficLight.Count; i++)
        {
            manage.TrafficLight[i].gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void DistanceSync()
    {
        for (int i = 0; i < manage.totalPlayer.Count; i++)
        {
            manage.PlayerDistUI[i].gameObject.SetActive(false);
        }

            for (int i = 0; i < manage.totalPlayer.Count; i++)
        {
            if (manage.totalPlayer[i].gameObject == this.gameObject)
            {
                float dist = Vector3.Distance(this.transform.position, manage.finish.transform.position);
                manage.PlayerDist[i] = 1 - (dist / manage.FinalDist);
                manage.PlayerDistUI[i].gameObject.SetActive(true);
               manage.PlayerDistUI[i].value = 1 - (dist / manage.FinalDist);
                manage.PlayerDistBG[i].color = new Color(255, 208,0,1);
                // manage.t1.text = manage.PlayerDist[i].ToString();

            }
            else
            {
                if(manage.PlayerDist[i]!=0)
                {
                    manage.PlayerDistUI[i].gameObject.SetActive(true);

                }else
                {
                    manage.PlayerDistUI[i].gameObject.SetActive(false);

                }

                manage.PlayerDistBG[i].color = new Color(255, 255, 255, 1);

                manage.PlayerDistUI[i].value = manage.PlayerDist[i];

            }
        }
    }
    public bool FirstTouchGameStart;
    [PunRPC]
    public void FirstTouchSync()
    {
      //  manage.FirstTouch += 1;
    }
        // Update is called once per frame
        void Update()
    {


        //horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (pv.IsMine)
        {
            rb2d.gravityScale = controlData.playerGravityScale;
            if (SecStart)
            {
                secondTaken += Time.deltaTime;
            }
                    pv.RPC("DistanceSync", RpcTarget.AllBuffered, null);
                
            
            if (Input.touchCount == 0)
            {
                
                    DOTouchCount = true;
            }
            frontCheck.transform.position = this.transform.position - FrontCheckOffset;
            BackCheck.transform.position = this.transform.position - BackCheckOffset;
            // runSpeed = 100f;
           

            horizontalMove = runSpeed;

        }
        //if(manage.totalPlayer.Count>=2)
        //{
        //    for (int i = 0; i < manage.totalPlayer.Count; i++)
        //    {
        //        if (manage.totalPlayer[i] == this.gameObject)
        //        {
        //            if(jump==false)
        //            {
        //                runSpeed =Mathf.Lerp(runSpeed, manage.speed[i],Time.deltaTime*manage.IncreaseRateSpeed);

        //            }else
        //            {
        //                runSpeed = runSpeed - 10f;
        //            }
        //        }
        //    }
        //}


        if ( Input.GetKey(KeyCode.DownArrow))
        {
            crouch = true;
            
            //animator.SetTrigger("2To4");
        } 
            else
            {
                crouch = false;
                //animator.SetTrigger("4To2");
            }

        if ( Input.GetKeyDown(KeyCode.UpArrow))
        {
            jump = true;
            
            //animator.SetTrigger("Jump");
        }
            else
            {
                jump = false;
            }
        if(Input.GetMouseButton(0))
        {
            if (pv.IsMine)
            {
                //if (FirstTouchGameStart == false)
                //{
                //    pv.RPC("FirstTouchSync", RpcTarget.AllBuffered, null);

                //    FirstTouchGameStart = true;
                //}
            }
         //   t1.text = Input.mousePosition.x.ToString();
            //if(Input.mousePosition.x>150f && Input.mousePosition.x < 600f)
            //{
            //    jump = true;
            //}

          //  if (Input.mousePosition.x > Screen.width * .25f && Input.mousePosition.x < Screen.width * .75f)
            {
                jump = true;
            }
        }
        else
        {
            jump = false;

        }

      //  if (Input.mousePosition.x > Screen.width * .25f && Input.mousePosition.x < Screen.width * .75f)
      if(true)
        {
            if(Input.touchCount > 1)
            {
                crouch = true;
                jump = false;


            }
        }
        else
        {
            crouch = false;

        }

    }
    void TowardsLobby()
    {
        PhotonNetwork.LeaveRoom();
     //   PhotonNetwork.LoadLevel(0);
    }
    void Quit()
    {
        Application.Quit();
    }
    IEnumerator WallJumpRoutine()
    {
        WallJumpActive = true;
        yield return null;
        //  yield return new WaitForSeconds(0.5f);
        WallJumpActive = false;

    }
    Vector3 movement;
    public Collider2D[] res;
    public Transform wallCheckPoint;
    public LayerMask WallLayer;
    public bool NormalMove;
    public bool straigtJump;
    public bool DOTouchCount;
    public GameObject frontCheck;
    public GameObject BackCheck;
    public ControlData controlData;
    public bool WallJumpActiveBool;
    public bool WallJumpActive;
    
    int wallBoostBuildup = 0;

    public bool NoRun;
  //  [PunRPC]
    public void PlayerPunished()
    {
        Statistics.stats.Pref("Stunned");
        if(pv.IsMine)
        {
            StartCoroutine(PlayerPunishedRoutine());

        }
    }
    IEnumerator PlayerPunishedRoutine()
    {
        //  runSpeed = 0;
       // runSpeed = controlData.TargetSpeed * 1.5f;
        NoRun = true;
        StartCoroutine(CharacterStop(transform.position));
        yield return new WaitForSeconds(4f);

        NoRun = false;
       // runSpeed = controlData.TargetSpeed;
     //  runSpeed = controlData.TargetSpeed;

    }
    IEnumerator CharacterStop(Vector3 pos)
    {
        while(NoRun)
        {
            transform.position = pos;
            yield return null;
        }
    }

    public float PrevXPos;
    private void FixedUpdate()
    {
        if(PrevXPos+0.1 >= transform.position.x)
        {
            PrevXPos = transform.position.x;
        }else
        {
            if(run==false)
            {
                if(res.Length!=0)
                {
                    manage.SpendOnWall += Time.deltaTime;
                 //   Debug.LogError("wallStay");

                }
                if(GetComponent<CharacterController2D>().m_Velocity.x > 130f)
                {
                    manage.SpendOnWall += Time.deltaTime;
                    Debug.LogError("wallStay");
                }


            }
           


        }
        if (pv.IsMine)
        {
            res = Physics2D.OverlapBoxAll(wallCheckPoint.position, new Vector2(wallCheckWi, wallCheckHi), 0.15f, WallLayer);
            if (GetComponent<CharacterController2D>().m_Grounded == false && res.Length == 0)
            {
                if (NormalMove == false)
                {
                    if(NoRun==false)
                    {
                        controller.Move(horizontalMove * Time.deltaTime, crouch, false);

                    }

                }

            }
            else
            {
                if (NoRun==false)
                {
                    controller.Move(horizontalMove * Time.deltaTime, crouch, false);
                }

            }

            if (!GetComponent<CharacterController2D>().m_Grounded && res.Length != 0)
            {
                if (straigtJump)
                {
                    StartCoroutine(WallJumpRoutine());
                   // Debug.LogError("Jump");
                    manage.JumpUsedSave();
                    straigtJump = false;
                }
            }

            if (GetComponent<CharacterController2D>().m_Grounded)
            {
                if(run==false)
                    manage.GroundTime += Time.deltaTime;
                if (straigtJump == false)
                {
                    straigtJump = true;
                }
                
                if (res.Length == 0)
                {
                    animator.SetBool("wallslide", false);

                    if (Input.touchCount == 1 || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (Input.mousePosition.x > Screen.width * .25f || Input.mousePosition.y > Screen.width * .25f)
                        {
                            if (DOTouchCount)
                            {

                                //     Debug.LogError("Jump");
                              //  if(run==failed)
                                     manage.JumpUsedSave();
                                controller.Move(0 * Time.deltaTime, crouch, true);
                                DOTouchCount = false;
                            }
                        }

                    }
                    else
                    {
                        if (wallCheckPoint != frontCheck)
                        {
                            wallCheckPoint = frontCheck.transform;
                        }
                        if (NormalMove == true)
                        {
                            NormalMove = false;
                        }
                    }


                }
                else
                {
                    if (Input.touchCount == 1  || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (Input.mousePosition.x > Screen.width * .25f || Input.mousePosition.y > Screen.width * .25f)
                        {
                            if (DOTouchCount)
                            {
                                if (NormalMove == false)
                                {
                                    runSpeed = controlData.BaseSpeed - 1;
                                    NormalMove = true;

                                }
                                
                                controller.Move(0 * Time.deltaTime, crouch, true);
                                DOTouchCount = false;
                            }
                        }
                    }

                }

            }
            else if (GetComponent<CharacterController2D>().m_Grounded == false && res.Length != 0)
            {
                if (NormalMove == false)
                {
                    runSpeed = controlData.BaseSpeed - 1;
                    NormalMove = true;

                }

                if (Input.touchCount == 1  || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (Input.mousePosition.x > Screen.width * .25f || Input.mousePosition.y > Screen.width * .25f)
                    {
                        if (DOTouchCount)
                    {
                        if (WallJumpActive == false)
                        {
                            if (wallCheckPoint.gameObject == BackCheck)
                            {

                                if (WallJumpActiveBool == false)
                                {
                                    Collider2D temp = res[0].GetComponent<Collider2D>();
                                    StartCoroutine(WallJumpRoutine());
                                    StartCoroutine(Walljumpactivate(true, temp));
                                    WallJumpActiveBool = true;

                                }

                            }
                            else if (wallCheckPoint.gameObject == frontCheck)
                            {
                                if (WallJumpActiveBool == false)
                                {
                                    Collider2D temp = res[0].GetComponent<Collider2D>();
                                    StartCoroutine(Walljumpactivate(false, temp));
                                    StartCoroutine(WallJumpRoutine());
                                    WallJumpActiveBool = true;
                                }


                            }
                        }
                    }
                }
                }

            }else if(GetComponent<CharacterController2D>().m_Grounded == false)
            {
                manage.AirTime += Time.deltaTime;

            }
            if (GetComponent<CharacterController2D>().m_Grounded == false && res.Length != 0)
            {

                if (rb2d.velocity.y < 0)
                {
                    //  print(GetComponent<CharacterController2D>().m_Grounded);
                    animator.SetBool("wallslide", true);
                    rb2d.velocity = new Vector2(rb2d.velocity.x, -controlData.WallSlideGravity);
                   //   rb2d.velocity = new Vector2(rb2d.velocity.x, controlData.terminalVelocity);
                }
                else
                {
                    animator.SetBool("wallslide", false);

                }

            }
            else if (GetComponent<CharacterController2D>().m_Grounded == false && res.Length == 0)
            {
                animator.SetBool("wallslide", false);

            }
            else if (GetComponent<CharacterController2D>().m_Grounded == true && res.Length != 0)
            {
                animator.SetBool("wallslide", false);

            }
            if (run == false)
            {
                if (runSpeed > controlData.TargetSpeed)
                {

                }

                if (runSpeed > controlData.BaseSpeed && runSpeed < controlData.TargetSpeed)
                {
                    runSpeed += Time.deltaTime * MinRunForce;

                }
                if (runSpeed == controlData.BaseSpeed)
                {
                    runSpeed += 1;
                }


                if (runSpeed < controlData.BaseSpeed && runSpeed > 0f)
                {
                    runSpeed += Time.deltaTime * controlData.MaxRunForce;

                }
                //  controller.Move(horizontalMove * Time.deltaTime, crouch, jump);


            }
            else
            {
                // runSpeed = 0;
                transform.position = winpos;
            }
            //if (GetComponent<Rigidbody2D>().velocity.y < controlData.terminalVelocity)
            //{
            //    //Debug.Log(m_Rigidbody2D.velocity.y);
            //    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, controlData.terminalVelocity);

            //}

            jump = false;

        //    t1.text = manage.reach.ToString();
            
        }else
        {
            transform.position = Vector3.Lerp(transform.position,movement,Time.deltaTime*10f);

        }

    }
    [PunRPC]
    public void Reach()
    {
        manage.reach = 10;
    }
    public Rigidbody2D rb2d;
    IEnumerator Walljumpactivate(bool front, Collider2D temp)
    {
        if (temp.GetComponent<Collider2D>())
        {
            temp.enabled = false;

        }
        yield return null;
        if (front)
        {
            transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);

            rb2d.velocity = new Vector2(0, 0);
            rb2d.angularVelocity = 0f;
            rb2d.AddForce(new Vector2((controlData.walljumpForceLeft * 2.5f * 1000f * Time.deltaTime), (controlData.m_JumpForce * controlData.walljumpAmplitudeLeft * 1000f * Time.deltaTime)));
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
          //  Debug.LogError("jump");
            manage.wallJumpSave();
            wallCheckPoint = frontCheck.transform;

        }
        else
        {
            transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
            rb2d.velocity = new Vector2(0, 0);
            rb2d.angularVelocity = 0f;
            rb2d.AddForce(new Vector2((-controlData.walljumpForceLeft * 1000f * 2.5f * Time.deltaTime), (controlData.m_JumpForce * controlData.walljumpAmplitudeLeft * 1000f * Time.deltaTime)));
          //  Debug.LogError("jump");
            manage.wallJumpSave();

            transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
            wallCheckPoint = BackCheck.transform;

        }

        wallBoostBuildup++;

        yield return null;
        StartCoroutine(coliderOff(temp));

    }
    IEnumerator coliderOff(Collider2D temp)
    {

        //  Debug.LogError(temp.enabled);
        yield return new WaitForSeconds(0.2f);  //0.2f
        if (temp.GetComponent<Collider2D>())
        {
            temp.enabled = true;
        }
        WallJumpActiveBool = false;


    }
    public void Crouch()
    {
        crouch = true;
    }
    public string info;
    public void ReloadApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator stop()
    {
        yield return new WaitForSeconds(1.1f);
        run = true;
        winpos = transform.position;
    }
    public Vector3 winpos;
    [SerializeField] private float wallCheckWi, wallCheckHi;
    public bool Finished;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
       
        if (pv.IsMine)
        {
           
            if (collision.tag == "Finish")
            {
                Debug.LogError("finish");
                Statistics.stats.Pref("FinishTrack" + manage.UI.selectedLevel);
                SecStart = false;
                Finished = true;
                if (manage.reach < 10)
                {
                    Statistics.stats.Pref("TotalWin");


                    won.gameObject.SetActive(true);
                    Camera.main.GetComponent<CameraFollow>().offset = new Vector3(0, 1.2f, -10f);
                    GetComponent<Animator>().SetBool("Idle",true);
                 //   GetComponent<SpriteRenderer>().sortingOrder = 2;
                    StartCoroutine(stop());
                    //    manage.won.gameObject.SetActive(true);

                    pv.RPC("Reach", RpcTarget.AllBuffered, null);
                    Destroy(failed);
                    return;
                }
                else
                {
                    if(failed!=null)
                    {
                        Statistics.stats.Pref("TotalLose");
                        failed.gameObject.SetActive(true);
                        Camera.main.GetComponent<CameraFollow>().offset = new Vector3(0, 1.2f, -10f);
                        GetComponent<Animator>().SetBool("Idle", true);

                        run = true;
                        winpos = transform.position;
                    }
                   

                }
                int count = 0;
                for (int i=0;i<manage.totalPlayer.Count;i++)
                {
                   
                    if(manage.totalPlayer[i].GetComponent<PlayerMovement>().Finished==true)
                    {
                        count += 1;
                    }

                }
                if(manage.UI.PlayerCount==count)
                {
                    pv.RPC("ScoreShow", RpcTarget.AllBuffered, null);

                }

                manage.GroundTimeSave();
            }
        }else
        {
            if (collision.tag == "Obstacle1")
            {
                 Physics.IgnoreCollision(collision.GetComponent<Collider>(), GetComponent<Collider>());

            }
        }
    }

    [PunRPC]
    public void ScoreShow()
    {
        manage.ScoreCardMenu.gameObject.SetActive(true);
        for (int i = 0; i < manage.totalPlayer.Count; i++)
        {
            for (int j = i+1; j < manage.totalPlayer.Count; j++)
            {
                if (manage.totalPlayer[i].GetComponent<PlayerMovement>().secondTaken > manage.totalPlayer[j].GetComponent<PlayerMovement>().secondTaken)
                {
                    GameObject temp = manage.totalPlayer[j];
                    manage.totalPlayer[j] = manage.totalPlayer[i];
                    manage.totalPlayer[i] = temp;

                }
            }
              
        }

            for (int i=0;i<manage.totalPlayer.Count;i++)
        {
          manage.ScoreCard[i].transform.GetChild(0).GetComponent<Text>().text = manage.totalPlayer[i].GetComponent<PlayerMovement>().username.ToString();
            float score = manage.totalPlayer[i].GetComponent<PlayerMovement>().secondTaken;
            manage.ScoreCard[i].transform.GetChild(1).GetComponent<Text>().text = score.ToString("#.00");
        }
    }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(username);
            stream.SendNext(Finished);
            stream.SendNext(secondTaken);
            
        }
        else if(stream.IsReading)
        {
           movement= (Vector3)stream.ReceiveNext();
           username = (string)stream.ReceiveNext();
           Finished = (bool)stream.ReceiveNext();
           secondTaken = (float)stream.ReceiveNext();
           
        }
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(wallCheckPoint.position, wallCheckRadius);
        Gizmos.DrawWireCube(wallCheckPoint.position, new Vector3(wallCheckWi, wallCheckHi, 0));
    }
}
