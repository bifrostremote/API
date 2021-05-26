using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BifrostApi.Session
{
    public class SessionHelper
    {
        public static Session GetCurrentSession(ISession session)
        {
            if (session.GetString("sessionData") == null)
                return new Session();

            var currentSession = JsonConvert.DeserializeObject<Session>(session.GetString("sessionData"));

            return currentSession;
        }

        public static bool IsSessionAuthenticated(ISession session)
        {
            if (session.GetString("sessionData") == null)
                return false;

            var currentSession = JsonConvert.DeserializeObject<Session>(session.GetString("sessionData"));

            return currentSession.isAuthenticated;
        }

        public static void SaveSession(Session session, ISession controllerSession)
        {
            // HACK: Avoid using primitive types to save session data.
            var sessionData = JsonConvert.SerializeObject(session);

            controllerSession.SetString("sessionData", sessionData);
        }
    }
}
