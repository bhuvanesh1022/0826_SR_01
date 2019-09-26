using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class ButtonClick : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    public bool ButtonClicked=false;
        //Detect if a click occurs
   
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        //Debug.Log(name + "Game Object Click in Progress");
        ButtonClicked = true;
      //  StartCoroutine(off());
    }
    IEnumerator off()
    {
        yield return new WaitForSeconds(0.1f);
        ButtonClicked = false;

    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        //Debug.Log(name + "No longer being clicked");
        ButtonClicked = false;
      //  this.gameObject.SetActive(false);

    }


}
