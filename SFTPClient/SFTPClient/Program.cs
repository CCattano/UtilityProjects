using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace SeedhostSFTP
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Parameters of the work to be done
            const string host = @"";
            const int port = 0;
            const string username = "";
            const string password = @"";
            const string fileLocation = "";
            const string keyword1 = "";
            const string keyword2 = "";
            const string newFileNamePrefix = @"";
            const string fileExt = @"";

            //Establish connection and initialize client to perform work
            ConnectionInfo connInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
            using SftpClient client = new SftpClient(connInfo);
            client.Connect();

            //Identify files that need to be changed
            List<SftpFile> files = client
                .ListDirectory($"{client.WorkingDirectory}/{fileLocation}/")
                .Where(file => file.Name.ToLower().Contains(keyword1) && file.Name.ToLower().Contains(keyword2))
                .ToList();

            files.ForEach(file =>
            {
                //Isolate pertinent value in current file name
                string isolatedValue = file.Name.Split(keyword2)[1].Split("_")[0];
                //Apply pertinent value to new file name convention
                string newName = $"{newFileNamePrefix}{isolatedValue}.{fileExt}";
                //Generate full file path for new file name
                string path = Path.GetDirectoryName(file.FullName);
                string newFile = $"{path}\\{newName}".Replace("\\", "/");

                //Use this WriteLine to verify the original file name has been correctly transformed to the new file name
                //Console.WriteLine(file.FullName + "\n\n" + newFile);

                //Use Console.WriteLine to verify the changes that are about to be made en masse
                client.RenameFile(file.FullName, newFile);
            });

            client.Disconnect();
        }
    }
}
