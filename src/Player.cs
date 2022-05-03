using Godot;
using System;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();
    [Export]
    public int Speed = 400;
    public Vector2 ScreenSize;
    public Vector2 target;

    public void OnPlayerBodyEntered(PhysicsBody2D body)
    {
        Hide();
        EmitSignal(nameof(Hit));
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }
    public void Start(Vector2 pos)
    {
        Position = pos;
        target = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch && @event.IsPressed())
        {
            InputEventScreenTouch touch = @event as InputEventScreenTouch;
            target = touch.Position;
        }
    }
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Connect("body_entered", this, nameof(OnPlayerBodyEntered));
        Hide();
    }
    public override void _Process(float delta)
    {
        var velocity = Vector2.Zero;
        if (OS.HasFeature("mobile"))
        {
            if (Position.DistanceTo(target) > 10)
            {
                velocity = target - Position;
            }
        }
        else
        {
            if (Input.IsActionPressed("move_right"))
            {
                velocity.x += 1;
            }
            else if (Input.IsActionPressed("move_left"))
            {
                velocity.x -= 1;
            }
            else if (Input.IsActionPressed("move_down"))
            {
                velocity.y += 1;
            }
            else if (Input.IsActionPressed("move_up"))
            {
                velocity.y -= 1;
            }
        }
        var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        if (velocity.x != 0)
        {
            animatedSprite.Animation = "walk";
            animatedSprite.FlipV = false;
            animatedSprite.FlipH = velocity.x < 0;
        }
        else if (velocity.y != 0)
        {
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
            animatedSprite.FlipH = false;
        }
        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite.Play();
        }
        else
        {
            animatedSprite.Stop();
        }
        Position += velocity * delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
            y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
        );
    }
}
