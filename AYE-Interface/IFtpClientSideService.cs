using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface;


public interface IFtpClientSideService
{
    string ServerAddress { get; set; }   // FTP服务器地址
    string Port { get; set; }             // FTP服务器端口号
    string Username { get; set; }       // FTP用户名
    string Password { get; set; }       // FTP密码

    Task<bool> UploadFileAsync(string localFilePath, string remoteFilePath);

    Task<bool> CreateDirectoryAsync(string remoteDirectoryPath);

    Task<string[]> GetFileListAsync(string remoteDirectoryPath);

    Task<string[]> GetDirectoryListAsync(string remoteDirectoryPath);

}