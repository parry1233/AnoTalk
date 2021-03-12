using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;//
using System.Net.Sockets;//
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CCU_Plus2._0
{
	/// <summary>
	/// RegisterWindow.xaml 的互動邏輯
	/// </summary>
	public partial class RegisterWindow : Window
	{
		private bool checkInteruptClose;
		//private Socket client;
		private string id, pw, name, depart, email,grade,gender;
		private Client clientConnect;
		public RegisterWindow()
		{
			InitializeComponent();
		}

		private void CheckIDBTN_Click(object sender, RoutedEventArgs e)
		{
			this.id = idTextBox.Text;
			this.clientConnect.AsyncSend("REGISTER_VERIFY_ID:"+this.id);
			Loading loading = new Loading();
			loading.Show();
		}

		private void idTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			idTextBox.Text = "";
		}

		private void idTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(idTextBox.Text.Length == 0)
			{
				idTextBox.Text = "Account";
			}
		}

		private void pwTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			pwTextBox.Text = "";
		}

		private void pwTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(pwTextBox.Text.Length ==0)
			{
				pwTextBox.Text = "Password";
			}
		}

		private void nameTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			nameTextBox.Text = "";
		}

		private void nameTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(nameTextBox.Text.Length ==0)
			{
				nameTextBox.Text = "Name";
			}
		}

		private void emailTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			emailTextBox.Text = "";
		}

		private void emailTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(emailTextBox.Text.Length ==0)
			{
				emailTextBox.Text = "Email";
			}
		}

		private void departTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			departTextBox.Text = "";
		}

		private void departTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(departTextBox.Text.Length == 0)
			{
				departTextBox.Text = "Department";
			}
		}


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void Register(object sender, RoutedEventArgs e)
		{
			bool checkErr = false;
			try
			{
				this.id = idTextBox.Text;
				this.pw = pwTextBox.Text;
				this.name = nameTextBox.Text;
				this.depart = departTextBox.Text;
				this.email = emailTextBox.Text;
				if (MaleRadioBTN.IsChecked.Value)
				{
					this.gender = "male";
				}
				else if (FemaleRadioBTN.IsChecked.Value)
				{
					this.gender = "female";
				}
				else
				{
					checkErr = true;
				}
				this.grade = GradeComboBox.Text;
			}
			catch (System.FormatException)
			{
				checkErr = true;
			}
			catch (System.NullReferenceException)
			{
				checkErr = true;
			}

			//check input finish, next step
			if (checkErr)
			{
				ERRinInput();
			}
			else if (!checkErr)
			{
				this.clientConnect.AsyncSend("REGISTER_VERIFY_EMAIL:" + this.email); 
				string[] info = new string[7];
				info[0] = this.id;
				info[1] = this.pw;
				info[2] = this.name;
				info[3] = this.depart;
				info[4] = this.grade;
				info[5] = this.gender;
				info[6] = this.email;
				VerifyCode verify = new VerifyCode(this.clientConnect.getSocket(), info);
				verify.Show();
				this.Close();
			}
		}

		public void RegisterTakeSocket(Socket pre_socket)
		{
			this.clientConnect = new Client(pre_socket);
		}

		public void ERRinInput()
		{
			MessageBox.Show("Input Error!");
		}
	}
}
