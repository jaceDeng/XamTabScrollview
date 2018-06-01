using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamTabScrollview
{
	public partial class MainPage : TopTabbedPage
	{
		public MainPage()
		{
			InitializeComponent(); 
            NavigationPage.SetHasNavigationBar(this, false);
        }
	}
}
