using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PJCAdmin.Models;
using PJCAdmin.Classes.Helpers;
using PJCAdmin.Classes.Helpers.APIModelHelpers;
using System.Web.Helpers;
using System.Web.Http.Controllers;

namespace PJCAdmin.ControllersAPI
{
    public class RoutineController : ApiController
    {
        private RoutineHelper helper;
        private Auth auth;

        public RoutineController()
        {
            helper = new RoutineHelper();
            DbHelper context = helper.getDBHelper();
            auth = new Auth(context);
            //string some = Request.Content.ToString();
        }

        // GET api/Routine?token=<token>
        public IEnumerable<Routine> Get(string token)
        {
            auth.authorizeToken(token);
            string userName = auth.getUserNameFromToken(token);
            
            return helper.getAllRoutinesAssignedToUserForSerialization(userName);
        }

        //GET api/Routine?token=<token>&username=<username>
        public IEnumerable<Routine> GetRoutineListForUser(string token, string username)
        {
            if (username == null) return null;
            auth.authorizeToken(token);
            return helper.getAllRoutinesAssignedToUserForSerialization(username);
        }

        /*[HttpGet]
        public IEnumerable<string> GetRoutineInfo(string token, string username, string routineTitle)
        {
            List<string> routineInfo = new List<string>();
            if (username == null) return null;
            auth.authorizeToken(token);
            //UserInfoModel temp = helper.getRoutineInfo(username);
            //userInfo.Add(temp.userName);
            //userInfo.Add(temp.phone);
            //userInfo.Add(temp.email);

            Routine temp = helper.getRoutineAssignedToByName(routineTitle);
            routineInfo.Add(temp.routineID.ToString());
            routineInfo.Add(temp.creatorUserName);
            routineInfo.Add(temp.assigneeUserName);
            routineInfo.Add(temp.routineTitle);
            routineInfo.Add(temp.isTimed.ToString());
            routineInfo.Add(temp.expectedDuration.ToString());
            routineInfo.Add(temp.isNotifiable.ToString());
            routineInfo.Add(temp.updatedDate.ToString());
            routineInfo.Add(temp.isDisabled.ToString());

            return routineInfo;
        }*/

        //POST api/Routine?token=<token>&create=<c,m,or d>&model=<jsonStringObject>
        [HttpPost]
        public HttpResponseMessage PostRoutine(PostRoutineRequest data)
        {
            string token = data.token;
            string create = data.create;

            PJCAdmin.Classes.Helpers.MVCModelHelpers.RoutineHelper helper = new Classes.Helpers.MVCModelHelpers.RoutineHelper();
            RoutineModel routine = Json.Decode<RoutineModel>(data.model.ToString());

            switch (create)
            {
                case "c": //create new job
                    //create the model in the database
                    helper.createRoutine(auth.getUserNameFromToken(token), routine);
                    break;

                case "m": //modify existing job
                    //find the routine that matches
                    //update the database for routine
                    try
                    {
                        helper.modifyExistingRoutine(auth.getUserNameFromToken(token), routine.routineTitle, routine);
                    }catch(InvalidOperationException)
                    {
                        helper.updateRoutine(auth.getUserNameFromToken(token), routine.routineTitle, routine);
                    }
                    break;

                case "d": //delete existing job
                    //find the routine that matches
                    //delete the matching routine from the database
                    helper.deleteRoutine(auth.getUserNameFromToken(token), routine.routineTitle, routine.assigneeUserName, false);
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Job Deleted");

                default: return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Create Code Not Recognized");
            }

            return Request.CreateResponse<string>(HttpStatusCode.Created, "Job Created"); 
        }

        public class PostRoutineRequest
        {
            public string token { get; set; }
            public string create { get; set; }
            public object model { get; set; }
        }

        protected override void Dispose(bool disposing)
        {
            helper.dispose();
            base.Dispose(disposing);
        }
        
        [HttpGet]
        public RoutineModel GetRoutineInfo(string token, string username, string routineTitle)
        {
            RoutineModel routineInfo = new RoutineModel();
            if (username == null) return null;
            auth.authorizeToken(token);
            //UserInfoModel temp = helper.getRoutineInfo(username);
            //userInfo.Add(temp.userName);
            //userInfo.Add(temp.phone);
            //userInfo.Add(temp.email);


            Routine temp = helper.getRoutineAssignedToByName(routineTitle);
            //routineInfo = (RoutineModel) temp;
            return routineInfo;
        }
    }
}
