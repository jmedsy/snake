using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        Head.notification = this;
    }

    // Update is called once per frame
    void Update()
    {
        //AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (Input.GetKeyDown("space"))
        {
            //PlayOuchAnim();
        }
    }

    public void PlayOuchAnim(float x, float y)
    {
        //this.transform.eulerAngles = new Vector3(0f, 0f, 45f);
        this.transform.position = new Vector3(x, y);
        anim.Play("Ouch", -1, 0f);
        anim.SetBool("isOuching", true);
    }

    public void PlayEatAnim(float x, float y)
    {
        //this.transform.eulerAngles = new Vector3(0f, 0f, 45f);
        this.transform.position = new Vector3(x, y);
        anim.Play("Eat", -1, 0f);
        anim.SetBool("isEating", true);
    }
}
