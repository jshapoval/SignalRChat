using System;
using Microsoft.AspNet.SignalR.Client;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hubConnection = new HubConnection("https://localhost:44339"); // http://localhost:53748
            var chat = hubConnection.CreateHubProxy("ChatHub");
            chat.On<string, string>("broadcastMessage", (userName, message) => 
            {
                Console.Write(userName + ": "); 
                Console.WriteLine(message);
            });
            hubConnection.Start().Wait();
            chat.Invoke("Notify", "Console app", hubConnection.ConnectionId);
            string msg = null;

            while ((msg = Console.ReadLine()) != null)
            {
                chat.Invoke("Send", "Console app", msg).Wait();
            }
        }
    }
}
