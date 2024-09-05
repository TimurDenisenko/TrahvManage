using System;
using System.Collections.Generic;
using System.Linq;

namespace TrahvManage.Models
{
    public static class HistoryConverter
    {
        public static string ConvertToString(TrahvContext db, string history)
        {
            int key = db.Chats.Count();
            string personalCode = db.Accounts.Find(UserState.Id).PersonalCode;
            return $"{personalCode}:::{history}";
        }
        public static List<Tuple<string,string>> ConvertFromString(string history)
        {
            string[] msgs = history.Split(new string[] { ",,," }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            foreach (string msg in msgs)
            {
                string[] values = msg.Split(new string[] { ":::" }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();
                list.Add(new Tuple<string, string>(values[0], values[1]));
            }
            return list;
        }
        public static string Concat(TrahvContext db, ChatModel newMessage) =>
            $"{db.Chats.Find(newMessage.Id).History},,,{ConvertToString(db, newMessage.History)}";
    }
}