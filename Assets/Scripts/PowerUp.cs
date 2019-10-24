using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PowerUp : MonoBehaviour
{

//powerup
    public bool canAttack = false;
    public Animator animator;
    public Button attackButton;
    public float throwableSpeed = 500.0f;

    public GameObject shuriken;
    public PhotonView pv;
    public GameObject SentBy;
    public string SentByusername;

    public SpriteRenderer sprit;

    public enum Power { SpeedRun, Shuriken, ShurikenAttack };

    public Power power;

    public List<GameObject> collectedCharacter = new List<GameObject>();

    private string ninja;

    /*
    private void OnEnable()
    {
        SentByusername = Manager.manage.userNameClass.userName;
    }
    */

    private void Start()
    {
        //ninja = Manager.manage.userNameClass.userName;

        //  animator = GetComponent<Animator>();
        //    attackButton = GetComponent<PlayerMovement>().manage.attackBtn;
        //   attackButton.onClick.AddListener(() => Attack());
        pv = GetComponent<PhotonView>();
        sprit = GetComponent<SpriteRenderer>();
        if(sprit==null)
        {
            sprit = transform.GetChild(0).GetComponent<SpriteRenderer>();

        }
        if(power == Power.ShurikenAttack)
        {
            StartCoroutine(destro());
         //   Destroy(this.gameObject, 6f);

        }
    }
    [PunRPC]
    void ShurikenHitUserName(string user,string name)
    {
        Manager.manage.ShurikenHitText(user, name);

        for (int i = 0; i < collectedCharacter.Count; i++)
        {
            if (SentBy == Manager.manage.LocalPlayer)
            {

                collectedCharacter.RemoveAt(i);
                break;
            }
        }

        Destroy(gameObject, 0.125f);
    }
        
    IEnumerator destro()
    {
        yield return new WaitForSeconds(5f);
        Statistics.stats.Pref("StunMissed");

        for (int i = 0; i < collectedCharacter.Count; i++)
        {
            if (SentBy == Manager.manage.LocalPlayer)
            {

                collectedCharacter.RemoveAt(i);
                break;
            }
        }
           if(collectedCharacter.Count==0)
        {
            Statistics.stats.Pref("StunMissed");
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        for (int i = 0; i < collectedCharacter.Count; i++)
        {
           Debug.Log(collectedCharacter.Count);
        }
        //  attackButton.GetComponent<Button>().enabled = canAttack;
        //  attackButton.GetComponent<Image>().enabled = canAttack;  
        if (power == Power.ShurikenAttack)
        {
            Vector3 move = transform.InverseTransformDirection(Vector3.right);
            transform.Translate(move);
        }
       
    }


    public void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;

        float move = GetComponent<PlayerMovement>().runSpeed;
        move *= .1f;
        if(pv.IsMine)
        {
            GameObject throwable = PhotonNetwork.Instantiate(shuriken.name, this.transform.position, Quaternion.identity);
            throwable.GetComponent<ThrowWeapon>().mine = gameObject;
            attackButton.GetComponent<Button>().enabled = false;
            attackButton.GetComponent<Image>().enabled = false;
        }
        //  GameObject throwable = Instantiate(shuriken, this.GetComponent<Transform>(), false);
        Debug.Log(move);
        

    }
    

    List<string> ShurikenUsername = new List<string>();
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (!collision.GetComponent<PlayerMovement>().Finished)
        //{
            //   Debug.Log("Triggered");
            if (power == Power.ShurikenAttack)
            {
                if (collision.tag == "Player")
                {
                    if (collision.gameObject != SentBy)
                    {
                        if  (!collision.GetComponent<PlayerMovement>().Finished)
                        {
                            /**/
                            //if (collision.GetComponent<PlayerMovement>().username != SentBy.GetComponent<PlayerMovement>().username)
                            //{
                            if (ShurikenUsername.Count <= 2)
                            {
                                if (!ShurikenUsername.Contains(collision.GetComponent<PlayerMovement>().username))
                                {
                                    ShurikenUsername.Add(collision.GetComponent<PlayerMovement>().username);
                                }
                                if (ShurikenUsername.Count == 2)
                                {
                                    ShurikenUsername.Clear();
                                }
                            }
                            //}




                            //pv.RPC("HitMsg", RpcTarget.AllBuffered, ninja, collision.GetComponent<PlayerMovement>().username);

                            if (ninja != Manager.manage.LocalPlayer.name)
                            {

                            }

                            //Debug.Log(collision.name);

                            //Manager.manage.ShurikenHitText(ninja, collision.GetComponent<PlayerMovement>().username);

                            if (pv.IsMine)
                            {
                                // Manager.manage.StartCoroutine(Manager.manage.ShowShurikenHitText());
                                Manager.manage.StartCoroutine(Manager.manage.SHurikenTHrownINst());
                            }
                            //   collision.gameObject.GetComponent<PlayerMovement>().pv.RPC("PlayerPunished", RpcTarget.AllBuffered, null);
                            if (!collectedCharacter.Contains(collision.gameObject))
                            {
                                collision.gameObject.GetComponent<PlayerMovement>().PlayerPunished();
                                //Manager.manage.ShurikenHitText(SentByusername, collision.GetComponent<PlayerMovement>().username);
                                //Debug.Log(SentByusername);
                                collectedCharacter.Add(collision.gameObject);
                                pv.RPC("ShurikenHitUserName", RpcTarget.AllBuffered, SentBy.GetComponent<PlayerMovement>().username, collectedCharacter[collectedCharacter.Count-1].GetComponent<PlayerMovement>().username);
                            }
                        }

                        //   this.gameObject.GetComponent<Collider2D>().enabled = false;
                        //  Destroy(this.gameObject);
                        //   StartCoroutine(PlayerPunished(collision.gameObject.GetComponent<PlayerMovement>()));
                    }
                    else
                    {
                        if (collision.gameObject != Manager.manage.LocalPlayer)
                        {
                            Debug.LogError("hehe");

                        }
                    }
                }
            }
            if (collision.tag == "Player")
            {

                if (!collectedCharacter.Contains(collision.gameObject))
                {
                    if (power == Power.SpeedRun)
                    {
                        if (collision.gameObject.GetComponent<PlayerMovement>().pv.IsMine)
                        {
                            if (Manager.manage.attackBtn.gameObject.activeInHierarchy || Manager.manage.ShurikenBtn.gameObject.activeInHierarchy)
                            {
                                Manager.manage.PowerUpReplacedSave();
                            }
                            Manager.manage.attackBtn.gameObject.SetActive(true);
                            pv.RPC("PowerUpDisableFunc", RpcTarget.AllBuffered, null);
                            Manager.manage.PowerUpCollectedSave();
                            Manager.manage.ShurikenBtn.gameObject.SetActive(false);

                        }


                    }
                    else if (power == Power.Shuriken)
                    {
                        if (collision.gameObject.GetComponent<PlayerMovement>().pv.IsMine)
                        {
                            if (Manager.manage.attackBtn.gameObject.activeInHierarchy || Manager.manage.ShurikenBtn.gameObject.activeInHierarchy)
                            {
                                Manager.manage.PowerUpReplacedSave();
                            }

                            Manager.manage.ShurikenBtn.gameObject.SetActive(true);
                            pv.RPC("PowerUpDisableFunc", RpcTarget.AllBuffered, null);
                            Manager.manage.PowerUpCollectedSave();
                            Manager.manage.attackBtn.gameObject.SetActive(false);
                        }
                    }

                    //  StartCoroutine(collision.GetComponent<PlayerMovement>().SpeedUp(collision.gameObject));
                    //    collectedCharacter.Add(collision.gameObject);
                }

            }
            if (collision.tag == "Shuriken")
            {
                //  canAttack = true;
                // GameObject collide = collision.gameObject;
                //  collide.SetActive(false);
            }
        //}
    }

    [PunRPC]
    public void HitMsg(string ninja, string col)
    {   
        Manager.manage.ShurikenHitText(ninja, col);
        Debug.Log(ninja);
    }

    [PunRPC]
    public void PowerUpDisableFunc()
    {
        StartCoroutine(PowerUpDisableRoutine());
    }

    IEnumerator PowerUpDisableRoutine()
    {
        GetComponent<Collider2D>().enabled = false;
        sprit.enabled = false;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider2D>().enabled = true;
        sprit.enabled = true;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SentByusername);
        }
        else if (stream.IsReading)
        {
            SentByusername = (string)stream.ReceiveNext();
        }
    }



}
