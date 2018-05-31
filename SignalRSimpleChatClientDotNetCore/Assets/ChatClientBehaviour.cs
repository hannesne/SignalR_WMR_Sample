using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using UnityEngine;

public class ChatClientBehaviour : MonoBehaviour
{

    public string SignalRServer;

    private TextMesh textComponent;
    private HubConnection connection;

    private readonly Queue<Action> actionQueue = new Queue<Action>();
    private readonly MyConsoleWriter consoleWriter = new MyConsoleWriter();

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
    void Start () {
        textComponent = GetComponent<TextMesh>();

        if (string.IsNullOrEmpty(SignalRServer) )
        {
            SignalRServer = "http://localhost:14883/chatHub";
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
        connection = new HubConnectionBuilder()
            .WithUrl(SignalRServer)
            //.ConfigureLogging(logging => logging.AddProvider(new MyConsoleWriter()))
            .Build();
        connection.On<string, string>("ReceiveMessage", ReceiveMessage);
        try
        {
            connection.StartAsync().Wait();
            connection.InvokeAsync("SendMessage", "hololens", "connected").Wait();

        }
        catch (Exception ex)
        {
            actionQueue.Enqueue(() =>
            {
                Debug.Log(ex.Message);
                Debug.Break();
            });
            
            throw;
        }
    }


    private void ReceiveMessage(string nick, string message)
    {
        QueueTextChange($"{nick} : {message}");
    }

    private class MyConsoleWriter : ILoggerProvider, Microsoft.Extensions.Logging.ILogger
    {
        private string category;

        public MyConsoleWriter()
        {
            
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            this.category = categoryName;
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var timestamp = DateTime.Now;

            var builder = new StringBuilder();
            
            builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
            builder.Append(" [");
            builder.Append(logLevel.ToString());
            builder.Append("] ");
            builder.Append(category);
            builder.Append(": ");
            builder.AppendLine(formatter(state, exception));

            if (exception != null)
            {
                builder.AppendLine(exception.ToString());
            }

            Debug.Log(builder.ToString());
        }
    }
}
