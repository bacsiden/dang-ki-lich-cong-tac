<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowReport.aspx.cs" Inherits="DKLCT.Reports.ShowReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>
<html>
<head>
    <title>Show Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" SizeToReportContent="False"
            Width="820px" Height="820px" ShowRefreshButton="false" AsyncRendering="false"
            DocumentMapCollapsed="True" PageCountMode="Actual" PromptAreaCollapsed="True">
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
