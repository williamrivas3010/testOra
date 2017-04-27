using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BussinesAccess.Extensions
{
    public static class DataExtensions
    {

        public static List<string> ToKeyWords(this string value)
        {
            return (value ?? "").ToLower().Split(' ').ToList();
        }


        static CultureInfo Getculture()
        {
            return new CultureInfo("en-US");
        }
      
        public static Tmodel Deserialize<Tmodel>(string value)where Tmodel : class,new()
        {
            if (string.IsNullOrEmpty(value)) return new Tmodel();
            System.Threading.Thread.CurrentThread.CurrentCulture = Getculture();
            System.Threading.Thread.CurrentThread.CurrentUICulture = Getculture();
            try
            {
                return JsonConvert.DeserializeObject<Tmodel>(value);
            }
            catch (Exception ex)
            {
                return new Tmodel();
            }
            
        }

        public static string Serialize<Tmodel>(this Tmodel model)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = Getculture();
            System.Threading.Thread.CurrentThread.CurrentUICulture = Getculture();
            return JsonConvert.SerializeObject(model);
        }


        public static T1 CopyFrom<T1, T2>(this T1 obj, T2 otherObject,bool excludeComplexTypes =true)
       where T1 : class
       where T2 : class
        {
            
            PropertyInfo[] srcFields = otherObject.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToArray();

            PropertyInfo[] destFields = obj.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);


            if (excludeComplexTypes)
            {
                destFields = (from property in obj.GetType().GetProperties()
                              let name = property.Name
                              let type = property.PropertyType
                              let value = property.GetValue(obj,
                                          (BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public),
                                          null, null, null)
                              where (type.IsPrimitive || type.IsEnum || type.IsValueType || type.IsSerializable)
                              select property).ToArray();
            }
                

                
            foreach (var property in srcFields)
            {
                var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
                if (dest != null && dest.CanWrite   )
                    dest.SetValue(obj, property.GetValue(otherObject, null), null);
            }

            return obj;
        }

        public static decimal ToDecimal(this string value)
        {
            var cleanString = string.Join("", (value ?? "").Where(c => char.IsDigit(c) || c == '.').ToList());
            return !string.IsNullOrEmpty(cleanString) ? Convert.ToDecimal(cleanString) : 0;
        }
        public static int ToInt(this string value)
        {
            var cleanString = string.Join("", (value ??"").Where(c => char.IsDigit(c) ).ToList());                
            return !string.IsNullOrEmpty(cleanString) ? Convert.ToInt32(cleanString) : 0;
           
        }

        public static int? ToNullableInt(this string value)
        {
            var cleanString = string.Join("", (value ?? "").Where(c => char.IsDigit(c)).ToList());
            return !string.IsNullOrEmpty(cleanString) ? Convert.ToInt32(cleanString) : (int?)null;
        }

        public static Tenum? ToEnum<Tenum>(this string stringval, Type enumtype) where Tenum : struct
        {
            var value = stringval.ToLower().Replace(" ", "");
            var values = Enum.GetValues(enumtype).Cast<Tenum>();
            var exists = values.Any(o => o.ToString().ToLower() == value) ||
                values.Any(p => (int)Enum.Parse(enumtype, p.ToString()) == value.ToNullableInt());

            if (!exists) return (Tenum?)null;

            var keyName = values.Where(v => v.ToString().ToLower() == value).Select(v=> v.ToString()).FirstOrDefault()
             ?? values.Where(p => (int)Enum.Parse(enumtype, p.ToString()) == value.ToNullableInt()).Select(v=> v.ToString()).FirstOrDefault();

            return exists ? (Tenum?)Enum.Parse(enumtype, keyName) : (Tenum?) null;
        }

        public static string EnumDescription(this Enum enumeration)
        {
            if (enumeration == null) return string.Empty;
            string value = enumeration.ToString();
            Type type = enumeration.GetType();
            //Use reflection to try and get the description attribute for the enumeration
            DescriptionAttribute[] descAttribute = (DescriptionAttribute[])type.GetField(value).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descAttribute.Length > 0 ? descAttribute[0].Description : value;
        }

      

        public static T ToObject<T>(this IDictionary<string, object> source)
      where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
    }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

    }


}
