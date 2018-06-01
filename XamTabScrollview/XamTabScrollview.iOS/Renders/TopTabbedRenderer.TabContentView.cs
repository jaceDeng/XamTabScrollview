using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XamTabScrollview.iOS.Renders
{
    internal class TabContentView : UIView, IUIPageViewControllerDataSource
    {
        Action<int> TabSwitch;
        UIPageViewController PageController { get; set; }
        UIViewController[] Controllers { get; set; }
        //  TabSwitchBlcok tabSwitch;
        public TabContentView(CGRect frame) : base(frame)
        {
            PageController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal);

            //结束滑动
             
            PageController.DidFinishAnimating += (sender, e) =>
            {
                int index = Array.FindIndex(Controllers, x => x.View.Tag == PageController.ViewControllers[0].View.Tag);
              
                TabSwitch?.Invoke(index);

            };


            //[[UIPageViewController alloc] initWithTransitionStyle: UIPageViewControllerTransitionStyleScroll navigationOrientation:UIPageViewControllerNavigationOrientationHorizontal options:nil];

            PageController.DataSource = this;
            PageController.View.Frame = this.Bounds;
            this.AddSubview(PageController.View);

        }
        public void ConfigParam(UIViewController[] controllers, int index, Action<int> tabSwitch)
        {
            TabSwitch = tabSwitch;
            //回调事件
            //_tabSwitch = tabSwitch;
            Controllers = controllers;
            // _tabSwitch = tabSwitch;
            PageController.SetViewControllers(new UIViewController[] { controllers[index] }, UIPageViewControllerNavigationDirection.Reverse, true, null);

        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            PageController.View.Frame = this.Bounds;
        }

        public void UpdateTab(int index)
        {
            //默认展示的第一个页面
            PageController.SetViewControllers(new UIViewController[] { Controllers[index] }, UIPageViewControllerNavigationDirection.Reverse, true, null);

        }
        //返回前一个
        public UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            int index = Array.FindIndex(Controllers, x => x == referenceViewController);
            //如果是第一个页面
            if (index == 0)
            {
                return null;
            }
            index--;
            return Controllers[index];
        }

        //返回后一个
        public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            int index = Array.FindIndex(Controllers, x => x == referenceViewController);

            if (index == (Controllers.Length - 1))
            {
                return null;
            }
            index++;
            return Controllers[index];
        }


    }
}