﻿using System;
using System.Linq;
using System.Management.Automation;
using static Microsoft.Rest.ClientRuntime.PowerShell.MarkdownRenderer;

namespace Microsoft.Rest.ClientRuntime.PowerShell
{
    [Cmdlet(VerbsData.Export, "HelpMarkdown")]
    [DoNotExport]
    public class ExportHelpMarkdown : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public PSModuleInfo ModuleInfo { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public PSObject[] FunctionInfo { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public PSObject[] HelpInfo { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string DocsFolder { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ExamplesFolder { get; set; }

        protected override void ProcessRecord()
        {
            var helpInfos = HelpInfo.Select(hi => hi.ToPsHelpInfo());
            var variantGroups = FunctionInfo.Select(fi => fi.BaseObject).Cast<FunctionInfo>()
                .Join(helpInfos, fi => fi.Name, phi => phi.CmdletName, (fi, phi) => fi.ToVariants(phi))
                .Select(va => new VariantGroup(ModuleInfo.Name, va.First().CmdletName, va, String.Empty));
            WriteMarkdowns(variantGroups, ModuleInfo.ToModuleInfo(), DocsFolder, ExamplesFolder);
        }
    }
}
