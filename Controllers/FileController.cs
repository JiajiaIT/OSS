using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OSS.Common;
using OSS.Models;
using System.IO;

namespace OSS.Controllers
{
    /// <summary>
    /// 文件资源管理
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("[action]")]
        public IActionResult Readlog()
        {
            try
            {
                var logPath = Directory.GetCurrentDirectory() + "/Content/log.txt";
                using (StreamReader sr = new StreamReader(logPath))
                {
                    List<Log> logs = new List<Log>();
                    while (true)
                    {
                        string str = sr.ReadLine();
                        if (String.IsNullOrEmpty(str))
                        {
                            break;
                        }
                        Log log = new Log()
                        {
                            Time = DateTime.Parse(str.Split("\t\t")[0]),
                            IP = str.Split("\t\t")[1],
                            Path = str.Split("\t\t")[2],
                            FileName = str.Split("\t\t")[3]
                        };
                        logs.Add(log);
                    }
                    if (logs.Count() > 0)
                    {
                        sr.Close();
                        var result = new Result<List<Log>>
                        {
                            Data = logs
                        };
                        return Json(result);

                    }
                    else
                    {
                        throw new Exception("没有数据");
                    }
                }
            }
            catch (Exception ex)
            {
                LMLog.Writelog("/File/Readlog，" + ex.Message);
                var result = new Result<string>
                {
                    Code = 404,
                    Msg = ex.Message
                };
                return Json(result);
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image">图片类型文件</param>
        /// <returns></returns>
        [HttpPost, Route("[action]")]
        public IActionResult SaveImage(IFormFile image)
        {
            try
            {
                var suffix = Path.GetExtension(image.FileName);
                if (suffix == ".jpg" || suffix == ".png" || suffix == ".gif" || suffix == ".bmp" || suffix == ".svg" || suffix == ".webp" || suffix == ".heic" || suffix == ".heif" || suffix == ".raw")
                {
                    //图片存放地址
                    var path = Directory.GetCurrentDirectory() + "/Content/" + "Image/";
                    //判断路径存不存在
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path); //不存在就创建文件夹
                    }
                    string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString().Split('-')[0] + suffix;
                    string savePath = path + imageName;
                    using (FileStream fs = System.IO.File.Create(savePath))
                    {
                        image.CopyTo(fs);
                        fs.Flush();
                    }
                    var result = new Result<string>
                    {
                        Data = imageName
                    };
                    var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    Tools.Writerlog(ip, "/Content/Image", imageName);
                    return Json(result);
                }
                else
                {
                    throw new Exception("请上传正确的格式的文件");
                }
            }
            catch (Exception ex)
            {
                LMLog.Writelog("/File/SaveImage，" + ex.Message);
                var result = new Result<string>
                {
                    Code = 404,
                    Msg = ex.Message
                };
                return Json(result);
            }
        }
        /// <summary>
        /// 上传视频
        /// </summary>
        /// <param name="video">视频类型文件</param>
        /// <returns></returns>
        [HttpPost, Route("[action]")]
        public IActionResult SaveVideo(IFormFile video)
        {
            try
            {
                var suffix = Path.GetExtension(video.FileName);
                if (suffix == ".mp4" || suffix == ".avi" || suffix == ".mov" || suffix == ".wmv" || suffix == ".flv" || suffix == ".mkv" || suffix == ".mpeg" || suffix == ".3gp" || suffix == ".webm" || suffix == ".ogg")
                {
                    //视频存放地址
                    var path = Directory.GetCurrentDirectory() + "/Content/" + "Video/";
                    //判断路径存不存在
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path); //不存在就创建文件夹
                    }
                    string videoName = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString().Split('-')[0] + suffix;
                    string savePath = path + videoName;
                    using (FileStream fs = System.IO.File.Create(savePath))
                    {
                        video.CopyTo(fs);
                        fs.Flush();
                    }
                    var result = new Result<string>
                    {
                        Data = videoName
                    };
                    var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    Tools.Writerlog(ip, "/Content/Video", videoName);
                    return Json(result);
                }
                else
                {
                    throw new Exception("请上传正确的格式的文件");
                }
            }
            catch (Exception ex)
            {
                LMLog.Writelog("/File/SaveVideo，" + ex.Message);
                var result = new Result<string>
                {
                    Code = 404,
                    Msg = ex.Message
                };
                return Json(result);
            }
        }
    }
}
