﻿using System.Web;
using System.Web.Optimization;

namespace FCStore
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerycookie").Include(
                        "~/Scripts/jquery.cookie.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-1.8.24.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerylazyload").Include(
                        "~/Scripts/jquery.lazyload.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryuibannerize").Include(
                        "~/Scripts/jquery.ui.bannerize.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css",
                "~/Content/qqReport.css"));

            bundles.Add(new StyleBundle("~/Manager/css").Include("~/Content/Manager.css"));

            bundles.Add(new StyleBundle("~/ZTree/css").Include("~/Content/zTreeStyle/zTreeStyle.css"));

            bundles.Add(new StyleBundle("~/Content/loginPage").Include("~/Content/LoginPage.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/themes/base/jquery.ui.bannerize.css"));

            bundles.Add(new StyleBundle("~/Content/MyCtrl/css").Include(
                        "~/Content/MyCtrl/MyCtrl.css"));

            bundles.Add(new ScriptBundle("~/UploadFile/css").Include(
                "~/Scripts/uploadify/uploadify.css"));

            bundles.Add(new ScriptBundle("~/xheditor/css").Include(
                "~/xheditor/ui/default/ui.css"));

            bundles.Add(new ScriptBundle("~/bundles/productDetail").Include(
                "~/Scripts/productDetail.js",
                "~/Scripts/productDetailRollImg.js"));

            bundles.Add(new ScriptBundle("~/bundles/HighChart").Include(
                "~/Scripts/highcharts.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                "~/Scripts/common.js",
                "~/Scripts/sidebar-follow-jquery.js",
                "~/Scripts/qqReport.js",
                "~/Scripts/main.js",
                "~/Scripts/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                "~/Scripts/common.js",
                "~/Scripts/login.js"));

            bundles.Add(new ScriptBundle("~/Manager/js").Include(
                "~/Scripts/common.js",
                "~/Scripts/manager.js",
                "~/Scripts/EvaluationGenerator.js"));

            bundles.Add(new ScriptBundle("~/UploadFile/js").Include(
                "~/Scripts/uploadify/jquery.uploadify.js"));

            bundles.Add(new ScriptBundle("~/ZTree/js").Include(
                "~/Scripts/ZTree/jquery.ztree.core-3.5.js",
                "~/Scripts/ZTree/jquery.ztree.excheck-3.5.js",
                "~/Scripts/ZTree/jquery.ztree.exedit-3.5.js"));

            bundles.Add(new ScriptBundle("~/xheditor/js").Include(
                "~/xheditor/script/xheditor-1.1.14-zh-cn.js"));

        }
    }
}