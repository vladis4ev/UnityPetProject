using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonyCowScript : MonoBehaviour {

    public bool folloMode;

    private Collider2D col;
    private bool startMove;
    private Vector3 endPos;
    private Animator animator;
    private string animName;
    private Vector3 randomPoint;
    private float randomDist;
    static private Vector3 fieldSizes;
    private float upBorderYard;

    void Awake() {
        animator = this.GetComponent<Animator>();
        col = this.GetComponent<Collider2D>();
        col.enabled = false;
    }

    void Start() {
        col.enabled = true;
        folloMode = false;
        PonyCowStop();

        upBorderYard = -Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height - MainScript.Instance.guardrailPixProp.height, 0f)).y;
        fieldSizes = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    void Update() {
        if (startMove) {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
            transform.position = Vector3.MoveTowards(transform.position, randomPoint, Time.deltaTime);
            if (transform.position == randomPoint)
                PonyCowStop();
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        //when a cow collides with an object, stop its movement
        if (coll.gameObject.tag == "ponyCow"
            || coll.gameObject.tag == "dog"
            || coll.gameObject.tag == "guardrail")
            PonyCowStop();
    }

    private void PonyCowFollow(Vector3 getDiff) {
        StopCoroutine("PonyCowMove");
        startMove = false;

        folloMode = true;//turns on the cow folloMode
        ChangeDirection(getDiff);//determine the direction of motion to switch the animation
    }

    private void PonyCowFollowLastStep(Vector3 endPos) {
        randomPoint = endPos;
        startMove = true;
    }

    private void PonyCowStop() {
        startMove = false;
        animator.speed = 0f;
        if (folloMode)
            return;

        animName = animator.runtimeAnimatorController.animationClips[Random.Range(0, animator.runtimeAnimatorController.animationClips.Length)].name;
        animator.Play(animName);
        StartCoroutine("PonyCowMove");
    }

    private IEnumerator PonyCowMove() {//free movement of a cow
        yield return new WaitForSeconds(Random.Range(2.5f, 10f));

        randomDist = Random.Range(5f, 10f) / 2;

        switch (animName) {
            case "ponyCow_anim_00":
                if (transform.position.y + randomDist > fieldSizes.y - 1)
                    randomPoint = new Vector3(transform.position.x, fieldSizes.y - 1, fieldSizes.y - 1);
                else
                    randomPoint = new Vector3(transform.position.x, transform.position.y + randomDist, transform.position.y + randomDist);
                break;
            case "ponyCow_anim_01":
                if (transform.position.x - randomDist < -fieldSizes.x + 1)
                    randomPoint = new Vector3(-fieldSizes.x + 1, transform.position.y, transform.position.y);
                else
                    randomPoint = new Vector3(transform.position.x - randomDist, transform.position.y, transform.position.y);
                break;
            case "ponyCow_anim_02":
                if (transform.position.y - randomDist < upBorderYard)
                    randomPoint = new Vector3(transform.position.x, upBorderYard, upBorderYard);
                else
                    randomPoint = new Vector3(transform.position.x, transform.position.y - randomDist, transform.position.y - randomDist);
                break;
            case "ponyCow_anim_03":
                if (transform.position.x + randomDist > fieldSizes.x - 1)
                    randomPoint = new Vector3(fieldSizes.x - 1, transform.position.y, transform.position.y);
                else
                    randomPoint = new Vector3(transform.position.x + randomDist, transform.position.y, transform.position.y);
                break;
        }
        animator.speed = 1f;
        startMove = true;
    }

    private void ChangeDirection(Vector3 getDiff) {
        if (animator.speed == 0f)
            animator.speed = 1f;

        if (getDiff.x < 0 && (getDiff.y <= 0.5 && getDiff.y >= -0.5)) 
            animName = "ponyCow_anim_03";
        else if (getDiff.x >= 0 && (getDiff.y <= 0.5 && getDiff.y >= -0.5))
            animName = "ponyCow_anim_01";
        else if (getDiff.y < 0 && (getDiff.x <= 0.5 && getDiff.x >= -0.5))
            animName = "ponyCow_anim_00";
        else if (getDiff.y >= 0 && (getDiff.x <= 0.5 && getDiff.x >= -0.5))
            animName = "ponyCow_anim_02";
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            animator.Play(animName);

    }
}
