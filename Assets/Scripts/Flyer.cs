using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Enemy
{
    [SerializeField] float rangeToStartChase;
    private bool isChasing;

    [SerializeField] float turnSpeed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isChasing)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < rangeToStartChase)
            {
                isChasing = true;
            }
        }
        else
        {
            if (!isRecoiling && player.pState.alive)
            {
                //Gets the direction of the player from the object's current position
                Vector3 direction = transform.position - player.transform.position;

                //Gives the angle of our object based on the direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                //Gives us the rotation angle we SHOULD be at using the angle from previous line
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                //Changes our rotation to match what we should be at
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                //Moves the object towards the player regardless of which way it is facing
                //transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

                //Moves the object towards the player only via it's "front"
                transform.position += -transform.right * speed * Time.deltaTime;
            }
        }

    }
}
