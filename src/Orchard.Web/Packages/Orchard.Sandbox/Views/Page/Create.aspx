<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<PageCreateViewModel>" %>

<%@ Import Namespace="Orchard.Sandbox.ViewModels" %>
<%@ Import Namespace="Orchard.Mvc.Html" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>TwoColumns</title>
    <link href="<%=ResolveUrl("~/Content/Site2.css") %>" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="header">
        <div id="innerheader">
            <% Html.Include("header"); %>
        </div>
    </div>
    <div id="page">
        <div id="sideBar">
            <% Html.Include("Navigation"); %>
        </div>
        <div id="main">
            <h3>
                Create Page</h3>
            <%using (Html.BeginForm()) { %>
            <ul>
                <li>
                    <%=Html.LabelFor(x => x.Name)%><%=Html.EditorFor(x => x.Name)%></li>
                <li>
                    <input type="submit" name="submit" value="Create" /></li>
            </ul>
            <%} %>
        </div>
        <div id="footer">
            <% Html.Include("footer"); %>
        </div>
    </div>
</body>
</html>