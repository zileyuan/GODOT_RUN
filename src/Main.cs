using Godot;
using System;

public class Main : Node
{
    [Export]
    public PackedScene MobScene;
    public int Score;
    public void GameOver()
    {
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<HUD>("HUD").ShowGameOver();
    }
    public void NewGame()
    {
        GetNode<AudioStreamPlayer>("Music").Play();
        GetTree().CallGroup("mobs", "queue_free");
        Score = 0;

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(Score);
        hud.ShowMessage("Get Ready!");
    }
    public void OnMobTimerTimeout()
    {
        // Note: Normally it is best to use explicit types rather than the `var`
        // keyword. However, var is acceptable to use here because the types are
        // obviously Mob and PathFollow2D, since they appear later on the line.

        // Create a new instance of the Mob scene.
        var mob = (Mob)MobScene.Instance();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.Offset = GD.Randi();

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
    public void OnScoreTimerTimeout()
    {
        Score++;
        GetNode<HUD>("HUD").UpdateScore(Score);
    }
    public void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
    public override void _Ready()
    {
        GD.Randomize();
        GetNode<Timer>("MobTimer").Connect("timeout", this, nameof(OnMobTimerTimeout));
        GetNode<Timer>("ScoreTimer").Connect("timeout", this, nameof(OnScoreTimerTimeout));
        GetNode<Timer>("StartTimer").Connect("timeout", this, nameof(OnStartTimerTimeout));
        GetNode<Player>("Player").Connect(nameof(Player.Hit), this, nameof(GameOver));
        GetNode<HUD>("HUD").Connect(nameof(HUD.StartGame), this, nameof(NewGame));
    }
}
