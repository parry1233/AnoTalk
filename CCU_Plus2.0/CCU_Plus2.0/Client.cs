using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CCU_Plus2._0
{
	class Client
	{
		//private bool register_done=false;
		private Socket client;
		public Client()
		{
			//建立套接字
			this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			AsyncConnect();
		}

		public Client(Socket s)
		{
			this.client = s;
			//AsyncConnect();
		}

		public Socket getSocket()
		{
			return client;
		}

		public void AsyncConnect()
		{
			try
			{
				//埠及IP
				IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse("8888"));
				//建立套接字
				//this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				//開始連線到伺服器
				this.client.BeginConnect(ipe, asyncResult =>
				{
					try
					{
						this.client.EndConnect(asyncResult);
						//向伺服器傳送訊息
						AsyncSend("客戶端已上線");
						//接受訊息
						AsyncReceive(this.client);
					}
					catch (SocketException e)
					{
						MessageBox.Show("客戶端錯誤回報: " + e.Message, "SocketERR");
					}
				}, null);

			}
			catch (SocketException e)
			{
				MessageBox.Show("客戶端錯誤回報: " + e.Message, "SocketERR");
			}
			catch (Exception e)
			{
				MessageBox.Show("客戶端錯誤回報: " + e.Message, "ERR");
			}
		}

		public void AsyncSend(string message)
		{
			if (this.client == null || message == string.Empty) return;
			//編碼
			byte[] data = Encoding.UTF8.GetBytes(message);
			try
			{

				this.client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
				{
					//完成傳送訊息
					int length = this.client.EndSend(asyncResult);
				}, null);
				/*App.Current.Dispatcher.Invoke((Action)(() =>
				{
					if(!message.Equals("客戶端已上線"))
					{
						this.window = new Loading();
						this.window.Show();
					}
				}));*/
			}
			catch (Exception e)
			{
				MessageBox.Show("客戶端錯誤回報: " + e.Message, "ERR");
			}
		}

		/// <summary>
		/// 接收訊息
		/// </summary>
		/// <param name="socket"></param>
		public string AsyncReceive(Socket socket)
		{
			byte[] data = new byte[1024];
			try
			{
				//UserMainWindow window = new UserMainWindow();
				string s = "";
				//開始接收資料
				socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
				asyncResult =>
				{
					try
					{
						char[] delimiterChars = { ':', '\t', '/' };//分隔符類型-->當遇到這些符號時切割字串
						string[] instuction = { "" };

						int length = socket.EndReceive(asyncResult);
						s = Encoding.UTF8.GetString(data);
						int i = s.IndexOf('\0');
						if (i >= 0)
						{
							s = s.Substring(0, i);
						}
						//MessageBox.Show(s);
						instuction = s.Split(delimiterChars);//split message to instruction
						App.Current.Dispatcher.Invoke((Action)(() =>
						{
							foreach (Window win in App.Current.Windows)
							{
								if (win.GetType() == typeof(MainWindow))
								{
									(win as MainWindow).LoginBTN.IsEnabled = true;
									(win as MainWindow).LoginBTNload.Visibility = Visibility.Collapsed;
								}
								/*if (win.GetType().Name.Equals("Loading"))
								{
									win.Close();
								}*/
							}
							if (instuction[0].Equals("LOGIN_PERMIT"))
							{
								MessageBox.Show("登錄成功，即將跳轉...");
								UserMainWindow window = new UserMainWindow(this.client);
								//window = new UserMainWindow(this.client);
								string[] input = new string[7];
								window.User_Info_Input(instuction);
								window.Show();
								//open login window and close all of the others
								foreach (Window win in App.Current.Windows)
								{
									if (win!=window)
									{
										win.Close();
									}
								}
							}
							else if(instuction[0].Equals("REGISTER_ACCEPT"))
							{
								MainWindow mainWindow = new MainWindow(this.client);
								mainWindow.Show();
								//open main window and close all of the others
								foreach (Window win in App.Current.Windows)
								{
									if (win != mainWindow)
									{
										win.Close();
									}
								}
							}
							else if (instuction[0].Equals("GROUP_MEMBER_ALL"))
							{
								foreach (Window win in App.Current.Windows)
								{
									if (win.GetType() == typeof(UserMainWindow))
									{
										string[] member = new string[instuction.Length-1];
										for(int a=1;a<instuction.Length;a++)
										{
											member[a - 1] = instuction[a]; 
										}
										(win as UserMainWindow).memberALL(member);
									}
								}
							}
							else if (instuction[0].Equals("GROUP_MEMBER_ADD"))
							{								
								foreach (Window win in App.Current.Windows)
								{
									if (win.GetType() == typeof(UserMainWindow))
									{
										(win as UserMainWindow).memberAdd(instuction[1]);
									}
								}
							}
							else if (instuction[0].Equals("GROUP_MEMBER_DEL"))
							{
								foreach (Window win in App.Current.Windows)
								{
									if (win.GetType() == typeof(UserMainWindow))
									{
										(win as UserMainWindow).memberLeave(instuction[1]);
									}
								}
							}
							else if (instuction[0].Equals("MESSAGE"))
							{
								//string input = "["+instuction[1] + "]:  "+instuction[2];

								foreach (Window win in App.Current.Windows)
								{
									if (win.GetType() == typeof(UserMainWindow))
									{
										(win as UserMainWindow).chatAdd(instuction[1],instuction[2]);
									}
								}
							}
							else
							{
								MessageBox.Show(s);
							}
						}));
					}
					catch (Exception)
					{
						AsyncReceive(socket);
					}


					AsyncReceive(socket);
				}, null);

				return s;
			}
			catch (Exception e)
			{
				MessageBox.Show("客戶端錯誤回報: " + e.Message, "SocketERR");
				return null;
			}
		}
	}
}
