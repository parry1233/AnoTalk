using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;//
using System.Net.Sockets;//
using System.Threading;//
using System.ComponentModel;//
using System.Data;//
using MySql.Data;//
using MySql.Data.MySqlClient;//
using System.Net.Mail;//

namespace ChatOnline_Server
{
	/// <summary>
	/// MainWindow.xaml 的互動邏輯
	/// </summary>
	public partial class MainWindow : Window
	{
		static string dbHost = "anotalk.czzx5egn4lyl.ap-northeast-1.rds.amazonaws.com";
		static string dbPort = "3306";
		static string dbUser = "admin";
		static string dbPass = "admin0000";
		static string dbName = "anotalk";
		static string conn_info = "server=" + dbHost + ";port=" + dbPort + ";user=" + dbUser + ";password=" + dbPass + ";database=" + dbName + ";oldguids=true";
		Socket socketListen;//用於監聽的socket
		Socket socketConnect;//用於通訊的socket
		string RemoteEndPoint;//客戶端的網路節點
		Dictionary<string, Socket> dicClient = new Dictionary<string, Socket>();//連線的客戶端集合
		loadWIndow load;
		BackgroundWorker worker;
		Dictionary<string, string> dicRDMText = new Dictionary<string, string>();
		Dictionary<string, string> dicOnline = new Dictionary<string, string>();
		public MainWindow()
		{
			InitializeComponent();
			worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;
			OpenServer();
			LoadProgress.Visibility = Visibility.Collapsed;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			ServerIPLabel.Content += " 127.0.0.1";
			ServerPortLabel.Content += "8888";
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < 100; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);
				Thread.Sleep(50);
			}
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			LoadProgress.Value = e.ProgressPercentage;
			//load.ConnectProgress.Value = e.ProgressPercentage;
			//load.PercentLabel.Content = e.ProgressPercentage.ToString() + "%";
			if (e.ProgressPercentage == 15)
			{
				LogTextBox.Text += "Open connect...15%\r\n";
			}
			else if (e.ProgressPercentage == 30)
			{
				LogTextBox.Text += "Try to connect...30%\r\n";
			}
			else if (e.ProgressPercentage == 60)
			{
				LogTextBox.Text += "Confirm access...60%\r\n";
			}
			else if (e.ProgressPercentage == 80)
			{
				LogTextBox.Text += "Last process...80%\r\n";
			}
			else if (e.ProgressPercentage >= 99)
			{
				LoadProgress.Visibility = Visibility.Collapsed;
				//load.Close();
				LogTextBox.Text += "Finished...100%\r\n";
			}
		}

		private void OpenServer()
		{
			try
			{
				//建立套接字
				IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse("8888"));
				socketListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				//繫結埠和IP
				socketListen.Bind(ipe);
				//設定監聽
				socketListen.Listen(10);//max length of the pending client connections queue=10(懸掛的最大客戶端連接數=10)
										//連線客戶端
				AsyncConnect(socketListen);
				//MessageBox.Show("OpenSuccess");
			}
			catch (Exception e)
			{
				MessageBox.Show("OpenServer: " + e.Message, "Server ERR");
			}
		}

		/// <summary>
		/// 連線到客戶端
		/// </summary>
		/// <param name="socket"></param>
		private void AsyncConnect(Socket socket)
		{
			try
			{
				socket.BeginAccept(asyncResult =>
				{
					//獲取客戶端套接字
					socketConnect = socket.EndAccept(asyncResult);
					RemoteEndPoint = socketConnect.RemoteEndPoint.ToString();
					dicClient.Add(RemoteEndPoint, socketConnect);//新增至客戶端集合
																 //使用Dispatcher物件跨執行緒
					Action methodDelegate = delegate ()
					{
						ClientComboBox.Items.Add(RemoteEndPoint);//新增客戶端埠號
					};
					this.Dispatcher.BeginInvoke(methodDelegate);
					//AsyncSend(socketConnect, string.Format("歡迎你{0}", socketConnect.RemoteEndPoint));
					AsyncReceive(socketConnect);
					AsyncConnect(socketListen);
				}, null);


			}
			catch (Exception e)
			{
				MessageBox.Show("AsyncConnect: " + e.Message, "Server ERR");
			}
		}

		/// <summary>
		/// 接收訊息
		/// </summary>
		/// <param name="client"></param>
		private void AsyncReceive(Socket socket)
		{
			byte[] data = new byte[1024];
			try
			{
				//開始接收訊息
				socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
				asyncResult =>
				{
					try
					{
						int length = socket.EndReceive(asyncResult);
						//MessageBox.Show(Encoding.UTF8.GetString(data));
						string s = "";
						char[] delimiterChars = { ':', '\t', '/' };//分隔符類型-->當遇到這些符號時切割字串
						string[] instuction = { "" };
						//使用Dispatcher物件跨執行緒
						Action methodDelegate = delegate ()
						{
							s = Encoding.UTF8.GetString(data);
							/*
							 * 將client的回傳的字串data之'\0'刪去
							 */
							int i = s.IndexOf('\0');
							if (i >= 0)
							{
								s = s.Substring(0, i);
							}
							LogTextBox.Text += "[ " + socket.RemoteEndPoint.ToString() + " ] " + s + "\r\n";
							instuction = s.Split(delimiterChars);
						};
						this.Dispatcher.BeginInvoke(methodDelegate);

						//char[] delimiterChars = { ',', '.', ':', '\t','/' };//分隔符類型-->當遇到這些符號時切割字串
						//string[] instuction = s.Split(delimiterChars);

						using (MySqlConnection conn_logIn = new MySqlConnection(conn_info))
						{
							try
							{
								conn_logIn.Open();
								if (instuction[0].Equals("LOGINTRY"))
								{
									//'binary' will let MySQL be case sensitive, distinguishing capital and lower case letters
									string checkAccountCMD = "SELECT * FROM user WHERE User_ID = binary @ID_in";
									MySqlCommand cmd = new MySqlCommand(checkAccountCMD, conn_logIn);
									cmd.Parameters.AddWithValue("@ID_in", instuction[1]);
									MySqlDataReader dataRead = cmd.ExecuteReader();

									if (dataRead.HasRows)
									{
										while (dataRead.Read())
										{
											if (dataRead["User_PW"].Equals(instuction[2]))
											{
												//string infoS = "LOGIN_PERMIT:" + dataRead["User_Name"];
												AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "LOGIN_PERMIT:"
													+ dataRead["User_ID"] + "/" + dataRead["User_PW"] + "/" + dataRead["User_Name"] + "/" + dataRead["User_Department"] + "/"
													+ dataRead["User_Grade"] + "/" + dataRead["User_Gender"] + "/" + dataRead["User_Email"]);
												//send User acount info
												string userName = dataRead["User_Name"] as string;
												dicOnline.Add(socket.RemoteEndPoint.ToString(), userName);
												foreach(KeyValuePair<string, string> user in dicOnline)
												{
													AsyncSend(dicClient[user.Key], "GROUP_MEMBER_ADD:" + userName);
												}

												Action methodDelegate_LogIn = delegate ()
												{
													LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) Log In Success, 新增用戶至聊天室"+ userName + "\r\n";
												};
												this.Dispatcher.BeginInvoke(methodDelegate_LogIn);
												//MessageBox.Show("Login Success");
											}
											else
											{
												AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "LOGIN FAILED");
												Action methodDelegate_LogIn = delegate ()
												{
													LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) Log In Failed\r\n";
												};
												this.Dispatcher.BeginInvoke(methodDelegate_LogIn);
												//MessageBox.Show("Login Failed");
											}
										}
									}
									else
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "LOGIN FAILED");
										Action methodDelegate_LogIn = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) Log In Failed\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_LogIn);
										//MessageBox.Show("Login Failed");
									}
								}
								else if (instuction[0].Equals("REGISTER_VERIFY_ID"))
								{
									string checkAccountCMD = "SELECT * FROM user WHERE User_ID = @ID_in";
									MySqlCommand cmd = new MySqlCommand(checkAccountCMD, conn_logIn);
									cmd.Parameters.AddWithValue("@ID_in", instuction[1]);
									MySqlDataReader dataRead = cmd.ExecuteReader();

									if (dataRead.HasRows)
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "ID_EXISTED");
										Action methodDelegate_VerifyId = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) ID_Existed\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
									}
									else
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "ID_OKAY");
										Action methodDelegate_VerifyId = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) ID_Available\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
									}
								}
								else if (instuction[0].Equals("REGISTER_VERIFY_IDPW"))
								{
									string checkAccountCMD = "SELECT * FROM user WHERE User_ID = @ID_in";
									MySqlCommand cmd = new MySqlCommand(checkAccountCMD, conn_logIn);
									cmd.Parameters.AddWithValue("@ID_in", instuction[1]);
									MySqlDataReader dataRead = cmd.ExecuteReader();

									if (dataRead.HasRows)
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "NEXT_DENY");
										Action methodDelegate_VerifyId = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) DENY CLIENT NEXT STEP cause by ID_EXISTED\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
									}
									else
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "NEXT_PERMIT:" + instuction[1] + "/" + instuction[2]);
										Action methodDelegate_VerifyId = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) PERMIT CLIENT NEXT STEP\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
									}
								}
								else if (instuction[0].Equals("REGISTER_VERIFY_EMAIL"))
								{
									/* Generate Random Code */
									Random rnd = new Random();
									var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
									var stringChars = new char[6];

									for (int i = 0; i < stringChars.Length; i++)
									{
										stringChars[i] = chars[rnd.Next(chars.Length)];
									}

									bool checkExist = false;
									foreach (KeyValuePair<string, string> item in dicRDMText)
									{
										if (socket.RemoteEndPoint.ToString().Equals(item.Key))
										{
											checkExist = true;
											dicRDMText[item.Key] = new string(stringChars);
											break;
										}
									}
									if (!checkExist)
									{
										dicRDMText.Add(socket.RemoteEndPoint.ToString(), new string(stringChars));
									}
									//this.RandomTXT = new string(stringChars);

									/* Send Email */
									try
									{
										System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
										msg.To.Add(instuction[1]);
										//msg.To.Add("b@b.com");可以發送給多人
										//msg.CC.Add("c@c.com");
										//msg.CC.Add("c@c.com");可以抄送副本給多人 
										//這裡可以隨便填，不是很重要
										msg.From = new MailAddress(instuction[1], "ChatOnline", System.Text.Encoding.UTF8);
										/* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
										msg.Subject = "Your Verify Code";//郵件標題
										msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
										msg.Body = "We appreciate you for using ChatOnline.\nYour Verify Code is: " + dicRDMText[socket.RemoteEndPoint.ToString()] + "."; //郵件內容
										msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
																					 //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
										msg.IsBodyHtml = true;//是否是HTML郵件 
															  //msg.Priority = MailPriority.High;//郵件優先級 

										SmtpClient client = new SmtpClient();
										client.Credentials = new System.Net.NetworkCredential("igoccu@gmail.com", "parry1233"); //這裡要填正確的帳號跟密碼
										client.Host = "smtp.gmail.com"; //設定smtp Server
										client.Port = 25; //設定Port
										client.EnableSsl = true; //gmail預設開啟驗證
										client.Send(msg); //寄出信件
										client.Dispose();
										msg.Dispose();

										Action methodDelegate_SendMail = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " Send Email to: " + instuction[1] + " 驗證碼:" + dicRDMText[socket.RemoteEndPoint.ToString()] + "\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_SendMail);
									}
									catch (Exception ex)
									{
										Action methodDelegate_SendMail = delegate ()
										{
											LogTextBox.Text += "Server ERR: " + ex.Message + "\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_SendMail);
									}
								}
								//client register
								else if (instuction[0].Equals("REGISTER_ACCOUNT"))
								{
									if (instuction[8].Equals(dicRDMText[socket.RemoteEndPoint.ToString()]))
									{
										string registerAccountCMD = "INSERT INTO user (User_ID,User_PW,User_Name,User_Department,User_Grade,User_Gender,User_Email) " +
											"VALUES (@ID_in,@PW_in,@Name_in,@Depart_in,@Grade_in,@Gender_in,@Email_in)";
										MySqlCommand cmd = new MySqlCommand(registerAccountCMD, conn_logIn);
										cmd.Parameters.AddWithValue("@ID_in", instuction[1]);
										cmd.Parameters.AddWithValue("@PW_in", instuction[2]);
										cmd.Parameters.AddWithValue("@Name_in", instuction[3]);
										cmd.Parameters.AddWithValue("@Depart_in", instuction[4]);
										cmd.Parameters.AddWithValue("@Grade_in", instuction[5]);
										cmd.Parameters.AddWithValue("@Gender_in", instuction[6]);
										cmd.Parameters.AddWithValue("@Email_in", instuction[7]);
										cmd.ExecuteNonQuery();
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "REGISTER_ACCEPT");
										Action methodDelegate_VerifyId = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
													+ " (結果) 創建帳號完成(" + instuction[1] + "/" + instuction[2] + "/" + instuction[3] + "/" + instuction[4] + "/" +
													instuction[5] + "/" + instuction[6] + "/" + instuction[7] + ")" + "\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
									}
									else
									{
										AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "REGISTER_DENY");
									}
								}
								else if (instuction[0].Equals("SEND_ALL"))
								{
									foreach(KeyValuePair<string, string> online in dicOnline)
									{
										AsyncSend(dicClient[online.Key], "MESSAGE:"+instuction[1]+"/"+instuction[2]);
									}
									Action methodDelegate_VerifyId = delegate ()
									{
										LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
												+ " (結果) 傳送訊息至群組" + "\r\n";
									};
									this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
								}
								else if (instuction[0].Equals("Get_Group_ALL"))
								{
									string dicOnline_temp = "";
									int count = 1;
									foreach (KeyValuePair<string, string> user in dicOnline)
									{
										if (count >= dicOnline.Count)
										{
											dicOnline_temp += user.Value;
										}
										else
										{
											dicOnline_temp += user.Value + "/";
											count++;
										}
									}
									foreach (KeyValuePair<string, string> user in dicOnline)
									{
										AsyncSend(dicClient[user.Key], "GROUP_MEMBER_ALL:" + dicOnline_temp);
									}
									Action methodDelegate_VerifyId = delegate ()
									{
										LogTextBox.Text += "[ SERVER ] 回應 " + socket.RemoteEndPoint.ToString()
												+ " (結果) 傳送線上成員清單至群組" + "\r\n";
									};
									this.Dispatcher.BeginInvoke(methodDelegate_VerifyId);
								}
							}
							catch (MySql.Data.MySqlClient.MySqlException ex)
							{
								switch (ex.Number)
								{
									case 0:
										Action methodDelegate_ERR_0 = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] Unpredicted incident occured.Fail to connect to database.\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_ERR_0);
										//MessageBox.Show("Unpredicted incident occured. Fail to connect to database.");
										break;
									case 1042:
										Action methodDelegate_ERR_1042 = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] Database IP error. Please check again.\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_ERR_1042);
										//MessageBox.Show("IP error. Please check again.");
										break;
									case 1045:
										Action methodDelegate_ERR_1045 = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] Database User account or password error. Please check again.\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_ERR_1045);
										//MessageBox.Show("User account or password error. Please check again");
										break;
									case 1062:
										Action methodDelegate_ERR_1062 = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] User account already existed. Please check again.\r\n";
										};
										this.Dispatcher.BeginInvoke(methodDelegate_ERR_1062);
										//MessageBox.Show("UUser account already existed. Please check again");
										break;
									case 1366:
										Action methodDelegate_ERR_1366 = delegate ()
										{
											LogTextBox.Text += "[ SERVER ] Incorrect vslue while INSERT, cannot insert to MySQL.\r\n";
											AsyncSend(dicClient[socket.RemoteEndPoint.ToString()], "REGISTER_ERROR");
										};
										this.Dispatcher.BeginInvoke(methodDelegate_ERR_1366);
										//MessageBox.Show("UUser account already existed. Please check again");
										break;
								}
							}

							conn_logIn.Close();
						}
					}
					catch (Exception)
					{
						AsyncReceive(socket);
					}

					AsyncReceive(socket);
				}, null);

			}
			catch (Exception e)
			{
				//傳送失敗，將該客戶端資訊刪除
				string deleteClient = socket.RemoteEndPoint.ToString();
				string userName = "";
				foreach (KeyValuePair<string, string> user in dicOnline)
				{
					if(deleteClient.Equals(user.Key))
					{
						userName = user.Value;
					}
				}
				foreach (KeyValuePair<string, string> user in dicOnline)
				{
					if(userName.Length !=0)
					{
						AsyncSend(dicClient[user.Key], "GROUP_MEMBER_DEL:" + userName);
					}
				}
				dicClient.Remove(deleteClient);
				dicOnline.Remove(deleteClient);
				string dicOnline_temp = "";
				int count=1;
				foreach (KeyValuePair<string, string> user in dicOnline)
				{
					if(count>=dicOnline.Count)
					{
						dicOnline_temp += user.Value;
					}
					else
					{
						dicOnline_temp += user.Value + "/";
						count++;
					}
				}
				//使用Dispatcher物件跨執行緒
				Action methodDelegate = delegate ()
				{
					LogTextBox.Text += "[ " + deleteClient + " ] 已離線\r\n";
					LogTextBox.Text += "[ SERVER ] 回應 " + deleteClient + " 自列表移除用戶: " + deleteClient + "\r\n";
					ClientComboBox.Items.Remove(deleteClient);
				};
				this.Dispatcher.BeginInvoke(methodDelegate);
				//MessageBox.Show("AsyncReceive: "+e.Message);
				foreach (KeyValuePair<string, string> user in dicOnline)
				{
					AsyncSend(dicClient[user.Key], "GROUP_MEMBER_ALL:" + dicOnline_temp);
					Thread.Sleep(1000);
				}
			}
		}

		/// <summary>
		/// 傳送訊息
		/// </summary>
		/// <param name="client"></param>
		/// <param name="p"></param>
		private void AsyncSend(Socket client, string message)
		{
			if (client == null || message == string.Empty) return;
			//資料轉碼
			byte[] data = Encoding.UTF8.GetBytes(message);
			try
			{
				//開始傳送訊息
				client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
				{
					//完成訊息傳送
					int length = client.EndSend(asyncResult);
				}, null);
			}
			catch (Exception e)
			{
				//傳送失敗，將該客戶端資訊刪除
				string deleteClient = client.RemoteEndPoint.ToString();
				dicClient.Remove(deleteClient);
				Action methodDelegate = delegate ()
				{
					ClientComboBox.Items.Remove(deleteClient);
				};
				//MessageBox.Show("AsyncSend: " + e.Message, "Server ERR");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (ClientComboBox.SelectedIndex == -1)
			{
				AsyncSend(socketConnect, SendTextBox.Text);
			}
			else
			{
				AsyncSend(dicClient[ClientComboBox.SelectedItem.ToString()], SendTextBox.Text);
			}
		}

		private void ConnectBTN_Click(object sender, RoutedEventArgs e)
		{
			//OpenServer();
			using (MySqlConnection conn_logIn = new MySqlConnection(conn_info))
			{
				try
				{
					worker.RunWorkerAsync();
					LoadProgress.Visibility = Visibility.Visible;
					//load = new loadWIndow();
					//load.ShowDialog();
					//conn_logIn.Open();
					//MessageBox.Show("Connect to MySQL(db4free.net) success.");
					LogTextBox.Text += "[ SERVER ] Connect to MySQL(db4free.net) success.\r\n";
				}
				catch (MySql.Data.MySqlClient.MySqlException ex)
				{
					switch (ex.Number)
					{
						case 0:
							LogTextBox.Text += "[ ERR ] Unpredicted incident occured. Fail to connect to database.";
							break;
						case 1042:
							LogTextBox.Text += "[ ERR ] IP error. Please check again.";
							break;
						case 1045:
							LogTextBox.Text += "[ ERR ] User account or password error. Please check again";
							break;
						default:
							LogTextBox.Text += "[ ERR ] Connect to database encountered error.";
							break;
					}
				}

				conn_logIn.Close();
			}
		}
	}
}
