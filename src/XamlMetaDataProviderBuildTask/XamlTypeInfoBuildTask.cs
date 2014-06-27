using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace XamlBuildTasks
{
	public class XamlTypeInfoBuildTask : Task
    {
		private const string XamlTypeInfoFileName = "XamlTypeInfo.g.cs";

		[Required]
		public string IntermediateOutputPath { get; set; }		
		
		public override bool Execute()
		{
			string filename = IntermediateOutputPath + XamlTypeInfoFileName;
			if (!System.IO.File.Exists(filename))
				return false;
			string code = System.IO.File.ReadAllText(filename);

			if (code.StartsWith("#pragma warning disable 1591")) //Already modified
				return true;
			int idx = code.IndexOf("[System.CodeDom.Compiler.GeneratedCodeAttribute");
			if (idx < 0)
				return false;
			string insert = "[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]\n";
			code = "#pragma warning disable 1591\n" + code.Substring(0, idx) + insert + code.Substring(idx) +
				"#pragma warning restore 1591\n";
			System.IO.File.WriteAllText(filename, code);
			return true;
		}
	}
}
