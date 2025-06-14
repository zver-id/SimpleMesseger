using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client;


public partial class ClientForm : Form
{
    private TextBox messageTextBox;
    private Button sendButton;
    private TextBox chatTextBox;

    private TcpClient client;
    private NetworkStream stream;

    public ClientForm()
    {
        messageTextBox = new TextBox { Width = 300 };
        sendButton = new Button { Text = "Отправить" };
        chatTextBox = new TextBox { Multiline = true, Width = 400, Height = 200, ReadOnly = true };

        sendButton.Click += SendButton_Click;

        Controls.Add(messageTextBox);
        Controls.Add(sendButton);
        Controls.Add(chatTextBox);

        messageTextBox.Top = 210;
        sendButton.Top = 210;
        sendButton.Left = 310;

        client = new TcpClient("127.0.0.1", 5000);
        stream = client.GetStream();
        InitializeComponent();
    }

    private void SendButton_Click(object sender, EventArgs e)
    {
        string message = messageTextBox.Text;
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);

        // Чтение ответа
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        chatTextBox.AppendText("Вы: " + message + "\n");
        chatTextBox.AppendText("Сервер: " + response + "\n");

        messageTextBox.Clear();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        stream.Close();
        client.Close();
        base.OnFormClosed(e);
    }
}
