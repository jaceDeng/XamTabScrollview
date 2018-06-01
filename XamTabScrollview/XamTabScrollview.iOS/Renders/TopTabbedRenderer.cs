using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Masonry;
using Foundation; 
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(XamTabScrollview.TopTabbedPage), typeof(XamTabScrollview.iOS.Renders.TopTabbedRenderer))]
namespace XamTabScrollview.iOS.Renders
{
    //https://github.com/momoxiaoming/AlTabScrollview/blob/master/TabControllerDemo/TabController.m
    public class TopTabbedRenderer : UIViewController, IVisualElementRenderer
    {
        private List<UIView> Tabs = new List<UIView>();
        private List<UIViewController> Controllers = new List<UIViewController>();
        bool _loaded;
        Size _queuedSize;
        public TopTabbedRenderer()
        {

        }

        private TabScrollview TabScrollview;
        private TabContentView TabContent;
        public VisualElement Element { get; private set; }
        public TopTabbedPage Tabbed => Element as TopTabbedPage;
        public UIView NativeView
        {
            get { return View; }
        }

        public UIViewController ViewController => this;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        private bool _appeared;

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
        }

        public void SetElement(VisualElement element)
        {

            //设置
            var oldElement = Element;
            Element = element;
            Tabbed.PropertyChanged += OnPropertyChanged;
            Tabbed.PagesChanged += OnPagesChanged; 
            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            OnPagesChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));



        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            //{
            //    var current = Tabbed.CurrentPage;
            //    if (current == null)
            //        return;

            //    var controller = GetViewController(current);
            //    if (controller == null)
            //        return;

            //    SelectedViewController = controller;
            //}
            //else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
            //    UpdateBarBackgroundColor();
            //else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
            //    UpdateBarTextColor();
            //else if (e.PropertyName == PrefersStatusBarHiddenProperty.PropertyName)
            //    UpdatePrefersStatusBarHiddenOnPages();
            //else if (e.PropertyName == PreferredStatusBarUpdateAnimationProperty.PropertyName)
            //    UpdateCurrentPagePreferredStatusBarUpdateAnimation();
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }

        void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o, i), Reset);

            //SetControllers();

            //UIViewController controller = null;
            //if (Tabbed.CurrentPage != null)
            //    controller = GetViewController(Tabbed.CurrentPage);
            //if (controller != null && controller != base.SelectedViewController)
            //    base.SelectedViewController = controller;

            //UpdateBarBackgroundColor();
            //UpdateBarTextColor();
        }


        public void SetElementSize(Size size)
        {
            if (_loaded)
                Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
            else
                _queuedSize = size;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (!_appeared || Element == null)
                return;

            _appeared = false;
            Tabbed.SendDisappearing();
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Tabbed.SendAppearing();
            this.View.BackgroundColor = UIColor.Red;
            int i = 0;
            foreach (var item in Tabbed.Children)
            {
                UILabel tab = new UILabel();
                tab.Font = tab.Font.WithSize(12);
                tab.TextAlignment = UITextAlignment.Center;
                tab.HighlightedTextColor = UIColor.White;
                tab.Text = item.Title;
                tab.TextColor = Tabbed.BarTextColor == Xamarin.Forms.Color.Default ? UIColor.White : Tabbed.BarTextColor.ToUIColor();
                
                //tab.BackgroundColor = UIColor.Red;
                Tabs.Add(tab);
                var renderer = Platform.GetRenderer(item);
                if (renderer == null)
                {
                    renderer = Platform.CreateRenderer(item);
                    Platform.SetRenderer(item, renderer);
                }
                UIViewController controller = renderer.ViewController;
                controller.View.Tag = i++;
                Controllers.Add(controller);
            }
            TabScrollview = new TabScrollview(CoreGraphics.CGRect.Empty);
            TabScrollview.BackgroundColor = Tabbed.BarBackgroundColor.ToUIColor();
            this.View.AddSubview(TabScrollview);
            TabScrollview.MakeConstraints(make =>
            {
                make.Height.EqualTo(new NSNumber(50));
                make.Left.EqualTo(this.View);
                make.Right.EqualTo(this.View);
                make.Top.EqualTo(this.View);
            });
            /*
               [_tabScrollView mas_makeConstraints:^(MASConstraintMaker *make) {
    make.height.equalTo(@50);
    make.left.equalTo(self.view);
    make.right.equalTo(self.view);
    make.top.equalTo(self.view);
}];
             */
            TabScrollview.ConfigParameter(this.View.Frame.Width / Tabs.Count, 50, Tabbed.TabIndicator.ToUIColor(), Tabs, 0, (index) =>
              {
                  TabContent.UpdateTab(index);

              });

            TabContent = new TabContentView(CoreGraphics.CGRect.Empty);
            this.View.AddSubview(TabContent);
            TabContent.BackgroundColor = UIColor.Purple;
            Masonry.UIViewExtensions.MakeConstraints(TabContent, (make) =>
            {

                make.Left.EqualTo(this.View);
                make.Right.EqualTo(this.View);
                make.Top.EqualTo(TabScrollview.Bottom());
                make.Bottom.EqualTo(this.View);
            });
            /*
           [_tabContent mas_makeConstraints:^(MASConstraintMaker *make) {
      make.left.equalTo(self.view);
      make.right.equalTo(self.view);
      make.top.equalTo(_tabScrollView.mas_bottom);
      make.bottom.equalTo(self.view);
  }];  
           */
            TabContent.ConfigParam(Controllers.ToArray(), 0, (index) =>
            {
                TabScrollview.UpdateTagLine(index);

            });
        }

    }
}