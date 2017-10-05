using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogScript : MonoBehaviour {
    private SpriteRenderer render;
    private bool startMove;
    private Vector3 endPos;
    private List<GameObject> ponyCowList = new List<GameObject>();
    //private float upBorderYard;

    void Start() {
        startMove = false;
        render = this.GetComponent<SpriteRenderer>();
        //upBorderYard = -Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height - MainScript.Instance.guardrailPixProp.height, 0f)).y;

    }

    void Update() {
        if (startMove) {
            transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * 1.5f);
            if (transform.position == endPos) {
                startMove = false;
            }
            if (ponyCowList.Count > 0) {//move the herd attached to the dog         
                AllPonyCowFollow(transform.position);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        //when colliding with a cow, we attach it to the current dog, provided that the folloMode is off
        if (coll.gameObject.tag == "ponyCow" && !coll.gameObject.GetComponent<PonyCowScript>().folloMode) {
            ponyCowList.Add(coll.gameObject);
            coll.gameObject.SendMessage("PonyCowFollow", coll.gameObject.transform.position - this.transform.position);
        }
    }
    
    private void DogSelect(bool isSelect) {
        if (isSelect)
            render.color = Color.red;//color the current dog
        else
            render.color = Color.white;
    }

    private void DogMove(Vector3 mousePos) {
        endPos = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        startMove = true;
    }

    private void AllPonyCowFollow(Vector3 dogPos) {
        foreach (GameObject go in ponyCowList) {
            if (Vector3.Distance(go.transform.position, dogPos) > .75f) {
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.y);
                go.transform.position = Vector3.MoveTowards(go.transform.position, dogPos, Time.deltaTime * .75f);
                go.SendMessage("ChangeDirection", go.transform.position - this.transform.position);//determine the direction of motion to switch the animation
            }
            if (!startMove)//at the completion of the movement of the dog, the herd takes the last steps to approach it
                go.SendMessage("PonyCowFollowLastStep", dogPos);
        }
    }
}

