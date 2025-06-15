using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using MessengerLibrary;

namespace SimpleClient;
public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    public ChatPacket chats;

    public void ConnectToServer(string ipAddress, int port)
    {
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();
        Console.WriteLine("Подключено к серверу.");

        // Чтение сообщений в фоне
        Thread readThread = new Thread(ReadMessages);
        readThread.Start();
    }

    private void ReadMessages()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string json = Encoding.UTF8.GetString(buffer);
            try
            {
                ChatPacket chatPacket = JsonSerializer.Deserialize<ChatPacket>(json);
                chats = chatPacket;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при разборе сообщения" + ex.Message);
            }
            
        }
    }

    public void PrintMessages(int chatIndex)
    {
        Console.Clear();

        try
        {
            if (chats.Chats.Count > chatIndex)
            {
                Console.WriteLine(chats.Chats[chatIndex].Name);

                foreach (Message message in chats.Chats[chatIndex].Messages)
                {
                    Console.WriteLine($"{message.Autor.Name}: {message.Text}");
                }
                Console.Write("> ");
            }
            else
            {
                Console.WriteLine("Пока нет доступных чатов");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка получения данных с сервера" + ex.Message + ex.StackTrace);
        }
    }

    public void SendObj<T>(T obj)
    {
        if (stream != null && stream.CanWrite)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }
    }
}

