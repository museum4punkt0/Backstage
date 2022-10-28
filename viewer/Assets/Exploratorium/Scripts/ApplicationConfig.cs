using System;
using System.Net;
using System.Net.Sockets;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using CommandLineParser;

namespace Exploratorium
{
    public class ApplicationConfig : MonoBehaviour
    {
        /*
        Exploratorium.exe
            --config [file-path]               # path to the config file
            --profile [string]                 # config file profile to load
            --syncname [string]                # override to network name of this app instance
            --syncclient [true|false]          # override to autostart client 
            --syncserver [true|false]          # override to autostart server
            --syncrole [Solo|Observer|Control] # override to autostart sync role
            --syncip [string]                  # override to the IP-address of the sync server
            --cmsurl [string]                  # override to the directus base URL
            --cmsuser [string]                 # override to the directus username
            --cmspass [string]                 # override to the directus password
        */

        private const string CLI = "Command Line Arguments";
        private const string ConfigArgName = "--config";
        private const string ProfileArgName = "--profile";
        private const string ClientNameArgName = "--syncname";
        private const string AutostartClientArgName = "--syncclient";
        private const string AutostartServerArgName = "--syncserver";
        private const string AutostartRoleArgName = "--syncrole";
        private const string ServerIPArgName = "--syncip";
        private const string CmsBaseUrlArgName = "--cmsurl";
        private const string CmsUsernameArgName = "--cmsuser";
        private const string CmsPasswordArgName = "--cmspass";

        [BoxGroup(CLI), SerializeField]
        private StringVariable configFilePath;

        [FormerlySerializedAs("profile")] [BoxGroup(CLI), SerializeField]
        private StringVariable configProfile;

        [BoxGroup(CLI), SerializeField]
        private BoolVariable autostartClient;

        [BoxGroup(CLI), SerializeField]
        private BoolVariable autostartServer;

        [BoxGroup(CLI), SerializeField]
        private PawnRoleVariable autostartRole;

        [FormerlySerializedAs("instanceName")] [BoxGroup(CLI), SerializeField]
        private StringVariable clientName;

        [FormerlySerializedAs("serverHostname")] [BoxGroup(CLI), SerializeField]
        private StringVariable serverIP;

        [BoxGroup(CLI), SerializeField]
        private StringVariable cmsBaseUrl;

        [BoxGroup(CLI), SerializeField]
        private StringVariable cmsUsername;

        [BoxGroup(CLI), SerializeField]
        private StringVariable cmsPassword;


        [SerializeField] private VoidEvent loadConfig;

        [SerializeField] private BoolVariable configReady;

        [SerializeField] private Shader resizeShader;

        /*[BoxGroup("Persistent variables")]
        [SerializeField] private AtomList persistentVariables;*/

        private bool _invalid;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void LoadCommandLine()
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone())
            {
                Debug.Log($"{nameof(ApplicationConfig)} : ParrelSync : THIS IS A CLONE PROJECT");
                string parrelArg = ParrelSync.ClonesManager.GetArgument();
                Debug.Log($"{nameof(ApplicationConfig)} : ParrelSync : Custom argument >>>{parrelArg}<<<");
                CommandLine.Init(parrelArg);
            }
            else
            {
                Debug.Log($"{nameof(ApplicationConfig)} : ParrelSync : THIS IS THE ORIGINAL PROJECT");
                Debug.Log(
                    $"{nameof(ApplicationConfig)} : ParrelSync : {nameof(Environment.CommandLine)} >>>{Environment.CommandLine}<<<");
                CommandLine.Init(Environment.CommandLine);
            }
#else
        Debug.Log($"{nameof(ApplicationConfig)} : {nameof(Environment.CommandLine)} >>>{Environment.CommandLine}<<<");
        CommandLine.Init(Environment.CommandLine);
#endif
        }

        private void Awake()
        {
            if (configReady != null)
                configReady.Value = false;
            var ip = GetLocalIPAddress();
            Debug.Log($"{nameof(ApplicationConfig)} : Local IP address is {ip}");
        }

        private void Start()
        {
            HandleArguments();
        }

        private void HandleArguments()
        {
            if (CommandLine.HasKey(ConfigArgName))
            {
                var argValue = CommandLineParser.CommandLine.GetString(ConfigArgName, configFilePath.InitialValue);
                configFilePath.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to config file = '{argValue}'");
            }

            if (CommandLine.HasKey(ProfileArgName))
            {
                var argValue = CommandLineParser.CommandLine.GetString(ProfileArgName, configProfile.InitialValue);
                configProfile.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to config profile = '{argValue}'");
            }

            // load config now and treat all subsequently parsed arguments as overrides
            loadConfig.Raise();

            if (CommandLine.HasKey(ClientNameArgName))
            {
                var argValue = CommandLine.GetString(ClientNameArgName, configProfile.InitialValue);
                clientName.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to client name = '{argValue}'");
            }

            if (CommandLine.HasKey(AutostartClientArgName))
            {
                var argValue = CommandLine.GetBool(AutostartClientArgName, autostartClient.InitialValue);
                autostartClient.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to autostart client = '{argValue}'");
            }

            if (CommandLine.HasKey(AutostartServerArgName))
            {
                var argValue = CommandLine.GetBool(AutostartServerArgName, autostartServer.InitialValue);
                autostartServer.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to autostart server = '{argValue}'");
            }

            if (CommandLine.HasKey(AutostartRoleArgName))
            {
                var argValue = CommandLine.GetEnum(AutostartRoleArgName, autostartRole.InitialValue);
                autostartRole.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to client role = '{argValue}'");
            }

            if (CommandLine.HasKey(ServerIPArgName))
            {
                var argValue = CommandLine.GetString(ServerIPArgName, serverIP.InitialValue);
                serverIP.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to server ip = '{argValue}'");
            }

            if (CommandLine.HasKey(CmsBaseUrlArgName))
            {
                var argValue = CommandLine.GetString(CmsBaseUrlArgName, cmsBaseUrl.InitialValue);
                cmsBaseUrl.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to CMD base URL '{argValue}'");
            }

            if (CommandLine.HasKey(CmsUsernameArgName))
            {
                var argValue = CommandLine.GetString(CmsUsernameArgName, cmsUsername.InitialValue);
                cmsUsername.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to CMS username '{argValue}'");
            }

            if (CommandLine.HasKey(CmsPasswordArgName))
            {
                var argValue = CommandLine.GetString(CmsPasswordArgName, cmsPassword.InitialValue);
                cmsPassword.Value = argValue;
                Debug.Log($"{nameof(ApplicationConfig)} : CL override to CMS password '{Redact(argValue)}'");
            }

            if (configReady != null)
                configReady.Value = true;
        }

        public static string Redact(string str)
        {
            return
                str.Length > 12 ? $"{str.Substring(0, 4)}...{str.Substring(str.Length - 4, 4)}" :
                str.Length > 8 ? $"{str.Substring(0, 3)}...{str.Substring(str.Length - 3, 3)}" :
                str.Length > 6 ? $"{str.Substring(0, 2)}...{str.Substring(str.Length - 2, 2)}" :
                str.Length > 3 ? $"{str.Substring(0, 1)}...{str.Substring(str.Length - 1, 1)}" :
                "...";
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}