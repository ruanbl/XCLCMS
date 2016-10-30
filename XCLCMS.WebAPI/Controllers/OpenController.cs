﻿using System.Threading.Tasks;
using System.Web.Http;
using XCLCMS.Data.WebAPIEntity;

namespace XCLCMS.WebAPI.Controllers
{
    /// <summary>
    /// 开放的API
    /// </summary>
    public class OpenController : BaseAPIController
    {
        private XCLCMS.Data.BLL.UserInfo userInfoBLL = new XCLCMS.Data.BLL.UserInfo();
        private XCLCMS.Data.BLL.Merchant merchantBLL = new XCLCMS.Data.BLL.Merchant();
        private XCLCMS.Data.BLL.MerchantApp merchantAppBLL = new XCLCMS.Data.BLL.MerchantApp();

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost]
        public async Task<APIResponseEntity<XCLCMS.Data.Model.Custom.UserInfoDetailModel>> LogonCheck([FromBody] APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.Open.LogonCheckEntity> request)
        {
            return await Task.Run(() =>
            {
                var response = new APIResponseEntity<XCLCMS.Data.Model.Custom.UserInfoDetailModel>();
                XCLCMS.Data.Model.UserInfo userModel = null;
                if (string.IsNullOrWhiteSpace(request.Body.UserToken))
                {
                    //用户名和密码登录
                    userModel = userInfoBLL.GetModel(request.Body.UserName, XCLCMS.WebAPI.Library.EncryptHelper.EncryptStringMD5(request.Body.Pwd));
                }
                else
                {
                    //token登录
                    userModel = XCLCMS.WebAPI.Library.Common.GetUserInfoByUserToken(request.Body.UserToken);
                }

                if (null == userModel)
                {
                    response.Message = string.Format("用户名或密码不正确！", request.Body.UserName);
                    response.IsSuccess = false;
                }
                else if (!string.Equals(userModel.UserState, XCLCMS.Data.CommonHelper.EnumType.UserStateEnum.N.ToString()))
                {
                    response.Message = string.Format("用户名{0}已被禁用！", request.Body.UserName);
                    response.IsSuccess = false;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Body = new Data.Model.Custom.UserInfoDetailModel();
                    //用户基本信息
                    response.Body.UserInfo = userModel;
                    //登录令牌
                    response.Body.Token = XCLCMS.WebAPI.Library.EncryptHelper.CreateToken(new Data.Model.Custom.UserNamePwd()
                    {
                        UserName = userModel.UserName,
                        Pwd = userModel.Pwd
                    });
                    //所在商户
                    response.Body.Merchant = this.merchantBLL.GetModel(userModel.FK_MerchantID);
                    //所在商户应用
                    response.Body.MerchantApp = this.merchantAppBLL.GetModel(userModel.FK_MerchantAppID);
                }

                //写入日志
                XCLNetLogger.Log.WriteLog(new XCLNetLogger.Model.LogModel()
                {
                    LogType = XCLCMS.Data.CommonHelper.EnumType.LogTypeEnum.LOGIN.ToString(),
                    LogLevel = XCLNetLogger.Config.LogConfig.LogLevel.INFO,
                    Title = string.Format("用户{0}，尝试登录系统{1}", request.Body.UserName, response.IsSuccess ? "成功" : "失败")
                });

                return response;
            });
        }
    }
}