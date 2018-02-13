using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.Style;
using PointVideoGallery.Models;
using PointVideoGallery.Services;


namespace PointVideoGallery.Api
{
    [RoutePrefix("api/v1/publish")]
    public class PublishApiController : ApiController
    {
        private readonly double _resHeight = 39.0D;

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("excelsheet")]
        public async Task<HttpResponseMessage> GetExcelSheet(DateTime s, DateTime? e)
        {
            if (s.Equals(DateTime.MinValue))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var endTime = e ?? s;

            if (s > endTime)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var result = Request.CreateResponse(HttpStatusCode.OK);

            List<ScheduleAdEvent> list = await new AdService().QueryScheduleAdEventByDateAsync(s, endTime);
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("報表");

                worksheet.Cells["A1"].Value = "項目";
                worksheet.Cells["B1"].Value = "上架時間(月/日 時間)";
                worksheet.Cells["C1"].Value = "下架時間(月/日 時間)";
                worksheet.Cells["D1"].Value = "上架位置";
                worksheet.Cells["E1"].Value = "輪播/隨機";
                worksheet.Cells["F1"].Value = "畫面示意";
                worksheet.Cells["G1"].Value = "廣告/APP連結";
                worksheet.Cells["H1"].Value = "圖片檔名";

                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                StringBuilder builder = new StringBuilder();
                string basePath = ConfigurationManager.AppSettings["LibraryIndexBasePath"];

                int row = 2;
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].AdEvent == null || list[i].AdEvent.Resources == null)
                        continue;

                    for (var j = 0; j < list[i].AdEvent.Resources.Count; j++, row++)
                    {
                        worksheet.Cells[$"A{row}"].Value = row - 1;
                        worksheet.Cells[$"B{row}"].Value = $"{list[i].ScheduleDate:yyyy-MM-dd} 00:00";
                        worksheet.Cells[$"C{row}"].Value = $"{list[i].ScheduleDateEnd:yyyy-MM-dd} 23:59";

                        if (list[i].AdEvent.LocationTags != null)
                        {
                            worksheet.Cells[$"D{row}"].Value =
                                $"{string.Join(" & ", list[i].AdEvent.LocationTags)}";
                        }

                        if (list[i].AdEvent.PlayOutMethod.Equals("random", StringComparison.OrdinalIgnoreCase))
                            worksheet.Cells[$"E{row}"].Value = $"隨機, 比重:{list[i].AdEvent.Resources[j].PlayoutWeight}";
                        else if(list[i].AdEvent.PlayOutMethod.Equals("interval", StringComparison.OrdinalIgnoreCase))
                            worksheet.Cells[$"E{row}"].Value = $"輪播";
                        // COLUMN F RESOURCE IMAGE
                        worksheet.Row(row).Height = _resHeight;
                        var path = Path.Combine(basePath, list[i].AdEvent.Resources[j].Path);
                        var fileInfo = new FileInfo(path);

                        if (File.Exists(path))
                            worksheet.Drawings.AddPicture($"{row}-{j}", fileInfo).SetPosition(row - 1, 0, 5, 0);
                        else
                            worksheet.Cells[$"F{row}"].Value += $"找不到{list[i].AdEvent.Resources[j].Path}";

                        // COLUMN G RESOURCE ACTION
                        if (list[i].AdEvent.Resources[j].Actions != null)
                            for (int k = 0; k < list[i].AdEvent.Resources[j].Actions.Count; k++)
                            {
                                if (list[i].AdEvent.Resources[j].Actions[k].Checked != 1)
                                    continue;
                                if (list[i].AdEvent.Resources[j].Actions[k].Type.Equals(
                                    "image", StringComparison.OrdinalIgnoreCase))
                                {
                                    worksheet.Row(row).Height = _resHeight;

                                    path = Path.Combine(basePath, list[i].AdEvent.Resources[j].Actions[k].Action);

                                    if (!File.Exists(path))
                                    {
                                        worksheet.Cells[$"G{row}"].Value +=
                                            $"找不到{list[i].AdEvent.Resources[j].Actions[k].Action}{Environment.NewLine}";
                                        continue;
                                    }
                                    fileInfo = new FileInfo(path);

                                    worksheet.Drawings.AddPicture($"{row}-{j}-{k}", fileInfo)
                                        .SetPosition(row - 1, k * (int) _resHeight + 1, 6, 0);
                                }
                                worksheet.Cells[$"G{row}"].Value +=
                                    $"{list[i].AdEvent.Resources[j].Actions[k].Type} - " +
                                    $"{list[i].AdEvent.Resources[j].Actions[k].Action}{Environment.NewLine}";
                            }

                        if (list[i].AdEvent.Resources != null)
                        {
                            worksheet.Cells[$"H{row}"].Value =
                                $"{string.Join(Environment.NewLine, list[i].AdEvent.Resources)}";
                        }
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Cells[worksheet.Dimension.Address].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                package.Save();

                var stream = new MemoryStream(package.GetAsByteArray());

                stream.Seek(0, SeekOrigin.Begin);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{DateTime.Now:yyyy-MM-dd}-廣告發佈表.xlsx"
                };
                return result;
            }
        }
    }
}