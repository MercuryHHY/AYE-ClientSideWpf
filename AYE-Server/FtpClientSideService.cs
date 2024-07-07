using AYE_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;

#if true

public class FtpClientSideService : IFtpClientSideService
{
    private readonly ILogger<FtpClientSideService> _logger;
    private readonly IGolalCacheManager _golalCacheManager;

    public FtpClientSideService(ILogger<FtpClientSideService> logger, IGolalCacheManager golalCacheManager)
    {
        this._logger = logger;
        this._golalCacheManager = golalCacheManager;

        this.ServerAddress = _golalCacheManager.GlobalFtpSetting.FtpServerAddress;
        this.Port = _golalCacheManager.GlobalFtpSetting.FtpPort;
        this.Username = _golalCacheManager.GlobalFtpSetting.FtpUser;
        this.Password = _golalCacheManager.GlobalFtpSetting.FtpPassword;
    }

    public string ServerAddress { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    /// <summary>
    /// 将本地客户端  文件的绝对路径下的存在的 文件 上传到服务器
    /// 或者说是 将本地的绝对路径下的文件的内容复制到 服务器的相对路径下，此相对路径要给出文件名
    /// </summary>
    /// <param name="localFilePath">本地客户端 文件路径下的存在的 文件   比如 "E:/文件测试文件夹/local_file.txt" </param>
    /// <param name="remoteFilePath">服务器的根目录下的相对路径下的文件，不存在则会创建 比如 "/hhy/local_file111.txt"</param>
    public async Task<bool> UploadFileAsync(string localFilePath, string remoteFilePath)
    {
        try
        {
            ///需要测试一下，如果服务器下此文件的相对路径不存在，是否会创建
            /// 是否需要 Task.Run()
            _logger.LogDebug("本地文件路径：" + localFilePath);
            _logger.LogDebug("远端文件路径：" + remoteFilePath);

            // 创建FTPWebRequest对象，指定上传文件的FTP地址和端口号
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ServerAddress}:{Port}/{remoteFilePath}");
            // 指定FTP操作为上传文件
            request.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定FTP服务器的登录凭证 用户名和密码
            request.Credentials = new NetworkCredential(Username, Password);


            //// 读取本地文件的二进制内容
            //byte[] fileContents = await File.ReadAllBytesAsync(localFilePath);

            //// 获取用于写入文件内容的请求流，并使用WriteAsync方法将fileContents字节数组异步写入请求流中。
            //// 这将把本地文件内容发送到FTP服务器。
            //using (Stream requestStream = await request.GetRequestStreamAsync())
            //{
            //    await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            //}
            try
            {
                // 使用FileStream直接打开本地文件，并将内容异步复制到FTP请求流中，一切为了提速
                await using (FileStream fileStream = File.OpenRead(localFilePath))
                await using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    // 利用CopyToAsync直接将文件流复制到请求流
                    await fileStream.CopyToAsync(requestStream);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            // 异步获取FTP服务器的响应，并打印出来
            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                _logger.LogDebug($"文件上传成功，回复状态是: {response.StatusDescription}");
            }
            return true;

        }
        catch (Exception e)
        {
            _logger.LogDebug($"本地文件路径:{localFilePath},远端文件路径:{remoteFilePath} ;文件上传报错; " + e.Message);
            return false;
        }

    }

    /// <summary>
    /// 与上面的函数正好相反
    /// 注意，一定要确保两个路径的正确性，不然的话必报错
    /// </summary>
    /// <param name="remoteFilePath">服务器下根目录的相对路径</param>
    /// <param name="localFilePath">本地的绝对路径</param>
    public async Task<bool> DownloadFileAsync(string remoteFilePath, string localFilePath)
    {
        // 创建FTPWebRequest对象，指定下载文件的FTP地址和端口号
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create
            ($"ftp://{ServerAddress}:{Port}/{remoteFilePath}");
        // 指定FTP操作为下载文件
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        // 指定FTP服务器的登录凭证 用户名和密码
        request.Credentials = new NetworkCredential(Username, Password);

        // 异步获取FTP服务器的响应
        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        {
            // 从响应中异步获取响应流
            using (Stream responseStream = response.GetResponseStream())
            {
                // 创建本地文件流，准备写入下载的内容
                using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create))
                {
                    // 异步将响应流中的内容复制到本地文件流中
                    await responseStream.CopyToAsync(fileStream);
                }
            }

            _logger.LogDebug($"Download file complete. Response: {response.StatusDescription}");
        }
        return true;
    }

    /// <summary>
    /// 将本地客户端的整个文件夹 上传到服务器下的根目录下的相对路径
    /// 注意啊，不会主动创建localDirectoryPath这个主文件夹名，没有则上传失败
    /// </summary>
    /// <param name="localDirectoryPath"></param>
    /// <param name="remoteDirectoryPath">相对路径</param>
    public async Task UploadDirectoryAsync(string localDirectoryPath, string remoteDirectoryPath)
    {
        // 获取本地目录下的所有文件和子目录的绝对路径
        string[] fileEntries = Directory.GetFiles(localDirectoryPath); //文件
        string[] subdirectoryEntries = Directory.GetDirectories(localDirectoryPath); //子目录

        // 上传文件
        foreach (string filePath in fileEntries)
        {
            // 使用Path.GetFileName方法从文件路径中提取文件名（包括文件扩展名）
            string fileName = Path.GetFileName(filePath);
            // 将根目录下的相对路径与此文件名（含后缀）拼接成一个完整的路径
            string remoteFilePath = Path.Combine(remoteDirectoryPath, fileName);
            // 上传
            await UploadFileAsync(filePath, remoteFilePath);
        }

        // 递归上传子目录
        foreach (string subdirectoryPath in subdirectoryEntries)
        {
            // 使用Path.GetFileName方法从子目录路径中提取子目录名称
            string directoryName = Path.GetFileName(subdirectoryPath);
            // 将根目录下的相对路径与此文件夹名（子目录名）拼接成一个完整的路径
            string remoteSubdirectoryPath = Path.Combine(remoteDirectoryPath, directoryName);

            // 创建远程子目录
            await CreateDirectoryAsync(remoteSubdirectoryPath);

            // 递归上传本地客户端的子目录中的文件和子目录到服务器的这个子目录中
            await UploadDirectoryAsync(subdirectoryPath, remoteSubdirectoryPath);
        }
    }


    /// <summary>
    /// 将服务器下根目录的文件夹下载到本地客户端
    /// </summary>
    /// <param name="remoteDirectoryPath"></param>
    /// <param name="localDirectoryPath"></param>
    public async void DownloadDirectory(string remoteDirectoryPath, string localDirectoryPath)
    {
        // 获取远程目录下的所有文件和子目录
        string[] fileEntries = await GetFileListAsync(remoteDirectoryPath);
        string[] subdirectoryEntries = await GetDirectoryListAsync(remoteDirectoryPath);

        // 下载文件
        foreach (string filePath in fileEntries)
        {
            // 构建本地文件路径
            string fileName = Path.GetFileName(filePath);//提取
            string filePath1 = Path.Combine(remoteDirectoryPath, fileName);
            string localFilePath = Path.Combine(localDirectoryPath, fileName);//合并成完整的本地客户端路径

            //这里的判断我已经在GetFileList内部做了一次，这里是多余的
            if (filePath1.Contains("."))
            {
                await DownloadFileAsync(filePath1, localFilePath);//
            }

        }

        // 递归下载子目录
        foreach (string subdirectoryPath in subdirectoryEntries)
        {
            // 拼接一个本地子目录路径
            string directoryName = GetDirectoryName(subdirectoryPath);
            string localSubdirectoryPath = Path.Combine(localDirectoryPath, directoryName);

            // 创建本地子目录
            //这个判断也是多余的
            if (!directoryName.Contains("."))
            {
                //本地文件目录的创建，直接调用Directory
                Directory.CreateDirectory(localSubdirectoryPath);

                string subdirectoryPath1 = Path.Combine("/", subdirectoryPath);
                // 递归下载子目录中的文件和子目录
                DownloadDirectory(subdirectoryPath1, localSubdirectoryPath);
            }

        }
    }

    /// <summary>
    /// 将服务器下根目录的文件夹下载到本地客户端
    /// </summary>
    /// <param name="remoteDirectoryPath"></param>
    /// <param name="localDirectoryPath"></param>
    /// <returns></returns>
    public async Task DownloadDirectoryAsync(string remoteDirectoryPath, string localDirectoryPath)
    {
        // 获取远程目录下的所有文件和子目录
        string[] fileEntries = await GetFileListAsync(remoteDirectoryPath);
        string[] subdirectoryEntries = await GetDirectoryListAsync(remoteDirectoryPath);

        // 下载文件
        foreach (string filePath in fileEntries)
        {
            // 构建本地文件路径
            string fileName = Path.GetFileName(filePath);
            string localFilePath = Path.Combine(localDirectoryPath, fileName);

            // 下载文件
            await DownloadFileAsync(filePath, localFilePath);
        }

        // 递归下载子目录
        foreach (string subdirectory in subdirectoryEntries)
        {
            string localSubdirectoryPath = Path.Combine(localDirectoryPath, subdirectory);

            // 创建本地子目录
            Directory.CreateDirectory(localSubdirectoryPath);

            // 递归下载子目录中的文件和子目录
            await DownloadDirectoryAsync(Path.Combine(remoteDirectoryPath, subdirectory), localSubdirectoryPath);
        }
    }


    /// <summary>
    ///
    /// 想在服务器创建文件夹，无法直接用Directory.CreateDirectory
    /// 只能自己写这个函数
    /// 如果存在则创建失败
    /// </summary>
    /// <param name="remoteDirectoryPath">根目录下需要创建的文件夹的相对路径</param>
    public async Task<bool> CreateDirectoryAsync(string remoteDirectoryPath)
    {
        try
        {
            _logger.LogDebug("创建路径：" + remoteDirectoryPath);
            // 创建FTPWebRequest对象，指定创建目录的FTP地址和端口号
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create
                ($"ftp://{ServerAddress}:{Port}/{remoteDirectoryPath}");
            // 指定FTP操作为创建目录
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            // 指定FTP服务器的登录凭证 用户名和密码
            request.Credentials = new NetworkCredential(Username, Password);

            // 异步获取FTP服务器的响应
            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                _logger.LogDebug($"Create directory的请求已经发送. Response: {response.StatusDescription}");
            }
            return true;
        }
        catch (WebException ex)
        {
            // 处理可能的异常，例如目录已存在的情况
            if (ex.Response is FtpWebResponse response)
            {
                _logger.LogDebug($"Error: {response.StatusDescription}+{ex.Message}");
                //response.Close();
            }
            else
            {
                _logger.LogDebug($"An error occurred: {ex.Message}");
            }
            return false;
        }
    }


    /// <summary>
    /// 删除服务器根目录下的相对路径下的文件
    /// </summary>
    /// <param name="remoteFilePath">根目录下的相对路径</param>
    public async Task DeleteFileAsync(string remoteFilePath)
    {
        try
        {
            // 创建FTPWebRequest对象，指定删除文件的FTP地址和端口号
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create
                ($"ftp://{ServerAddress}:{Port}/{remoteFilePath}");
            // 指定FTP操作为删除文件
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            // 指定FTP服务器的登录凭证 用户名和密码
            request.Credentials = new NetworkCredential(Username, Password);

            // 异步获取FTP服务器的响应
            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                Console.WriteLine($"Delete file complete. Response: {response.StatusDescription}");
            }
        }
        catch (WebException ex)
        {
            // 处理可能的异常，例如文件不存在或无权限删除
            if (ex.Response is FtpWebResponse response)
            {
                Console.WriteLine($"Error: {response.StatusDescription}");
                response.Close();
            }
            else
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="remoteDirectoryPath">相对路径</param>
    public async Task DeleteDirectoryAsync(string remoteDirectoryPath)
    {
        try
        {
            // 获取目录下的所有文件和子目录
            string[] fileEntries = await GetFileListAsync(remoteDirectoryPath);
            string[] subdirectoryEntries = await GetDirectoryListAsync(remoteDirectoryPath);

            // 并行删除文件
            var fileDeleteTasks = fileEntries.Select(filePath => DeleteFileAsync(filePath));
            await Task.WhenAll(fileDeleteTasks);

            // 递归并行删除子目录
            var directoryDeleteTasks = subdirectoryEntries.Select(subdirectoryPath => DeleteDirectoryAsync(subdirectoryPath));
            await Task.WhenAll(directoryDeleteTasks);

            // 删除目录自身
            await DeleteEmptyDirectoryAsync(remoteDirectoryPath);
        }
        catch (Exception ex)
        {
            // 这里可以根据需要记录日志或进行其他异常处理操作
            Console.WriteLine($"An error occurred while deleting directory: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除空文件夹
    /// </summary>
    /// <param name="remoteDirectoryPath"></param>
    public async Task DeleteEmptyDirectoryAsync(string remoteDirectoryPath)
    {
        try
        {
            // 创建FTPWebRequest对象，指定删除空目录的FTP地址和端口号
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create
                ($"ftp://{ServerAddress}:{Port}/{remoteDirectoryPath}");
            // 指定FTP操作为删除目录
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            // 指定FTP服务器的登录凭证 用户名和密码
            request.Credentials = new NetworkCredential(Username, Password);

            // 异步获取FTP服务器的响应
            using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
            {
                Console.WriteLine($"Delete directory complete. Response: {response.StatusDescription}");
            }
        }
        catch (WebException ex)
        {
            // 处理可能的异常，例如目录不存在或非空目录
            if (ex.Response is FtpWebResponse response)
            {
                Console.WriteLine($"Error: {response.StatusDescription}");
                response.Close();  // 如果不使用using语句，则需要显式关闭
            }
            else
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }


    /// <summary>
    /// 得到 远程服务器上面的 该路径remoteDirectoryPath下的 文件列表
    /// </summary>
    /// <param name="remoteDirectoryPath"></param>
    /// <returns></returns>
    public async Task<string[]> GetFileListAsync(string remoteDirectoryPath)
    {
        // 创建FTPWebRequest对象
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ServerAddress}:{Port}/{remoteDirectoryPath}");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential(Username, Password);

        // 异步获取FTP服务器的响应
        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var fileList = new List<string>();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // 仅添加包含'.'的行，即文件名
                    if (line.Contains("."))
                    {
                        fileList.Add(line);
                    }
                }

                Console.WriteLine($"Get file list complete. Response: {response.StatusDescription}");
                return fileList.ToArray();
            }
        }
    }


    /// <summary>
    /// 得到  远程服务器上面 该路径remoteDirectoryPath下的 文件夹  列表
    /// </summary>
    /// <param name="remoteDirectoryPath"></param>
    /// <returns></returns>
    public async Task<string[]> GetDirectoryListAsync(string remoteDirectoryPath)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ServerAddress}:{Port}/{remoteDirectoryPath}");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential(Username, Password);

        using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var directoryList = new List<string>();
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // 直接使用line变量，因为不再需要转换为绝对路径
                    if (line != "." && line != ".." && !line.Contains("."))
                    {
                        directoryList.Add(line);
                    }
                }

                Console.WriteLine($"Get directory list complete. Response: {response.StatusDescription}");
                return directoryList.ToArray();
            }
        }
    }

    /// <summary>
    /// 处理这个路径，得到该路径最后的 文件夹名
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    private static string GetDirectoryName(string directoryPath)
    {
        // 移除目录的最后一个斜杠
        return directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            .Last();
    }




    // GetHMIDB($"ftp://uploadhis:{password}@{ip}/operationlog/operationlog.db", "operationlog");
    /// <summary>
    /// 处理sqllite数据库
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="saveFileName"></param>
    private void GetHMIDB(string uri, string saveFileName)
    {
        string dbPath = AppDomain.CurrentDomain.BaseDirectory + $"/{saveFileName}.db";
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        FileStream fs = null;
        FtpWebResponse response = null;
        using (fs = new FileStream(dbPath, FileMode.Create))
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                request.ContentOffset = fs.Length;
                using (response = (FtpWebResponse)request.GetResponse())
                {
                    fs.Position = fs.Length;
                    byte[] buffer = new byte[1024 * 4];
                    int count = response.GetResponseStream().Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        fs.Write(buffer, 0, count);
                        count = response.GetResponseStream().Read(buffer, 0, buffer.Length);
                    }
                    response.GetResponseStream().Close();
                }
                fs.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    response?.Close();
                }
                catch (Exception)
                {

                }
                try
                {
                    fs?.Close();
                }
                catch (Exception)
                {

                }
            }
        }
    }


}








#endif 