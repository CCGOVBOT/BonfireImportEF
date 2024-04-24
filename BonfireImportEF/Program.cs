using BonfireImportEF.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using WinSCP;

namespace BonfireImportEF
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"appSettings.json", true, true);
                var config = builder.Build();

                string strReadFilePath = config["ReadFileLocation"].ToString();
                string strReadFileName = "";
                string strLogPath = config["LogFileLocation"].ToString() + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                // 1. Delete files before downloading
                System.IO.DirectoryInfo di = new DirectoryInfo(strReadFilePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }


                // 2. Access to Bonfire and download file
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "integration-sftp-us-east-1.bf-node.com",
                    UserName = "organization-547-cookcounty",
                    SshHostKeyFingerprint = "ssh-rsa 2048 ZKFLHopUlsRqKxB/OMqswegZsJpp/UhfWTt0jIRpZb0=",
                    SshPrivateKeyPath = config["PrivateKeyLocation"].ToString()
                };

                sessionOptions.AddRawSettings("FSProtocol", "2");

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    string remotePath = "/opportunities-report";
                    string localPath = strReadFilePath;

                    // Get list of files in the directory
                    RemoteDirectoryInfo directoryInfo = session.ListDirectory(remotePath);

                    // Select the most recent file
                    RemoteFileInfo latest =
                        directoryInfo.Files
                            .Where(file => !file.IsDirectory)
                            .OrderByDescending(file => file.LastWriteTime)
                            .FirstOrDefault();

                    // Any file at all?
                    if (latest == null)
                    {
                        throw new Exception("No file found");
                    }

                    // Download the selected file
                    strReadFileName = latest.Name;
                    session.GetFileToDirectory(latest.FullName, localPath);
                }

                // 3. Process the file
                using (StreamWriter writer = new StreamWriter(new FileStream(strLogPath, FileMode.Create, FileAccess.Write)))
                {
                    writer.WriteLine("Process Begin");

                    int itemErrorCount = 0;
                    //var lines = File.ReadAllLines(strReadFilePath + strReadFileName);
                    var lines = File.ReadAllLines(@"C:\BonfireProcess\Opportunity\LocalFileTest\OpportunitiesReportTest.csv");
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string strSoliciationNumber = "";
                        string[] test;
                        try //Line Item Level
                        {
                            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                            string line = lines[i];
                            string[] values = csvParser.Split(line);

                            for (int j = 0; j < values.Length; j++)
                            {
                                values[j] = values[j].TrimStart('"');
                                values[j] = values[j].TrimEnd('"');
                            }

                            strSoliciationNumber = values[0].ToString();
                            test = values;
                            if (values != null)
                            {
                                InsertOrUpdate(values);
                            }
                        }
                        catch (Exception ex)
                        {
                            itemErrorCount++;
                            writer.WriteLine("  Process Item Error: " + strSoliciationNumber);
                            writer.WriteLine("    Details: " + ex.Message);
                        }
                    }

                    writer.WriteLine("Process End");
                    writer.WriteLine("");
                    writer.WriteLine("Total Errors:" + itemErrorCount);
                }

                EmailWithAttachment(strLogPath);

            }
            catch (Exception ex)
            {
                EmailAlert(ex.Message);
                //string a = "";aa
            }
        }

        static void EmailAlert(string strMessage)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();

            SmtpClient client = new SmtpClient();
            client.Host = config["SMTP:Host"];
            client.Port = Int32.Parse(config["SMTP:Port"]);

            MailMessage message = new MailMessage();
            var addresses = config["SMTP:EmailTo"];
            foreach (var address in addresses.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(address);
            }
            message.From = new MailAddress(config["SMTP:EmailFrom"]);

            message.IsBodyHtml = true;
            message.Subject = "Bonfilre Import Error";
            message.Body = strMessage;

            client.Send(message);
        }

        static void EmailWithAttachment(string strFilePath)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();

            SmtpClient client = new SmtpClient();
            client.Host = config["SMTP:Host"];
            client.Port = Int32.Parse(config["SMTP:Port"]);

            MailMessage message = new MailMessage();

            var addresses = config["SMTP:EmailTo"];
            foreach (var address in addresses.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(address);
            }
            message.From = new MailAddress(config["SMTP:EmailFrom"]);
            message.IsBodyHtml = true;
            message.Subject = "Opportunity Report";
            message.Body = "Opportunity Report";

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
            contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
            contentType.Name = "Log.txt";

            message.Attachments.Add(new Attachment(strFilePath, contentType));
            client.Send(message);
        }

        static void InsertOrUpdate(string[] values)
        {
            string strSoliciationNumber = values[0].ToString();
            string strType = values[1].ToString();
            string strDescription = values[2].ToString();
            string strDepartment = values[3].ToString();
            string[] commodityCodes = values[4].ToString().Split('|');
            string strCommodityCode = commodityCodes[0] != "" ? commodityCodes[0] : "00";
            string strCommodityCodeGroup = values[5].ToString();
            DateTime dateAdvertised = DateTime.Parse(values[6].ToString());
            DateTime dateDue = DateTime.Parse(values[7].ToString());
            string strStatus = values[8].ToString();
            string strURL = values[9].ToString();

            // Filter and Mapping
            //if (strStatus.ToUpper() != "CANCELLED" && strType.ToUpper() != "OTHER"
            //    && strType.ToUpper() != "SS")
            if (strStatus.ToUpper() == "OPEN"
                && (strType.ToUpper() == "IFB" || strType.ToUpper() == "RFP" || strType.ToUpper() == "RFSQ" || strType.ToUpper() == "RFS"))
            {
                // Bids
                if (strType.ToUpper() == "IFB")
                {
                    strType = "C";
                }
                
                // Request for Proposals
                else if (strType.ToUpper() == "RFP")
                {
                    strType = "R";
                }

                // Request for Qualifications
                else if (strType.ToUpper() == "RFSQ")
                {
                    strType = "Q";
                }

                // ARPA
                else if (strType.ToUpper() == "RFS")
                {
                    strType = "H";
                }


                var context = new Procurement_EDSContext();
                strCommodityCode = strCommodityCode.Substring(0, 2) + "000000";
                var edsCommodityGroupID = context.BidListCommodityGroups
                                                .Where(d => d.Code == strCommodityCode)
                                                .Select(d => d.CommodityGroupId)
                                                .FirstOrDefault();
                if (edsCommodityGroupID == 0)
                {
                    strCommodityCode = "9900000";
                }


            
                var oldBid = context.BidListContracts
                                .Where(d => d.ContractNumber == strSoliciationNumber && d.Agency == "C")
                                .OrderByDescending(d => d.DateStamp)
                                .FirstOrDefault();

                if (oldBid == null)
                {// Insert
                    var bid = new BidListContract()
                    {
                        ContractNumber = strSoliciationNumber,
                        Rebid = 0,
                        Cfor = strDescription,
                        Cfrom = strDepartment,
                        DatePosted = dateAdvertised,    // DateTime.Parse("2021-07-20 00:00:00"),         
                        BidOpening = dateDue,           // DateTime.Parse("2022-12-29 00:00:00"),         
                        BidExpiration = dateDue,        // DateTime.Parse("2022-12-29 00:00:00"),      

                        OperatorId = "BonFire",
                        DateStamp = DateTime.Now,
                        Notification = 0,

                        Agency = "C",
                        Subdivision = "C",
                        Type = strType,                                                 
                        StatusValue = "Y",
                        CommodityGroupId = edsCommodityGroupID,                     
                        FileType = "Contract",
                        File0 = "",
                        LinkTo = "https://" + strURL

                    };
                    context.BidListContracts.Add(bid);
                    context.SaveChanges();
                }
                else
                {// Update
                    oldBid.ContractNumber = strSoliciationNumber;
                    oldBid.Rebid = 0;
                    oldBid.Cfor = strDescription;
                    oldBid.Cfrom = strDepartment;
                    oldBid.DatePosted = dateAdvertised;     //DateTime.Parse("2021-07-20 00:00:00");          
                    oldBid.BidOpening = dateDue;            // DateTime.Parse("2022-12-29 00:00:00");          
                    oldBid.BidExpiration = dateDue;         //DateTime.Parse("2022-12-29 00:00:00");       
                    oldBid.OperatorId = "BonFire";
                    oldBid.DateStamp = DateTime.Now;
                    oldBid.Notification = 0;

                    oldBid.Agency = "C";
                    oldBid.Subdivision = "C";
                    oldBid.Type = strType;                                                  
                    oldBid.StatusValue = "Y";
                    oldBid.CommodityGroupId = edsCommodityGroupID;                                        
                    oldBid.FileType = "Contract";
                    oldBid.File0 = "";
                    oldBid.LinkTo = "https://" + strURL;
                }
                context.SaveChanges();
            }


            
        }

    }
}
