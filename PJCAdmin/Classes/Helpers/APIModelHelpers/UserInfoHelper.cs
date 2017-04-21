using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using PJCAdmin.Models;

namespace PJCAdmin.Classes.Helpers.APIModelHelpers
{
    public class UserInfoHelper
    {
        private DbHelper helper;

        public UserInfoHelper(DbHelper h = null)
        {
            if (h == null)
                helper = new DbHelper();
            else
                helper = h;
        }

        public DbHelper getDBHelper()
        {
            return helper;
        }

        public UserInfoModel getJobCoachInfo(string username)
        {
            return getUserInfo(helper.findUserName(username).jobCoachUserName);
        }
        public UserInfoModel getParentInfo(string username)
        {
            return getUserInfo(helper.findUserName(username).guardianUserName);
        }
        public UserInfoModel getUserInfo(string username)
        {
            MembershipUser user = System.Web.Security.Membership.GetUser(username);
            ProfileBase profile = ProfileBase.Create(username);

            UserInfoModel info = new UserInfoModel();
            info.userName = username;
            info.phone = profile.GetPropertyValue("PhoneNumber").ToString();
            info.email = user.Email;

            return info;
        }

        //for user list for app
        public List<UserInfoModel> getListOfUsersAssignedToCoach(string CoachUsername)
        {
            List<UserInfoModel> userList = new List<UserInfoModel>();

            //UserName12 is collection where self is jobcoach
            foreach (PJCAdmin.Models.UserName usr in helper.findUserName(CoachUsername).UserName12)
            {
                UserInfoModel info = getUserInfo(usr.userName1);
                userList.Add(info);
            }

            return userList;
        }
    }
}