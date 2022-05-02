using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();
    public override void _Ready()
    {
        GetNode<Button>("StartButton").Connect("pressed", this, nameof(OnStartButtonPressed));
        GetNode<Timer>("MessageTimer").Connect("timeout", this, nameof(OnMessageTimerTimeout));
    }
    public void OnStartButtonPressed()
    {
        GetNode<Button>("StartButton").Hide();
        EmitSignal(nameof(StartGame));
    }

    public void OnMessageTimerTimeout()
    {
        GetNode<Label>("Message").Hide();
    }
    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }
    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();
        GetNode<Timer>("MessageTimer").Start();
    }
    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

        var messageTimer = GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, "timeout");

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the\nCreeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        GetNode<Button>("StartButton").Show();
    }
}
