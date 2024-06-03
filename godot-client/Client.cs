using Godot;
using GoGameClient.script.cmd;
using GoGameClient.script.global;
using GoGameClient.script.network.webSocket;
using GoGameClient.script.tool;

namespace GoGameClient;

public partial class Client : Node2D
{
    private SocketControl _socketControl;

    private void ButtonPressed()
    {
        GD.Print("Hello world!111");
        SocketControl.Instance().Request(new BodyRequestConfig<HelloReq, HelloReq>
        {
            Title = "Hello",
            Cmd = (int)Router.Common,
            CmdMethod = (int)CommonRouter.Here,
            Data = new HelloReq
            {
                Name = "HelloReq"
            },
            Result = helloReq => { GD.Print(helloReq.Name); }
        });
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var button = new Button();
        button.Text = "Click me";
        button.Pressed += ButtonPressed;
        button.SetSize(new Vector2(200,200));
        
        AddChild(button);
        

        _socketControl = SocketControl.Instance();
        _socketControl.SetUrl(Config.GetIp(), Config.GetPort(), Config.GetContext());
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}