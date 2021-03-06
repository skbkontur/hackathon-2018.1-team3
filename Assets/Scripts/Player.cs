﻿using System;
using Extensions;
using UnityEngine;
using Weapons;

public class Player : MovingObject
{
    private Rigidbody2D rb2d;
    private CircleCollider2D c2d;
    private PlayerAttributes playerAttributes;
    private bool allowedMoveUp;
    private bool allowedMoveDown;
    private bool allowedMoveLeft;
    private bool allowedMoveRight;
    private Weapon playerWeapon;
    private float timeBeforeNextShoot;
    public GameObject Bullet;

    protected override float ObjectSpeed
    {
        get { return 5.0f; }
    }

    // Use this for initialization
	void Start ()
	{
	    rb2d = GetComponent<Rigidbody2D>();
	    c2d = GetComponent<CircleCollider2D>();
	    playerAttributes = GetComponent<PlayerAttributes>();
	    playerWeapon = playerAttributes.CurrentPlayerWeapon
	        ? playerAttributes.CurrentPlayerWeapon
	        : gameObject.AddComponent<Katana>();
	}
	
	// Update is called once per frame
    void Update()
    {
        AttemptToMove();
        AttemptToAttack();
    }

    private void AttemptToMove()
    {
        var currentMovement = GetCurrentMovement();
        if (Math.Abs(currentMovement.x) < Math.Abs(currentMovement.y))
        {
            currentMovement.x = 0;
        }
        else
        {
            currentMovement.y = 0;
        }

        if (currentMovement.HasMovementAtAnyAxis())
        {
            var direction = GetCurrentDirection(currentMovement);
            PlayerAttributes.Direction = direction;
            MoveObject(rb2d, currentMovement);


            if (Input.GetButton("Drink"))
            {
                var attr = GetComponent<PlayerAttributes>();
                if (attr.flaskCharges > 0 && attr.health < attr.maxHealth)
                {
                    attr.flaskCharges--;
                    attr.health = attr.maxHealth;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb2d.velocity = new Vector2(0, 0);
            collision.rigidbody.velocity = new Vector2(0, 0);
        }

        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            playerAttributes.ChangeHealthValue(-20);
        }
    }
    
    private void AttemptToAttack()
    {
        if (timeBeforeNextShoot > float.Epsilon)
        {
            timeBeforeNextShoot -= Time.deltaTime;
            return;
        }

        var fire = Input.GetKeyUp("space");
        if (fire)
        {
            Instantiate(Bullet, rb2d.position + GetBulletInstantionPositionByDirection(), Quaternion.identity);
            timeBeforeNextShoot = Time.deltaTime * 50;
        }
    }
     
    private Vector2 GetBulletInstantionPositionByDirection()
    {
        var direction = PlayerAttributes.Direction;
        var x = 0f;
        var y = 0f;
        if (direction == Direction.Right)
            x = c2d.radius + 1f;
        else if (direction == Direction.Left)
            x = -c2d.radius - 1f;
        else if (direction == Direction.Top)
            y = c2d.radius + 1f;
        else if (direction == Direction.Down)
            y = -c2d.radius - 1f;
		
        return new Vector2(x, y);
    }

    private Vector2 GetCurrentMovement()
    {
        var inputMupltiplier = GetMovementMultiplier();
        var xMovement = inputMupltiplier * Input.GetAxisRaw("Horizontal");
        var yMovement = inputMupltiplier * Input.GetAxisRaw("Vertical");

        return new Vector3
        {
            x = xMovement,
            y = yMovement,
            z = 0
        };
    }

    private Direction GetCurrentDirection(Vector2 currentMovement)
    {
        if (currentMovement.x >= float.Epsilon)
            return Direction.Right;
        if (currentMovement.x < -float.Epsilon)
            return Direction.Left;
        
        return currentMovement.y >= float.Epsilon
            ? Direction.Top
            : Direction.Down;
    }
}

