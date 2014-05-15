using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TcmDependencies
{
	class Program
	{
		private static String ApplicationPath
		{
			get
			{
				return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			}
		}

		private static void ExtractFromGAC(String destinationPath)
		{
			String[] dependenciesGAC = new String[] 
			{ 
				"Tridion.Common.Core",
				"Tridion.Common",
				"Tridion.ContentManager.Common",
				"Tridion.ContentManager.CoreService.Client",
				"Tridion.ContentManager.CoreService",
				"Tridion.ContentManager.Data.AdoNet",
				"Tridion.ContentManager.Data.AdoNet.Sql",
				"Tridion.ContentManager.Data",
				"Tridion.ContentManager",
				"Tridion.ContentManager.Interop.cm_bl",
				"Tridion.ContentManager.Interop.cm_defines",
				"Tridion.ContentManager.Interop.cm_sys",
				"Tridion.ContentManager.Interop.cm_tom",
				"Tridion.ContentManager.Interop.Licensing",
				"Tridion.ContentManager.Localization",
				"Tridion.ContentManager.Publishing",
				"Tridion.ContentManager.TemplateTypes",				
				"Tridion.ContentManager.Templating",
				"Tridion.ContentManager.TypeRegistration",
				"Tridion.Logging",
				"Tridion.Security",
				"Microsoft.Practices.EnterpriseLibrary.Common",
				"Microsoft.Practices.EnterpriseLibrary.Logging",
				"Microsoft.Practices.ObjectBuilder"
			};

			foreach (String dependencyGAC in dependenciesGAC)
			{
				String assemblyPath = GACUtil.GetGACAssembly(dependencyGAC);

				if (String.IsNullOrEmpty(assemblyPath))
					Console.WriteLine("[!] Could not locate {0} in Global Assembly cache.", dependencyGAC);
				else
				{
					File.Copy(assemblyPath, Path.Combine(destinationPath, dependencyGAC + ".dll"), true);
					Console.WriteLine("[i] Extracted {0} from Global Assembly cache.", dependencyGAC);
				}
			}
		}

		private static void ExtractFromTridion(String destinationPath)
		{
			String[] dependenciesTridion = new String[]
			{
				@"web\TemplateBuilder\Tridion.ContentManager.Templating.CompoundTemplates.DomainModel.dll.deploy"
			};
			
			String tridionHome = Path.GetFullPath(Environment.GetEnvironmentVariable("TRIDION_HOME"));

			if (!Directory.Exists(tridionHome))
				throw new Exception(String.Format("TRIDION_HOME {0} could not be found.", tridionHome));

			foreach (String dependencyTridion in dependenciesTridion)
			{
				String sourceFilename = Path.Combine(tridionHome, dependencyTridion);
				String destinationFilename = Path.GetFileName(sourceFilename);
				destinationFilename = destinationFilename.EndsWith(".deploy", StringComparison.OrdinalIgnoreCase) ? destinationFilename.Substring(0, destinationFilename.Length - 7) : destinationFilename;
				
				if (!File.Exists(sourceFilename))
					Console.WriteLine("[!] Could not locate {0} in Tridion installation.", dependencyTridion);

				File.Copy(sourceFilename, Path.Combine(destinationPath, destinationFilename), true);

				Console.WriteLine("[i] Extracted {0} from Tridion installation.", dependencyTridion);
			}
		}

		static void Main(string[] args)
		{
			if (args.Count() != 1)
			{
				Console.WriteLine("[i] Usage: TcmDependencies <destination folder>");
				return;
			}

			// Expand relative paths
			String destinationPath = Path.GetFullPath(args[0]);

			// Ensure the directory is created
			Directory.CreateDirectory(destinationPath);

			if (!Directory.Exists(destinationPath))
			{
				Console.WriteLine("[!] Destination path {0} does not exist.", destinationPath);
				return;
			}

			ExtractFromGAC(destinationPath);
			ExtractFromTridion(destinationPath);
		}
	}
}
