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
using MySql.Data;//
using MySql.Data.MySqlClient;//
using System.Net;//
using System.Net.Sockets;//
using System.Threading;

namespace CCU_Plus2._0
{

	/// <summary>
	/// MainWindow.xaml 的互動邏輯
	/// </summary>
	public partial class MainWindow : Window
	{
		//private Socket client;
		private Client clientConnect;
		/*static string dbHost = "db4frese.net";sss
		static string dbPort = "3306";
		static string dbUser = "igoccu";
		static string dbPass = "admin0000";
		static string dbName = "ccuigo";
		static string conn_info = "server=" + dbHost + ";port=" + dbPort + ";user=" + dbUser + ";password=" + dbPass + ";database=" + dbName + ";oldguids=true";*/
		public MainWindow()
		{
			InitializeComponent();
			this.clientConnect = new Client();
			LoginBTNload.Visibility = Visibility.Collapsed;
		}

		public MainWindow(Socket pre_socket)
		{
			InitializeComponent();
			this.clientConnect = new Client(pre_socket);
			LoginBTNload.Visibility = Visibility.Collapsed;
		}

		private void IDtext_beSelected(object sender, RoutedEventArgs e)
		{
			textbox_ID.Text = "";
		}

		private void PWtext_beSelected(object sender, RoutedEventArgs e)
		{
			textbox_PW.Text = "";
		}

		private void LoginBTN_Click(object sender, RoutedEventArgs e)
		{
			string send = "LOGINTRY:" + textbox_ID.Text + "/" + textbox_PW.Text;
			//AsyncSend(this.client,send);
			this.clientConnect.AsyncSend(send);
			//Loading loading = new Loading();
			//loading.Show();
			LoginBTN.IsEnabled = false;
			LoginBTNload.Visibility = Visibility.Visible;
		}
		private void RegisterBTN_Click(object sender, RoutedEventArgs e)
		{
			RegisterWindow registerWindow = new RegisterWindow();
			registerWindow.Show();
			registerWindow.RegisterTakeSocket(this.clientConnect.getSocket());
			this.Close();
		}
	}
}