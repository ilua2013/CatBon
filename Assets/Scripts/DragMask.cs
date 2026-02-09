using UnityEngine;
using System.Collections;

public class DragMask : MonoBehaviour {

	public AudioClip[] myClip;
	public bool maskOnCharacter = false;
	public bool lockedObject = false;
	public Vector2 originalPos;
	public Vector2 objPosition;
	public bool canMove = false;
	public bool objMoved = false;
	public Vector2 targetPosition;
	public bool animationParticle = false;

	void Start(){
		originalPos = transform.position; //verify the original position of the object
	}
	
	void Update(){

		
		{

			

				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

				//Will only be able to move the mask
				if (hit.collider != null && hit.collider.gameObject.tag == "Draggable"&& hit.collider.gameObject.name == this.gameObject.name ){ 
					canMove = true;
				}

				
					if (Input.GetMouseButton(0)){
					if (!lockedObject&&canMove)
				{

							//Moves the object
							Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
							Vector2 objPosition = Camera.main.ScreenToWorldPoint (mousePosition);
							transform.position = objPosition;
							objMoved = true;
						
					}
		}

				if (Input.GetMouseButtonUp(0))
				{
					
					canMove = false;

					if (maskOnCharacter == true && !lockedObject) {
						//Lock the object on the character
						GetComponent<AudioSource> ().PlayOneShot (myClip[0]);
						lockedObject = true;
						
						transform.position = targetPosition;
						animationParticle = true;

					} 
					if(maskOnCharacter == false && !lockedObject && objMoved == true) {
					    
						//http://itween.pixelplacement.com/documentation.php
						iTween.MoveTo(gameObject,originalPos,1f); //moves the object back to its original position smoothly
						GetComponent<AudioSource> ().PlayOneShot (myClip[1]);
						objMoved = false;
					}
				}
			
		}
	}

	public void OnTriggerEnter2D(Collider2D collision) {	
		maskOnCharacter = true;
	}
		
	public void OnTriggerExit2D(Collider2D collision){
		maskOnCharacter = false;
	}

}

