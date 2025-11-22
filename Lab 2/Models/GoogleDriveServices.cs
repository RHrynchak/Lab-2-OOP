using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Lab_2.Models
{
    public class GoogleDriveService
    {
        private DriveService _service;
        private readonly string _applicationName = "Lab_2";
        private readonly string[] _scopes = { DriveService.Scope.DriveFile };
        private readonly Logger _logger = Logger.GetInstance();

        public void Initialize(string credentialsPath = "credentials.json")
        {
            try
            {
                _logger.LogInfo("Ініціалізація Google Drive Service...");

                UserCredential credential;

                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        _scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

                    _logger.LogInfo($"Credentials збережено у: {credPath}");
                }

                _service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _applicationName,
                });

                _logger.LogInfo("✅ Google Drive Service ініціалізовано успішно");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ініціалізація Google Drive", ex);
                throw;
            }
        }

        public string UploadFile(string filePath, string mimeType)
        {
            if (_service == null)
                throw new InvalidOperationException("Google Drive Service не ініціалізовано. Викличте Initialize() спочатку.");

            try
            {
                _logger.LogInfo($"Завантаження файлу на Google Drive: {filePath}");

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(filePath),
                    MimeType = mimeType
                };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = _service.Files.Create(fileMetadata, stream, mimeType);
                    request.Fields = "id, name, webViewLink, webContentLink";
                    request.Upload();
                }

                var file = request.ResponseBody;

                if (file != null)
                {
                    _logger.LogInfo($"✅ Файл завантажено: {file.Name} (ID: {file.Id})");
                    _logger.LogInfo($"   Посилання: {file.WebViewLink}");
                    return file.Id;
                }
                else
                {
                    throw new Exception("Не вдалося завантажити файл на Google Drive");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Завантаження файлу {filePath} на Google Drive", ex);
                throw;
            }
        }

        public void DownloadFile(string fileId, string savePath)
        {
            if (_service == null)
                throw new InvalidOperationException("Google Drive Service не ініціалізовано.");

            try
            {
                _logger.LogInfo($"Завантаження файлу з Google Drive: ID={fileId}");

                var request = _service.Files.Get(fileId);

                using (var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    request.Download(stream);
                }

                _logger.LogInfo($"✅ Файл збережено: {savePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Завантаження файлу з Google Drive (ID: {fileId})", ex);
                throw;
            }
        }

        public IList<Google.Apis.Drive.v3.Data.File> ListFiles(int maxResults = 10)
        {
            if (_service == null)
                throw new InvalidOperationException("Google Drive Service не ініціалізовано.");

            try
            {
                _logger.LogInfo("Отримання списку файлів з Google Drive...");

                var request = _service.Files.List();
                request.PageSize = maxResults;
                request.Fields = "files(id, name, mimeType, createdTime, modifiedTime, size)";

                var result = request.Execute();
                var files = result.Files;

                _logger.LogInfo($"✅ Знайдено {files.Count} файлів");

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError("Отримання списку файлів", ex);
                throw;
            }
        }

        public void DeleteFile(string fileId)
        {
            if (_service == null)
                throw new InvalidOperationException("Google Drive Service не ініціалізовано.");

            try
            {
                _logger.LogInfo($"Видалення файлу: ID={fileId}");

                _service.Files.Delete(fileId).Execute();

                _logger.LogInfo($"✅ Файл видалено");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Видалення файлу (ID: {fileId})", ex);
                throw;
            }
        }

        public IList<Google.Apis.Drive.v3.Data.File> SearchFiles(string fileName)
        {
            if (_service == null)
                throw new InvalidOperationException("Google Drive Service не ініціалізовано.");

            try
            {
                _logger.LogInfo($"Пошук файлів: {fileName}");

                var request = _service.Files.List();
                request.Q = $"name contains '{fileName}'";
                request.Fields = "files(id, name, mimeType, webViewLink)";

                var result = request.Execute();
                var files = result.Files;

                _logger.LogInfo($"✅ Знайдено {files.Count} файлів");

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Пошук файлів '{fileName}'", ex);
                throw;
            }
        }

        public static string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".xml" => "application/xml",
                ".html" => "text/html",
                ".htm" => "text/html",
                ".txt" => "text/plain",
                ".json" => "application/json",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
