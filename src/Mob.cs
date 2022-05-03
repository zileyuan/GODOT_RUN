using Godot;
using System;

public class Mob : RigidBody2D
{
    public void OnExited()
    {
        QueueFree();
    }
    public override void _Ready()
    {
        GetNode<VisibilityNotifier2D>("VisibilityNotifier2D").Connect("screen_exited", this, nameof(OnExited));
        var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animSprite.Playing = true;
        string[] mobTypes = animSprite.Frames.GetAnimationNames();
        animSprite.Animation = mobTypes[GD.Randi() % mobTypes.Length];
    }
}
