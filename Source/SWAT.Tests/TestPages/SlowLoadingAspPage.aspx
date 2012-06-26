<%@ Page Language="C#" %>
<script runat="server">
	void Page_Load()
	{
		DoTimeout(3);
	}
	void DoTimeout(int sec)
	{
		DateTime runTime = DateTime.Now.AddSeconds(sec);
        while (DateTime.Now < runTime) { }
	}
</script>
<html>
<head id="Head1" runat="server" >
   <title>Slow ASP Page</title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      <input id="makeSureIExist">
    </div>
  </form>
</body>
</html>