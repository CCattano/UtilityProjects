using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToggleEthernet
{
    class Program
    {
        static void Main(string[] args)
        {
            string ethernetInterface = GetEthernetAdapterDetails();

            if (string.IsNullOrWhiteSpace(ethernetInterface))
            {
                Console.WriteLine("\nEthernet Adapter could not be found in the list of network adapters. Please try again.\r\n\nPress any key to close...\n");
                Console.ReadKey();
                return;
            }

            List<string> interfaceDetails = ethernetInterface.Split("\r\n")?.ToList() ?? new List<string>();
            interfaceDetails = interfaceDetails.Select(detail => Regex.Replace(detail, @"(\s+)", " ")?.ToLower()).ToList();
            string interfaceStatus = interfaceDetails.FirstOrDefault(part => part?.Contains("administrative state") ?? false)?.Split(":")?[1]?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(interfaceStatus))
            {
                Console.WriteLine("\nEthernet Adapter Status could not be determined. Please try again.\r\n\nPress any key to close...\n");
                Console.ReadKey();
                return;
            }

            string state = interfaceStatus == "enabled" ? "disable" : "enable";
            ToggleEthernetAdapter(state);

            if (state.Equals("disable"))
            {
                RestartWLANService();
            }
        }

        private static string GetEthernetAdapterDetails()
        {
            string ethernetInterface;
            using (Process getStatus = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    Arguments = "/C netsh interface show interface Ethernet",
                    FileName = "cmd.exe"
                }
            })
            {
                getStatus.Start();
                ethernetInterface = getStatus.StandardOutput.ReadToEnd();
                getStatus.WaitForExit();
            }
            return ethernetInterface;
        }

        private static void ToggleEthernetAdapter(string state)
        {
            using (Process toggleStatus = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"/C netsh interface set interface Ethernet {state}",
                    FileName = "cmd.exe"
                }
            })
            {
                toggleStatus.Start();
                toggleStatus.WaitForExit();
            }
        }

        private static void RestartWLANService()
        {
            string wlanServiceState = QueryWLANServiceState();
            if (string.IsNullOrWhiteSpace(wlanServiceState))
            {
                Console.WriteLine("The status of the WLAN AutoConfig Windows Service could not be determined.\n\nIt's status was not modified in any way.\r\n\nPress any key to close...\n");
                Console.ReadKey();
                return;
            }
            else if (wlanServiceState == "4 running")
            {
                return;
            }
            else if (wlanServiceState == "1 stopped")
            {
                using (Process startWLANService = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = $"/C sc start wlansvc",
                        FileName = "cmd.exe"
                    }
                })
                {
                    startWLANService.Start();
                    startWLANService.WaitForExit();
                }
            }
        }

        private static string QueryWLANServiceState()
        {
            string wlanServiceOutput = string.Empty;
            using (Process stopWLANService = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"/C sc query wlansvc",
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe"
                }
            })
            {
                stopWLANService.Start();
                wlanServiceOutput = stopWLANService.StandardOutput.ReadToEnd();
                stopWLANService.WaitForExit();
            }
            List<string> wlanServiceDetails = wlanServiceOutput.Split("\r\n")?.ToList() ?? new List<string>();
            wlanServiceDetails = wlanServiceDetails.Select(detail => Regex.Replace(detail, @"(\s+)", " ")?.ToLower()).ToList();
            string wlanServiceStatus = wlanServiceDetails.FirstOrDefault(part => part?.Contains("state") ?? false)?.Split(":")?[1]?.Trim() ?? string.Empty;
            return wlanServiceStatus;
        }
    }
}
