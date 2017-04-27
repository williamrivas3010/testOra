using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Syncfusion.XlsIO;


namespace Extensions
{
   public static class ExcelExtensions
    {
        static ExcelExtensions()
        {
            TemplatedTypes = new[] { "" }; 
        }






        public static FileStreamResult BasicXls<T>(string fullFilePath, IEnumerable<T> model, bool includeTitle, int firstRow, Dictionary<string, string> headers, params string[] excludedColums)
        {
            var dataTable = CreateDataTable(model, excludedColums ?? new string[] { ""});

            return CreateExcelStream(
                fullFilePath,
                CopyHeadersAction(headers),
                CopyDataTableAction(dataTable, includeTitle, firstRow)
                ).ToFile(fullFilePath);
        }

        public static Stream ToBasicXls<T>(this IEnumerable<T> model, bool includeTitle, int firstRow, Dictionary<string, string> headers, params string[] excludedColums)
        {
            var dataTable = CreateDataTable(model, excludedColums ?? new string[] { "" });

            return CreateExcelStream(null, CopyHeadersAction(headers), CopyDataTableAction(dataTable, includeTitle, firstRow));
            
        }



        public static FileStreamResult ToFile(this Stream stream, string filePath)
        {
            var file = new FileStreamResult(stream, Path.GetExtension(filePath).GetContentType());
            file.FileDownloadName = Path.GetFileName(filePath);              

            return file;
        }
        public static FileStreamResult ToFile(this MemoryStream stream, string filePath)
        {
            var file = new FileStreamResult(stream, Path.GetExtension(filePath).GetContentType());
            file.FileDownloadName = Path.GetFileName(filePath);
            return file;
        }

        #region Excel Engine

        private static DataTable CreateDataTable<T>(IEnumerable<T> collection, params string[] excluded)
        {
            var allExcluded = new HashSet<string>(excluded);
            allExcluded.Add("Id");
            allExcluded.Add("Timestamp");
            var displayProperties = typeof(T).GetProperties().Where(p => !allExcluded.Contains(p.Name)).OrderBy(p => p.MetadataToken).ToArray();
            string[] formats = new string[displayProperties.Length];
            string[] templates = new string[displayProperties.Length];
            bool[] isEnumerable = new bool[displayProperties.Length];

            var excludedTemplates = new HashSet<Type>();
            excludedTemplates.Add(typeof(Link<>));

            DataTable dt = new DataTable("DataTable");
            //Inspect the properties and create the columns in the DataTable 
            for (int i = 0; i < displayProperties.Length; i++)
            {
                PropertyInfo prop = displayProperties[i];
                templates[i] = excludedTemplates.Contains(prop.PropertyType) ? null : prop.Template(ref isEnumerable[i], ref formats[i]);
                string name = prop.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Select(d => d.Name).SingleOrDefault() ?? prop.Name;
                dt.Columns.Add(name, typeof(string));
            }

            //Populate the data table 
            foreach (T item in collection)
            {
                DataRow dr = dt.NewRow();
                dr.BeginEdit();
                for (int i = 0; i < displayProperties.Length; i++)
                {
                    PropertyInfo prop = displayProperties[i];
                    object value = prop.GetValue(item, null);
                    if (value != null)
                    {
                        if (isEnumerable[i])
                        {
                            var list = (System.Collections.IEnumerable)value;
                            StringBuilder sb = new StringBuilder();
                            foreach (object val in list)
                            {
                                sb.Append(templates[i] != null ? "" : "");//TODO Implement RenderViewToString
                                // RenderRazorViewToString(templates[i], val) : val).Append(", ");
                            }
                            sb.Length = Math.Max(0, sb.Length - 2);
                            dr[i] = sb.ToString();
                        }
                        else
                        {
                            dr[i] = formats[i] != null
                                ? string.Format(formats[i], value)
                                : templates[i] != null
                                    ? ""
                                    : value.ToString();
                            //  RenderRazorViewToString(templates[i], value) : value.ToString();
                        }
                    }
                }
                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private static Action<IWorkbook> CopyDataTableAction(DataTable dataTable, bool includeTitle, int firstRow = 2, int firstColumn = 1, int worksheetIndex = 0)
        {
            return workbook => workbook.Worksheets[worksheetIndex].ImportDataTable(dataTable, includeTitle, firstRow, firstColumn);
        }
        private static Action<IWorkbook> CopyHeadersAction(Dictionary<string, string> headers, int worksheetIndex = 0)
        {
            if (headers != null)
            {
                return workbook =>
                {
                    var xlsFeuille = workbook.Worksheets[worksheetIndex];
                    foreach (var kvp in headers)
                    {
                        xlsFeuille.FindFirst(kvp.Key, ExcelFindType.Text).Value = kvp.Value;
                    }
                };
            }
            return workbook => { };
        }



        public static Stream CreateExcelStream(string filePath , params Action<IWorkbook>[] fillActions)
        {
            MemoryStream stream = new MemoryStream();
            using (ExcelEngine xlsEng = new ExcelEngine())
            {
                IApplication xlsApp = xlsEng.Excel;
                xlsApp.UseNativeStorage = false;
                IWorkbook workbook = !string.IsNullOrEmpty(filePath)? xlsApp.Workbooks.Open(filePath) : xlsApp.Workbooks.Create();

                foreach (var action in fillActions)
                    action(workbook);

                workbook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
                workbook.Close();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }     


        public static string GetContentType(this string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            switch (extension)
            {
                case ".pot":
                case ".pps":
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".wcm":
                case ".wdb":
                case ".wks":
                case ".wps":
                    return "application/vnd.ms-works";
                case ".doc":
                case ".dot":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                case ".xla":
                case ".xlc":
                case ".xlm":
                case ".xlt":
                case ".xlw":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pdf":
                    return "application/pdf";
                case ".zip":
                    return "application/zip";
                default:
                    return "application/octetstream";
            }
        }

        public static string Template(this PropertyInfo prop, ref bool isEnumerable, ref string format)
        {
            format = prop.GetCustomAttributes(typeof(DisplayFormatAttribute), false).Cast<DisplayFormatAttribute>()
                .Select(d => d.DataFormatString).SingleOrDefault();

            if (format != null)
                return null;

            string templateName = prop.GetCustomAttributes(typeof(UIHintAttribute), false).Cast<UIHintAttribute>()
                .Select(d => d.UIHint).SingleOrDefault();

            if (templateName == null)
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    isEnumerable = true;
                    templateName = prop.PropertyType.GetGenericArguments()[0].RemoveNullable().Name;
                }
                else
                {
                    templateName = prop.PropertyType.RemoveNullable().Name;
                }
                if (!TemplatedTypes.Contains(templateName))
                {
                    return null;
                }
            }
            return string.Format("~/Views/Shared/DisplayTemplates/{0}.cshtml", templateName);
        }
        private static Type RemoveNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
        public static string[] TemplatedTypes { get; set; }

        public static object Filters { get; set; }
        #endregion

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static void ToFile(this FileStreamResult input, string filename)
        {
            using (Stream file = File.Create(filename))
            {
                CopyStream(input.FileStream, file);
            }
        }

    }
}
