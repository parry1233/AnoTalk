using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CCU_Plus2._0
{
	/// <summary>
	/// VerifyCode.xaml 的互動邏輯
	/// </summary>
	public partial class VerifyCode : Window
	{
		private int time_count;
		private string[] info = new string[8];
		private Client clientConnect;
		public VerifyCode()
		{
			InitializeComponent();
			this.clientConnect = new Client();
			this.time_count = 0;
			TimerStart();
			ReVerifyCodeBTN.IsEnabled = false;
			ReVerifyCodeBTN.Visibility = Visibility.Hidden;
		}

		public VerifyCode(Socket pre_socket, string[] input)
		{
			InitializeComponent();
			this.clientConnect = new Client(pre_socket);
			this.time_count = 0;
			for (int a = 0; a <= 6; a++)
			{
				this.info[a] = input[a];
			}
			TimerStart();
			ReVerifyCodeBTN.IsEnabled = false;
			ReVerifyCodeBTN.Visibility = Visibility.Hidden;
		}

		public void TimerStart()
		{
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			this.time_count++;
			if (this.time_count >= 6)
			{
				this.time_count = 0;
				ReVerifyCodeBTN.IsEnabled = true;
				ReVerifyCodeBTN.Visibility = Visibility.Visible;
			}
		}

		private void ConfirmBTN_Click_1(object sender, RoutedEventArgs e)
		{
			this.info[7] = CodeTextBox.Text;
			this.clientConnect.AsyncSend("REGISTER_ACCOUNT:"
					+ this.info[0] + "/" + this.info[1] + "/" + this.info[2] + "/" + this.info[3] + "/" +
					this.info[4] + "/" + this.info[5] + "/" + this.info[6] + "/" + this.info[7]);
		}

		private void ReVerifyCodeBTN_Click(object sender, RoutedEventArgs e)
		{
			this.clientConnect.AsyncSend("REGISTER_VERIFY_EMAIL:" + this.info[6]);
			ReVerifyCodeBTN.Visibility = Visibility.Hidden;
		}

		private void CodeTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			CodeTextBox.Text = "";
		}

		private void CodeTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if(CodeTextBox.Text.Length ==0 )
			{
				CodeTextBox.Text = "(Verify Code)";
			}
		}
	}
}
