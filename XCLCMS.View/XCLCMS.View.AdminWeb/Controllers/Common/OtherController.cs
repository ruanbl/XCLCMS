﻿using System.Web.Mvc;

namespace XCLCMS.View.AdminWeb.Controllers.Common
{
    /// <summary>
    /// 其它开放的Controller
    /// </summary>
    public class OtherController : XCLCMS.Lib.Base.AdminBaseController
    {
        /// <summary>
        /// 跳转至文件管理页
        /// </summary>
        public ActionResult FileManagerFileList()
        {
            return Redirect(XCLCMS.Lib.SysWebSetting.Setting.SettingModel.FileManager_FileListURL);
        }

        /// <summary>
        /// 跳转至文件上传页
        /// </summary>
        /// <returns></returns>
        public ActionResult FileManagerUpload()
        {
            return Redirect(XCLCMS.Lib.SysWebSetting.Setting.SettingModel.FileManager_FileUploadURL);
        }

        /// <summary>
        /// 404页面
        /// </summary>
        public ActionResult Error404()
        {
            return View("~/Views/Common/Error.cshtml", new XCLNetTools.Message.MessageModel()
            {
                ErrorCode = "404",
                IsSuccess = false,
                Message = "您访问的页面不存在！",
                FromUrl = XCLCMS.View.AdminWeb.Common.WebCommon.RefferUrl,
                Url = XCLNetTools.StringHander.FormHelper.GetString("aspxerrorpath")
            });
        }
    }
}