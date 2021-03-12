using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CCU_Plus2._0
{
	/// <summary>
	/// Contact.xaml 的互動邏輯
	/// </summary>
	public partial class Contact : Page
	{
		private string[] memberName = new string[3];
		private string[] memberJob = new string[3];
		private string[] memberImage = new string[3];
		private int currentpage;
		public Contact(int page,string[] name,string[] job,string[] imagelink)
		{
			InitializeComponent();
			if(page == 1)
			{
				Back.IsEnabled = false;
			}
			else if(page == 3)
			{
				NextAndContact.Content = "Contact";
			}
			this.currentpage = page;
			this.memberName = name;
			this.memberJob = job;
			this.memberImage = imagelink;
			string link = this.memberImage[page - 1];
			image.Source = new BitmapImage(new Uri(link, UriKind.Relative));
			Name.Content = memberName[page - 1];
			Job.Content = memberJob[page - 1];
		}

		private void NextAndContact_Click(object sender, RoutedEventArgs e)
		{
			if(NextAndContact.Content.Equals("Contact"))
			{
				Process.Start("mailto:ch.parry1233@gmail.com");
			}
			else
			{
				Contact contact = new Contact(this.currentpage + 1, this.memberName, this.memberJob, this.memberImage);
				this.NavigationService.Navigate(contact);
			}
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			Contact contact = new Contact(this.currentpage - 1, this.memberName, this.memberJob, this.memberImage);
			this.NavigationService.Navigate(contact);
		}
	}
}
