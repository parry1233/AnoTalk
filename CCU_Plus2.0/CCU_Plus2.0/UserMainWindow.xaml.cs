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

namespace CCU_Plus2._0
{
	/// <summary>
	/// UserMainWindow.xaml 的互動邏輯
	/// </summary>
	public partial class UserMainWindow : Window
	{
		private string user_name,user_id,user_pw,user_depart,user_gender,user_grade,user_email;
		private Client clientConnect;
		private ChatRoom chatRoom;
		public UserMainWindow()
		{
			InitializeComponent();
			this.clientConnect = new Client();
		}

		public UserMainWindow(Socket pre_socket)
		{
			InitializeComponent();
			this.clientConnect = new Client(pre_socket);
			this.chatRoom = new ChatRoom();
			frame.NavigationService.Navigate(this.chatRoom);
			this.clientConnect.AsyncSend("Get_Group_ALL");
		}

		public void User_Info_Input(string[] user)
		{
			this.user_id = user[1];
			this.user_pw = user[2];
			this.user_name = user[3];
			this.user_depart = user[4];
			this.user_grade = user[5];
			this.user_gender = user[6];
			this.user_email = user[7];
			if (this.user_gender.Equals("male"))
			{
				image.Source = new BitmapImage(new Uri(@"Icon/user_boy.png", UriKind.Relative));
			}
			else if (this.user_gender.Equals("female"))
			{
				image.Source = new BitmapImage(new Uri(@"Icon/user_girl.png", UriKind.Relative));
			}
			else
			{
				image.Source = new BitmapImage(new Uri(@"Icon/user_sleep.png", UriKind.Relative));
			}
			UserName.Text = this.user_name;
		}

		private void SendBTN_Click(object sender, RoutedEventArgs e)
		{
			this.clientConnect.AsyncSend("SEND_ALL:" +this.user_name+"/"+SendBox.Text);
			SendBox.Text = "";
		}

		public void chatAdd(string name, string s)
		{
			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			if (name.Equals(this.user_name))
			{
				//TextBlock tb_name = new TextBlock();
				//tb_name.Text = name;
				TextBlock tb = new TextBlock();
				tb.Background = Brushes.LightBlue;
				tb.Text = s;
				tb.VerticalAlignment = VerticalAlignment.Center;
				tb.HorizontalAlignment = HorizontalAlignment.Center;
				//MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.User };
				//sp.Children.Add(icon);
				//sp.Children.Add(tb_name);
				//sp.Children.Add(tb);
				Border border = new Border();
				border.CornerRadius = new CornerRadius(10.0);
				border.Background = Brushes.LightBlue;
				border.Height = 38;
				//border.Width = 100 + tb.Width + 100;
				border.BorderBrush = Brushes.LightBlue;
				border.BorderThickness = new Thickness(10, 10, 10, 10);
				border.Child = tb;
				sp.Children.Add(border);
				sp.Margin = new Thickness { Top = 10 };
				sp.HorizontalAlignment = HorizontalAlignment.Right;
			}
			else
			{
				//border.BorderThickness = new Thickness(3, 3, 3, 3);
				TextBlock tb_name = new TextBlock();
				tb_name.Text = name;
				tb_name.VerticalAlignment = VerticalAlignment.Center;
				TextBlock tb = new TextBlock();
				//tb.Background = Brushes.LightBlue;
				tb.Text = s;
				tb.VerticalAlignment = VerticalAlignment.Center;
				tb.HorizontalAlignment = HorizontalAlignment.Center;
				MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.User };
				icon.VerticalAlignment = VerticalAlignment.Center;
				icon.Height = 30;
				icon.Width = 30;
				sp.Children.Add(icon);
				sp.Children.Add(tb_name);

				Border border = new Border();
				border.CornerRadius = new CornerRadius(10.0);
				border.Background = Brushes.LightBlue;
				border.Height = 38;
				border.Margin = new System.Windows.Thickness { Left = 10 };
				//border.Width = tb.Width + 100;
				border.BorderBrush = Brushes.LightBlue;
				border.BorderThickness = new Thickness(10, 10, 10, 10);
				border.Child = tb;
				sp.Children.Add(border);
				sp.Margin = new Thickness { Top = 10 };
			}
			chatRoom.ChatBox.Children.Add(sp);
		}

		private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		public void memberAdd(string s)
		{
			StackPanel sp = new StackPanel();
			TextBlock tb = new TextBlock();
			tb.Background = Brushes.LightGreen;
			tb.Text = s+" 加入聊天";
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.Foreground = Brushes.Gray;

			sp.Children.Add(tb);
			sp.Margin = new Thickness { Top = 10 };
			sp.HorizontalAlignment = HorizontalAlignment.Center;
			chatRoom.ChatBox.Children.Add(sp);
		}

		public void memberLeave(string s)
		{
			StackPanel sp = new StackPanel();
			TextBlock tb = new TextBlock();
			tb.Background = Brushes.LightGreen;
			tb.Text = s + " 離開聊天";
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.Foreground = Brushes.Gray;

			sp.Children.Add(tb);
			sp.Margin = new Thickness { Top = 10 };
			sp.HorizontalAlignment = HorizontalAlignment.Center;
			chatRoom.ChatBox.Children.Add(sp);
		}

		public void memberALL(string[] allin)
		{
			chatRoom.MemberBox.Children.Clear();
			for(int i=0;i<allin.Length;i++)
			{
				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				TextBlock tb_name = new TextBlock();
				tb_name.Text = allin[i];
				tb_name.VerticalAlignment = VerticalAlignment.Center;
				tb_name.HorizontalAlignment = HorizontalAlignment.Center;
				MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.UserBadge };
				icon.VerticalAlignment = VerticalAlignment.Center;
				icon.Height = 30;
				icon.Width = 30;
				sp.Children.Add(icon);

				Border border = new Border();
				border.CornerRadius = new CornerRadius(10.0);
				border.Background = Brushes.Orange;
				border.Height = 38;
				border.Margin = new System.Windows.Thickness { Left = 5 };
				//border.Width = tb.Width + 100;
				border.BorderBrush = Brushes.Orange;
				border.BorderThickness = new Thickness(10, 10, 10, 10);
				border.Child = tb_name;
				sp.Children.Add(border);
				sp.Margin = new Thickness { Top = 10 };
				chatRoom.MemberBox.Children.Add(sp);
			}
		}

		private void GoChat_Click(object sender, RoutedEventArgs e)
		{
			frame.NavigationService.Navigate(this.chatRoom);
		}

		private void GoAccount_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("-帳戶資訊-\r\n帳號: " + this.user_id + "\r\n" + "密碼: " + this.user_pw + "\r\n" + "帳戶名稱: " + this.user_name + "\r\n" +
				"帳戶系所: " + this.user_depart + "系" + this.user_grade + "年級\r\n" + "帳戶性別: " + this.user_gender + "\r\n" + "帳戶信箱: " + this.user_email + "\r\n"
				, "Account", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void GoContact_Click(object sender, RoutedEventArgs e)
		{
			string[] name = new string[] { "鍾秉諭", "楊允彤", "周筱容" };
			string[] job = new string[] { "後端程式碼撰寫", "前端版面設計", "PPT報告製作" };
			string[] link= new string[] { "Icon/banana.jpg", "Icon/user_female.png", "Icon/user_female.png" };
			Contact contact = new Contact(1,name,job,link);
			frame.NavigationService.Navigate(contact);
		}

		private void GoOut_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
