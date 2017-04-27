using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public static class States
    {
        public static List<Tuple<string, string>> ToList()
        {
            return StatesName.Select(i => Tuple.Create(i.Key, i.Value)).ToList();
        }

        public static List<Tuple<string, string>> Search(string keywords)
        {
            var key = keywords.ToLower().Trim();
            return ToList()
                .Where(st => st.Item2.ToLower().Contains(key) ||
                             st.Item1.ToLower().Contains(key))
                             .ToList();
        }

        public static List<Tuple<string, string>> SearchAsync(string keywords)
        {
            var key = keywords.ToLower().Trim();
            if (!string.IsNullOrEmpty(keywords))
            {
                return ToList()
                 .Where(st => st.Item2.ToLower().Contains(key) ||
                              st.Item1.ToLower().Contains(key))
                              .ToList();
            }
            else
            {
               return ToList().ToList();
            }
            
        }


        public static string GetCodeFor(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) return null;

            if (StatesName.ContainsKey(stateName)) return stateName;

            var stateCode =  StatesName.FirstOrDefault(c => c.Value.ToLower().Contains(stateName.ToLower())) ;
            

            return stateCode.Key;

        }


        public static Dictionary<string, string> StatesName = new Dictionary<string, string>()
            
        {
        {"AL","Alabama"},
        {"AK","Alaska"},
        {"AP","Armed Forces, Pacific"},
        {"AR","Arkanzas"},
        {"AS", "American Somoa"},
        {"AZ","Arizona"},     
        {"CA","California"},
        {"CO","Colorado"},
        {"CT","Connecticut"},
        {"DC","District Of Columbia"},
        {"DE","Delaware"},
        {"FM","Federate States of Micronesia"},
        {"FL","Florida"},
        {"GA","Georgia"},
        
        {"GU","Guam"},
        {"HI","Hawaii"},
        {"IA","Iowa"},
        {"ID","Idaho"},
        {"IL","Illinois"},
        {"IN","Indiana"},        
        {"KS","Kansas"},
        {"KY","Kentucky"},
        {"LA","Louisiana"},
        {"MA","Massachusetts"},
        {"MD","Maryland"},
        {"ME","Maine"},
        {"MH","Marshall Islands"},       
        {"MI","Michigan"},
        {"MN","Minnesota"},
        {"MS","Mississippi"},
        {"MO","Missouri"},
        {"MP","Northern Mariana Islands"},
        {"MT","Montana"},
        {"NC","North Carolina"},
        {"ND","North Dakota"},
        {"NE","Nebraska"},
        {"NH","New Hampshire"},
        {"NJ","New Jersey"},
        {"NM","New Mexico"},
        {"NV","Nevada"},
        {"NY","New York"},
        {"OH","Ohio"},  
        {"OK","Oklahoma"},
        {"OR","Oregon"},
        {"PA","Pennsylvania"},
        {"PR","Puerto Rico"},
        {"RI","Rhode Island"},
        {"SC","South Carolina"},
        {"SD","South Dakota"},
        {"TN","Tennessee"},
        {"TX","Texas"},
        {"UT","Utah"},
        {"VT","Vermont"},
        {"VA","Virginia"},
        {"VI","Virgin Islands"},
        {"WA","Washington"},
        {"WV","West Virginia"},
        {"WI","Wisconsin"},
        {"WY","Wyoming"},

        };

        public static bool Any(string state)
        {
            return StatesName.ContainsKey(state.Trim().ToUpper());
        }
        public static bool AnyValue(string state)
        {
            return StatesName.ContainsValue(state.TrimStart().TrimEnd());
        }
    }
    
}
