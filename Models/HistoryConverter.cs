using System;
using System.Collections.Generic;
using System.Linq;

namespace TrahvManage.Models
{
    public class HistoryConverter
    {
        public string ConvertToString(TrahvContext db, string history)
        {
            int key = db.Chats.Count();
            string personalCode = db.Accounts.Find(UserState.Id).PersonalCode;
            return $"{key}:::{personalCode}:::{history}";
        }
        public Dictionary<int,Tuple<string,string>> ConvertFromString(string history)
        {
            string[] msgs = history.Split(',');
            Dictionary<int, Tuple<string, string>> dictionary = new Dictionary<int, Tuple<string, string>>();
            foreach (string msg in msgs)
            {
                string[] values = msg.Split(new string[] { ":::" }, StringSplitOptions.None);
                dictionary.Add(Convert.ToInt32(values[0]), new Tuple<string, string>(values[1], values[2]));
            }
            return dictionary;
        }
    }
}