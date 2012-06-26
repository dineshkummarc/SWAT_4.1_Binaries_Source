<%@ Page Language="C#" %>
<script runat="server">
    protected void DoLongCode(object sender, EventArgs e)
    {
        DateTime runTime = DateTime.Now.AddSeconds(50);
        while (DateTime.Now < runTime) { }
        Response.Redirect("TestPage.htm");
    }
</script>
<html>
<head id="Head1" runat="server" >
   <title>Slow ASP Page</title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      <asp:Button id="slowNavButton" 
         runat="server" 
         onclick="DoLongCode" 
         Text="Navigate" >
       </asp:Button>
    </div>
  </form>
</body>
</html>