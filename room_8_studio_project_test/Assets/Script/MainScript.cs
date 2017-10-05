using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public static MainScript Instance { get; private set; }

    [SerializeField]
    private GameObject dog;
    [SerializeField]
    private GameObject ponyCow;
    private int totalDogs;
    [SerializeField]
    private int totalPonyCow;
    [SerializeField]
    private GameObject guardrail;

    private Camera mainCam;
    private string dogTag;
    private GameObject currentDog;
    private Rect ponyCowPixProp;
    public Rect guardrailPixProp;
    private Renderer ponyCowRender;
    private Renderer guardrailRender;
    private Vector2 mousePos2D;

    void Awake(){
        Instance = this;
        mainCam = Camera.main;
        ponyCowRender = ponyCow.GetComponent<Renderer>();
        ponyCowPixProp = BoundsToScreenRect(ponyCowRender.bounds);//cow rect parameters in pixels
        guardrailRender = guardrail.GetComponent<Renderer>();
        guardrailPixProp = BoundsToScreenRect(guardrailRender.bounds);//yard rect parameters in pixels
        dogTag = dog.tag;

        totalDogs = 2;
        CreateGameObjects(totalDogs, dog);
        if (totalPonyCow <= 5) totalPonyCow = 10;//number of cows is not less than 5
        CreateGameObjects(totalPonyCow, ponyCow);
    }

    void Start () {

    }

    void Update(){
        if (!Input.GetMouseButton(0))

            return;

        else if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            //switch dogs by clicking on them
            if (hit.collider != null && hit.collider.gameObject.tag == dogTag && currentDog != hit.collider.gameObject){   
                if(currentDog != null) currentDog.SendMessage("DogSelect", false);
                currentDog = hit.collider.gameObject;
                currentDog.SendMessage("DogSelect", true);
                return;
            }else{
                //moving point of the current dog
                if (currentDog != null) currentDog.SendMessage("DogMove", mousePos);
            }
        }
    }

    private void CreateGameObjects(int max, GameObject go){
        for(int i = 0; i < max; i++){
            Instantiate(go, SetStartPos(go.tag, i), Quaternion.identity);
        }
    }

    private Vector3 SetStartPos(string tagName, int ind){
        Vector3 startPos;
        if (tagName == dogTag) {
            startPos = new Vector3(ind % 2 == 0 ? -1 : 1,  -2f, 0f);
        } else {
            //random position of the fielders within the field
            startPos = mainCam.ScreenToWorldPoint(new Vector3(Random.Range(ponyCowPixProp.width, Screen.width - ponyCowPixProp.width), Random.Range(ponyCowPixProp.height + guardrailPixProp.height, Screen.height - ponyCowPixProp.height), 0f));
            startPos = new Vector3(startPos.x, startPos.y, startPos.y);
        }
        return startPos;
    }

    public Rect BoundsToScreenRect(Bounds bounds){

        Vector3 origin = mainCam.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
        Vector3 extent = mainCam.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
}
