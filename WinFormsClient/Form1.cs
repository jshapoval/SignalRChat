using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using SignalRMvcChat.Models;

namespace WinFormsClient
{
    public partial class Form1 : Form
    {
        private HubConnection connection;

        public Form1()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:44339/ChatHub") //53797
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }
        //var userName = Context.User.Identity.Name;

        //    if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
        //await Clients.User(Context.UserIdentifier).SendAsync("Receive", message, userName);
        //await Clients.User(to).SendAsync("Receive", message, userName);
        private async void Form1_Load(object sender, EventArgs e)
        {
            connection.On<string, string>("Receive", (message, user) =>
            {
                this.Invoke((Action) (() =>
                {
                    // if (to == txtFrom.Text)
                    {
                        var newMessage = $"{user}: {message}";
                        messagesList.Items.Add(newMessage);
                    }

                }));
            });

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                //connectButton.IsEnabled = false;
                // sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

       

        // метод приема сообщений
        //private void ReceiveMessages()
        //{
        //    try
        //    {
        //      //  IPEndPoint remoteIp = null;
        //            byte[] data = client.Receive(ref remoteIp);
        //            string message = Encoding.Unicode.GetString(data);

        //            // добавляем полученное сообщение в текстовое поле
        //            this.Invoke(new MethodInvoker(() =>
        //            {
        //                string time = DateTime.Now.ToShortTimeString();
        //                chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
        //            }));

        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        // обработчик нажатия кнопки sendButton
        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string message = String.Format("{0}: {1}", userName, messageTextBox.Text);
                //byte[] data = Encoding.Unicode.GetBytes(message);
                //client.Send(data, data.Length, HOST, REMOTEPORT);
                //messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private async void btSend_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    await connection.InvokeAsync("Send",
            //        txtMessage.Text, txtTo.Text);//fromTxt?
            //    messagesList.Items.Add($"{txtFrom.Text}: {txtMessage.Text}");
            //}
            //catch (Exception ex)
            //{
            //    messagesList.Items.Add(ex.Message);
            //}


            try
            {
              //  await Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //РЕГИСТРАЦИЯ
        protected void btnInsert_Click(object sender, EventArgs e)
        {
         //string CONNECTION_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SignalRChat;User Id=Julia;Password=a72a48a1000225;Integrated Security=False";
        
         //   using (Microsoft.Data.SqlClient.SqlConnection connection = new SqlConnection(CONNECTION_STRING))
         //   {
         //       connection.Open();

         //       var queryForCheck = string.Format(
         //           @"SELECT * FROM Users WHERE Email = '{0}'
         //            set @ReturnValue = @@rowCount;", txtFrom.Text);

         //       var command = new SqlCommand(queryForCheck, connection);

         //       var returnValue = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
         //       returnValue.Direction = ParameterDirection.InputOutput;
         //       returnValue.Value = -1;
         //       var result = (int)command.Parameters["@ReturnValue"].Value;

         //       if (result > 0)
         //       {
         //           txtFrom.Clear();
         //           txtPassword.Clear();
         //           throw new ApplicationException("please, input another login");
         //       }

         //       else
         //       {
         //           var queryForInsert = string.Format(
         //               @"INSERT INTO Users (Email, Password) VALUES ({0})", String.Concat("'", txtFrom, "'", ", ", "'", txtPassword, "'")
         //           );

         //           var insertCommand = new SqlCommand(queryForInsert, connection);
         //           insertCommand.ExecuteNonQuery();
         //           authLabel.Visible = true;
         //       }
         //   }
        }

    private void txtTo_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Click(object sender, EventArgs e)
        {
          //  this.DialogResult = true;
        }
        public string Login
        {
            get { return txtFrom.Text; }
        }

        public string Password
        {
            get { return txtPassword.Text; }
        }

        private void loginPicBox_Click(object sender, EventArgs e)
        {
            //var request = new XMLHttpRequest();
            //request.open("POST", "/token", true);
            //request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.addEventListener("load", function() {
                
            //});
            //// отправляем запрос на аутентификацию
            //request.send("username=" + txtFrom +
            //             "&password=" + txtPassword);
        }

        //public void Load()
        //{
        //    if (request.status < 400)
        //    { // если запрос успешный

        //        let data = JSON.parse(request.response);
        //        token = data.access_token;

        //        document.getElementById("sendBtn").disabled = false;

        //        hubConnection.start()       // начинаем соединение с хабом
        //            .catch (err => {
        //            console.error(err.toString());
        //            loginPicBox.Visible = true;
        //            sendPicBox.Visible = true;
        //        });
        //    }
        //    else
        //    {
        //        console.log("Status", request.status);
        //        console.log(request.responseText);
        //    }
        //}
    }
}
