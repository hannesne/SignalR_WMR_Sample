using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using UnityEngine;

public class ChatClientBehaviour : MonoBehaviour
{

    public string SignalRServer;

    private TextMesh textComponent;
    private HubConnection connection;
    private IHubProxy proxy;

    private readonly Queue<Action> actionQueue = new Queue<Action>();
    private readonly MyConsoleWriter consoleWriter = new MyConsoleWriter();

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
    void Start () {
        textComponent = GetComponent<TextMesh>();

        if (string.IsNullOrEmpty(SignalRServer) )
        {
            SignalRServer = "http://signalrsimplechatserverdotnet46.azurewebsites.net";
            QueueTextChange($"No connection specified. Defaulting to {SignalRServer}");
        }


        if (!Uri.IsWellFormedUriString(SignalRServer, UriKind.Absolute))
        {
            QueueTextChange($"Connection URL is not valid");

        }
        else
        {
            Task.Run(() => Connect());
        }
    }

    void Update()
    {

        while (actionQueue.Count > 0)
        {
            actionQueue.Dequeue()();
        }
    }

    private void QueueTextChange(string message)
    {
        actionQueue.Enqueue(() => SetText(message));
    }

    private void Connection_Error(Exception obj)
    {
        QueueTextChange($"Connection Error : {obj.Message}");
         
    }

    private void SetText(string message)
    {
        textComponent.text = message;
    }
    

    private void Connect()
    {
        QueueTextChange("Initiating connection");
        connection = new HubConnection(SignalRServer);
        proxy = connection.CreateHubProxy("Chat");
        connection.StateChanged += OnStateChanged; 
        connection.Error += Connection_Error;
        connection.TraceLevel = TraceLevels.All;
        connection.TraceWriter = consoleWriter;

        proxy.On<string, string>("Send", OnSend);
        try
        {
            connection.Start().Wait();

            proxy.Invoke("Send", "hololens", "connected").Wait();

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Break();
            throw;
        }
    }

    private void OnStateChanged(StateChange stateChange)
    {
        switch (stateChange.NewState)
        {
            case ConnectionState.Disconnected:
                QueueTextChange("Disconnected");
                return;
            case ConnectionState.Connecting:
            case ConnectionState.Reconnecting:
                QueueTextChange("Connecting");
                return;
            default:
                return;
        }

    }

    private void OnSend(string nick, string message)
    {
        QueueTextChange($"{nick} : {message}");
    }

    private class MyConsoleWriter : TextWriter
    {
        public MyConsoleWriter()
        {
            Encoding = Encoding.ASCII;
        }
        public override void Write(char value)
        {
            Message += value;
            if (Message.EndsWith(NewLine, StringComparison.Ordinal))
            {
                Debug.Log(Message);
                Message = "";
            }

        }

        public override Encoding Encoding { get; }

        private string Message { get; set; } = "";
    }
}
