using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WallPaperService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            //this.AfterInstall += ProjectInstaller_AfterInstall;
        }

        //private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        //{
        //    ConnectionOptions coOptions = new ConnectionOptions();
        //    coOptions.Impersonation = ImpersonationLevel.Impersonate;
        //    ManagementScope mgmtScope = new System.Management.ManagementScope(@"root\CIMV2", coOptions);
        //    mgmtScope.Connect();
        //    ManagementObject wmiService;
        //    wmiService = new ManagementObject("Win32_Service.Name='" + this.serviceInstaller1.ServiceName + "'");
        //    ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
        //    InParam["DesktopInteract"] = true;
        //    ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", InParam, null);
        //}
        public override void Install(IDictionary stateServer)
        {
            Microsoft.Win32.RegistryKey system,
             //HKEY_LOCAL_MACHINE/Services/CurrentControlSet
             currentControlSet,
             //.../Services
             services,
             //.../<Service Name>
             service,
             //.../Parameters - this is where you can put service-specific configuration
             config;

            try
            {
                //Let the project installer do its job
                base.Install(stateServer);

                //Open the HKEY_LOCAL_MACHINE/SYSTEM key
                system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
                //Open CurrentControlSet
                currentControlSet = system.OpenSubKey("CurrentControlSet");
                //Go to the services key
                services = currentControlSet.OpenSubKey("Services");
                //Open the key for your service, and allow writing
                service = services.OpenSubKey(this.serviceInstaller1.ServiceName, true);
                //Add your service's description as a REG_SZ value named "Description"
                service.SetValue("Description", "Auto change wall paper");
                //(Optional) Add some custom information your service will use...
                //允许服务与桌面交互
                service.SetValue("Type", 0x00000110);
                config = service.CreateSubKey("Parameters");
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception was thrown during service installation:/n" + e.ToString());
            }
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            var services = ServiceController.GetServices();
            var service = services.FirstOrDefault(x => x.ServiceName == this.serviceInstaller1.ServiceName);
            if (service != null && service.Status != ServiceControllerStatus.Running)
            {
                service.Start();
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            var services = ServiceController.GetServices();
            var service = services.FirstOrDefault(x => x.ServiceName == this.serviceInstaller1.ServiceName);
            if (service != null && service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
            }
            base.OnBeforeUninstall(savedState);
        }

        public override void Uninstall(IDictionary stateServer)
        {
            Microsoft.Win32.RegistryKey system,
             currentControlSet,
             services,
             service;

            try
            {
                //Drill down to the service key and open it with write permission
                system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
                currentControlSet = system.OpenSubKey("CurrentControlSet");
                services = currentControlSet.OpenSubKey("Services");
                service = services.OpenSubKey(this.serviceInstaller1.ServiceName, true);
                //Delete any keys you created during installation (or that your service created)
                service.DeleteSubKeyTree("Parameters");
                //...
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception encountered while uninstalling service:/n" + e.ToString());
            }
            finally
            {
                //Let the project installer do its job
                base.Uninstall(stateServer);
            }
        }

    }
}
