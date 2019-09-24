using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [SerializeField] ParticleSystem pfxBoost, pfxWallBoost;

    public int MaxSpeedBoost = 0;

    public int MaxStunned = 0;
    public int MaxStunUsed = 0;
    public int MaxJump = 0;

    public PowerUp temp;

    public float currentDist;

    public List<Anima2D.SpriteMeshInstance> Order = new List<Anima2D.SpriteMeshInstance>();
  
        private void Start()
    {

        /*
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
            Debug.Log("Active");
        }
        else
        {
            playerMovement = this.GetComponent<PlayerMovement>();
            Debug.Log("Check");
        }
        */
        
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        manage = GameObject.Find("Manager").GetComponent<Manager>();
        controlData = GameObject.Find("ControlData").GetComponent<ControlData>();

       rb2d.gravityScale = controlData.playerGravityScale;

        //  username = controlData.userName;

        //  manage.totalPlayerCharacterNo.Add(manage.UI.chosenCharacter);
        //   t1 = GameObject.Find("t1").GetComponent<Text>();

        //while(PhotonNetwork.CountOfPlayers!=2)
        //{
        //    yield return null;
        //}
        run = true;
         animator.SetBool("Idle", true);
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
            Debug.Log("changing");

            manage.startBtn.onClick.AddListener(() => startcountFunc());
            FrontCheckOffset = this.transform.position - frontCheck.transform.position;
            BackCheckOffset = this.transform.position - BackCheck.transform.position;
           // GetComponent<SpriteRenderer>() .sortingOrder = 2;
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
        GetComponent<Rigidbody2D>().isKinematic = true;

        //    GetComponent<CharacterController2D>().OnLandEvent.AddListener(OnCharacterLanded);
    }

    const int MIN_WALL_BOOST = 2, MAX_WALL_BOOST = 5;
    const float BOOST_TIME_PER_WALL_BOOST = 0.2f;

    void OnCharacterLanded() {
        if (wallBoostBuildup > MIN_WALL_BOOST) StartCoroutine(SpeedUp(gameObject, Mathf.Min(wallBoostBuildup - MIN_WALL_BOOST, MAX_WALL_BOOST) * BOOST_TIME_PER_WALL_BOOST));
        wallBoostBuildup = 0;
    }

    public GameObject ShurikenObj;

    public static PlayerMovement playerMovement;
        
    public void ShurikenaAttackFunc()
    {
        //Debug.Log(this.username);
        MaxStunUsed++;
        StartCoroutine(zoomOut());
        if (pv.IsMine) {
            manage.BoostAudioSource.clip = manage.ShurikenHitSound;
            manage.BoostAudioSource.Play();
        }
        temp = PhotonNetwork.Instantiate(Manager.manage.ShurikenPrefab.name, ShurikenObj.transform.position, Quaternion.identity).GetComponent<PowerUp>();
        //pv.RPC("NinjaName", RpcTarget.AllBuffered, username);
        //Debug.Log(username);
        temp.SentBy = gameObject;
        temp.SentByusername = username;
        manage.ShurikenBtn.gameObject.SetActive(false);
        manage.PowerUpUsedSave();
        Statistics.stats.Pref("StunHit");

    }

    [PunRPC]
    public void NinjaName(string ninja)
    {
        temp.SentByusername = ninja;
        Debug.Log(ninja);
    }

    [PunRPC]
    public void RunGlobalSound() {
        manage.BoostAudioSource.clip = manage.RunBoostSound;
        manage.BoostAudioSource.Play();
    }

    IEnumerator CameraShake(float duration,float Magnitude,float vect)
    {
        Vector3 orgPos = Camera.main.transform.localPosition;
        float elapse = 0;
        while(elapse<duration)
        {
            float x = Random.Range(-vect,vect)*Magnitude;
            float y = Random.Range(-vect, vect) * Magnitude;
            Camera.main.transform.localPosition = new Vector3(orgPos.x+x, orgPos.y+y, orgPos.z);
            elapse += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = orgPos;
    }
    IEnumerator ZoomIn()
    {
        Camera.main.GetComponent<CameraFollow>().offset.y = 2;
        while (Camera.main.orthographicSize>5)
        {
            Camera.main.orthographicSize -= 0.5f;
            yield return null;
        }
        Camera.main.orthographicSize = 7f;
        yield return new WaitForSeconds(1f);
        while (Camera.main.orthographicSize < 10)
        {
            Camera.main.orthographicSize += 0.1f;
            yield return null;
        }
        Camera.main.orthographicSize = 10f;
        Camera.main.GetComponent<CameraFollow>().offset.y = 6;

    }
    IEnumerator zoomOut()
    {
        Camera.main.GetComponent<CameraFollow>().offset.x = 15f;

        while (Camera.main.orthographicSize < 13)
        {
            Camera.main.orthographicSize += 0.5f;
            yield return null;
        }
        Camera.main.orthographicSize = 13f;
        yield return new WaitForSeconds(0.5f);

        while (Camera.main.orthographicSize > 10)
        {
            Camera.main.orthographicSize -= 0.1f;
            yield return null;
        }
         Camera.main.orthographicSize = 10f;
        Camera.main.GetComponent<CameraFollow>().offset.x = 6.5f;

        
    }
    public void speedUpFunc()
    {
        StartCoroutine(CameraShake(0.15f, 0.2f, 0.5f));

        MaxSpeedBoost++;
        StartCoroutine(ZoomIn());
        if (pv.IsMine) {

            pv.RPC("RunGlobalSound", RpcTarget.AllBuffered, null);

        }
        manage.PowerUpUsedSave();
        StartCoroutine(SpeedUp(this.gameObject,1.5f));
        Manager.manage.attackBtn.gameObject.SetActive(false);

    }
    bool InitBoost;
    public IEnumerator InitSpeedUp() {
        manage.InitialBoost = false;
        if (pv.IsMine) {
            if (InitBoost == false) {
                InitBoost = true;
                manage.t1.gameObject.SetActive(true);
                int s = 0;
                s += 1;
                manage.t1.text = s.ToString();
                Manager.manage.attackBtn.interactable = false;
                if (pfxBoost) pfxBoost.Play();
                float temp1 = runSpeed;
                controlData.TargetSpeed += 50;
                controlData.MaxRunForce += 150;
                MinRunForce += 150;
                //  runSpeed += Time.deltaTime * controlData.MaxRunForce * 100;
                //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed*1.5f;
                yield return new WaitForSeconds(1.5f);
                manage.BooseTimeSave();
                Debug.Log("BoosterUSed");
                Debug.Log(manage.InitialBoost);
                //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed/1.5f ;
                //   runSpeed += Time.deltaTime * controlData.MaxRunForce / 100;
                controlData.TargetSpeed -= 50;
                controlData.MaxRunForce -= 150;
                MinRunForce -= 150;
                runSpeed = temp1;
                runSpeed += 1;
                if (pfxBoost) pfxBoost.Stop();
                Manager.manage.attackBtn.interactable = true;
            }
        }
    }

    public IEnumerator SpeedUp(GameObject temp, float speedUpTime)
    {
        animator.SetBool("boostrun", true);


        if (pv.IsMine)
        {
            Manager.manage.attackBtn.interactable = false;
            if (pfxBoost) pfxBoost.Play();
            float temp1 = runSpeed;
            controlData.TargetSpeed += 50;
            controlData.MaxRunForce += 2000;
            MinRunForce += 2000;
            //  runSpeed += Time.deltaTime * controlData.MaxRunForce * 100;
            //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed*1.5f;
            yield return new WaitForSeconds( 0.5f);
            manage.Screen_Power.gameObject.SetActive(true);

            yield return new WaitForSeconds(speedUpTime-0.5f);
            manage.BooseTimeSave();
            Debug.Log("BoosterUSed");
            //temp.GetComponent<PlayerMovement>().controlData.TargetSpeed = temp.GetComponent<PlayerMovement>().controlData.TargetSpeed/1.5f ;
            //   runSpeed += Time.deltaTime * controlData.MaxRunForce / 100;
            controlData.TargetSpeed -= 50;
            controlData.MaxRunForce -= 2000;
            MinRunForce -= 2000;
            runSpeed = temp1;
            runSpeed += 1;
            manage.BoostAudioSource.Stop();

            if (pfxBoost) pfxBoost.Stop();
            Manager.manage.attackBtn.interactable = true;
            manage.Screen_Power.gameObject.SetActive(false);

        }

        animator.SetBool("boostrun", false);
        animator.SetTrigger("run");

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
        while (manage.startCount !=1 ) 
        {
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
        pv.RPC("RunSync", RpcTarget.AllBuffered, null);

        
          runSpeed = 10;
        animator.SetBool("Idle", false);

        for (int i = 0; i < manage.TrafficLight.Count; i++)
        {
            manage.TrafficLight[i].gameObject.SetActive(false);
        }
        secondTaken = 0;
        SecStart = true;
        animator.SetTrigger("run");

     //   Camera.main.orthographicSize = 10;

    }
    [PunRPC]
    public void RunSync() {
        run = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
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
            if (manage.totalPlayer[i].gameObject != gameObject)
            {
                if (manage.PlayerDist[i] != 0)
                {
                    manage.PlayerDistUI[i].gameObject.SetActive(true);

                }
                else
                {
                    manage.PlayerDistUI[i].gameObject.SetActive(false);

                }

                manage.PlayerDistBG[i].color = new Color(255, 255, 255, 1);

                manage.PlayerDistUI[i].value = manage.PlayerDist[i];
            }
            else
            {
                currentDist = Vector3.Distance(transform.position, manage.finish.transform.position);
                manage.PlayerDist[i] = 1 - (currentDist / manage.FinalDist);
                manage.PlayerDistUI[i].gameObject.SetActive(true);
                manage.PlayerDistUI[i].value = 1 - (currentDist / manage.FinalDist);
                manage.PlayerDistBG[i].color = new Color(255, 208, 0, 1);
                manage.t1.text = manage.PlayerDist[i].ToString();
            }
        }

        for (int i = 0; i < manage.totalPlayer.Count; i++)
        {
            for (int j = i + 1; j < manage.totalPlayer.Count; j++)
            {
                if (manage.totalPlayer[i].GetComponent<PlayerMovement>().currentDist > manage.totalPlayer[j].GetComponent<PlayerMovement>().currentDist)
                {
                    GameObject winner = manage.totalPlayer[j];
                    manage.totalPlayer[j] = manage.totalPlayer[i];
                    manage.totalPlayer[i] = winner;
                }
            }

        }

        manage.PlayerPos = manage.totalPlayer;
        manage.PlayerPos.Sort(delegate (GameObject a, GameObject b)
        {
            return (a.GetComponent<PlayerMovement>().currentDist).CompareTo(b.GetComponent<PlayerMovement>().currentDist);
        });

        int PlayerPosition = manage.PlayerPos.IndexOf(gameObject) + 1;
        int totalPlayer = manage.totalPlayer.Count;
        manage.t3.text = PlayerPosition.ToString() + "/" + totalPlayer.ToString();

        if (totalPlayer > 1 )
        {
            if (PlayerPosition == totalPlayer)
            {
                manage.t3.color = Color.red;
            }
            else if (PlayerPosition == 1)
            {
                manage.t3.color = Color.green;
            }
            else
            {
                manage.t3.color = Color.yellow;
            }
        }
        else
        {
            manage.t3.color = Color.white;
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

        if (Input.GetKeyDown(KeyCode.Q))
        {

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

    [PunRPC]
    public void shurikenSoundGlobal()
    {
        manage.BoostAudioSource.clip = manage.ShurikenStunSound;
        manage.BoostAudioSource.Play();
        
        //Debug.Log(ninja);
    }

    public void PlayerPunished()
    {
        
        MaxStunned++;
        if (pv.IsMine) {
            StartCoroutine(CameraShake(0.15f, 0.2f, 2));

            pv.RPC("shurikenSoundGlobal", RpcTarget.AllBuffered, null);
            
           
        }
        Statistics.stats.Pref("Stunned");
        if(pv.IsMine)
        {
            StartCoroutine(PlayerPunishedRoutine());

        }
    }
    IEnumerator PlayerPunishedRoutine()
    {
       
        NoRun = true;
        bool Shurikentemp=false; bool SpeedTemp=false;
        if(manage.attackBtn.gameObject.activeInHierarchy)
        {
            SpeedTemp = true;
            manage.attackBtn.gameObject.SetActive(false);
        }
       
        if (manage.ShurikenBtn.gameObject.activeInHierarchy)
        {
            Shurikentemp = true;
            manage.ShurikenBtn.gameObject.SetActive(false);
        }
        animator.SetBool("stun", true);
        StartCoroutine(CharacterStop(transform.position));

        yield return new WaitForSeconds(0.25f);
        while (Camera.main.GetComponent<CameraFollow>().offset.y > 1f)
        {
            Camera.main.GetComponent<CameraFollow>().offset.y -= 0.5f;
            Camera.main.GetComponent<CameraFollow>().offset.x -= 0.5f;

            yield return null;
        }
        Camera.main.GetComponent<CameraFollow>().offset.y = 1f;
        Camera.main.GetComponent<CameraFollow>().offset.x = 1f;

        while (Camera.main.orthographicSize > 7.5f)
        {
            Camera.main.orthographicSize -= 0.5f;
            yield return null;
        }
        Camera.main.orthographicSize = 7.5f;

        
        //  runSpeed = 0;
        // runSpeed = controlData.TargetSpeed * 1.5f;
        yield return new WaitForSeconds(1.75f);

        NoRun = false;

        animator.SetBool("stun", false);
        animator.SetTrigger("run");

        // runSpeed = controlData.TargetSpeed;
        //  runSpeed = controlData.TargetSpeed;
        while (Camera.main.GetComponent<CameraFollow>().offset.y < 6f)
        {
            Camera.main.GetComponent<CameraFollow>().offset.y += 0.5f;
            Camera.main.GetComponent<CameraFollow>().offset.x += 0.5f;

            yield return null;
        }
        Camera.main.GetComponent<CameraFollow>().offset.y = 6f;
        Camera.main.GetComponent<CameraFollow>().offset.x = 6.5f;
        while (Camera.main.orthographicSize < 10f)
        {
            Camera.main.orthographicSize += 0.5f;
            yield return null;
        }
        Camera.main.orthographicSize = 10f;

        if(Shurikentemp)
        {
            manage.ShurikenBtn.gameObject.SetActive(true);
        }
        if(SpeedTemp)
        {
            manage.attackBtn.gameObject.SetActive(true);
        }

    }
    IEnumerator CharacterStop(Vector3 pos)
    {
        while(NoRun)
        {
            transform.position = pos;
            yield return null;
        }
    }

    public void oppositeJump() {
        transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
        rb2d.velocity = new Vector2(0, 0);
        rb2d.angularVelocity = 0f;
        rb2d.AddForce(new Vector2((-controlData.walljumpForceLeft * 1000f * 2.5f * Time.deltaTime), (controlData.m_JumpForce * controlData.walljumpAmplitudeLeft * 1000f * Time.deltaTime)));
        //  Debug.LogError("jump");
        manage.wallJumpSave();

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        wallCheckPoint = BackCheck.transform;
    }

    public float PrevXPos;
    public float StraightJumpX;
    private void FixedUpdate()
    {

        if (manage.attackBtn.gameObject.activeInHierarchy) {

            if (Input.GetKeyDown(KeyCode.Space)) {
                speedUpFunc();
            }
        }
        if (manage.ShurikenBtn.gameObject.activeInHierarchy) {

            if (Input.GetKeyDown(KeyCode.Space)) {
                ShurikenaAttackFunc();
            }
        }
        if (PrevXPos+0.1 >= transform.position.x)
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

                }


            }
           


        }
        if (Input.touchCount == 1 || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (manage.TrafficLight[2].activeInHierarchy)
            {
                manage.InitialBoost = true;
            }

        }
        if (manage.InitialBoost)
        {
            if (!manage.TrafficLight[2].activeInHierarchy)
            {
                StartCoroutine(InitSpeedUp());
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
                    Debug.LogError("straigh jump");
                    StraightJumpX = transform.position.x;
              //      StartCoroutine(WallJumpRoutine());
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
                                manage.BoostAudioSource.clip = manage.JumpClip;
                                manage.BoostAudioSource.Play();
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
                        if (Input.mousePosition.x > Screen.width * .25f || Input.mousePosition.y > Screen.width * .25f )
                        {
                            if (DOTouchCount)
                            {
                                if (NormalMove == false)
                                {
                                    runSpeed = controlData.BaseSpeed - 1;
                                    NormalMove = true;

                                }
                                manage.BoostAudioSource.clip = manage.JumpClip;
                                manage.BoostAudioSource.Play();
                    Debug.LogError("straigh jump");
                             //   oppositeJump();
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
                  //  rb2d.velocity = new Vector2(0, -controlData.WallSlideGravity);

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
        manage.BoostAudioSource.clip = manage.WallJumpClip;
        manage.BoostAudioSource.Play();
        if (temp!=null) {
            if (temp.GetComponent<Collider2D>()) {
                temp.enabled = false;

            }
        }
        yield return null;
        if (front)
        {
            transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);

            rb2d.velocity = new Vector2(0, 0);
            rb2d.angularVelocity = 0f;
            rb2d.AddForce(new Vector2((controlData.walljumpForceLeft * 2.5f * 1000f * Time.deltaTime), (controlData.m_JumpForce * controlData.walljumpAmplitudeLeft * 1000f * Time.deltaTime)));
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
         //   Debug.LogError("jump");
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
        if (pfxWallBoost && wallBoostBuildup > MIN_WALL_BOOST) pfxWallBoost.Play();

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

    public void UserNameShow()
    {
        MaxSpeedUsername.Clear();
        MaxStunnedUsername.Clear();
        MaxStunUsedUsername.Clear();
        MaxJumpUsername.Clear();

        Debug.LogError("s");
        for (int i = 0; i < manage.totalPlayer.Count; i++)
        {

           // if (MaxSpeedBoost1 == manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxJump)
            {
                if(!MaxJumpUsername.Contains(manage.totalPlayer[i].GetComponent<PlayerMovement>().username))
                {
                    MaxJumpUsername.Add(manage.totalPlayer[i].GetComponent<PlayerMovement>().username);

                }

              //  Debug.LogError(MaxSpeedBoost1);
                Debug.LogError(manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxJump);


            }

        }
        for(int i=0;i< MaxJumpUsername.Count;i++)
        {
                manage.MaxUsed[0].GetComponent<Text>().text += MaxJumpUsername[i];
            manage.MaxUsed[0].GetComponent<Text>().text += ",";
        }
    }
 //  public
    public List<string> MaxSpeedUsername = new List<string>();
    public List<string> MaxStunnedUsername = new List<string>();
    public List<string> MaxStunUsedUsername = new List<string>();
    public List<string> MaxJumpUsername = new List<string>();

    IEnumerator EndCameraLerp()
    {
        Camera.main.GetComponent<CameraFollow>().offset.y = 2;
        while (Camera.main.orthographicSize > 5)
        {
            Camera.main.orthographicSize -= 0.1f;
            yield return null;
        }
        Camera.main.orthographicSize = 5f;
        yield return new WaitForSeconds(1f);
        while (Camera.main.orthographicSize < 10)
        {
            Camera.main.orthographicSize += 0.05f;
            yield return null;
        }
        Camera.main.orthographicSize = 10f;
        Camera.main.GetComponent<CameraFollow>().offset.y = 6;

    }

    [PunRPC]
    public void NewScoreCard() {

        if (manage.LocalPlayer.GetComponent<PlayerMovement>().Finished) {

            manage.ScoreCardMenu.gameObject.SetActive(true);
            manage.MaxUsedMenu.gameObject.SetActive(true);

            for (int i = 0; i < manage.scoreShow; i++) {
                manage.ScoreCard[i].transform.GetChild(0).GetComponent<Text>().text = manage.playerReached[i].username.ToString();
                float score = manage.playerReached[i].secondTaken;
                manage.ScoreCard[i].transform.GetChild(1).GetComponent<Text>().text = score.ToString("#.00");
            }
            int MaxSpeedBoost1 = 0, MaxStunned1 = 0, MaxStunUsed1 = 0, MaxJump1 = 0;
            string MaxSpeedBoost_s="", MaxStunned_s="", MaxStunUsed_s="", MaxJump_s = "";
            for (int i=0;i<manage.totalPlayer.Count;i++)
            {
                if(MaxSpeedBoost1 < manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxSpeedBoost)
                {
                    MaxSpeedBoost1 = manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxSpeedBoost;
                    MaxSpeedBoost_s= manage.totalPlayer[i].GetComponent<PlayerMovement>().username;
                }
                if (MaxStunned1 < manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxStunned)
                {
                    MaxStunned1 = manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxStunned;
                    MaxStunned_s = manage.totalPlayer[i].GetComponent<PlayerMovement>().username;
                }
                if (MaxStunUsed1 < manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxStunUsed)
                {
                    MaxStunUsed1 = manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxStunUsed;
                    MaxStunUsed_s = manage.totalPlayer[i].GetComponent<PlayerMovement>().username;
                }
                if (MaxJump1 < manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxJump)
                {
                    MaxJump1 = manage.totalPlayer[i].GetComponent<PlayerMovement>().MaxJump;
                    MaxJump_s = manage.totalPlayer[i].GetComponent<PlayerMovement>().username;
                }
            }
           // UserNameShow();

              manage.MaxUsed[0].GetComponent<Text>().text = MaxSpeedBoost_s;
             manage.MaxUsed[1].GetComponent<Text>().text = MaxStunned_s;
             manage.MaxUsed[2].GetComponent<Text>().text = MaxStunUsed_s;
              manage.MaxUsed[3].GetComponent<Text>().text = MaxJump_s;


        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish") {
            if (!manage.playerReached.Contains(this)) {
                manage.scoreShow += 1;
                manage.playerReached.Add(this);
                manage.playerReachedSec.Add(this.secondTaken);
            }
          //  pv.RPC("Reach", RpcTarget.AllBuffered, null);

        }
        if (pv.IsMine)
        {

            if (collision.tag == "Finish")
            {
                Debug.LogError("finish");
                Statistics.stats.Pref("FinishTrack" + manage.UI.selectedLevel);
                SecStart = false;
                Finished = true;

                if (manage.reach < 10) {
                    Statistics.stats.Pref("TotalWin");


                    won.gameObject.SetActive(true);
                    Camera.main.GetComponent<CameraFollow>().offset = new Vector3(0, 1.2f, -10f);
                    animator.SetBool("Idle", true);
                    //   GetComponent<SpriteRenderer>().sortingOrder = 2;
                    StartCoroutine(stop());
                    //    manage.won.gameObject.SetActive(true);

                    pv.RPC("Reach", RpcTarget.AllBuffered, null);
                    Destroy(failed);
                    //   pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);

                    //  ScoreShow();
                     pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);
                    //  UserNameShow();
                 //   StartCoroutine(EndCameraLerp());
                    animator.SetBool("win", true);

                    return;
                }
                else {
                    if (failed != null) {
                        Statistics.stats.Pref("TotalLose");
                        Camera.main.GetComponent<CameraFollow>().offset = new Vector3(0, 1.2f, -10f);
                        animator.SetBool("Idle", true);

                        run = true;
                        winpos = transform.position;
                        //  pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);

                        //  ScoreShow();

                    }
                    //  pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);


                }
                int count = 0;
                for (int i = 0; i < manage.totalPlayer.Count; i++) {

                    if (manage.totalPlayer[i].GetComponent<PlayerMovement>().Finished == true) {
                        count += 1;
                    }

                }
                // pv.RPC("ScoreShow", RpcTarget.AllBuffered, null);
                if (manage.UI.PlayerCount == count) {
                    //   pv.RPC("ScoreShow", RpcTarget.AllBuffered, null);

                }

                manage.GroundTimeSave();
                //  pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);
                if (manage.playerReached[0].gameObject==this.gameObject)
                {
                    won.gameObject.SetActive(true);
                    animator.SetBool("win", true);
                    Debug.LogError("Triggered");

                }
                else
                {
                    failed.SetActive(true);
                    animator.SetBool("loss", true);
                }

            }
        }
        else
        {
            if (collision.tag == "Obstacle1")
            {
                 Physics.IgnoreCollision(collision.GetComponent<Collider>(), GetComponent<Collider>());

            }
        }


          pv.RPC("NewScoreCard", RpcTarget.AllBuffered, null);
       // StartCoroutine(EndCameraLerp());

    }


    //  [PunRPC]
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
            stream.SendNext(MaxSpeedBoost);
            stream.SendNext(MaxStunned);
            stream.SendNext(MaxStunUsed);
            stream.SendNext(MaxJump);
            //stream.SendNext(temp.SentByusername);



        }
        else if(stream.IsReading)
        {
           movement= (Vector3)stream.ReceiveNext();
           username = (string)stream.ReceiveNext();
           Finished = (bool)stream.ReceiveNext();
           secondTaken = (float)stream.ReceiveNext();
           MaxSpeedBoost=(int)stream.ReceiveNext();
           MaxStunned = (int)stream.ReceiveNext();
           MaxStunUsed = (int)stream.ReceiveNext();
           MaxJump = (int)stream.ReceiveNext();
           //temp.SentByusername = (string)stream.ReceiveNext();


        }
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(wallCheckPoint.position, wallCheckRadius);
        Gizmos.DrawWireCube(wallCheckPoint.position, new Vector3(wallCheckWi, wallCheckHi, 0));
    }
}
