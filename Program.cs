using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRTester
{
    [Command(Name = "simplecurl", Description = "A very simple signalr client")]
    [HelpOption("-?")]
    internal class Program
    {
        private bool Exit = false;
        private HubConnection connection;

        public static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        [Required]
        [Option(Description = "The SignalR complete url")]
        public string Url { get; }

        [Option(Description = "True to write all the traces from signalR client")]
        public bool Trace { get; }

        private void OnExecute()
        {
            SetupConnection();
            do
            {
                Thread.Sleep(5000);
            } while (Exit == false);

        }

        private void StartCommandParser()
        {
            do
            {
                Console.WriteLine("'send method' to send a message to the method");
                Console.WriteLine("'observe method' to observe a message from the method");
                Console.WriteLine("'exit now' to finish the application");
                string raw = Prompt.GetString("Write your command:");
                string[] split = raw.Split(' ');
                if (split.Length > 1)
                {
                    string command = split[0];
                    string parameter = split[1];
                    switch (command)
                    {
                        case "send":
                            Send(parameter);
                            break;
                        case "observe":
                            Subscribe(parameter);
                            break;
                        case "exit":
                            Exit = true;
                            Environment.Exit(-1);
                            break;
                    }

                }
            } while (true);

        }

        private void Send(string methodName)
        {
            List<object> parameters = new List<object>();
            string parameter = string.Empty;
            do
            {
                parameter = Prompt.GetString(
                    "Write the parameters and press enter or just press enter to send a message without parameters");

                if (!string.IsNullOrWhiteSpace(parameter))
                {
                    try
                    {
                        object newObject = JsonConvert.DeserializeObject(parameter);
                        parameters.Add(newObject);
                    }
                    catch
                    {
                        parameters.Add(parameter);
                    }
                }
            } while (!string.IsNullOrWhiteSpace(parameter));

            connection.SendCoreAsync(methodName, parameters.ToArray()).Wait();
        }

        private void SetupConnection()
        {
            IHubConnectionBuilder builder = new HubConnectionBuilder()
                .WithUrl(Url);

            if (Trace)
            {
                builder.ConfigureLogging(logging =>
                {
                    // Log to the Console
                    logging.AddConsole();

                    // This will set ALL logging to Debug level
                    logging.SetMinimumLevel(LogLevel.Debug);
                });
            }

            connection = builder.Build();

            connection.Closed += async (error) =>
            {
                Console.WriteLine("connection closed");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                Console.WriteLine("Trying to reconnect");
                await connection.StartAsync();
            };

            connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine($"There was an error opening the connection:{Url}",
                        task?.Exception?.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                    StartCommandParser();
                }
            });
        }

        private void Subscribe(string method)
        {
            int numberOfParameters = Prompt.GetInt("Enter the number of parameters expected");

            List<Type> types = new List<Type>();
            for (int i = 0; i < numberOfParameters; i++)
            {
                types.Add(typeof(object));
            }

            connection.On(method, types.ToArray(), (x) =>
            {
                Task task = new Task(() =>
                {
                    Console.WriteLine("Message Received: " + method);
                    foreach (object message in x)
                    {
                        Console.WriteLine("Parameter ---------- ");
                        Console.WriteLine(message);
                        Console.WriteLine("--------- ---------- ");
                    }
                });

                task.RunSynchronously();
                return task;
            });
        }
    }
}

